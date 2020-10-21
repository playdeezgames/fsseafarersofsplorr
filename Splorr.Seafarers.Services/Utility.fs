namespace Splorr.Seafarers.Services

open System
open Splorr.Seafarers.DataStore
open Splorr.Common


module Utility =
    type TermListSource = string -> string list
    type TermSource = unit -> string list
    
    type RandomContext =
        abstract member random : Random ref
    let internal GenerateFromRange
            (context : CommonContext)
            (minimum : float, maximum: float)
            : float =
        (context :?> RandomContext).random.Value.NextDouble() * (maximum-minimum) + minimum

    let internal SortListRandomly 
            (context : CommonContext) =
        let context = context :?> RandomContext
        List.sortBy (fun _ -> context.random.Value.Next())

    let internal PickFromListRandomly
            (context : CommonContext) =
        SortListRandomly context >> List.head

    let internal GenerateSupplyOrDemand 
            (context : CommonContext)
            : float =
        let random = (context :?> RandomContext).random.Value
        (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + 3.0

    let internal GenerateFromWeightedValues
            (context : CommonContext) 
            (candidates : Map<'T, float>)
            : 'T option=
        let totalWeight =
            candidates
            |> Map.fold 
                (fun accumulator _ weight -> accumulator + weight) 0.0
        let generated = (context :?> RandomContext).random.Value.NextDouble() * totalWeight
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

    type GetTermsContext =
        abstract member termListSource : TermListSource ref
    let internal GetTermList
            (context : CommonContext)
            (termType : string)
            : string list =
        (context :?> GetTermsContext).termListSource.Value termType

    type TermGeneratorContext =
        abstract member termListSource : TermListSource
    let internal GenerateFromTermList
            (context : CommonContext)
            (termType: string)
            : string =
        termType
        |> GetTermList context
        |> PickFromListRandomly context
