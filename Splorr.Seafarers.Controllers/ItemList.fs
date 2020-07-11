namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module ItemList = 
    let private RunWithIsland (sink:MessageSink) (location:Location) (island:Island) (avatarId:string) (world: World) : Gamestate option =
        [
            "" |> Line
            (Heading, "Items for sale:" |> Line) |> Hued
        ]
        |> List.iter sink
        island.Items
        |> Set.iter (fun item -> 
            let descriptor = world.Items.[item]
            let sellPrice: float = descriptor |> Item.DetermineSalePrice world.Commodities island.Markets
            let buyPrice: float = descriptor |> Item.DeterminePurchasePrice world.Commodities island.Markets
            sprintf "\t%s Sells at:%f Bought for:%f" descriptor.DisplayName sellPrice buyPrice |> Line |> sink
            ())
        (Dock, location, world)
        |> Gamestate.Docked
        |> Some

    let Run (sink:MessageSink) =
        Docked.RunBoilerplate (RunWithIsland sink)
    

