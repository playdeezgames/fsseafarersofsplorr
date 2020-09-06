namespace Splorr.Seafarers.Services
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

module Shipmate =
    let Create
            (shipmateStatisticTemplateSource   : ShipmateStatisticTemplateSource)
            (shipmateSingleStatisticSink       : ShipmateSingleStatisticSink)
            (rationItemSource                  : RationItemSource)
            (shipmateRationItemSink            : ShipmateRationItemSink)
            (avatarId                          : string)
            (shipmateId                        : ShipmateIdentifier)
            : unit =
        rationItemSource()
        |> shipmateRationItemSink avatarId shipmateId 
        shipmateStatisticTemplateSource()
        |> Map.iter
            (fun identifier statisticTemplate ->
                shipmateSingleStatisticSink avatarId shipmateId (identifier, statisticTemplate |> Statistic.CreateFromTemplate |> Some))

    let GetStatus
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (avatarId                      : string)
            (shipmateId                    : ShipmateIdentifier)
            : ShipmateStatus=
        let health = shipmateSingleStatisticSource avatarId shipmateId ShipmateStatisticIdentifier.Health |> Option.get
        if health.CurrentValue <= health.MinimumValue then
            ZeroHealth |> Dead
        else
            let turn = shipmateSingleStatisticSource avatarId shipmateId ShipmateStatisticIdentifier.Turn |> Option.get
            if turn.CurrentValue >= turn.MaximumValue then
                OldAge |> Dead
            else
                Alive

    let TransformStatistic 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (identifier : ShipmateStatisticIdentifier) 
            (transform  : Statistic -> Statistic option) 
            (avatarId: string)
            (shipmateId: ShipmateIdentifier) 
            : unit =
        identifier
        |> shipmateSingleStatisticSource avatarId shipmateId
        |> Option.iter
            (fun s -> 
                shipmateSingleStatisticSink avatarId shipmateId (identifier, (s |> transform) ) )

    let Eat 
            (shipmateRationItemSource      : ShipmateRationItemSource)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (inventory                     : AvatarInventory) 
            (avatarId                      : string)
            (shipmateId                    : ShipmateIdentifier)
            : AvatarInventory * bool * bool =
        let satietyDecrease = -1.0
        let satietyIncrease = 1.0
        let rationConsumptionRate = 1UL
        let rationItems = shipmateRationItemSource avatarId shipmateId
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
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
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
                shipmateSingleStatisticSource 
                    avatarId
                    shipmateId
                    ShipmateStatisticIdentifier.Satiety
                |> Option.get
            if satiety.CurrentValue > satiety.MinimumValue then
                TransformStatistic 
                    shipmateSingleStatisticSource
                    shipmateSingleStatisticSink
                    ShipmateStatisticIdentifier.Satiety 
                    (Statistic.ChangeCurrentBy (satietyDecrease) >> Some) 
                    avatarId
                    shipmateId
                (inventory, false, false)
            else
                TransformStatistic 
                    shipmateSingleStatisticSource
                    shipmateSingleStatisticSink
                    ShipmateStatisticIdentifier.Turn 
                    (Statistic.ChangeMaximumBy (satietyDecrease) >> Some)
                    avatarId
                    shipmateId
                (inventory, false, true)
