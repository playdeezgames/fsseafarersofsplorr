namespace Splorr.Seafarers.Services

open System

type ServiceContext =
    interface
    end

module Utility =
    type RandomContext =
        inherit ServiceContext
        abstract member random : Random
    let SortListRandomly 
            (context : ServiceContext) =
        let context = context :?> RandomContext
        List.sortBy (fun _ -> context.random.Next())

    let PickRandomly
            (context : ServiceContext) =
        SortListRandomly context >> List.head

    let SupplyDemandGenerator 
            (context : ServiceContext)
            : float =
        let random = (context :?> RandomContext).random
        (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + 3.0

    let WeightedGenerator
            (context : ServiceContext) 
            (candidates : Map<'T, float>)
            : 'T option=
        let totalWeight =
            candidates
            |> Map.fold 
                (fun accumulator _ weight -> accumulator + weight) 0.0
        let generated = (context :?> RandomContext).random.NextDouble() * totalWeight
        candidates
        |> Map.fold
            (fun (result, weightLeft) item weight -> 
                match result with
                | Some _ ->
                    (result, weightLeft)
                | _ ->
                    let weightLeft = weightLeft - weight
                    if weightLeft <=0.0 then
                        (item |> Some, weightLeft)
                    else
                        (result, weightLeft)) (None, generated)
        |> fst

