module AtSeaTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Views

let world = World.Create()
let sink(_:string) : unit = ()

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit command.`` () =
    let actual = 
        world
        |> AtSea.Run (fun ()->Some Quit) sink
    Assert.AreEqual(world |> AtSea |> ConfirmQuit |> Some, actual)

[<Test>]
let ``Run.It returns AtSea when given invalid command.`` () =
    let actual =
        world
        |> AtSea.Run (fun()->None) sink
    Assert.AreEqual(world |> AtSea |> Some, actual)

[<Test>]
let ``Run.It returns AtSea with new speed when given Set Speed command.`` () =
    let newSpeed = 0.5
    let actual =
        world
        |> AtSea.Run (fun()->newSpeed |> SetCommand.Speed |> Command.Set |> Some) sink
    Assert.AreEqual({world with Avatar = { world.Avatar with Speed=newSpeed}; Messages=["You set your speed to 0.500000."] }|> AtSea |> Some, actual)

[<Test>]
let ``Run.It returns AtSea with new heading when given Set Heading command.`` () =
    let newHeading = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
    let actual =
        world
        |> AtSea.Run (fun()->newHeading |> SetCommand.Heading |> Command.Set |> Some) sink
    Assert.AreEqual({world with Avatar = { world.Avatar with Heading = newHeading |> Dms.ToFloat}; Messages=["You set your heading to 1\u00b02'3.000000\"."] }|> AtSea |> Some, actual)

[<Test>]
let ``Run.It moves the avatar when given Move command.`` () =
    let actual =
        world
        |> AtSea.Run (fun()->Move |> Some) sink
    Assert.AreEqual({world with Avatar = {world.Avatar with X=1.0}; Messages=["Steady as she goes."]; Turn=1u} |> AtSea |> Some, actual)

[<Test>]
let ``Run.It initiates At Sea Help when given the Help command.`` () =
    let actual =
        world
        |> AtSea.Run (fun()->Command.Help |> Some) sink
    Assert.AreEqual(world |> AtSea |> Help |> Some, actual)
