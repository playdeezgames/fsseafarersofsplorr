namespace Splorr.Seafarers.Models

type AvatarStatisticTemplate =
    {
        StatisticId   : AvatarStatisticIdentifier
        StatisticName : string
        MinimumValue  : float
        MaximumValue  : float
        CurrentValue  : float
    }
