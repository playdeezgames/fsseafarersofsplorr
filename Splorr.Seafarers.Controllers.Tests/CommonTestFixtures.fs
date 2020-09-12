module CommonTestFixtures

open Splorr.Seafarers.Controllers
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System
open NUnit.Framework

let internal connectionString = 
    "Data Source=:memory:;Version=3;New=True;"

let internal random = 
    Random()

let internal sinkDummy 
        (_ : Message) 
        : unit = 
    ()

let internal toSource 
        (command:Command option) 
        : unit -> Command option= 
    fun () -> command

let internal createConnection () : SQLiteConnection =
    new SQLiteConnection(connectionString)

let internal avatarId : string = ""

let private statisticDescriptors =
    [
        (ShipmateStatisticIdentifier.Satiety,{
            StatisticName="satiety"
            MinimumValue=0.0
            CurrentValue=100.0
            MaximumValue=100.0
        })
        (ShipmateStatisticIdentifier.Health,{
            StatisticName="health"
            MinimumValue=0.0
            CurrentValue=100.0
            MaximumValue=100.0
        })
        (ShipmateStatisticIdentifier.Turn,{
            StatisticName="turn"
            MinimumValue=0.0
            CurrentValue=0.0
            MaximumValue=50000.0
        })
        (ShipmateStatisticIdentifier.Money,{
            StatisticName="money"
            MinimumValue=0.0
            CurrentValue=0.0
            MaximumValue=1000000000.0
        })
    ]
let internal shipmateStatisticTemplateSourceStub () =
    statisticDescriptors
    |> Map.ofList
let internal vesselStatisticTemplateSourceDummy () 
        : Map<VesselStatisticIdentifier, StatisticTemplate>= 
    Map.empty

let internal vesselStatisticSinkDummy (_) (_) = 
    ()

let internal vesselSingleStatisticSourceStub (_) (identifier: VesselStatisticIdentifier) = 
    match identifier with 
    | VesselStatisticIdentifier.FoulRate ->
        {MinimumValue=0.001; MaximumValue=0.001; CurrentValue=0.001} |> Some
    | VesselStatisticIdentifier.Tonnage ->
        {MinimumValue=100.0; MaximumValue=100.0; CurrentValue=100.0} |> Some
    | VesselStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0} |> Some
    | VesselStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0} |> Some
    | VesselStatisticIdentifier.ViewDistance ->
        {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
    | VesselStatisticIdentifier.DockDistance ->
        {MinimumValue=1.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
    | VesselStatisticIdentifier.Speed ->
        {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
    | VesselStatisticIdentifier.Heading ->
        {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
    | _ ->
        None

let internal vesselSingleStatisticSinkStub (_) (_) = ()

let internal avatarMessageSourceDummy (_) = []
let internal avatarMessageSinkStub (_) (_) = ()
let internal avatarMessagePurgerStub (_) = ()

let internal avatarMessageSinkExplode (_) (_) =
    raise (System.NotImplementedException "avatarMessageSinkExplode")

let internal avatarMessageSinkExpected (expected:string list) (_) (message:string) =
    match expected |> List.tryFind ((=) message) with
    | None ->
        Assert.Fail(sprintf "did not find '%s'" message)
    | _ ->
        ()


let avatarMessagesSinkFake (expected:string list) (_) (actual:string) : unit =
    match expected |> List.tryFind (fun x -> x = actual) with
    | Some _ ->
        ()
    | _ ->
        Assert.Fail (actual |> sprintf "Invalid Message Received - `%s`")


let internal adverbSource()          : string list = [ "woefully" ]
let internal adjectiveSource()       : string list = [ "tatty" ]
let internal objectNameSource()      : string list = [ "thing" ]
let internal personNameSource()      : string list = [ "george" ]
let internal personAdjectiveSource() : string list = [ "ugly" ]
let internal professionSource()      : string list = [ "poopsmith" ]
let internal termSources = 
    (adverbSource, adjectiveSource, objectNameSource, personNameSource, personAdjectiveSource, professionSource)

let internal termNameSource() = []

let internal shipmateRationItemSourceStub (_) (_) = [ 1UL ]
let internal shipmateRationItemSinkStub (_) (_) (_) = ()

let internal rationItemSourceStub () = [ 1UL ]

let internal worldSingleStatisticSourceStub (identifier: WorldStatisticIdentifier) : Statistic =
    match identifier with 
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "worldSingleStatisticSourceStub")

let internal shipmateSingleStatisticSinkStub (_) (_) (_) =
    ()

let internal shipmateSingleStatisticSourceStub (_) (_) (identifier:ShipmateStatisticIdentifier) =
    match identifier with
    | ShipmateStatisticIdentifier.Satiety
    | ShipmateStatisticIdentifier.Health ->
        Statistic.Create (0.0, 100.0) 100.0 |> Some
    | ShipmateStatisticIdentifier.Turn ->
        Statistic.Create (0.0, 50000.0) 0.0 |> Some
    | ShipmateStatisticIdentifier.Money ->
        Statistic.Create (0.0, 1000000.0) 0.0 |> Some
    | ShipmateStatisticIdentifier.Reputation ->
        Statistic.Create (-1000.0, 1000.0) 0.0 |> Some
    | _ -> 
        raise (System.NotImplementedException (identifier.ToString() |> sprintf "shipmateSingleStatisticSourceStub %s"))
        None
    

let internal avatarShipmateSourceStub (_) =
    [Primary]

let internal avatarInventorySourceStub (_) =
    Map.empty
let internal avatarInventorySinkStub (_) (_) =
    ()

let internal avatarSingleMetricSinkStub (_) (actual:Metric * uint64) = ()
let internal avatarSingleMetricSinkExplode (_) (actual:Metric * uint64) =
    raise (System.NotImplementedException (sprintf "avatarSingleMetricSinkExplode - %s %u" ((actual|>fst).ToString()) (actual |> snd)))
let internal assertAvatarSingleMetricSink (expected:(Metric * uint64) list) (_) (actual:Metric * uint64) =
    let found = 
        expected
        |> List.tryPick
            (fun e -> 
                if e = actual then
                    Some ()
                else
                    None)
    if found.IsNone then
        Assert.Fail(sprintf "assertAvatarSingleMetricSink %s %u" ((actual |> fst).ToString()) (actual |> snd))
let internal avatarSingleMetricSourceStub (_) (_) =
    0UL

let internal avatarMetricSourceStub (_) =
    (System.Enum.GetValues(typedefof<Metric>)) :?> (Metric array)
    |> Array.map (fun m -> (m, 1UL))
    |> Map.ofArray

let internal avatarJobSinkStub (_) (_) = ()
let internal avatarJobSourceStub (_) = None

let internal avatarIslandSingleMetricSinkStub (_) (_) (_) (_) = ()
let internal avatarIslandSingleMetricSourceStub (_) (_) (_) = None

let internal islandSingleNameSinkStub (_) (_) = ()
let internal islandSingleNameSourceStub (_) = None
let internal islandSingleFeatureSourceStub (_) (_) = false
let internal islandLocationByNameSourceStub (_) = None

let internal islandSingleStatisticSinkStub (_) (_) = ()
let internal islandStatisticTemplateSourceStub () = Map.empty
let internal islandSingleStatisticSourceStub (location: Location) (identifier : IslandStatisticIdentifier) = 
    match location, identifier with
    | _, IslandStatisticIdentifier.CareenDistance ->
        Statistic.Create (0.1, 0.1) 0.1
        |> Some
    | _ ->
        None

let islandJobSinkStub (_) (_) = ()

let islandJobSourceStub (_) = []

let islandSourceStub () = [(0.0, 0.0)]

let islandJobPurgerStub (_) (_) = ()

let islandSingleJobSourceStub (_) (_) = None

let islandFeatureSourceStub (_) = []

let internal avatarIslandFeatureSinkDummy (_) : unit =
    raise (System.NotImplementedException "avatarIslandFeatureSinkDummy")

type TestAtSeaRunContext 
        (
            avatarInventorySink: AvatarInventorySink, 
            avatarInventorySource: AvatarInventorySource,
            avatarIslandFeatureSink : AvatarIslandFeatureSink,
            avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource,
            avatarJobSink: AvatarJobSink,
            avatarJobSource: AvatarJobSource,
            avatarMessagePurger: AvatarMessagePurger,
            avatarMessageSink: AvatarMessageSink,
            avatarMessageSource: AvatarMessageSource,
            avatarShipmateSource: AvatarShipmateSource,
            avatarSingleMetricSink: AvatarSingleMetricSink,
            avatarSingleMetricSource: AvatarSingleMetricSource,
            commoditySource: CommoditySource,
            epochSecondsSource: EpochSecondsSource,
            islandItemSink: IslandItemSink,
            islandItemSource: IslandItemSource,
            islandJobSink: IslandJobSink,
            islandJobSource: IslandJobSource,
            islandLocationByNameSource: IslandLocationByNameSource,
            islandMarketSink: IslandMarketSink,
            islandMarketSource: IslandMarketSource,
            islandSingleNameSource: IslandSingleNameSource,
            islandSingleStatisticSource: IslandSingleStatisticSource,
            islandSource: IslandSource,
            itemSource: ItemSource,
            shipmateRationItemSource: ShipmateRationItemSource,
            shipmateSingleStatisticSink: ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource: ShipmateSingleStatisticSource,
            termSources: TermSources,
            vesselSingleStatisticSink: VesselSingleStatisticSink,
            vesselSingleStatisticSource: VesselSingleStatisticSource,
            worldSingleStatisticSource: WorldSingleStatisticSource
        ) =
    interface UtilitySortListRandomlyContext with
        member _.random : Random = random

    interface IslandGetDisplayNameContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.islandSingleNameSource         : IslandSingleNameSource = islandSingleNameSource

    interface IslandAddVisitContext with
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.epochSecondsSource : EpochSecondsSource = epochSecondsSource

    interface IslandGenerateCommoditiesContext with
        member _.commoditySource: CommoditySource = commoditySource
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
        member _.random : Random = random

    interface IslandGenerateItemsContext with
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.itemSource: ItemSource = itemSource
        member _.random: Random = random

    interface VesselTransformFoulingContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface VesselBefoulContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface ShipmateGetStatusContext with
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface ShipmateTransformStatisticContext with
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface ShipmateEatContext with
        member _.shipmateRationItemSource: ShipmateRationItemSource = shipmateRationItemSource
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface AvatarGetPositionContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarGetSpeedContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarGetHeadingContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarSetPositionContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarSetSpeedContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarSetHeadingContext with
        member _.vesselSingleStatisticSink : VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource : VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarAddMetricContext with
        member _.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member _.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource

    interface AvatarEatContext with
        member _.avatarInventorySink : AvatarInventorySink=avatarInventorySink
        member _.avatarInventorySource : AvatarInventorySource=avatarInventorySource
        member _.avatarShipmateSource : AvatarShipmateSource=avatarShipmateSource

    interface AvatarGetCurrentFoulingContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AvatarGetPrimaryStatisticContext with
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarAbandonJobContext with
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
    interface WorldClearMessagesContext with
        member this.avatarMessagePurger: AvatarMessagePurger = avatarMessagePurger
    interface AvatarMoveContext with
        member _.vesselSingleStatisticSource   : VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AtSeaHandleCommandContext with
        member _.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        member _.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
        member _.islandLocationByNameSource: IslandLocationByNameSource = islandLocationByNameSource
        member _.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        member _.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
        member _.shipmateRationItemSource: ShipmateRationItemSource = shipmateRationItemSource
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource   : VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AtSeaRunContext with
        member _.avatarMessageSource: AvatarMessageSource = avatarMessageSource
    interface WorldUpdateChartsContext with
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.islandSource: IslandSource = islandSource
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AvatarAddMessagesContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface WorldAddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface AvatarCompleteJobContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.avatarJobSource : AvatarJobSource = avatarJobSource

    interface WorldDockContext with
        member _.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.avatarJobSink: AvatarJobSink = avatarJobSink
        member _.avatarJobSource: AvatarJobSource = avatarJobSource
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        member _.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member _.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
        member _.commoditySource: CommoditySource = commoditySource
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.islandJobSink: IslandJobSink = islandJobSink
        member _.islandJobSource: IslandJobSource = islandJobSource
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
        member _.islandSource: IslandSource = islandSource
        member _.itemSource: ItemSource = itemSource
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
        member _.termSources: TermSources = termSources
        member _.worldSingleStatisticSource: WorldSingleStatisticSource = worldSingleStatisticSource
    interface AvatarTransformShipmatesContext with
        member _.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource

let commandSourceExplode () : Command option =
    raise (System.NotImplementedException "There should be no input handling here!")

let commandSourceFake (expectedCommand:Command option) : unit -> Command option =
    fun () -> expectedCommand