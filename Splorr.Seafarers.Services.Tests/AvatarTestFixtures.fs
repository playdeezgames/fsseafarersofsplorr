module AvatarTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

let internal random = System.Random()
let internal rewardRange = (1.0,10.0)
let internal singleLocation = [(0.0, 0.0)] |> Set.ofList
let internal avatar =
    Avatar.Create (10.0, 1.0) statisticDescriptors [1UL] (0.0,0.0)
let internal avatarNoStats =
    {avatar with 
        Shipmates =
            avatar.Shipmates
            |> Array.map (fun x -> {x with Statistics = Map.empty})}
let internal deadAvatar =
    avatar
    |> Avatar.TransformShipmate (Shipmate.TransformStatistic AvatarStatisticIdentifier.Health (fun x-> {x with CurrentValue = x.MinimumValue} |> Some)) 0u
let internal oldAvatar =
    avatar
    |> Avatar.TransformShipmate (Shipmate.TransformStatistic AvatarStatisticIdentifier.Turn (fun x-> {x with CurrentValue = x.MaximumValue} |> Some)) 0u
let internal job =
    Job.Create random rewardRange singleLocation
let internal employedAvatar =
    {avatar with Job = job |> Some; Money=10.0; Reputation=(-5.0)}
let internal rationedAvatar =
    {avatar with Inventory = Map.empty |> Map.add 1UL 1u}
let internal hoarderAvatar =
    {avatar with Inventory = Map.empty |> Map.add 1UL 100u}