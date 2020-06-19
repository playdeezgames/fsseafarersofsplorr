namespace Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

module World =
    let Create() : World =
        {
            Messages = []
            Avatar =
                {
                   X = 0.0
                   Y = 0.0
                   Heading = 0.0
                   Speed = 1.0
                }
        }

    let ClearMessages(world:World) : World =
        {world with Messages=[]}

    let AddMessages(messages: string list) (world:World) : World =
        {world with Messages = List.append world.Messages messages}

    let SetSpeed (speed:float) (world:World) : World = 
        raise (System.NotImplementedException "No Unit Tests")
