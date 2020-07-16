namespace Splorr.Seafarers.Models

type WorldConfiguration =
    {
        WorldSize: Location
        MinimumIslandDistance: float
        MaximumGenerationTries: uint32
        RewardRange: float * float
    }

type World =
    {
        Avatars     : Map<string,Avatar>
        Islands     : Map<Location, Island>

        RewardRange : float * float
    }
    