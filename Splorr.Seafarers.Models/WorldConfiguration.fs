namespace Splorr.Seafarers.Models

type WorldConfiguration =
    {
        WorldSize              : Location//TODO: this comes from 
        AvatarDistances        : float * float//TODO: remove this, because it comes from vessel stats
        MinimumIslandDistance  : float
        MaximumGenerationTries : uint32
        RewardRange            : float * float
        RationItems            : uint64 list//TODO: this becomes its own source
        StatisticDescriptors   : ShipmateStatisticTemplate list//TODO: this becomes its own source
    }
