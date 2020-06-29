namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module PriceList =
    let private RunWithIsland (sink:MessageSink) (island:Island) (world: World) : unit =
        [
            ""
            "Commodity Prices:"
        ]
        |> List.iter sink
        island.Markets
        |> Map.filter (fun _ v -> v.Traded)
        |> Map.iter
            (fun commodity market ->
                world.Commodities
                |> Map.tryFind commodity
                |> Option.iter
                    (fun descriptor ->
                        sprintf "\t%s Sells at:%f Bought for:%f" descriptor.Name (market |> Market.DetermineSalePrice descriptor) (market |> Market.DeterminePurchasePrice descriptor)
                        |> sink))

    let Run (sink:MessageSink) (location:Location) (world: World) : Gamestate option =
        world.Islands 
        |> Map.tryFind location
        |> Option.iter (fun island ->
            RunWithIsland sink island world)
        (location, world)
        |> Gamestate.Docked
        |> Some


