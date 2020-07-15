namespace Splorr.Seafarers.Models

type ItemDescriptor =
    {
        ItemId      : uint64
        ItemName    : string
        Commodities : Map<uint64, float>
        Occurrence  : float
        Tonnage     : float
    }

