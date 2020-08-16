﻿namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type CommoditySource = unit -> Map<uint64, CommodityDescriptor>
type ItemSource = unit -> Map<uint64, ItemDescriptor>
type IslandMarketSource = Location -> Map<uint64, Market>
type IslandMarketSink = Location -> Map<uint64, Market> -> unit
type IslandItemSource = Location -> Set<uint64>
type IslandItemSink   = Location -> Set<uint64>->unit
type IslandSingleMarketSource = Location -> uint64 -> Market option
type IslandSingleMarketSink = Location -> (uint64 * Market) -> unit


module Island =
    let Create() : Island =
        {
            Name           = ""
            AvatarVisits   = Map.empty
            Jobs           = []
            CareenDistance = 0.1 //TODO: dont hardcode this
        }

    let SetName 
            (name   : string) 
            (island : Island) 
            : Island =
        {island with Name = name}

    let GetDisplayName 
            (avatarId : string) 
            (island   : Island) 
            : string =
        match island.AvatarVisits |> Map.tryFind avatarId with
        | Some x when x.VisitCount.IsSome ->
            island.Name
        | _ ->
            "(unknown)"
    
    let AddVisit 
            (turn     : float) 
            (avatarId : string) 
            (island   : Island) 
            : Island =
        match island.AvatarVisits |> Map.tryFind avatarId with
        | None ->
            {island with 
                AvatarVisits = 
                    island.AvatarVisits 
                    |> Map.add avatarId {VisitCount = 1u |> Some; LastVisit = Some turn}}
        | Some x when x.LastVisit.IsNone ->
            {island with
                AvatarVisits =
                    island.AvatarVisits
                    |> Map.add avatarId {VisitCount = ((x.VisitCount |> Option.defaultValue 0u)+1u) |> Some; LastVisit = Some turn}}
        | Some x when x.LastVisit.IsSome && x.LastVisit.Value<turn ->
            {island with
                AvatarVisits =
                    island.AvatarVisits
                    |> Map.add avatarId {VisitCount = ((x.VisitCount |> Option.defaultValue 0u)+1u) |> Some; LastVisit = Some turn}}
        | _ -> island

    let GenerateJobs 
            (termSources                : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (worldSingleStatisticSource : WorldSingleStatisticSource)
            (random                     : Random) 
            (destinations               : Set<Location>) 
            (island                     : Island) 
            : Island =
        if island.Jobs.IsEmpty && not destinations.IsEmpty then
            {island with 
                Jobs = 
                    [
                        Job.Create 
                            termSources 
                            worldSingleStatisticSource 
                            random 
                            destinations
                    ] 
                    |> List.append island.Jobs}
        else
            island

    let RemoveJob 
            (index  : uint32) 
            (island : Island) 
            : Island * (Job option) =
        let taken, left =
            island.Jobs
            |> List.zip [1u..(island.Jobs.Length |> uint32)]
            |> List.partition 
                (fun (idx, _)->idx=index)
        {island with Jobs = left |> List.map snd}, taken |> List.map snd |> List.tryHead

    let MakeKnown 
            (avatarId : string) 
            (island   : Island) 
            : Island =
        match island.AvatarVisits |> Map.tryFind avatarId with
        | None ->
            {island with 
                AvatarVisits =
                    island.AvatarVisits
                    |> Map.add avatarId {VisitCount=0u |> Some; LastVisit=None}}
        | Some x when x.VisitCount = None -> //TODO: i dislike this copypasta
            {island with 
                AvatarVisits =
                    island.AvatarVisits
                    |> Map.add avatarId {VisitCount=0u |> Some; LastVisit=None}}
        | _ -> island

    let MakeSeen 
            (avatarId : string) 
            (island   : Island) 
            : Island =
        match island.AvatarVisits |> Map.tryFind avatarId with
        | None ->
            {island with 
                AvatarVisits =
                    island.AvatarVisits
                    |> Map.add avatarId {VisitCount=None; LastVisit=None}}
        | _ -> island

    let private SupplyDemandGenerator 
            (random:Random) 
            : float = //TODO: move this function out!
        (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + 3.0

    let GenerateCommodities 
            (commoditySource    : CommoditySource)
            (islandMarketSource : IslandMarketSource) 
            (islandMarketSink   : IslandMarketSink) 
            (random             : Random) 
            (location           : Location) 
            : unit =
        let islandMarkets = islandMarketSource location
        if islandMarkets.IsEmpty then
            let commodities =
                commoditySource()
            commodities
            |> Map.fold
                (fun a commodity _->
                    let market = 
                        {
                            Supply=random |> SupplyDemandGenerator
                            Demand=random |> SupplyDemandGenerator
                        }
                    a
                    |> Map.add commodity market) islandMarkets
            |> islandMarketSink location

    let GenerateItems 
            (islandItemSource : IslandItemSource) 
            (islandItemSink   : IslandItemSink) 
            (random           : Random) 
            (itemSource       : ItemSource)
            (location         : Location) 
            : unit =
        let items = itemSource()
        let islandItems = islandItemSource location
        if islandItems.IsEmpty then
            items
            |> Map.fold 
                (fun a k v -> 
                    if random.NextDouble() < v.Occurrence then
                        a |> Set.add k
                    else
                        a) islandItems
            |> islandItemSink location

    let private ChangeMarketDemand 
            (islandSingleMarketSource : IslandSingleMarketSource)
            (islandSingleMarketSink   : IslandSingleMarketSink) 
            (commodity                : uint64) 
            (change                   : float) 
            (location                 : Location) 
            : unit =
        commodity
        |> islandSingleMarketSource location
        |> Option.map (Market.ChangeDemand change)
        |> Option.iter (fun market -> islandSingleMarketSink location (commodity, market))

        

    let private ChangeMarketSupply 
            (islandSingleMarketSource : IslandSingleMarketSource)
            (islandSingleMarketSink   : IslandSingleMarketSink) 
            (commodity                : uint64) 
            (change                   : float) 
            (location                 : Location) 
            : unit =
        commodity
        |> islandSingleMarketSource location
        |> Option.map (Market.ChangeSupply change)
        |> Option.iter (fun market -> islandSingleMarketSink location (commodity, market))

    let UpdateMarketForItemSale 
            (islandSingleMarketSource : IslandSingleMarketSource) 
            (islandSingleMarketSink   : IslandSingleMarketSink) 
            (commoditySource          : CommoditySource)
            (descriptor               : ItemDescriptor) 
            (quantitySold             : uint64) 
            (location                 : Location) 
            : unit =
        let commodities = commoditySource()
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantitySold |> float) * commodities.[commodity].SaleFactor
                ChangeMarketDemand islandSingleMarketSource islandSingleMarketSink commodity totalQuantity location)

    let UpdateMarketForItemPurchase 
            (islandSingleMarketSource : IslandSingleMarketSource) 
            (islandSingleMarketSink   : IslandSingleMarketSink) 
            (commoditySource          : CommoditySource)
            (descriptor               : ItemDescriptor) 
            (quantityPurchased        : uint64) 
            (location                 : Location) 
            : unit =
        let commodities = commoditySource()
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantityPurchased |> float) * commodities.[commodity].PurchaseFactor
                ChangeMarketSupply islandSingleMarketSource islandSingleMarketSink commodity totalQuantity location)
