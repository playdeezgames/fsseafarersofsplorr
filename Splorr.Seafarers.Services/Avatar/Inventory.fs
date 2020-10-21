namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models
open Splorr.Common

module AvatarInventory =
    type AvatarInventorySource = string -> Inventory
    type AvatarInventorySink = string -> Inventory -> unit

    type GetInventoryContext =
        abstract member avatarInventorySource : AvatarInventorySource ref
    let internal GetInventory
            (context : CommonContext)
            (avatarId : string)
            : Inventory =
        (context :?> GetInventoryContext).avatarInventorySource.Value avatarId

    let internal GetUsedTonnage
            (context : CommonContext)
            (items : Map<uint64, ItemDescriptor>) //TODO: to source
            (avatarId : string) 
            : float =
        (0.0, GetInventory context avatarId)
        ||> Map.fold
            (fun result item quantity -> 
                let d = items.[item]
                result + (quantity |> float) * d.Tonnage)

    let internal GetItemCount 
            (context : CommonContext)
            (item                  : uint64) 
            (avatarId              : string) 
            : uint64 =
        match avatarId |> GetInventory context |> Map.tryFind item with
        | Some x -> x
        | None -> 0UL

    type SetInventoryContext =
        abstract member avatarInventorySink   : AvatarInventorySink ref
    let internal SetInventory
            (context : CommonContext)
            (avatarId : string)
            (inventory : Inventory)
            : unit =
        (context :?> SetInventoryContext).avatarInventorySink.Value avatarId inventory
    let internal AddInventory 
            (context : CommonContext)
            (item : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
        let newQuantity = (avatarId |> GetItemCount context item) + quantity
        avatarId
        |> GetInventory context
        |> Map.add item newQuantity
        |> SetInventory context avatarId

    let internal RemoveInventory 
            (context  : CommonContext)
            (item     : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
        let inventory = 
            avatarId 
            |>  GetInventory context
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
        |> SetInventory context avatarId

