namespace Splorr.Seafarers.Models

type Vessel =
    {
        Tonnage: float
        Fouling: Map<Side,Statistic>
        FoulRate: float
    }

