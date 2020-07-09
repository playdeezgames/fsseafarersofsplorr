namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Island =
    let Create() : Island =
        {
            Name = ""
            AvatarVisits = Map.empty
            Jobs = []
            Markets = Map.empty
            Items = Set.empty
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

    let GenerateCommodities (random:System.Random) (commodities:Map<Commodity, CommodityDescriptor>) (island:Island) : Island =
        if island.Markets.IsEmpty then
            commodities
            |> Map.fold
                (fun isle commodity descriptor->
                    let market = 
                        {
                            Supply=random |> SupplyDemandGenerator
                            Demand=random |> SupplyDemandGenerator
                            Traded = (random.NextDouble())<descriptor.Occurrence
                        }
                    {isle with
                        Markets = 
                            isle.Markets 
                            |> Map.add commodity market}
                    ) island
        else
            island

    let GenerateItems (random:System.Random) (items:Map<Item, ItemDescriptor>) (island:Island) : Island =
        if island.Items.IsEmpty then
            items
            |> Map.fold 
                (fun a k v -> 
                    if random.NextDouble() < v.Occurrence then
                        {a with Items = a.Items |> Set.add k}
                    else
                        a) island
        else
            island

    let private ChangeMarketDemand (commodity:Commodity) (value:float) (island:Island) : Island =
        let market = island.Markets.[commodity]
        let updatedMarket = {market with Demand = market.Demand + value}
        {island with Markets = island.Markets |> Map.add commodity updatedMarket}

    let private ChangeMarketSupply (commodity:Commodity) (value:float) (island:Island) : Island =
        let market = island.Markets.[commodity]
        let updatedMarket = {market with Supply = market.Supply + value}
        {island with Markets = island.Markets |> Map.add commodity updatedMarket}

    let UpdateMarketForItemSale (commodities:Map<Commodity,CommodityDescriptor>) (descriptor:ItemDescriptor) (quantity:uint32) (island:Island) : Island =
        descriptor.Commodities
        |> Map.map (fun k v -> v * (quantity |> float) * commodities.[k].SaleFactor)
        |> Map.fold (fun i k v -> i |> ChangeMarketDemand k v) island

    let UpdateMarketForItemPurchase (commodities:Map<Commodity,CommodityDescriptor>) (descriptor:ItemDescriptor) (quantity:uint32) (island:Island) : Island =
        descriptor.Commodities
        |> Map.map (fun k v -> v * (quantity |> float) * commodities.[k].PurchaseFactor)
        |> Map.fold (fun i k v -> i |> ChangeMarketSupply k v) island
