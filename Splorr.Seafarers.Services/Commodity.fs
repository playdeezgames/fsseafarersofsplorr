namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type CommoditySource     = unit -> Map<uint64, CommodityDescriptor>

module Commodity =
    type GetCommoditiesContext =
        inherit OperatingContext
        abstract member commoditySource: CommoditySource
    let GetCommodities
            (context:OperatingContext)
            : Map<uint64, CommodityDescriptor> =
        (context :?> GetCommoditiesContext).commoditySource()
