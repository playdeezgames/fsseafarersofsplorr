namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Jobs = 
    let Run  (sink:MessageSink) (location:Location, world: World) : Gamestate option =
        //TODO: check for island at location and show jobs
        (location, world)
        |> Gamestate.Docked
        |> Some



