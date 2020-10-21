namespace Splorr.Seafarers.Services

open Splorr.Seafarers.Models

module Market =
    let internal DetermineSalePrice 
            (descriptor : CommodityDescriptor,
                market     : Market) 
            : float =
        descriptor.BasePrice * market.Demand / market.Supply

    let internal DeterminePurchasePrice 
            (descriptor : CommodityDescriptor,
                market     : Market) 
            : float =
        (DetermineSalePrice (descriptor, market)) * (1.0-descriptor.Discount)

    let internal ChangeDemand 
            (changeBy : float, 
                market   : Market) 
            : Market =
        {market with 
            Demand = market.Demand + changeBy}
        
    let internal ChangeSupply 
            (changeBy : float,
                market   : Market) 
            : Market =
        {market with 
            Supply = market.Supply + changeBy}
