module IslandFeatureTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestIslandFeatureRunContext
        (avatarMessageSink,
        avatarMessageSource,
        islandSingleFeatureSource,
        islandSingleNameSource,
        islandSingleStatisticSource,
        shipmateSingleStatisticSource) =
    interface IslandFeatureRunContext with
        member _.islandSingleNameSource : IslandSingleNameSource = islandSingleNameSource
        member _.islandSingleFeatureSource : IslandSingleFeatureSource = islandSingleFeatureSource

    interface AvatarAddMessagesContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink

    interface IslandFeatureRunDarkAlleyContext with
        member _.avatarMessageSource : AvatarMessageSource = avatarMessageSource
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink
        member _.islandSingleStatisticSource   : IslandSingleStatisticSource = islandSingleStatisticSource
        member _.shipmateSingleStatisticSource : ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``Run.It should return AtSea when the given island does not exist.`` () =
    let givenLocation = invalidLocation
    let givenAvatarId = avatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected=
        givenAvatarId
        |> Gamestate.InPlay
        |> Some
    let context = 
        TestIslandFeatureRunContext
            (avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSourceStub,
            islandSingleNameSourceStub,
            islandSingleStatisticSourceStub,
            shipmateSingleStatisticSourceStub) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            commandSourceExplode
            sinkDummy
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
        givenAvatarId
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) = Some ""
    let context = 
        TestIslandFeatureRunContext
            (avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSourceStub,
            islandSingleNameSource,
            islandSingleStatisticSourceStub,
            shipmateSingleStatisticSourceStub) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            commandSourceExplode
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It should return Dock state when dark alley exists but the player does not have minimum gambling stakes.`` () =
    let givenLocation = noDarkAlleyIslandLocation
    let givenAvatarId = avatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected=
        givenAvatarId
        |> Gamestate.InPlay 
        |> Some
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleNameSource (_) = Some ""
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0, 5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (4.0, 4.0) 4.0 |> Some
    let context = 
        TestIslandFeatureRunContext
            (avatarMessageSinkExpected ["Come back when you've got more money!"],
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            commandSourceExplode
            sinkDummy
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
    let islandSingleStatisticSource (_) (_) = 
        Statistic.Create (5.0,5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = 
        Statistic.Create (5.0,5.0) 5.0 |> Some
    let expected =
        avatarId
        |> Gamestate.InPlay
        |> Some
    let context = 
        TestIslandFeatureRunContext
            (avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Leave))
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.When in the dark alley, the help command will take the player to the help state for the dark alley.`` () =
    let givenLocation = darkAlleyIslandLocation
    let givenAvatarId = avatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let islandSingleNameSource (_) = Some ""
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let expected =
        avatarId
        |> Gamestate.InPlay
        |> Gamestate.Help
        |> Some
    let context = 
        TestIslandFeatureRunContext
            (avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Help))
            sinkDummy
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
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let expected =
        ("Maybe try 'help'?",
            avatarId
            |> Gamestate.InPlay)
        |> Gamestate.ErrorMessage
        |> Some
    let context = 
        TestIslandFeatureRunContext
            (avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> IslandFeatureRunContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake None)
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    