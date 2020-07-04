module AvatarTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let internal random = System.Random()
let internal rewardRange = (1.0,10.0)
let internal singleLocation = [(0.0, 0.0)] |> Set.ofList
let internal avatar =
    Avatar.Create(0.0,0.0)
let internal job =
    Job.Create random rewardRange singleLocation
let internal employedAvatar =
    {avatar with Job = job |> Some; Money=10.0; Reputation=(-5.0)}
let internal rationedAvatar =
    {avatar with Inventory = Map.empty |> Map.add Ration 1u}
let internal hoarderAvatar =
    {avatar with Inventory = Map.empty |> Map.add Ration 100u}