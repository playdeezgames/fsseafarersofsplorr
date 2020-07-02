namespace Splorr.Seafarers.Models

type ItemDescriptor =
    {
        DisplayName: string
        Commodities: Map<Commodity, float>
        Occurrence: float
    }

