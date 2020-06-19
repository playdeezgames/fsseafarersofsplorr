module Splorr.Seafarers.Views.Tests.AtSea

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Views

let world = World.Create()

[<Test>]
let ``Run function.When Quit command, return ConfirmQuit`` () =
    let actual = 
        world
        |> AtSea.Run (fun ()->Some Quit) (fun _->())
    Assert.AreEqual(world |> AtSea |> ConfirmQuit |> Some, actual)

[<Test>]
let ``Run function.When invalid command, return AtSea`` () =
    let actual =
        world
        |> AtSea.Run (fun()->None) (fun _->())
    Assert.AreEqual(world |> AtSea |> Some, actual)

[<Test>]
let ``Run function.When Set Speed command, return AtSea with new speed`` () =
    let newSpeed = 0.5
    let actual =
        world
        |> AtSea.Run (fun()->newSpeed |> SetCommand.Speed |> Command.Set |> Some) (fun _->())
    Assert.AreEqual({world with Avatar = { world.Avatar with Speed=newSpeed}; Messages=["You set your speed to 0.500000."] }|> AtSea |> Some, actual)
