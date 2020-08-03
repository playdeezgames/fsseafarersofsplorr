module CommonTestFixtures

open Splorr.Seafarers.Models

let internal avatarId = ""
let internal statisticDescriptors =
    [
        {StatisticId = AvatarStatisticIdentifier.Satiety; StatisticName="satiety"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        {StatisticId = AvatarStatisticIdentifier.Health; StatisticName="health"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        {StatisticId = AvatarStatisticIdentifier.Turn; StatisticName="turn"; MinimumValue=0.0; CurrentValue=0.0;MaximumValue=50000.0}
    ]
let internal adverbSource()          : string list = [ "woefully" ]
let internal adjectiveSource()       : string list = [ "tatty" ]
let internal objectNameSource()      : string list = [ "thing" ]
let internal personNameSource()      : string list = [ "george" ]
let internal personAdjectiveSource() : string list = [ "ugly" ]
let internal professionSource()      : string list = [ "poopsmith" ]
let internal termSources = 
    (adverbSource, adjectiveSource, objectNameSource, personNameSource, personAdjectiveSource, professionSource)
let internal nameSource() = []