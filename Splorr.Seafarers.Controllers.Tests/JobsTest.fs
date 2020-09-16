module JobsTest

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures

let private dockWorld = 
    Fixtures.Common.Dummy.AvatarId

let private dockLocation = (0.0, 0.0)

[<Test>]
let ``Run.It returns Docked with the given location and world.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let expected = 
        inputWorld
        |> Gamestate.InPlay 
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> Jobs.Run 
            islandJobSourceStub
            islandSingleNameSourceStub
            islandSourceStub
            sinkDummy
    Assert.AreEqual(expected, actual)

