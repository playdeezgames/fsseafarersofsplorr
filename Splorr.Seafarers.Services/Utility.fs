namespace Splorr.Seafarers.Services

open System

type OperatingContext =
    interface
    end

module Utility =
    type SortListRandomlyContext =
        inherit OperatingContext
        abstract member random : Random
    let SortListRandomly 
            (context : OperatingContext) =
        let context = context :?> SortListRandomlyContext
        List.sortBy (fun _ -> context.random.Next())

    let PickRandomly
            (context : OperatingContext) =
        SortListRandomly context >> List.head
