namespace Splorr.Seafarers.Models

type Side =
    | Port
    | Starboard

type Vessel =
    {
        Tonnage: float
        Fouling: Map<Side,Statistic>
        FoulRate: float
    }

