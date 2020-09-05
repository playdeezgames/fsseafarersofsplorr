namespace Splorr.Seafarers.Services

open System

type UtilitySortListRandomlyContext =
    abstract member random : Random

type UtilityPickRandomlyContext = 
    inherit UtilitySortListRandomlyContext

module Utility =
    let SortListRandomly 
            (context : UtilitySortListRandomlyContext) =
        List.sortBy (fun _ -> context.random.Next())
        
    let PickRandomly
            (context : UtilityPickRandomlyContext) =
        SortListRandomly context >> List.head
