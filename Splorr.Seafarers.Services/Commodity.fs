namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module Commodity =
    type CommoditySource     = unit -> Map<uint64, CommodityDescriptor>
    
    type GetCommoditiesContext =
        abstract member commoditySource: CommoditySource ref
    let internal GetCommodities
            (context:CommonContext)
            : Map<uint64, CommodityDescriptor> =
        (context :?> GetCommoditiesContext).commoditySource.Value()
