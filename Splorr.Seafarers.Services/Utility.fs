namespace Splorr.Seafarers.Services

open System

module Utility =
    let SortListRandomly 
            (random : Random) =
        List.sortBy (fun _ -> random.Next())
        
    let PickRandomly
            (random : Random) =
        SortListRandomly random >> List.head
