namespace Splorr.Seafarers.Services

open Splorr.Seafarers.Models
open System

type ItemTable = Map<uint64, ItemDescriptor>
type ItemSource = unit -> ItemTable
type ItemSingleSource    = uint64 -> ItemDescriptor option

module Item =
    type GetListContext =
        inherit ServiceContext
        abstract member itemSource : ItemSource
    let GetList
            (context : ServiceContext)
            : ItemTable =
        (context :?> GetListContext).itemSource ()

    type GetContext =
        inherit ServiceContext
        abstract member itemSingleSource   : ItemSingleSource
    let Get
            (context : ServiceContext)
            (index : uint64)
            : ItemDescriptor option =
        (context :?> GetContext).itemSingleSource index


