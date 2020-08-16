﻿module CommonTestFixtures

open Splorr.Seafarers.Models

let internal avatarId = ""
let internal statisticDescriptors =
    [
        {StatisticId = ShipmateStatisticIdentifier.Satiety; StatisticName="satiety"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        {StatisticId = ShipmateStatisticIdentifier.Health; StatisticName="health"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        {StatisticId = ShipmateStatisticIdentifier.Turn; StatisticName="turn"; MinimumValue=0.0; CurrentValue=0.0;MaximumValue=50000.0}
        {StatisticId = ShipmateStatisticIdentifier.Money; StatisticName="money"; MinimumValue=0.0; CurrentValue=0.0;MaximumValue=1000000000.0}
        {StatisticId = ShipmateStatisticIdentifier.Reputation; StatisticName="reputation"; MinimumValue=(-1000000000.0); CurrentValue=0.0;MaximumValue=1000000000.0}
    ]
let internal shipmateStatisticTemplateSource () =
    statisticDescriptors
    |> List.map (fun descriptor -> (descriptor.StatisticId, descriptor))
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
