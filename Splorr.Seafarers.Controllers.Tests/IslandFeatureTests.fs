module IslandFeatureTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures
open AtSeaTestFixtures
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestIslandFeatureRunContext
        (islandSingleFeatureSource,
        islandSingleNameSource) =
    interface IslandFeatureRunContext with
        member _.islandSingleNameSource : IslandSingleNameSource = islandSingleNameSource
        member _.islandSingleFeatureSource : IslandSingleFeatureSource = islandSingleFeatureSource

[<Test>]
let ``Run.It should return AtSea when the given island does not exist.`` () =
    let givenLocation = invalidLocation
    let givenAvatarId = avatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected=
        givenAvatarId 
        |> Gamestate.AtSea 
        |> Some
    let context = 
        TestIslandFeatureRunContext
            (islandSingleFeatureSourceStub,
            islandSingleNameSourceStub) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            commandSourceExplode
            sinkStub
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It should return Dock state when the given island exists but does not have the given feature.`` () =
    let givenLocation = noDarkAlleyIslandLocation
    let givenAvatarId = avatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected=
        (Dock, givenLocation, givenAvatarId) 
        |> Gamestate.Docked 
        |> Some
    let islandSingleNameSource (_) = Some ""
    let context = 
        TestIslandFeatureRunContext
            (islandSingleFeatureSourceStub,
            islandSingleNameSource) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            commandSourceExplode
            sinkStub
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.When in the dark alley, the leave command will take the player back to the dock.`` () =
    let givenLocation = darkAlleyIslandLocation
    let givenAvatarId = avatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let islandSingleNameSource (_) = Some ""
    let islandSingleFeatureSource (_) (_) = true
    let expected =
        (Dock, givenLocation, avatarId)
        |> Gamestate.Docked
        |> Some
    let context = 
        TestIslandFeatureRunContext
            (islandSingleFeatureSource,
            islandSingleNameSource) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Leave))
            sinkStub
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.When in the dark alley, the an invalid command gives you an error message.`` () =
    let givenLocation = darkAlleyIslandLocation
    let givenAvatarId = avatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let islandSingleNameSource (_) = Some ""
    let islandSingleFeatureSource (_) (_) = true
    let expected =
        ("Maybe try 'help'?",
            (Feature givenFeature, givenLocation, avatarId)
            |> Gamestate.Docked)
        |> Gamestate.ErrorMessage
        |> Some
    let context = 
        TestIslandFeatureRunContext
            (islandSingleFeatureSource,
            islandSingleNameSource) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake None)
            sinkStub
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    