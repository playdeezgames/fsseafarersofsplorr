module CommonTestFixtures

open Splorr.Seafarers.Models
open NUnit.Framework

let internal avatarId = ""
let internal statisticDescriptors =
    [
        ShipmateStatisticIdentifier.Satiety, {StatisticName="satiety"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        ShipmateStatisticIdentifier.Health, {StatisticName="health"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        ShipmateStatisticIdentifier.Turn, {StatisticName="turn"; MinimumValue=0.0; CurrentValue=0.0;MaximumValue=50000.0}
        ShipmateStatisticIdentifier.Money, {StatisticName="money"; MinimumValue=0.0; CurrentValue=0.0;MaximumValue=1000000000.0}
        ShipmateStatisticIdentifier.Reputation, {StatisticName="reputation"; MinimumValue=(-1000000000.0); CurrentValue=0.0;MaximumValue=1000000000.0}
    ]
let internal shipmateStatisticTemplateSourceStub () =
    statisticDescriptors
    |> Map.ofList
let internal adverbSource()          : string list = [ "woefully" ]
let internal adjectiveSource()       : string list = [ "tatty" ]
let internal objectNameSource()      : string list = [ "thing" ]
let internal personNameSource()      : string list = [ "george" ]
let internal personAdjectiveSource() : string list = [ "ugly" ]
let internal professionSource()      : string list = [ "poopsmith" ]
let internal termSourcesStub = 
    (adverbSource, adjectiveSource, objectNameSource, personNameSource, personAdjectiveSource, professionSource)
let internal nameSource() = []

let internal avatarMessageSinkStub (_) (_) = ()

let internal shipmateRationItemSinkStub (_) (_) (_) = ()
let internal shipmateRationItemSourceStub (_) (_) = [1UL]
let internal rationItemSourceStub () = [1UL]

let internal worldSingleStatisticSourceStub (identifier:WorldStatisticIdentifier) : Statistic =
    match identifier with 
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "worldSingleStatisticSourceStub")

let internal shipmateSingleStatisticSourceStub (_) (_) (_) = 
    None
let internal shipmateSingleStatisticSinkStub (_) (_) (_) =
    ()
let internal avatarShipmateSourceStub (_) =
    []
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
let internal avatarSingleMetricSourceStub (_) (_) = 0UL
let internal avatarJobSinkStub (_) (_) = ()
let internal avatarJobSourceStub (_) = None
let internal avatarIslandSingleMetricSinkStub (_) (_) (_) (_) = ()
let internal avatarIslandSingleMetricSourceStub (_) (_) (_) = None
let internal islandSingleNameSinkStub (_) (_) = ()

let internal islandSingleStatisticSinkStub (_) (_) = ()
let internal islandStatisticTemplateSourceStub () = Map.empty

let internal islandSourceStub() = []

let internal avatarMessagesSinkFake (messages:string list) (_) (message) =
    match messages |> List.tryFind (fun x->x=message) with
    | Some _ ->
        ()
    | None ->
        Assert.Fail(message |> sprintf "Received an invalid message - `%s`.")
let internal avatarExpectedMessageSink (expected:string) (_) (actual:string) =
    Assert.AreEqual(expected, actual)


let internal islandSingleMarketSinkStub (_) (_) = ()//BOOM

let internal islandItemSourceStub (_) = Set.empty
let internal islandItemSinkStub (_) (_) = ()
let internal islandMarketSourceStub (_) = Map.empty
let internal islandSingleMarketSourceStub (_) (_) = None
let internal islandMarketSinkStub (_) (_) = ()

let internal bogusAvatarId = "bogus"
let internal random = System.Random()
let internal soloIslandSingleStatisticSource (identfier: WorldStatisticIdentifier) : Statistic =
    match identfier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=30.0; MaximumValue=30.0; CurrentValue=30.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | WorldStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=10.0; CurrentValue=5.5}
    | WorldStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "soloIslandSingleStatisticSource")
let internal vesselStatisticTemplateSourceStub () = Map.empty
let internal vesselStatisticSinkStub (_) (_) = ()

let internal vesselSingleStatisticSourceStub (_) (identifier:VesselStatisticIdentifier) =
    match identifier with
    | VesselStatisticIdentifier.ViewDistance ->
        {MinimumValue=10.0; CurrentValue=10.0; MaximumValue=10.0} |> Some
    | VesselStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
    | VesselStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
    | _ -> None
let internal defaultRewardrange = (1.0,10.0)
let internal fabricatedDestinationList = [(0.0, 0.0)] |> Set.ofList

let internal commoditySource() = 
    Map.empty
    |> Map.add 
        1UL 
        {
            CommodityName=""
            BasePrice=1.0
            PurchaseFactor=1.0
            SaleFactor=1.0
            Discount=0.5
        }

let internal genericWorldItemSource () = 
    Map.empty
    |> Map.add 
        1UL 
        {
            ItemName="item under test"
            Commodities= Map.empty |> Map.add 1UL 1.0
            Occurrence=1.0
            Tonnage = 1.0
        }
