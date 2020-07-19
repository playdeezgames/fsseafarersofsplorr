module AtSeaTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures
open System.Data.SQLite
open Splorr.Seafarers.Persistence.Schema

let mutable private connection:SQLiteConnection = null
let mutable private functionUnderTest: CommandSource -> MessageSink -> string -> World -> Gamestate option = fun _ _ _ _ -> None

[<SetUp>]
let ``Set up function under test and connection there used.`` () =
    connection <- new SQLiteConnection("Data Source=:memory:;Version=3;New=True;")
    connection.Open()
    //TODO: this query is in two place, so consolidate!
    use command = new SQLiteCommand(Tables.Commodities,connection)
    command.ExecuteNonQuery() |> ignore
    //TODO: this query is in two place, so consolidate!
    use command = new SQLiteCommand(Tables.Items,connection)
    command.ExecuteNonQuery() |> ignore
    //TODO: this query is in two place, so consolidate!
    use command = new SQLiteCommand(Tables.CommodityItems,connection)
    command.ExecuteNonQuery() |> ignore
    functionUnderTest <- AtSea.Run random connection

[<TearDown>]
let ``Tear down connection used for function under test `` () =
    connection.Close()
    connection.Dispose()

[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let input = deadWorld   
    let inputSource(): Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let expected =
        input.Avatars.[avatarId].Messages
        |> Gamestate.GameOver
        |> Some
    let actual =
        input
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given invalid command.`` () =
    let input = world
    let inputSource = 
        None 
        |> toSource
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> functionUnderTest inputSource sinkStub avatarId
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
    let expectedMessages = ["You set your speed to 0.500000."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Speed=newSpeed
            Messages = expectedMessages}
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> functionUnderTest inputSource sinkStub avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea with new heading when given Set Heading command.`` () =
    let newHeading = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
    let input = world
    let inputSource = 
        newHeading 
        |> SetCommand.Heading 
        |> Command.Set 
        |> Some 
        |> toSource
    let expectedMessages = ["You set your heading to 1\u00b02'3.000000\"."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Heading = 
                newHeading 
                |> Dms.ToFloat
            Messages = expectedMessages}
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> functionUnderTest inputSource sinkStub avatarId
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
    let expectedAvatar =
        {input.Avatars.[avatarId] with 
            Position = (6.0,5.0)
            Messages = expectedMessages
            Vessel   = input.Avatars.[avatarId].Vessel |> Vessel.Befoul
            Metrics  = Map.empty |> Map.add Metric.Moved 1u}
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic StatisticIdentifier.Satiety (fun x -> {x with CurrentValue=99.0} |> Some)) 0u
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic StatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some)) 0u
    let expected = 
        {input with 
            Avatars  = input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given the Dock command and there is no near enough island.`` () =
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
        |> functionUnderTest inputSource sinkStub avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at Dock) when given the Dock command and there is a near enough island.`` () =
    let input = dockWorld
    let inputSource = Command.Dock |> Some |> toSource
    let expectedLocation = (0.0, 0.0)
    let expectedIsland = 
        input.Islands.[expectedLocation] 
        |> Island.AddVisit input.Avatars.[avatarId].Shipmates.[0].Statistics.[StatisticIdentifier.Turn].CurrentValue avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and changes heading when given a Head For command and the given island exists and is known.`` () =
    let input = headForWorldVisited
    let inputSource = "yermom" |> Command.HeadFor |> Some |> toSource
    let expectedMessages = ["You set your heading to 180\u00b00'0.000000\"."; "You head for `yermom`."]
    let expectedAvatar = {input.Avatars.[avatarId] with Heading = System.Math.PI; Messages=expectedMessages}
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual = 
        input
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and returns AtSea when the avatar is too far away from an island to careen.`` () =
    let inputAvatar = 
        {dockWorld.Avatars.[avatarId] with
            Position = (10.0, 10.0)}
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
        |> World.AddMessages avatarId ["You cannot careen here."]
        |> Gamestate.AtSea
        |> Some
    let actual =
        input
        |> functionUnderTest inputSource sinkStub avatarId
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
        |> functionUnderTest inputSource sinkStub avatarId
    Assert.AreEqual(expected, actual)
    
