namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

type TradeQuantity =
    | Maximum
    | Specific of uint64

module WorldIslandTrading = 
    let private FindItemByName 
            (itemName : string) 
            (items    : Map<uint64, ItemDescriptor>) 
            : (uint64 * ItemDescriptor) option =
        items
        |> Map.tryPick
            (fun itemId descriptor ->
                if descriptor.ItemName = itemName then
                    Some (itemId,descriptor)
                else
                    None)

    let internal BuyItems 
            (context                       : CommonContext)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (avatarId                      : string) 
            : unit =
        let items = Item.GetList context
        match items 
            |> FindItemByName itemName, 
                Island.GetList context 
                |> List.tryFind (fun x-> x = location) with
        | Some (item, descriptor) , Some _ ->
            let unitPrice = 
                IslandMarket.DetermineSalePrice 
                    context
                    item 
                    location
            let availableTonnage = 
                Vessel.GetStatistic
                    context
                    avatarId 
                    VesselStatisticIdentifier.Tonnage 
                |> Option.map 
                    Statistic.GetCurrentValue 
                |> Option.get
            let usedTonnage =
                avatarId
                |> AvatarInventory.GetUsedTonnage
                    context
                    items
            let quantity =
                match tradeQuantity with
                | Specific amount -> amount
                | Maximum -> min (floor(availableTonnage / descriptor.Tonnage)) (floor((avatarId |> AvatarShipmates.GetMoney context) / unitPrice)) |> uint64
            let price = (quantity |> float) * unitPrice
            let tonnageNeeded = (quantity |> float) * descriptor.Tonnage
            if price > (avatarId |> AvatarShipmates.GetMoney context) then
                avatarId
                |> AvatarMessages.Add context ["You don't have enough money."]
            elif usedTonnage + tonnageNeeded > availableTonnage then
                avatarId
                |> AvatarMessages.Add context ["You don't have enough tonnage."]
            elif quantity = 0UL then
                avatarId
                |> AvatarMessages.Add context ["You don't have enough money to buy any of those."]
            else
                Island.UpdateMarketForItemSale 
                    context
                    descriptor 
                    quantity 
                    location
                avatarId
                |> AvatarMessages.Add context [(quantity, descriptor.ItemName) ||> sprintf "You complete the purchase of %u %s."]
                avatarId
                |> AvatarShipmates.SpendMoney 
                    context
                    price 
                avatarId
                |> AvatarInventory.AddInventory 
                    context
                    item 
                    quantity
        | None, Some _ ->
            avatarId
            |> AvatarMessages.Add context ["Round these parts, we don't sell things like that."]
        | _ ->
            avatarId
            |> AvatarMessages.Add context ["You cannot buy items here."]

    let internal SellItems 
            (context : CommonContext)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (avatarId                      : string) 
            : unit =
        let items = Item.GetList context
        match items |> FindItemByName itemName, Island.GetList context |> List.tryFind ((=)location) with
        | Some (item, descriptor), Some _ ->
            let quantity = 
                match tradeQuantity with
                | Specific q -> q
                | Maximum -> 
                    avatarId 
                    |> AvatarInventory.GetItemCount context item
            if quantity > (avatarId |> AvatarInventory.GetItemCount context item) then
                avatarId
                |> AvatarMessages.Add context ["You don't have enough of those to sell."]
            elif quantity = 0UL then
                avatarId
                |> AvatarMessages.Add context ["You don't have any of those to sell."]
            else
                let unitPrice = 
                    IslandMarket.DeterminePurchasePrice 
                        context
                        item 
                        location
                let price = (quantity |> float) * unitPrice
                Island.UpdateMarketForItemPurchase 
                    context
                    descriptor 
                    quantity 
                    location
                avatarId
                |> AvatarMessages.Add context [(quantity, descriptor.ItemName) ||> sprintf "You complete the sale of %u %s."]
                AvatarShipmates.EarnMoney 
                    context
                    price 
                    avatarId
                avatarId
                |> AvatarInventory.RemoveInventory 
                    context
                    item 
                    quantity 
        | None, Some _ ->
            avatarId
            |> AvatarMessages.Add context ["Round these parts, we don't buy things like that."]
        | _ ->
            avatarId
            |> AvatarMessages.Add context ["You cannot sell items here."]



