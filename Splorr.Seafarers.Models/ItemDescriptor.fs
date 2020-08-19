namespace Splorr.Seafarers.Models

type ItemDescriptor =
    {
        ItemName    : string
        Commodities : Map<uint64, float>
        Occurrence  : float
        Tonnage     : float
    }

