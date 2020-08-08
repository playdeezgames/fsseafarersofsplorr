namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TradeQuantity =
    | Maximum
    | Specific of uint32

type AvatarMessagePurger = string -> unit

module World =
    let private GenerateIslandName //TODO: move to world generator?
            (random:Random) 
            : string =
        let consonants = [| "h"; "k"; "l"; "m"; "p" |]
        let vowels = [| "a"; "e"; "i"; "o"; "u" |]
        let vowel = random.Next(2)>0
        let nameLength = random.Next(3) + random.Next(3) + random.Next(3) + 3
        [1..(nameLength)]
        |> List.map 
            (fun i -> i % 2 = (if vowel then 1 else 0))
        |> List.map
            (fun v -> 
                if v then
                    vowels.[random.Next(vowels.Length)]
                else
                    consonants.[random.Next(consonants.Length)])
        |> List.reduce (+)

    let rec private GenerateIslandNames  //TODO: move to world generator?
            (random:Random) 
            (nameCount:int) 
            (names: Set<string>) 
            : List<string> =
        if names.Count>=nameCount then
            names
            |> Set.toList
            |> Utility.SortListRandomly random
            |> List.take nameCount
        else
            names
            |> Set.add (GenerateIslandName random)
            |> GenerateIslandNames random nameCount

    let SetIsland 
            (location:Location) 
            (island:Island option) 
            (world:World) 
            : World =
        match island with 
        | Some i ->
            {world with Islands = world.Islands |> Map.add location i}
        | None ->
            {world with Islands = world.Islands |> Map.remove location}
            

    let TransformIsland 
            (location  : Location) 
            (transform : Island->Island option) 
            (world     : World) 
            : World =
        (world.Islands
        |> Map.tryFind location
        |> Option.bind transform
        |> SetIsland location) world

    let private NameIslands  //TODO: move to world generator?
            (nameSource : TermSource)
            (random     : Random) 
            (world      : World) 
            : World =
        GenerateIslandNames 
            random 
            (world.Islands.Count) 
            (nameSource() |> Set.ofList)
        |> Utility.SortListRandomly random
        |> List.zip (world.Islands |> Map.toList |> List.map fst)
        |> List.fold
            (fun w (l,n) -> w |> TransformIsland l (Island.SetName n >> Some)) world

    let rec private GenerateIslands  //TODO: move to world generator?
            (nameSource    : TermSource)
            (configuration : WorldConfiguration) 
            (random        : Random) 
            (currentTry    : uint32) 
            (world         : World) 
            : World =
        if currentTry>=configuration.MaximumGenerationTries then
            world
            |> NameIslands nameSource random
        else
            let candidateLocation = (random.NextDouble() * (configuration.WorldSize |> fst), random.NextDouble() * (configuration.WorldSize |> snd))
            if world.Islands |> Map.exists(fun k _ ->(Location.DistanceTo candidateLocation k) < configuration.MinimumIslandDistance) then
                GenerateIslands nameSource configuration random (currentTry+1u) world
            else
                GenerateIslands nameSource configuration random 0u {world with Islands = world.Islands |> Map.add candidateLocation (Island.Create())}

    let UpdateCharts 
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (world    : World) 
            : World =
        let viewDistance = 
            vesselSingleStatisticSource world.AvatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        world.Avatars
        |> Map.tryFind world.AvatarId
        |> Option.fold 
            (fun w avatar -> 
                let avatarPosition = Avatar.GetPosition vesselSingleStatisticSource w.AvatarId |> Option.get
                w.Islands
                |> Map.filter
                    (fun location island -> 
                        (island.AvatarVisits.ContainsKey world.AvatarId |> not) && ((avatarPosition |> Location.DistanceTo location)<=viewDistance))
                |> Map.fold
                    (fun a location _ ->
                        a
                        |> TransformIsland location (Island.MakeSeen world.AvatarId >> Some)) w) world

    let Create 
            (nameSource                    : TermSource)
            (vesselStatisticTemplateSource : unit -> Map<VesselStatisticIdentifier, VesselStatisticTemplate>)
            (vesselStatisticSink           : string -> Map<VesselStatisticIdentifier, Statistic> -> unit)
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (configuration                 : WorldConfiguration) 
            (random                        : Random) 
            (avatarId                      : string): World =
        {
            AvatarId = avatarId
            Avatars = 
                Map.empty
                |> Map.add 
                    avatarId 
                    (Avatar.Create 
                        vesselStatisticTemplateSource
                        vesselStatisticSink
                        avatarId
                        configuration.StatisticDescriptors 
                        configuration.RationItems)
            Islands = Map.empty
        }
        |> GenerateIslands nameSource configuration random 0u
        |> UpdateCharts vesselSingleStatisticSource

    let TransformAvatar 
            
            (transform : Avatar -> Avatar option) 
            (world     : World) : World =
        let avatarId = world.AvatarId
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.bind transform
        |> Option.fold
            (fun w avatar -> 
                {w with Avatars = w.Avatars|> Map.add avatarId avatar}) {world with Avatars = world.Avatars |> Map.remove avatarId}

    let ClearMessages 
            (avatarMessagePurger : AvatarMessagePurger) 
            (world               : World)
            : unit =
        avatarMessagePurger world.AvatarId

    let AddMessages
            (avatarMessageSink : AvatarMessageSink)
            (messages          : string list) 
            (world             : World) 
            : unit =
        Avatar.AddMessages avatarMessageSink messages world.AvatarId

    let SetSpeed 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarMessageSink           : AvatarMessageSink)
            (speed                       : float) 
            (world                       : World) 
            : unit = 
        world.AvatarId
        |> Avatar.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink speed 
        world.AvatarId
        |> Avatar.GetSpeed vesselSingleStatisticSource
        |> Option.iter
            (fun newSpeed ->
                world
                |> AddMessages avatarMessageSink [newSpeed |> sprintf "You set your speed to %.2f."])

    let SetHeading 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarMessageSink           : AvatarMessageSink)
            (heading                     : float) 
            (world                       : World) 
            : unit =
        world.AvatarId
        |> Avatar.SetHeading vesselSingleStatisticSource vesselSingleStatisticSink heading 
        world.AvatarId
        |> Avatar.GetHeading vesselSingleStatisticSource
        |> Option.iter
            (fun newHeading ->
                world
                |> AddMessages avatarMessageSink [newHeading |> Angle.ToDegrees |> Angle.ToString |> sprintf "You set your heading to %s." ])

    //a bool is not sufficient!
    //an avatar may be dead because of health
    //an avatar may be dead because of turns
    let IsAvatarAlive 
            (world    : World) 
            : bool =
        match world.Avatars |> Map.tryFind world.AvatarId with
        | Some avatar -> 
            match avatar with
            | Avatar.ALIVE -> true
            | Avatar.ZERO_HEALTH -> false
            | Avatar.OLD_AGE -> false
        | _ -> false
        

    let rec Move 
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink   : string->VesselStatisticIdentifier*Statistic->unit)
            (avatarMessageSink           : AvatarMessageSink)
            (distance                    : uint32) 
            (world                       : World) 
            : World =
        let avatarId = world.AvatarId
        match distance, world.Avatars |> Map.tryFind avatarId with
        | x, Some _ when x > 0u ->
            world
            |> AddMessages avatarMessageSink [ "Steady as she goes." ]
            let steppedWorld = 
                world
                |> TransformAvatar (Avatar.Move vesselSingleStatisticSource vesselSingleStatisticSink avatarId >> Some)
                |> UpdateCharts vesselSingleStatisticSource
            if IsAvatarAlive steppedWorld |> not then
                steppedWorld
                |> AddMessages avatarMessageSink [ "You die of old age!" ]
                steppedWorld
            else
                Move vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSink (x-1u) steppedWorld
        | _ -> 
            world

    let GetNearbyLocations
            (from                        : Location) 
            (maximumDistance             : float) 
            (world                       : World) 
            : Location list =
        world.Islands
        |> Map.toList
        |> List.map fst
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)


    let private DoJobCompletion 
            (avatarMessageSink : AvatarMessageSink)
            (location          : Location) 
            (job               : Job) 
            (world             : World) 
            : World = 
        if location = job.Destination then
            world
            |> AddMessages avatarMessageSink [ "You complete your job." ]
            world
            |> TransformAvatar (Avatar.CompleteJob >> Some)
        else
            world

    let Dock 
            (termSources        : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (commoditySource    : unit ->Map<uint64, CommodityDescriptor>) 
            (itemSource         : unit->Map<uint64, ItemDescriptor>) 
            (islandMarketSource : Location->Map<uint64, Market>) 
            (islandMarketSink   : Location->Map<uint64, Market>->unit) 
            (islandItemSource   : Location->Set<uint64>) 
            (islandItemSink     : Location->Set<uint64>->unit) 
            (avatarMessageSink  : AvatarMessageSink)
            (random             : Random) 
            (rewardRange        : float*float) 
            (location           : Location) 
            (world              : World) 
            : World =
        let avatarId = world.AvatarId
        match world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some island, Some avatar ->
            let destinations =
                world.Islands
                |> Map.toList
                |> List.map fst
                |> Set.ofList
                |> Set.remove location
            let updatedIsland = 
                island
                |> Island.AddVisit world.Avatars.[avatarId].Shipmates.[0].Statistics.[ShipmateStatisticIdentifier.Turn].CurrentValue avatarId//only when this counts as a new visit...
                |> Island.GenerateJobs termSources random rewardRange destinations 
            Island.GenerateCommodities commoditySource islandMarketSource islandMarketSink random location
            Island.GenerateItems islandItemSource islandItemSink random itemSource location
            let oldVisitCount =
                island.AvatarVisits
                |> Map.tryFind avatarId
                |> Option.bind (fun x -> x.VisitCount)
                |> Option.defaultValue 0u
            let newVisitCount =
                updatedIsland.AvatarVisits
                |> Map.tryFind avatarId
                |> Option.bind (fun x -> x.VisitCount)
                |> Option.defaultValue 0u
            world
            |> AddMessages (avatarMessageSink : AvatarMessageSink) [ "You dock." ]
            world
            |> TransformIsland location 
                (fun _ -> updatedIsland |> Some)
            |> Option.foldBack (DoJobCompletion avatarMessageSink location) avatar.Job
            |> TransformAvatar (Avatar.AddMetric Metric.VisitedIsland (if newVisitCount > oldVisitCount then 1u else 0u) >> Some)//...should this add to the metric
        | _, Some _ -> 
            world
            |> AddMessages (avatarMessageSink : AvatarMessageSink) [ "There is no place to dock there." ]
            world
        | _ ->
            world

    let DistanceTo 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarMessageSink           : AvatarMessageSink)
            (islandName                  : string) 
            (world                       : World) 
            : unit =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && (v.AvatarVisits.ContainsKey world.AvatarId) && v.AvatarVisits.[world.AvatarId].VisitCount.IsSome then
                        Some k
                    else
                        None)
        match location, Avatar.GetPosition vesselSingleStatisticSource world.AvatarId with
        | Some l, Some avatarPosition ->
            world
            |> AddMessages avatarMessageSink [ (islandName, Location.DistanceTo l avatarPosition ) ||> sprintf "Distance to `%s` is %f." ]
        | _, Some _ ->
            world
            |> AddMessages avatarMessageSink [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            ()

    let HeadFor
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarMessageSink           : AvatarMessageSink)
            (islandName                  : string) 
            (world                       : World) 
            : unit =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && (v.AvatarVisits.ContainsKey world.AvatarId) && v.AvatarVisits.[world.AvatarId].VisitCount.IsSome then
                        Some k
                    else
                        None)
        match location, Avatar.GetPosition vesselSingleStatisticSource world.AvatarId with
        | Some l, Some avatarPosition ->
            [
                AddMessages avatarMessageSink [ islandName |> sprintf "You head for `%s`." ]
                SetHeading vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSink (Location.HeadingTo avatarPosition l |> Angle.ToDegrees)
            ]
            |> List.iter (fun f -> f world)
        | _, Some _ ->
            world
            |> AddMessages avatarMessageSink [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            ()

    let AcceptJob 
            (avatarMessageSink : AvatarMessageSink)
            (jobIndex          : uint32) 
            (location          : Location) 
            (world             : World) 
            : World =
        let avatarId = world.AvatarId
        match jobIndex, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId, world.Avatars |> Map.tryFind avatarId |> Option.bind (fun a -> a.Job) with
        | 0u, _, _, _ ->
            world
            |> AddMessages avatarMessageSink [ "That job is currently unavailable." ]
            world
        | _, Some island, Some _,None ->
            match island |> Island.RemoveJob jobIndex with
            | isle, Some job ->
                world
                |> AddMessages avatarMessageSink [ "You accepted the job!" ]
                world
                |> SetIsland location (isle |> Some)
                |> TransformIsland job.Destination (Island.MakeKnown avatarId >> Some)
                |> TransformAvatar 
                    (Avatar.SetJob job 
                    >> Avatar.AddMetric Metric.AcceptedJob 1u
                    >> Some)
            | _ ->
                world
                |> AddMessages avatarMessageSink [ "That job is currently unavailable." ]
                world
        | _, Some island, Some _, Some job ->
            world
            |> AddMessages avatarMessageSink [ "You must complete or abandon your current job before taking on a new one." ]
            world
        | _ -> 
            world

    let AbandonJob
            (avatarMessageSink : AvatarMessageSink)
            (world             : World) 
            : World =
        let avatarId = world.AvatarId
        match world.Avatars |> Map.tryFind avatarId |> Option.bind (fun a -> a.Job) with
        | Some _ ->
            world
            |> AddMessages avatarMessageSink [ "You abandon your job." ]
            world
            |> TransformAvatar (Avatar.AbandonJob >> Some)
        | _ ->
            world
            |> AddMessages avatarMessageSink [ "You have no job to abandon." ]
            world
    
    //TODO: this function is in the wrong place!
    let private FindItemByName 
            (itemName : string) 
            (items    : Map<uint64, ItemDescriptor>) 
            : (uint64 * ItemDescriptor) option =
        items
        |> Map.tryPick (fun k v -> if v.ItemName = itemName then Some (k,v) else None)


    let BuyItems 
            (islandMarketSource          : IslandMarketSource)
            (islandSingleMarketSource    : IslandSingleMarketSource) 
            (islandSingleMarketSink      : Location->(uint64 * Market)->unit) 
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (avatarMessageSink           : AvatarMessageSink)
            (commoditySource             : CommoditySource) 
            (items                       : Map<uint64, ItemDescriptor>) 
            (location                    : Location) 
            (tradeQuantity               : TradeQuantity) 
            (itemName                    : string) 
            (world                       : World) 
            : World =
        let avatarId = world.AvatarId
        match items |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some (item, descriptor) , Some island, Some avatar->
            let markets =
                islandMarketSource location
            let unitPrice = 
                Item.DetermineSalePrice (commoditySource()) markets descriptor 
            let availableTonnage = vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Tonnage |> Option.map (fun x->x.CurrentValue) |> Option.get
            let usedTonnage =
                avatar
                |> Avatar.GetUsedTonnage items
            let quantity =
                match tradeQuantity with
                | Specific amount -> amount
                | Maximum -> min (floor(availableTonnage / descriptor.Tonnage)) (floor(avatar.Money / unitPrice)) |> uint32
            let price = (quantity |> float) * unitPrice
            let tonnageNeeded = (quantity |> float) * descriptor.Tonnage
            if price > avatar.Money then
                world
                |> AddMessages avatarMessageSink ["You don't have enough money."]
                world
            elif usedTonnage + tonnageNeeded > availableTonnage then
                world
                |> AddMessages avatarMessageSink ["You don't have enough tonnage."]
                world
            elif quantity = 0u then
                world
                |> AddMessages avatarMessageSink ["You don't have enough money to buy any of those."]
                world
            else
                Island.UpdateMarketForItemSale islandSingleMarketSource islandSingleMarketSink commoditySource descriptor quantity location
                world
                |> AddMessages avatarMessageSink [(quantity, descriptor.ItemName) ||> sprintf "You complete the purchase of %u %s."]
                world
                |> TransformAvatar (Avatar.SpendMoney price >> Avatar.AddInventory item quantity >> Some)//TODO: once this returns unit, this whole function returns unit
        | None, Some island, Some _ ->
            world
            |> AddMessages avatarMessageSink ["Round these parts, we don't sell things like that."]
            world
        | _ ->
            world
            |> AddMessages avatarMessageSink ["You cannot buy items here."]
            world

    let SellItems 
            (islandMarketSource       : Location -> Map<uint64, Market>) 
            (islandSingleMarketSource : Location -> uint64            -> Market option) 
            (islandSingleMarketSink   : Location -> (uint64 * Market) -> unit) 
            (avatarMessageSink        : AvatarMessageSink)
            (commoditySource          : CommoditySource) //TODO: this should move to top of parameter list
            (items                    : Map<uint64, ItemDescriptor>) //TODO: this should move to top and become source
            (location                 : Location) 
            (tradeQuantity            : TradeQuantity) 
            (itemName                 : string) 
            (world                    : World) 
            : World =
        let avatarId = world.AvatarId
        match items |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some (item, descriptor), Some _, Some avatar ->
            let quantity = 
                match tradeQuantity with
                | Specific q -> q
                | Maximum -> 
                    avatar 
                    |> Avatar.GetItemCount item
            if quantity > (avatar |> Avatar.GetItemCount item) then
                world
                |> AddMessages avatarMessageSink ["You don't have enough of those to sell."]
                world
            elif quantity = 0u then
                world
                |> AddMessages avatarMessageSink ["You don't have any of those to sell."]
                world
            else
                let markets = islandMarketSource location
                let unitPrice = 
                    Item.DeterminePurchasePrice (commoditySource()) markets descriptor 
                let price = (quantity |> float) * unitPrice
                Island.UpdateMarketForItemPurchase islandSingleMarketSource islandSingleMarketSink commoditySource descriptor quantity location
                world
                |> AddMessages avatarMessageSink [(quantity, descriptor.ItemName) ||> sprintf "You complete the sale of %u %s."]
                world
                |> TransformAvatar (Avatar.EarnMoney price >> Avatar.RemoveInventory item quantity >> Some)
        | None, Some island, Some _ ->
            world
            |> AddMessages avatarMessageSink ["Round these parts, we don't buy things like that."]
            world
        | _ ->
            world
            |> AddMessages avatarMessageSink ["You cannot sell items here."]
            world

    let CleanHull 
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink   : string->VesselStatisticIdentifier*Statistic->unit)
            (side     : Side) 
            (world    : World) 
            : World =
        let avatarId = world.AvatarId
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.fold
            (fun w _ -> 
                w
                |> TransformAvatar (Avatar.CleanHull vesselSingleStatisticSource vesselSingleStatisticSink avatarId side >> Some)) world
