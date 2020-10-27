namespace Splorr.Seafarers.Services

open System
open Splorr.Seafarers.DataStore
open Splorr.Common


module Utility =
    type RandomContext =
        abstract member random : Random ref
    let internal GenerateFromRange
            (context : CommonContext)
            (minimum : float, maximum: float)
            : float =
        (context :?> RandomContext).random.Value.NextDouble() * (maximum-minimum) + minimum

    type TermListSource = string -> string list
    type GetTermsContext =
        abstract member termListSource : TermListSource ref
    let internal GetTermList
            (context : CommonContext)
            (termType : string)
            : string list =
        (context :?> GetTermsContext).termListSource.Value termType

    let internal SortListRandomly 
            (context : CommonContext) =
        List.sortBy (fun _ -> GenerateFromRange context (0.0, 1.0))

    let internal PickFromListRandomly
            (context : CommonContext) =
        SortListRandomly context >> List.head

    let internal GenerateSupplyOrDemand 
            (context : CommonContext)
            : float =
        [
            GenerateFromRange context (1.0, 6.0)
            GenerateFromRange context (1.0, 6.0)
            GenerateFromRange context (1.0, 6.0)
        ]
        |> List.reduce (+)

    let internal GenerateFromWeightedValues
            (context : CommonContext) 
            (candidates : Map<'T, float>)
            : 'T option=
        let totalWeight =
            candidates
            |> Map.toList
            |> List.map snd
            |> List.reduce (+)
        let generated = 
            GenerateFromRange context (0.0, totalWeight)
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

    let internal GenerateFromTermList
            (context : CommonContext)
            (termType: string)
            : string =
        termType
        |> GetTermList context
        |> PickFromListRandomly context
