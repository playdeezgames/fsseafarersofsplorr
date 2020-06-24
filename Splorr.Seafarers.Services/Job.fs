namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Job =
    let Create (random:System.Random) (rewardMinimum:float, rewardMaximum: float) (destinations:Set<Location>) : Job =
        {
            Destination = 
                destinations
                |> Set.toList
                |> List.sortBy (fun _ -> random.Next())
                |> List.head
            Reward = random.NextDouble() * (rewardMaximum - rewardMinimum) + rewardMinimum
        }
