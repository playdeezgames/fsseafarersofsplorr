namespace Splorr.Seafarers.Services

open Splorr.Seafarers.Models
open System

type ItemTable = Map<uint64, ItemDescriptor>
type ItemSource = unit -> ItemTable

module Item =
    type GetListContext =
        inherit ServiceContext
        abstract member itemSource : ItemSource
    let GetList
            (context : ServiceContext)
            : ItemTable =
        (context :?> GetListContext).itemSource ()



