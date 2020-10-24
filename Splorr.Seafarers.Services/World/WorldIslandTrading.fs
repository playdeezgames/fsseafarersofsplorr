﻿namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

type TradeQuantity =
    | Maximum
    | Specific of uint64

module WorldIslandTrading = 
    let private CannotBuyItems
            (context : CommonContext) = 
        AvatarMessages.Add context ["You cannot buy items here."]

    let private ItemNotForSale
            (context : CommonContext) =
        AvatarMessages.Add context ["Round these parts, we don't sell things like that."]

    let private InsufficientFunds
            (context : CommonContext) =
        AvatarMessages.Add context ["You don't have enough money."]

    let private InsufficientTonnage
            (context : CommonContext) =
        AvatarMessages.Add context ["You don't have enough tonnage."]

    let private ZeroQuantity
            (context : CommonContext) =
        AvatarMessages.Add context ["You don't have enough money to buy any of those."]

    let private CompletePurchase
            (context : CommonContext) 
            (item : uint64)
            (descriptor: ItemDescriptor)
            (quantity: uint64)
            (price:float)
            (location: Location)
            (avatarId: string)
            : unit =
        Island.UpdateMarketForItemSale 
            context
            descriptor 
            quantity 
            location
        avatarId
        |> AvatarMessages.Add context [ (quantity, descriptor.ItemName) ||> sprintf "You complete the purchase of %u %s." ]
        avatarId
        |> AvatarShipmates.SpendMoney 
            context
            price 
        avatarId
        |> AvatarInventory.AddInventory 
            context
            item 
            quantity

    let private BuyItemWhenIslandItemExists
            (context : CommonContext)
            (location : Location) 
            (tradeQuantity : TradeQuantity) 
            (item : uint64) 
            (descriptor : ItemDescriptor)
            (avatarId : string) 
            : unit = 
        let unitPrice = 
            IslandMarket.DetermineSalePrice 
                context
                item 
                location
        let unitTonnage = descriptor.Tonnage
        let unusedTonnage = AvatarInventory.GetUnusedTonnage context avatarId
        let quantity =
            match tradeQuantity with
            | Specific amount -> amount
            | Maximum -> min (floor(unusedTonnage / unitTonnage)) (floor((avatarId |> AvatarShipmates.GetMoney context) / unitPrice)) |> uint64
        let floatQuantity = quantity |> float
        let price = floatQuantity* unitPrice
        let tonnageNeeded = floatQuantity * unitTonnage
        if price > (avatarId |> AvatarShipmates.GetMoney context) then
            InsufficientFunds context avatarId
        elif tonnageNeeded > unusedTonnage then
            InsufficientTonnage context avatarId
        elif quantity = 0UL then
            ZeroQuantity context avatarId
        else
            CompletePurchase context item descriptor quantity price location avatarId


    let private BuyItemWhenIslandExists
            (context : CommonContext)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (avatarId                      : string) 
            : unit =
        match Item.FindItemByName context itemName with
        | Some (item, descriptor) ->
            BuyItemWhenIslandItemExists context location tradeQuantity item descriptor avatarId
        | None ->
            ItemNotForSale context avatarId


    let internal BuyItems 
            (context                       : CommonContext)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (avatarId                      : string) 
            : unit =
        if Island.Exists context location then
            BuyItemWhenIslandExists context location tradeQuantity itemName avatarId
        else
            CannotBuyItems context avatarId
            

    let internal SellItems 
            (context : CommonContext)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (avatarId                      : string) 
            : unit =
        match Item.FindItemByName context itemName, Island.GetList context |> List.tryFind ((=)location) with
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


