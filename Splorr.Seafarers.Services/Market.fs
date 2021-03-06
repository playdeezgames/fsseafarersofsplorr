namespace Splorr.Seafarers.Services

open Splorr.Seafarers.Models

module Market =
    let DetermineSalePrice 
            (descriptor : CommodityDescriptor,
                market     : Market) 
            : float =
        descriptor.BasePrice * market.Demand / market.Supply

    let DeterminePurchasePrice 
            (descriptor : CommodityDescriptor,
                market     : Market) 
            : float =
        (DetermineSalePrice (descriptor, market)) * (1.0-descriptor.Discount)

    let ChangeDemand 
            (changeBy : float, 
                market   : Market) 
            : Market =
        {market with 
            Demand = market.Demand + changeBy}
        
    let ChangeSupply 
            (changeBy : float,
                market   : Market) 
            : Market =
        {market with 
            Supply = market.Supply + changeBy}
