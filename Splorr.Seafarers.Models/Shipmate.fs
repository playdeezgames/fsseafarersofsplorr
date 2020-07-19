namespace Splorr.Seafarers.Models

type Shipmate =
    {
        Statistics   : Map<StatisticIdentifier, Statistic>
        RationItems  : uint64 list
    }

