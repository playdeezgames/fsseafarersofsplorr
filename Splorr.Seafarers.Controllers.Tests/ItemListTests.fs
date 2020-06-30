module ItemListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures

[<Test>]
let ``Run.It returns Docked (at Shop) gamestate.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let subjects = (Shop, subjectLocation, subjectWorld)
    let expected = subjects |> Gamestate.Docked |> Some
    let actual = 
        (subjectLocation, subjectWorld)
        ||> ItemList.Run sink
    Assert.AreEqual(expected, actual)