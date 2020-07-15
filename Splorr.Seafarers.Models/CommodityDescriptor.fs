namespace Splorr.Seafarers.Models

type CommodityDescriptor =
    {
        CommodityId    : uint64
        CommodityName  : string
        BasePrice      : float
        PurchaseFactor : float
        SaleFactor     : float
        Discount       : float
    }

