namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type DemiseType =
    | ZeroHealth
    | OldAge

type ShipmateStatus =
    | Alive
    | Dead of DemiseType

type ShipmateRationItemSource = string -> ShipmateIdentifier -> uint64 list
type ShipmateRationItemSink = string -> ShipmateIdentifier -> uint64 list -> unit
type ShipmateStatisticTemplateSource = unit -> Map<ShipmateStatisticIdentifier, StatisticTemplate>
type ShipmateSingleStatisticSink = string -> ShipmateIdentifier -> (ShipmateStatisticIdentifier * Statistic option) -> unit
type ShipmateSingleStatisticSource = string -> ShipmateIdentifier -> ShipmateStatisticIdentifier -> Statistic option
type Inventory = Map<uint64,uint64>

module Shipmate =
    type CreateContext =
        inherit ServiceContext
        abstract member shipmateStatisticTemplateSource   : ShipmateStatisticTemplateSource
        abstract member shipmateSingleStatisticSink       : ShipmateSingleStatisticSink
        abstract member rationItemSource                  : RationItemSource
        abstract member shipmateRationItemSink            : ShipmateRationItemSink
    let Create
            (context    : ServiceContext)
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : unit =
        let context = context :?> CreateContext
        context.rationItemSource()
        |> context.shipmateRationItemSink avatarId shipmateId 
        context.shipmateStatisticTemplateSource()
        |> Map.iter
            (fun identifier statisticTemplate ->
                context.shipmateSingleStatisticSink avatarId shipmateId (identifier, statisticTemplate |> Statistic.CreateFromTemplate |> Some))

    type GetStatusContext =
        inherit ServiceContext
        abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    let GetStatus
            (context    : ServiceContext)
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : ShipmateStatus=
        let context = context :?> GetStatusContext
        let health = context.shipmateSingleStatisticSource avatarId shipmateId ShipmateStatisticIdentifier.Health |> Option.get
        if health.CurrentValue <= health.MinimumValue then
            ZeroHealth |> Dead
        else
            let turn = context.shipmateSingleStatisticSource avatarId shipmateId ShipmateStatisticIdentifier.Turn |> Option.get
            if turn.CurrentValue >= turn.MaximumValue then
                OldAge |> Dead
            else
                Alive

    type GetStatisticContext =
        inherit ServiceContext
        abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    let GetStatistic
            (context: ServiceContext)
            (avatarId : string)
            (shipmateId : ShipmateIdentifier)
            (identifier : ShipmateStatisticIdentifier)
            : Statistic option =
        (context :?> GetStatisticContext).shipmateSingleStatisticSource avatarId shipmateId identifier
    
    type TransformStatisticContext =
        inherit ServiceContext
        abstract member shipmateSingleStatisticSink   : ShipmateSingleStatisticSink
        abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    let TransformStatistic 
            (context    : ServiceContext)
            (identifier : ShipmateStatisticIdentifier) 
            (transform  : Statistic -> Statistic option) 
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier) 
            : unit =
        let context = context :?> TransformStatisticContext
        identifier
        |> context.shipmateSingleStatisticSource avatarId shipmateId
        |> Option.iter
            (fun s -> 
                context.shipmateSingleStatisticSink avatarId shipmateId (identifier, (s |> transform) ) )

    type EatContext =
        inherit ServiceContext
        abstract member shipmateRationItemSource      : ShipmateRationItemSource
        abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    let Eat 
            (context    : ServiceContext)
            (inventory  : Inventory) 
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : Inventory * bool * bool =
        let context = context :?> EatContext
        let satietyDecrease = -1.0
        let satietyIncrease = 1.0
        let rationConsumptionRate = 1UL
        let rationItem =
            context.shipmateRationItemSource avatarId shipmateId
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
                |> Map.filter (fun _ v -> v > 0UL)
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
