module AvatarTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

let internal random = System.Random()
let internal rewardRange = (1.0,10.0)
let internal singleLocation = [(0.0, 0.0)] |> Set.ofList
let private vesselStatisticTemplateSourceStub () = Map.empty
let private vesselStatisticSinkStub (_) (_) = ()
let internal vesselSingleStatisticSource (_) (identifier) = 
    match identifier with
    | VesselStatisticIdentifier.FoulRate ->
        {MinimumValue = 0.001; CurrentValue=0.001; MaximumValue=0.001} |> Some
    | VesselStatisticIdentifier.Speed ->
        {MinimumValue = 0.0; CurrentValue=1.0; MaximumValue=1.0} |> Some
    | VesselStatisticIdentifier.Heading ->
        {MinimumValue = 0.0; CurrentValue=0.0; MaximumValue=6.3} |> Some
    | _ ->
        None
let internal vesselSingleStatisticSink (_) (_) = ()


let internal avatar =
    Avatar.Create vesselStatisticTemplateSourceStub vesselStatisticSinkStub avatarId statisticDescriptors [1UL] (0.0,0.0)
let internal avatarId = "avatar"
let internal avatarNoStats =
    {avatar with 
        Shipmates =
            avatar.Shipmates
            |> Array.map (fun x -> {x with Statistics = Map.empty})}
let internal deadAvatar =
    avatar
    |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Health (fun x-> {x with CurrentValue = x.MinimumValue} |> Some)) 0u
let internal oldAvatar =
    avatar
    |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Turn (fun x-> {x with CurrentValue = x.MaximumValue} |> Some)) 0u
let internal job =
    Job.Create termSources random rewardRange singleLocation
let internal employedAvatar =
    {avatar with Job = job |> Some; Money=10.0; Reputation=(-5.0)}
let internal rationedAvatar =
    {avatar with Inventory = Map.empty |> Map.add 1UL 1u}
let internal hoarderAvatar =
    {avatar with Inventory = Map.empty |> Map.add 1UL 100u}