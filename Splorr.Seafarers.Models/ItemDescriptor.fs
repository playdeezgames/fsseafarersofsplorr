namespace Splorr.Seafarers.Models

type ItemDescriptor =
    {
        DisplayName : string
        Commodities : Map<uint64, float>
        Occurrence  : float
        Tonnage     : float
    }

