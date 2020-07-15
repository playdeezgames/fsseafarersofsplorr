﻿module ItemListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures

[<Test>]
let ``Run.It returns Docked (at Shop) gamestate.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let expected = 
        (Dock, inputLocation, inputWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual = 
        (inputLocation, avatarId, inputWorld)
        |||> ItemList.Run commodities smallWorldItems sinkStub
    Assert.AreEqual(expected, actual)