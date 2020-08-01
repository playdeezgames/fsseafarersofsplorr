namespace Splorr.Seafarers.Models

type StatisticTemplate<'T> =
    {
        StatisticId   : 'T
        StatisticName : string
        MinimumValue  : float
        MaximumValue  : float
        CurrentValue  : float
    }