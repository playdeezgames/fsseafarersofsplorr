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
type Inventory = Map<uint64,uint64>

module Shipmate =
    type GetStatisticTemplatesContext =
        inherit ServiceContext
        abstract member shipmateStatisticTemplateSource   : ShipmateStatisticTemplateSource
    let private GetStatisticTemplates
            (context : ServiceContext)
            : Map<ShipmateStatisticIdentifier, StatisticTemplate> =
        (context :?> GetStatisticTemplatesContext).shipmateStatisticTemplateSource()

    type CreateContext =
        inherit ServiceContext
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
        GetStatisticTemplates context
        |> Map.iter
            (fun identifier statisticTemplate ->
                ShipmateStatistic.Put context avatarId shipmateId identifier (statisticTemplate |> Statistic.CreateFromTemplate |> Some))

    let GetStatus
            (context    : ServiceContext)
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : ShipmateStatus=
        let health = ShipmateStatistic.Get context avatarId shipmateId ShipmateStatisticIdentifier.Health |> Option.get
        if health.CurrentValue <= health.MinimumValue then
            ZeroHealth |> Dead
        else
            let turn = ShipmateStatistic.Get context avatarId shipmateId ShipmateStatisticIdentifier.Turn |> Option.get
            if turn.CurrentValue >= turn.MaximumValue then
                OldAge |> Dead
            else
                Alive

    type EatContext =
        inherit ServiceContext
        abstract member shipmateRationItemSource      : ShipmateRationItemSource
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
            ShipmateStatistic.Transform 
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
                ShipmateStatistic.Get context
                    avatarId
                    shipmateId
                    ShipmateStatisticIdentifier.Satiety
                |> Option.get
            if satiety.CurrentValue > satiety.MinimumValue then
                ShipmateStatistic.Transform 
                    context
                    ShipmateStatisticIdentifier.Satiety 
                    (Statistic.ChangeCurrentBy (satietyDecrease) >> Some) 
                    avatarId
                    shipmateId
                (inventory, false, false)
            else
                ShipmateStatistic.Transform 
                    context
                    ShipmateStatisticIdentifier.Turn 
                    (Statistic.ChangeMaximumBy (satietyDecrease) >> Some)
                    avatarId
                    shipmateId
                (inventory, false, true)
