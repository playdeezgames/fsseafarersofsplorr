module JobsTest

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open Splorr.Seafarers.Services

let private dockWorld = 
    Fixtures.Common.Dummy.AvatarId

let private dockLocation = (0.0, 0.0)

type TestJobRunContext
            (islandJobSource,
            islandSingleNameSource,
            islandSource) =
        interface IslandName.GetNameContext with
            member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        interface IslandJob.GetContext with
            member this.islandJobSource: IslandJobSource = islandJobSource
        interface Island.GetListContext with
            member this.islandSource: IslandSource = islandSource

[<Test>]
let ``Run.It returns Docked with the given location and world.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let expected = 
        inputWorld
        |> Gamestate.InPlay 
        |> Some
    let context = 
        TestJobRunContext
            (islandJobSourceStub,
            islandSingleNameSourceStub,
            islandSourceStub) :> CommonContext
    let actual =
        (inputLocation, inputWorld)
        ||> Jobs.Run 
            context
            
            sinkDummy
    Assert.AreEqual(expected, actual)

