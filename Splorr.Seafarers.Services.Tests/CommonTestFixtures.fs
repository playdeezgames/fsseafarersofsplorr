module CommonTestFixtures

open Splorr.Seafarers.Models

let internal avatarId = ""
let internal statisticDescriptors =
    [
        {StatisticId = AvatarStatisticIdentifier.Satiety; StatisticName="satiety"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        {StatisticId = AvatarStatisticIdentifier.Health; StatisticName="health"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        {StatisticId = AvatarStatisticIdentifier.Turn; StatisticName="turn"; MinimumValue=0.0; CurrentValue=0.0;MaximumValue=50000.0}
    ]
let internal adverbSource () = ["woefully"]
