namespace Splorr.Seafarers.Models

type Shipmate =
    {
        Statistics   : Map<AvatarStatisticIdentifier, Statistic>
        RationItems  : uint64 list
    }

