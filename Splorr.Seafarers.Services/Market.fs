namespace Splorr.Seafarers.Services

open Splorr.Seafarers.Models
open System

module Market =
    let DetermineSalePrice (descriptor:CommodityDescriptor) (market:Market) : float =
        descriptor.BasePrice * market.Demand / market.Supply

    let DeterminePurchasePrice (descriptor:CommodityDescriptor) =
        DetermineSalePrice descriptor
        >> (*) (1.0-descriptor.Discount)
        
