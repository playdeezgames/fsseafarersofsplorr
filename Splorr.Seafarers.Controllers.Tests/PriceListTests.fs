﻿module PriceListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures

[<Test>]
let ``Run.It returns Docked (at Dock) gamestate.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let subjects = (Dock, subjectLocation, subjectWorld)
    let expected = subjects |> Gamestate.Docked |> Some
    let actual = 
        (subjectLocation, subjectWorld)
        ||> PriceList.Run sink
    Assert.AreEqual(expected, actual)