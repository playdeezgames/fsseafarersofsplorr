namespace Splorr.Seafarers.Models

type StatisticIdentifier =
    | Satiety          = 1
    | Health           = 2
    | Turn             = 3
    | StarboardFouling = 4
    | PortFouling      = 5

type StatisticDescriptor =
    {
        StatisticId   : StatisticIdentifier
        StatisticName : string
        MinimumValue  : float
        MaximumValue  : float
        CurrentValue  : float
    }

type Statistic =
    {
        MinimumValue : float
        MaximumValue : float
        CurrentValue : float
    }

