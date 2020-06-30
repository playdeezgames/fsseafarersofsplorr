namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module ItemList = 
    let private RunWithIsland (sink:MessageSink) (location:Location) (island:Island) (world: World) : Gamestate option =
        [
            ""
            "Items for sale:"
        ]
        |> List.iter sink
        (Shop, location, world)
        |> Gamestate.Docked
        |> Some

    let Run (sink:MessageSink) =
        Docked.RunBoilerplate (RunWithIsland sink)
    

