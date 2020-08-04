namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

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
            (rationItems          : uint64 list) 
            (statisticDescriptors : ShipmateStatisticTemplate list) 
            : Shipmate =
        {
            RationItems = rationItems
            Statistics = Map.empty
        }
        |> List.foldBack 
            (fun i a -> 
                a
                |> SetStatistic i.StatisticId (Statistic.Create (i.MinimumValue, i.MaximumValue) i.CurrentValue |> Some)) statisticDescriptors

    let TransformStatistic 
            (identifier:ShipmateStatisticIdentifier) 
            (transform:Statistic -> Statistic option) 
            (mate:Shipmate) 
            : Shipmate =
        mate
        |> GetStatistic identifier
        |> Option.fold
            (fun a s -> 
                a 
                |> SetStatistic identifier (s |> transform) ) mate

    let Eat 
            (inventory:Map<uint64, uint32>) 
            (mate:Shipmate) 
            : Shipmate * Map<uint64, uint32> * bool =
        let satietyDecrease = -1.0
        let satietyIncrease = 1.0
        let rationConsumptionRate = 1u
        let rationItem =
            mate.RationItems
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
