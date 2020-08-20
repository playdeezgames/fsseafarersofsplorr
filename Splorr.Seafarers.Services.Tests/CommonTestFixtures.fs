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
let internal shipmateStatisticTemplateSource () =
    statisticDescriptors
    |> Map.ofList
let internal adverbSource()          : string list = [ "woefully" ]
let internal adjectiveSource()       : string list = [ "tatty" ]
let internal objectNameSource()      : string list = [ "thing" ]
let internal personNameSource()      : string list = [ "george" ]
let internal personAdjectiveSource() : string list = [ "ugly" ]
let internal professionSource()      : string list = [ "poopsmith" ]
let internal termSources = 
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