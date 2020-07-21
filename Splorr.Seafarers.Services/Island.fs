﻿namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Island =
    let Create() : Island =
        {
            Name           = ""
            AvatarVisits   = Map.empty
            Jobs           = []
            Markets        = Map.empty
            CareenDistance = 0.1 //TODO: dont hardcode this
        }

    let SetName (name:string) (island:Island) : Island =
        {island with Name = name}

    let GetDisplayName (avatarId:string) (island:Island) : string =
        match island.AvatarVisits |> Map.tryFind avatarId with
        | Some _ ->
            island.Name
        | None ->
            "????"
    
    let AddVisit (turn: float) (avatarId:string) (island:Island) : Island =
        match island.AvatarVisits |> Map.tryFind avatarId with
        | None ->
            {island with 
                AvatarVisits = 
                    island.AvatarVisits 
                    |> Map.add avatarId {VisitCount = 1u; LastVisit = Some turn}}
        | Some x when x.LastVisit.IsNone ->
            {island with
                AvatarVisits =
                    island.AvatarVisits
                    |> Map.add avatarId {VisitCount = x.VisitCount+1u; LastVisit = Some turn}}
        | Some x when x.LastVisit.IsSome && x.LastVisit.Value<turn ->
            {island with
                AvatarVisits =
                    island.AvatarVisits
                    |> Map.add avatarId {VisitCount = x.VisitCount+1u; LastVisit = Some turn}}
        | _ -> island

    let GenerateJobs (random:System.Random) (rewardRange:float*float) (destinations:Set<Location>) (island:Island) : Island =
        if island.Jobs.IsEmpty && not destinations.IsEmpty then
            {island with Jobs = [Job.Create random rewardRange destinations] |> List.append island.Jobs}
        else
            island

    let RemoveJob (index:uint32) (island:Island) : Island * (Job option) =
        let taken, left =
            island.Jobs
            |> List.zip [1u..(island.Jobs.Length |> uint32)]
            |> List.partition 
                (fun (idx, _)->idx=index)
        {island with Jobs = left |> List.map snd}, taken |> List.map snd |> List.tryHead

    let MakeKnown (avatarId:string) (island:Island) : Island =
        match island.AvatarVisits |> Map.tryFind avatarId with
        | None ->
            {island with 
                AvatarVisits =
                    island.AvatarVisits
                    |> Map.add avatarId {VisitCount=0u; LastVisit=None}}
        | _ -> island

    let private SupplyDemandGenerator (random:System.Random) : float =
        (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + 3.0

    let GenerateCommodities (random:System.Random) (commodities:Map<uint64, CommodityDescriptor>) (island:Island) : Island =
        if island.Markets.IsEmpty then
            commodities
            |> Map.fold
                (fun isle commodity descriptor->
                    let market = 
                        {
                            Supply=random |> SupplyDemandGenerator
                            Demand=random |> SupplyDemandGenerator
                        }
                    {isle with
                        Markets = 
                            isle.Markets 
                            |> Map.add commodity market}
                    ) island
        else
            island

    let GenerateItems (islandItemSource:Location->Set<uint64>) (islandItemSink:Location->Set<uint64>->unit) (random:System.Random) (items:Map<uint64, ItemDescriptor>) (location:Location) : unit =
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

    let private ChangeMarketDemand (commodity:uint64) (value:float) (island:Island) : Island =
        let market = island.Markets.[commodity]
        let updatedMarket = {market with Demand = market.Demand + value}
        {island with Markets = island.Markets |> Map.add commodity updatedMarket}

    let private ChangeMarketSupply (commodity:uint64) (value:float) (island:Island) : Island =
        let market = island.Markets.[commodity]
        let updatedMarket = {market with Supply = market.Supply + value}
        {island with Markets = island.Markets |> Map.add commodity updatedMarket}

    let UpdateMarketForItemSale (commodities:Map<uint64,CommodityDescriptor>) (descriptor:ItemDescriptor) (quantity:uint32) (island:Island) : Island =
        descriptor.Commodities
        |> Map.map (fun k v -> v * (quantity |> float) * commodities.[k].SaleFactor)
        |> Map.fold (fun i k v -> i |> ChangeMarketDemand k v) island

    let UpdateMarketForItemPurchase (commodities:Map<uint64,CommodityDescriptor>) (descriptor:ItemDescriptor) (quantity:uint32) (island:Island) : Island =
        descriptor.Commodities
        |> Map.map (fun k v -> v * (quantity |> float) * commodities.[k].PurchaseFactor)
        |> Map.fold (fun i k v -> i |> ChangeMarketSupply k v) island
