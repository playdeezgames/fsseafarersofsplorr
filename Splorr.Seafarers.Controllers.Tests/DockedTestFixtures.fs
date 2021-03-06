module DockedTestFixtures

open CommonTestFixtures
open Splorr.Seafarers.Models

let internal dockWorld = 
    Fixtures.Common.Dummy.AvatarId

let internal dockLocation : Location = (0.0, 0.0)

let internal deadDockWorld =
    dockWorld

let internal deadDockLocation = dockLocation

let internal commoditySource() : Map<uint64, CommodityDescriptor> = 
    [
        1UL, 
            {
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
    Fixtures.Common.Dummy.AvatarId

let internal smallWorldIslandLocation = 
    (0.0, 0.0)

let internal noDarkAlleyIslandLocation =
    (10.0, 20.0)

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

let internal shopWorld = smallWorldDocked

let internal abandonJobWorld =
    dockWorld
    

let internal dockedItemMarketSourceStub (_) = 
    Map.empty

let internal dockedItemSingleMarketSourceStub (_) (_) = 
    None

let internal dockedItemSingleMarketSinkStub (_) (_) = ()

let internal invalidLocation : Location = (-1.0, -2.0)
