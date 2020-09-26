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
            (random:Random) //TODO: contextify
            : float =
        (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + 3.0


