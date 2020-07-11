namespace Splorr.Seafarers.Models

type ItemDescriptor =
    {
        DisplayName: string
        Commodities: Map<uint, float>
        Occurrence: float
    }

