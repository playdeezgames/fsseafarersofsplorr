namespace Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

module World =
    let Create() : World =
        {
            Messages = []
            Avatar = Avatar.Create()
        }

    let ClearMessages(world:World) : World =
        {world with Messages=[]}

    let AddMessages(messages: string list) (world:World) : World =
        {world with Messages = List.append world.Messages messages}

    let SetSpeed (speed:float) (world:World) : World = 
        let updatedAvatar =
            world.Avatar
            |> Avatar.SetSpeed speed
        let message = updatedAvatar.Speed |> sprintf "You set your speed to %f."
        {world with Avatar = updatedAvatar}
        |> AddMessages [ message ]

    let SetHeading (heading:Dms) (world:World) : World =
        {world with Avatar = world.Avatar |> Avatar.SetHeading heading}
        |> AddMessages [ heading |> Dms.ToString |> sprintf "You set your heading to %s." ]

    let Move(world:World) :World =
        {world with Avatar = world.Avatar |> Avatar.Move}
        |> AddMessages [ "Steady as she goes." ]
