namespace Splorr.Seafarers.Models

type Shipmate =
    {
        Statistics   : Map<ShipmateStatisticIdentifier, Statistic>
        RationItems  : uint64 list
    }

