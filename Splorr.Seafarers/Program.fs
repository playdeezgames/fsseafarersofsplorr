open Splorr.Seafarers
open Splorr.Seafarers.Services

[<EntryPoint>]
let main argv =
    {
        MinimumIslandDistance=10.0
        WorldSize=(100.0,100.0)
        MaximumGenerationTries=500u
        RewardRange=(1.0,10.0)
        Commodities = Map.empty
    }
    |> Runner.Run
    0
