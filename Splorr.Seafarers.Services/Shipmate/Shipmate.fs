namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

type DemiseType =
    | ZeroHealth
    | OldAge

type ShipmateStatus =
    | Alive
    | Dead of DemiseType

type Inventory = Map<uint64,uint64>

module Shipmate =
    type ShipmateRationItemSource = string -> ShipmateIdentifier -> uint64 list
    type ShipmateRationItemSink = string * ShipmateIdentifier * uint64 list -> unit
    type ShipmateStatisticTemplateSource = unit -> Map<ShipmateStatisticIdentifier, StatisticTemplate>
    
    type GetStatisticTemplatesContext =
        abstract member shipmateStatisticTemplateSource   : ShipmateStatisticTemplateSource ref
    let private GetStatisticTemplates
            (context : CommonContext)
            : Map<ShipmateStatisticIdentifier, StatisticTemplate> =
        (context :?> GetStatisticTemplatesContext).shipmateStatisticTemplateSource.Value()

    type GetGlobalRationItemsContext =
        abstract member rationItemSource                  : IslandMarket.RationItemSource ref
    let private GetGlobalRationItems
            (context : CommonContext)
            : uint64 list =
        (context :?> GetGlobalRationItemsContext).rationItemSource.Value()

    type SetRationItemsContext =
        abstract member shipmateRationItemSink            : ShipmateRationItemSink ref
    let private SetRationItems
            (context : CommonContext)
            (avatarId : string)
            (identifier : ShipmateIdentifier)
            (items : uint64 list)
            : unit =
        (context :?> SetRationItemsContext).shipmateRationItemSink.Value (avatarId, identifier, items)

    let internal Create
            (context    : CommonContext)
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : unit =
        GetGlobalRationItems context
        |> SetRationItems context avatarId shipmateId 
        GetStatisticTemplates context
        |> Map.iter
            (fun identifier statisticTemplate ->
                ShipmateStatistic.Put context avatarId shipmateId identifier (statisticTemplate |> Statistic.CreateFromTemplate |> Some))

    let internal GetStatus
            (context    : CommonContext)
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

    type GetRationItemsContext =
        abstract member shipmateRationItemSource      : ShipmateRationItemSource ref
    let internal GetRationItems
            (context : CommonContext)
            (avatarId : string)
            (identifier: ShipmateIdentifier)
            : uint64 list = 
        (context :?> GetRationItemsContext).shipmateRationItemSource.Value avatarId identifier

    let internal Eat 
            (context    : CommonContext)
            (inventory  : Inventory) 
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : Inventory * bool * bool =
        let satietyDecrease = -1.0
        let satietyIncrease = 1.0
        let rationConsumptionRate = 1UL
        let rationItem =
            GetRationItems context avatarId shipmateId
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
