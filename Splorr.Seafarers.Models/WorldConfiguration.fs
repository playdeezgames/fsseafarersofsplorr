namespace Splorr.Seafarers.Models

type WorldConfiguration =
    {
        WorldSize: Location
        MinimumIslandDistance: float
        MaximumGenerationTries: uint32
        RewardRange: float * float
        RationItems: uint64 list
        StatisticDescriptors : AvatarStatisticTemplate list
    }
