﻿module JobsTest

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
        interface Jobs.RunIslandContext with
            member this.islandJobSource: IslandJobSource = islandJobSource
            member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        interface Jobs.RunContext with
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
            islandSourceStub) :> ServiceContext
    let actual =
        (inputLocation, inputWorld)
        ||> Jobs.Run 
            context
            
            sinkDummy
    Assert.AreEqual(expected, actual)

