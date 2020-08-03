namespace Splorr.Seafarers.Models

type WorldConfiguration =
    {
        WorldSize              : Location
        AvatarDistances        : float * float
        MinimumIslandDistance  : float
        MaximumGenerationTries : uint32
        RewardRange            : float * float
        RationItems            : uint64 list
        StatisticDescriptors   : AvatarStatisticTemplate list
    }
