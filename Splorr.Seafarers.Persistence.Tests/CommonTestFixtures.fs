module CommonTestFixtures

open Splorr.Seafarers.Models

let internal connectionString = "Data Source=:memory:;Version=3;New=True;"
let internal avatarId = ""
let internal world : World =
    {
        Avatars = 
            [avatarId, {
                Messages = []
                Position = (1.0, 2.0)
                Heading = 3.0
                Speed = 4.0
                ViewDistance = 5.0
                DockDistance = 6.0
                Money = 7.0
                Reputation= 8.0
                Job = None
                Inventory = Map.empty
                Satiety = {MinimumValue=9.0; CurrentValue=10.0; MaximumValue=11.0}
                Health = {MinimumValue=12.0; CurrentValue=13.0; MaximumValue=14.0}
                Turn = {MinimumValue=15.0;CurrentValue=16.0;MaximumValue=17.0}
            }] |> Map.ofList
        Islands = Map.empty
        RewardRange = (1.0, 10.0)
        Commodities= Map.empty
        Items = Map.empty
    }

