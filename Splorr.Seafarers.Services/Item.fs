namespace Splorr.Seafarers.Services

open Splorr.Seafarers.Models
open System
open Splorr.Common

module Item =
    type ItemTable = Map<uint64, ItemDescriptor>
    type ItemSource = unit -> ItemTable
    type ItemSingleSource    = uint64 -> ItemDescriptor option

    type GetListContext =
        abstract member itemSource : ItemSource ref
    let internal GetList
            (context : CommonContext)
            : ItemTable =
        (context :?> GetListContext).itemSource.Value ()

    type GetContext =
        abstract member itemSingleSource : ItemSingleSource ref
    let internal Get
            (context : CommonContext)
            (index : uint64)
            : ItemDescriptor option =
        (context :?> GetContext).itemSingleSource.Value index

    let internal FindItemByName 
            (context : CommonContext)
            (itemName : string) 
            : (uint64 * ItemDescriptor) option =
        GetList context
        |> Map.tryPick
            (fun itemId descriptor ->
                if descriptor.ItemName = itemName then
                    Some (itemId,descriptor)
                else
                    None)


