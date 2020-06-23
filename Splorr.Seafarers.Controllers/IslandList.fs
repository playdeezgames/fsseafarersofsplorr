namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module IslandList =
    let private lesser (a:uint32) (b:uint32) = if a<b then a else b

    let private RunWorld (sink:MessageSink) (page:uint32) (pageSize:uint32) (world:World) : unit = 
        [
            ""
            "Known Islands:"
        ]
        |> List.iter sink
        let knownIslands =
            world.Islands
            |> Map.toList
            |> List.map snd
            |> List.filter(fun x -> x.VisitCount.IsSome)
            |> List.sortBy(fun x->x.Name)
        let totalItems = knownIslands |> List.length |> uint32
        let totalPages = (totalItems + (pageSize-1u)) / pageSize
        let skippedItems = page * pageSize
        ((page+1u), totalPages) ||> sprintf "Page %u of %u" |> sink
        if page < totalPages then
            knownIslands
            |> List.skip (skippedItems |> int)
            |> List.take ((lesser pageSize (totalItems-skippedItems)) |> int)
            |> List.iter (fun i -> i.Name |> sink)
        else
            "(end of list)" |> sink
    
    let private pageSize = 20u


    let Run (sink:MessageSink) (page:uint32) (gamestate: Gamestate) : Gamestate option =
        gamestate 
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld sink page pageSize)
        gamestate
        |> Some