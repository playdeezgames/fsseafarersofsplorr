namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

type AvatarInventorySource = string -> Inventory
type AvatarInventorySink = string -> Inventory -> unit

module AvatarInventory =
    type GetInventoryContext =
        inherit ServiceContext
        abstract member avatarInventorySource : AvatarInventorySource
    let GetInventory
            (context : ServiceContext)
            (avatarId : string)
            : Inventory =
        (context :?> GetInventoryContext).avatarInventorySource avatarId

    type GetUsedTonnageContext =
        inherit ServiceContext
        abstract member avatarInventorySource : AvatarInventorySource
    let GetUsedTonnage
            (context : ServiceContext)
            (items : Map<uint64, ItemDescriptor>) //TODO: to source
            (avatarId : string) 
            : float =
        let context = context :?> GetUsedTonnageContext
        (0.0, avatarId |> context.avatarInventorySource)
        ||> Map.fold
            (fun result item quantity -> 
                let d = items.[item]
                result + (quantity |> float) * d.Tonnage)

    type GetItemCountContext =
        inherit ServiceContext
        abstract member avatarInventorySource : AvatarInventorySource
    let GetItemCount 
            (context : ServiceContext)
            (item                  : uint64) 
            (avatarId              : string) 
            : uint64 =
        let context = context :?> GetItemCountContext
        match avatarId |> context.avatarInventorySource |> Map.tryFind item with
        | Some x -> x
        | None -> 0UL

    type AddInventoryContext =
        inherit ServiceContext
        abstract member avatarInventorySink   : AvatarInventorySink
        abstract member avatarInventorySource : AvatarInventorySource
    let AddInventory 
            (context : ServiceContext)
            (item : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
        let context = context :?> AddInventoryContext
        let newQuantity = (avatarId |> GetItemCount context item) + quantity
        avatarId
        |> context.avatarInventorySource
        |> Map.add item newQuantity
        |> context.avatarInventorySink avatarId

    type RemoveInventoryContext =
        inherit ServiceContext
        abstract member avatarInventorySource : AvatarInventorySource
        abstract member avatarInventorySink   : AvatarInventorySink
    let RemoveInventory 
            (context  : ServiceContext)
            (item     : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
        let context = context :?> RemoveInventoryContext
        let inventory = 
            avatarId 
            |>  context.avatarInventorySource
        match inventory.TryFind item with
        | Some count ->
            if count > quantity then
                inventory
                |> Map.add item (count-quantity)
            else
                inventory 
                |> Map.remove item
        | _ ->
            inventory
        |> context.avatarInventorySink avatarId

