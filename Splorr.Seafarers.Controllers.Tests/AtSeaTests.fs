module AtSeaTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures

let private functionUnderTest (vesselSingleStatisticSource:VesselSingleStatisticSource) = 
    AtSea.Run 
        termSources
        atSeaCommoditySource 
        atSeaItemSource 
        atSeaIslandMarketSource 
        atSeaIslandMarketSink 
        atSeaIslandItemSource 
        atSeaIslandItemSink 
        vesselSingleStatisticSource
        vesselSingleStatisticSinkStub
        random 
        (0.0, 0.0)

let private functionUsuallyUnderTest = functionUnderTest vesselSingleStatisticSourceStub

[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let input = deadWorld   
    let inputSource() : Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let expected =
        input.Avatars.[avatarId].Messages
        |> Gamestate.GameOver
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit command.`` () =
    let input = world
    let inputSource = 
        Command.Quit 
        |> Some 
        |> toSource
    let expected = 
        input
        |> Gamestate.AtSea 
        |> Gamestate.ConfirmQuit 
        |> Some
    let actual = 
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns InvalidInput when given invalid command.`` () =
    let input = world
    let inputSource = 
        None 
        |> toSource
    let expected = 
        ("Maybe try 'help'?",input 
        |> Gamestate.AtSea)
        |> Gamestate.ErrorMessage
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea with new speed when given Set Speed command.`` () =
    let newSpeed = 0.5
    let input = world
    let inputSource = 
        newSpeed 
        |> SetCommand.Speed 
        |> Command.Set 
        |> Some 
        |> toSource
    let expectedMessages = ["You set your speed to 1.00."]//note - the statistic sink does not actually track speed, so this value is "wrong" but the behavior is correct
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Messages = expectedMessages}
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea with new heading when given Set Heading command.`` () =
    let inputHeading = 1.5
    let input = world
    let inputSource = 
        inputHeading 
        |> SetCommand.Heading 
        |> Command.Set 
        |> Some 
        |> toSource
    let expectedMessages = 
        [
            "You set your heading to 0.00\u00b0." //note - because of stub function the actual heading is not stored, just testing that a message is added
        ]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Messages = expectedMessages}
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It moves the avatar when given Move command.`` () =
    let input = world
    let inputSource = 
        1u 
        |> Command.Move 
        |> Some 
        |> toSource
    let expectedMessages = ["Steady as she goes."]
    let expectedPosition = (6.0,5.0)
    let expectedAvatar =
        {input.Avatars.[avatarId] with 
            Messages = expectedMessages
            Metrics  = Map.empty |> Map.add Metric.Moved 1u}
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Satiety (fun x -> {x with CurrentValue=99.0} |> Some)) 0u
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some)) 0u
    let expected = 
        {input with 
            Avatars  = input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea Help when given the Help command.`` () =
    let input = world
    let inputSource = 
        Command.Help 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Gamestate.Help 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea Metrics when given the Metrics command.`` () =
    let input = world
    let inputSource = 
        Command.Metrics 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Gamestate.Metrics 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea Inventory when given the Inventory command.`` () =
    let input = world
    let inputSource = 
        Command.Inventory 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Gamestate.Inventory 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Main Menu when given the Menu command.`` () =
    let input = world
    let inputSource = 
        Command.Menu 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Some 
        |> Gamestate.MainMenu 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Island List when given the Islands command.`` () =
    let input = world
    let inputSource = 
        0u 
        |> Command.Islands 
        |> Some 
        |> toSource
    let expected = 
        (0u, input |> Gamestate.AtSea) 
        |> Gamestate.IslandList 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given the Dock command and there is no sufficiently close island.`` () =
    let input = emptyWorld
    let inputSource = 
        Command.Dock 
        |> Some 
        |> toSource
    let expectedMessages = ["There is no place to dock."]
    let expectedAvatar =
        {input.Avatars.[avatarId] with
            Messages=expectedMessages}
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
        |>Gamestate.AtSea
        |>Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at Dock) when given the Dock command and there is a near enough island.`` () =
    let input = dockWorld
    let inputSource = Command.Dock |> Some |> toSource
    let expectedLocation = (0.0, 0.0)
    let expectedIsland = 
        input.Islands.[expectedLocation] 
        |> Island.AddVisit input.Avatars.[avatarId].Shipmates.[0].Statistics.[ShipmateStatisticIdentifier.Turn].CurrentValue avatarId
    let expectedIslands = 
        input.Islands 
        |> Map.add expectedLocation expectedIsland
    let expectedMessages = ["You dock."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Messages = expectedMessages
            Metrics = input.Avatars.[avatarId].Metrics |> Map.add Metric.VisitedIsland 1u}
    let expectedWorld = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar
            Islands = expectedIslands }
    let expected = 
        (Dock, expectedLocation, expectedWorld)
        |>Gamestate.Docked
        |>Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given a Head For command and the given island does not exist.`` () =
    let input = headForWorldUnvisited
    let inputSource = 
        "foo" 
        |> Command.HeadFor 
        |> Some 
        |> toSource
    let expectedMessages = ["I don't know how to get to `foo`."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with Messages = expectedMessages}
    let expected = 
        {input with 
            Avatars= input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual = 
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given a Head For command and the given island exists but is not known.`` () =
    let input = headForWorldUnvisited
    let inputSource = 
        "yermom" 
        |> Command.HeadFor 
        |> Some 
        |> toSource
    let expectedMessages = ["I don't know how to get to `yermom`."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with Messages = expectedMessages}
    let expected = 
        {input with 
            Avatars=input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual = 
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and changes heading when given a Head For command and the given island exists and is known.`` () =
    let input = headForWorldVisited
    let inputSource = "yermom" |> Command.HeadFor |> Some |> toSource
    let expectedMessages = 
        [
            "You set your heading to 0.00\u00b0." //heading not actually set because of stub functions, so really just test that a message is added
            "You head for `yermom`."
        ]
    let expectedAvatar = {input.Avatars.[avatarId] with Messages=expectedMessages}
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual = 
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Chart when given the command Chart.`` () =
    let input = world
    let inputChartName = "chartname"
    let inputSource = 
        inputChartName
        |> Command.Chart 
        |> Some 
        |> toSource
    let expected = 
        (inputChartName, input)
        |> Gamestate.Chart 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let input = world
    let inputSource = 
        Command.Status 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Gamestate.Status 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given the command Abandon Job and the avatar has no current job.`` () =
    let input = dockWorld
    let inputSource = 
        Job 
        |> Command.Abandon 
        |> Some 
        |> toSource
    let expectedMessages = ["You have no job to abandon."]
    let expectedAvatar =
        {input.Avatars.[avatarId] with Messages = expectedMessages}
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and abandons the job when given the command Abandon Job and the avatar has a current job.`` () =
    let input = abandonJobWorld
    let inputSource = 
        Job 
        |> Command.Abandon 
        |> Some 
        |> toSource
    let expectedMessages = ["You abandon your job."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Job=None
            Reputation = input.Avatars.[avatarId].Reputation - 1.0
            Messages = expectedMessages
            Metrics = input.Avatars.[avatarId].Metrics |> Map.add Metric.AbandonedJob 1u}
    let expected = 
        {input with 
            Avatars= input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and returns AtSea when the avatar is too far away from an island to careen.`` () =
    let inputPosition = (10.0, 10.0)
    let inputAvatar = 
        dockWorld.Avatars.[avatarId]
    let input = 
        {dockWorld with
            Avatars = 
                dockWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputSource = 
        Port
        |> Command.Careen
        |> Some 
        |> toSource
    let expected =
        input
        |> World.AddMessages ["You cannot careen here."]
        |> Gamestate.AtSea
        |> Some
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PortFouling
        | VesselStatisticIdentifier.StarboardFouling ->
            None
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=inputPosition |> fst} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=inputPosition |> snd} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.DockDistance ->
            {MinimumValue=1.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let actual =
        input
        |> (functionUnderTest vesselSingleStatisticSource) inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Careen Port when given the careen port command and the avatar is sufficiently close to an island to careen.`` () =
    let input = dockWorld
    let inputSource = 
        Port
        |> Command.Careen
        |> Some 
        |> toSource
    let expected =
        (Port, input)
        |> Gamestate.Careened
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Run.It adds a message when given a Distance To command with an island name that does not exist.`` () =
    let input = dockWorld
    let inputName = "$$$$$$"
    let inputSource = 
        inputName
        |> Command.DistanceTo
        |> Some 
        |> toSource
    let expectedWorld =
        input
        |> World.TransformAvatar (Avatar.AddMessages [inputName |> sprintf "I don't know how to get to `%s`."] >> Some)
    let expected =
        expectedWorld
        |> Gamestate.AtSea
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given a Distance To command with an island name that does exist.`` () =
    let input = 
        dockWorld
        |> World.TransformIsland (0.0, 0.0) (Island.MakeKnown dockWorld.AvatarId >> Some)
    let inputIslandName = input.Islands.[(0.0, 0.0)].Name
    let inputSource = 
        inputIslandName
        |> Command.DistanceTo
        |> Some 
        |> toSource
    let expectedWorld =
        input
        |> World.TransformAvatar (Avatar.AddMessages [inputIslandName |> sprintf "Distance to `%s` is 0.000000."] >> Some)
    let expected =
        expectedWorld
        |> Gamestate.AtSea
        |> Some
    let actual =
        input
        |> functionUsuallyUnderTest inputSource sinkStub
    Assert.AreEqual(expected, actual)