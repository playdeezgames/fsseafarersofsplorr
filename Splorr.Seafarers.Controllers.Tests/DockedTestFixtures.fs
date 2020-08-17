module DockedTestFixtures

open CommonTestFixtures
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open AtSeaTestFixtures

let internal dockWorld = 
    World.Create
        termNameSource
        dockWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random 
        avatarId

let internal dockLocation : Location = (0.0, 0.0)

let internal deadDockWorld =
    {dockWorld with 
        Avatars = 
            Map.empty 
            |> Map.add 
                avatarId 
                (dockWorld.Avatars.[avatarId])}

let internal deadDockLocation = dockLocation

let internal commoditySource() : Map<uint64, CommodityDescriptor> = 
    [
        1UL, 
            {
                CommodityId    = 1UL
                CommodityName  = "commodity under test"
                BasePrice      = 1.0
                PurchaseFactor = 1.0
                SaleFactor     = 1.0
                Discount       = 0.5
            }
    ] 
    |> Map.ofList

let internal itemSource () : Map<uint64, ItemDescriptor> =
    [
        1UL, 
            {
                ItemId = 1UL
                ItemName="item under test"
                Commodities=
                    Map.empty 
                    |> Map.add 1UL 1.0
                Occurrence =1.0
                Tonnage=1.0
            }
    ] 
    |> Map.ofList

let internal smallWorldSingleStatisticSource (identifier: WorldStatisticIdentifier) : Statistic =
    match identifier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=500.0; MaximumValue=500.0; CurrentValue=500.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=5.0; MaximumValue=5.0; CurrentValue=5.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | WorldStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=0.5}
    | WorldStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=0.5}
    | _ ->
        raise (System.NotImplementedException (sprintf "smallWorldSingleStatisticSource - %s" (identifier.ToString())))

let internal smallWorld = 
    World.Create 
        termNameSource
        smallWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random 
        avatarId

let internal smallWorldIslandLocation = 
    smallWorld.Islands 
    |> Map.toList 
    |> List.map fst 
    |> List.head

let private smallWorldIslandItemSource (_) = 
    Set.empty 
    |> Set.add 1UL

let private smallWorldIslandItemSink (_) (_) = ()

let private smallWorldIslandMarketSource (_) = 
    [
        1UL, 
            {
                Supply=1.0
                Demand=1.0
            }
    ] 
    |> Map.ofList

let private smallWorldIslandMarketSink (_) (_) = ()

let internal smallWorldDocked = 
    smallWorld 
    |> World.Dock 
        termSources
        commoditySource 
        itemSource
        smallWorldSingleStatisticSource
        smallWorldIslandMarketSource 
        smallWorldIslandMarketSink 
        smallWorldIslandItemSource 
        smallWorldIslandItemSink 
        shipmateSingleStatisticSourceStub
        shipmateSingleStatisticSinkStub
        avatarMessageSinkStub
        random 
        smallWorldIslandLocation

let internal shopWorld = smallWorldDocked

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar (fun avatar -> {avatar with Job=Some { FlavorText="";Reward=0.0; Destination=(0.0,0.0)}} |> Some)

let internal dockedItemMarketSourceStub (_) = 
    Map.empty

let internal dockedItemSingleMarketSourceStub (_) (_) = 
    None

let internal dockedItemSingleMarketSinkStub (_) (_) = ()
