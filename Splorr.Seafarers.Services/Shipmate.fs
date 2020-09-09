﻿namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type DemiseType =
    | ZeroHealth
    | OldAge

type ShipmateStatus =
    | Alive
    | Dead of DemiseType

type ShipmateRationItemSource = string -> ShipmateIdentifier -> uint64 list
type ShipmateRationItemSink = string -> ShipmateIdentifier -> uint64 list -> unit
type RationItemSource = unit -> uint64 list
type ShipmateStatisticTemplateSource = unit -> Map<ShipmateStatisticIdentifier, StatisticTemplate>
type ShipmateSingleStatisticSink = string -> ShipmateIdentifier -> (ShipmateStatisticIdentifier * Statistic option) -> unit
type ShipmateSingleStatisticSource = string -> ShipmateIdentifier -> ShipmateStatisticIdentifier -> Statistic option
type AvatarInventory = Map<uint64,uint64>

type ShipmateCreateContext =
    abstract member shipmateStatisticTemplateSource   : ShipmateStatisticTemplateSource
    abstract member shipmateSingleStatisticSink       : ShipmateSingleStatisticSink
    abstract member rationItemSource                  : RationItemSource
    abstract member shipmateRationItemSink            : ShipmateRationItemSink

type ShipmateGetStatusContext =
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource

type ShipmateTransformStatisticContext =
    abstract member shipmateSingleStatisticSink   : ShipmateSingleStatisticSink
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource


type ShipmateEatContext =
    inherit ShipmateTransformStatisticContext
    abstract member shipmateRationItemSource      : ShipmateRationItemSource
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource

module Shipmate =
    let Create
            (context    : ShipmateCreateContext)
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : unit =
        context.rationItemSource()
        |> context.shipmateRationItemSink avatarId shipmateId 
        context.shipmateStatisticTemplateSource()
        |> Map.iter
            (fun identifier statisticTemplate ->
                context.shipmateSingleStatisticSink avatarId shipmateId (identifier, statisticTemplate |> Statistic.CreateFromTemplate |> Some))

    let GetStatus
            (context    : ShipmateGetStatusContext)
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : ShipmateStatus=
        let health = context.shipmateSingleStatisticSource avatarId shipmateId ShipmateStatisticIdentifier.Health |> Option.get
        if health.CurrentValue <= health.MinimumValue then
            ZeroHealth |> Dead
        else
            let turn = context.shipmateSingleStatisticSource avatarId shipmateId ShipmateStatisticIdentifier.Turn |> Option.get
            if turn.CurrentValue >= turn.MaximumValue then
                OldAge |> Dead
            else
                Alive

    let TransformStatistic 
            (context : ShipmateTransformStatisticContext)
            (identifier : ShipmateStatisticIdentifier) 
            (transform  : Statistic -> Statistic option) 
            (avatarId: string)
            (shipmateId: ShipmateIdentifier) 
            : unit =
        identifier
        |> context.shipmateSingleStatisticSource avatarId shipmateId
        |> Option.iter
            (fun s -> 
                context.shipmateSingleStatisticSink avatarId shipmateId (identifier, (s |> transform) ) )

    let Eat 
            (context    : ShipmateEatContext)
            (inventory  : AvatarInventory) 
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : AvatarInventory * bool * bool =
        let satietyDecrease = -1.0
        let satietyIncrease = 1.0
        let rationConsumptionRate = 1UL
        let rationItems = context.shipmateRationItemSource avatarId shipmateId
        let rationItem =
            rationItems
            |> List.tryPick 
                (fun item -> 
                    match inventory |> Map.tryFind item with
                    | Some count when count >= rationConsumptionRate ->
                        item |> Some
                    | _ -> None)
        
        match rationItem with
        | Some item ->
            TransformStatistic 
                context
                ShipmateStatisticIdentifier.Satiety 
                (Statistic.ChangeCurrentBy satietyIncrease >> Some)
                avatarId
                shipmateId
            let updatedInventory =
                inventory
                |> Map.add item (inventory.[item] - rationConsumptionRate)
                |> Map.filter (fun k v -> v > 0UL)
            (updatedInventory, true, false)
        | _ ->
            let satiety = 
                context.shipmateSingleStatisticSource 
                    avatarId
                    shipmateId
                    ShipmateStatisticIdentifier.Satiety
                |> Option.get
            if satiety.CurrentValue > satiety.MinimumValue then
                TransformStatistic 
                    context
                    ShipmateStatisticIdentifier.Satiety 
                    (Statistic.ChangeCurrentBy (satietyDecrease) >> Some) 
                    avatarId
                    shipmateId
                (inventory, false, false)
            else
                TransformStatistic 
                    context
                    ShipmateStatisticIdentifier.Turn 
                    (Statistic.ChangeMaximumBy (satietyDecrease) >> Some)
                    avatarId
                    shipmateId
                (inventory, false, true)
