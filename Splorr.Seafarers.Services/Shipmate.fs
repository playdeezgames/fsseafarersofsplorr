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

module Shipmate =
    let SetStatistic 
            (identifier : ShipmateStatisticIdentifier) 
            (statistic  : Statistic option) 
            (mate       : Shipmate) 
            : Shipmate =
        match statistic with
        | Some stat ->
            {mate with Statistics = mate.Statistics |> Map.add identifier stat}
        | None -> 
            {mate with Statistics = mate.Statistics |> Map.remove identifier}

    let GetStatistic 
            (identifier : ShipmateStatisticIdentifier) 
            (mate       : Shipmate) 
            : Statistic option =
        mate.Statistics
        |> Map.tryFind identifier   

    let Create 
            (shipmateRationItemSink : ShipmateRationItemSink)
            (rationItems            : uint64 list) //TODO: becomes ShipmateRationItemTemplateSource
            (statisticDescriptors   : ShipmateStatisticTemplate list) //TODO: becomes ShipmateStatisticTemplateSource
            (avatarId               : string)
            (shipmateId             : ShipmateIdentifier)
            : Shipmate =
        shipmateRationItemSink avatarId shipmateId rationItems
        {
            Statistics = Map.empty
        }
        |> List.foldBack 
            (fun identifier shipMate -> 
                shipMate
                |> SetStatistic identifier.StatisticId (Statistic.Create (identifier.MinimumValue, identifier.MaximumValue) identifier.CurrentValue |> Some)) statisticDescriptors

    let GetStatus
            (shipmate : Shipmate)
            : ShipmateStatus=
        if shipmate.Statistics.[ShipmateStatisticIdentifier.Health].CurrentValue <= shipmate.Statistics.[ShipmateStatisticIdentifier.Health].MinimumValue then
            ZeroHealth |> Dead
        elif shipmate.Statistics.[ShipmateStatisticIdentifier.Turn].CurrentValue >= shipmate.Statistics.[ShipmateStatisticIdentifier.Turn].MaximumValue then
            OldAge |> Dead
        else
            Alive

    let TransformStatistic 
            (identifier : ShipmateStatisticIdentifier) 
            (transform  : Statistic -> Statistic option) 
            (mate       : Shipmate) 
            : Shipmate =
        mate
        |> GetStatistic identifier
        |> Option.fold
            (fun a s -> 
                a 
                |> SetStatistic identifier (s |> transform) ) mate

    let Eat 
            (shipmateRationItemSource : ShipmateRationItemSource)
            (inventory                : Map<uint64, uint32>) 
            (avatarId                 : string)
            (shipmateId               : ShipmateIdentifier)
            (mate                     : Shipmate) 
            : Shipmate * Map<uint64, uint32> * bool =
        let satietyDecrease = -1.0
        let satietyIncrease = 1.0
        let rationConsumptionRate = 1u
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
            let updatedMate = 
                mate
                |> TransformStatistic ShipmateStatisticIdentifier.Satiety (Statistic.ChangeCurrentBy satietyIncrease >> Some)
            let updatedInventory =
                inventory
                |> Map.add item (inventory.[item] - rationConsumptionRate)
                |> Map.filter (fun k v -> v > 0u)
            (updatedMate, updatedInventory, true)
        | _ ->
            if mate.Statistics.[ShipmateStatisticIdentifier.Satiety].CurrentValue > mate.Statistics.[ShipmateStatisticIdentifier.Satiety].MinimumValue then
                (mate
                |> TransformStatistic ShipmateStatisticIdentifier.Satiety (Statistic.ChangeCurrentBy (satietyDecrease) >> Some), inventory, false)
            else
                (mate
                |> TransformStatistic ShipmateStatisticIdentifier.Turn (Statistic.ChangeMaximumBy (satietyDecrease) >> Some), inventory, false)
