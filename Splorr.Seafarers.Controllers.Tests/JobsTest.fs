module JobsTest

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures
open AtSeaTestFixtures

let private dockWorld = 
    avatarId

let private dockLocation = (0.0, 0.0)

[<Test>]
let ``Run.It returns Docked with the given location and world.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let expected = 
        (Dock, inputLocation, inputWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> Jobs.Run 
            islandJobSourceStub
            islandSingleNameSourceStub
            islandSourceStub
            sinkStub
    Assert.AreEqual(expected, actual)

