module CommandSourceTests

open NUnit.Framework
open Splorr.Seafarers
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

[<Test>]
let ``Parse.It returns Map with empty string command when given ["chart"]`` () =
    let actual =
        [ "chart" ]
        |> CommandSource.Parse
    Assert.AreEqual("" |> Command.Chart |> Some, actual)

[<Test>]
let ``Parse.It returns Map foo command when given ["chart";"foo"]`` () =
    let actual =
        [ "chart"; "foo" ]
        |> CommandSource.Parse
    Assert.AreEqual("foo" |> Command.Chart |> Some, actual)

[<Test>]
let ``Parse.It returns Map foo command when given ["map";"foo";"foo"]`` () =
    let actual =
        [ "chart"; "foo"; "foo" ]
        |> CommandSource.Parse
    Assert.AreEqual("foo foo" |> Command.Chart |> Some, actual)

[<Test>]
let ``Parse.It returns Quit command when given ["quit"]`` () =
    let actual =
        [ "quit" ]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Quit |> Some, actual)

[<Test>]
let ``Parse.It returns Metrics command when given ["metrics"]`` () =
    let actual =
        [ "metrics" ]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Metrics |> Some, actual)

[<Test>]
let ``Parse.It returns Help command when given ["help"]`` () =
    let actual =
        [ "help" ]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Help |> Some, actual)

[<Test>]
let ``Parse.It returns Yes command when given ["yes"]`` () =
    let actual =
        [ "yes" ]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Yes |> Some, actual)

[<Test>]
let ``Parse.It returns Yes command when given ["no"]`` () =
    let actual =
        [ "no" ]
        |> CommandSource.Parse
    Assert.AreEqual(Command.No |> Some, actual)

[<Test>]
let ``Parse.It returns Move 1u command when given ["move"]`` () =
    let actual =
        [ "move" ]
        |> CommandSource.Parse
    Assert.AreEqual(1u |> Command.Move |> Some, actual)

[<Test>]
let ``Parse.It returns Move 2u command when given ["move";"2"]`` () =
    let actual =
        [ "move"; "2" ]
        |> CommandSource.Parse
    Assert.AreEqual(2u |> Command.Move |> Some, actual)


[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1"]`` () =
    let actual =
        [ "set"; "heading"; "1"]
        |> CommandSource.Parse
    Assert.AreEqual(1.0 |> SetCommand.Heading |> Command.Set |> Some, actual)

[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1.5"]`` () =
    let actual =
        [ "set"; "heading"; "1.5"]
        |> CommandSource.Parse
    Assert.AreEqual(1.5 |> SetCommand.Heading |> Command.Set |> Some, actual)

[<Test>]
let ``Parse.It returns Set Speed 1 command when given ["set";"speed";"1"]`` () =
    let actual =
        [ "set"; "speed"; "1"]
        |> CommandSource.Parse
    Assert.AreEqual(1.0|>Speed|>Command.Set|>Some, actual)

[<Test>]
let ``Parse.It returns Start command when given ["start"]`` () =
    let actual =
        [ "start"]
        |> CommandSource.Parse
    match actual with
    | Some (Command.Start _) ->
        ()
    | _ ->
        Assert.Fail("A start command was not generated.")

[<Test>]
let ``Parse.It returns Items command when given ["items"]`` () =
    let actual =
        [ "items"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Items|>Some, actual)

[<Test>]
let ``Parse.It returns Menu command when given ["menu"]`` () =
    let actual =
        [ "menu"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Menu|>Some, actual)

[<Test>]
let ``Parse.It returns Abandon Game command when given ["abandon"; "game"]`` () =
    let actual =
        [ "abandon"; "game"]
        |> CommandSource.Parse
    Assert.AreEqual(Game |> Command.Abandon|>Some, actual)

[<Test>]
let ``Parse.It returns Abandon Job command when given ["abandon"; "job"]`` () =
    let actual =
        [ "abandon"; "job"]
        |> CommandSource.Parse
    Assert.AreEqual(Job |> Command.Abandon|>Some, actual)


[<Test>]
let ``Parse.It returns Resume command when given ["resume"]`` () =
    let actual =
        [ "resume"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Resume|>Some, actual)

[<Test>]
let ``Parse.It returns Dock command when given ["dock"]`` () =
    let actual =
        [ "dock"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Dock|>Some, actual)

[<Test>]
let ``Parse.It returns Undock command when given ["undock"]`` () =
    let actual =
        [ "undock"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Undock|>Some, actual)

[<Test>]
let ``Parse.It returns Status command when given ["status"]`` () =
    let actual =
        [ "status"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Status |> Some, actual)

[<Test>]
let ``Parse.It returns Jobs command when given ["jobs"]`` () =
    let actual =
        [ "jobs" ]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Jobs |> Some, actual)

[<Test>]
let ``Parse.It returns Islands 0 command when given ["islands"]`` () =
    let actual =
        [ "islands"]
        |> CommandSource.Parse
    Assert.AreEqual(0u |> Command.Islands |> Some, actual)

[<Test>]
let ``Parse.It returns Inventory command when given ["inventory"]`` () =
    let actual =
        [ "inventory"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Inventory |> Some, actual)

[<Test>]
let ``Parse.It returns Leave command when given ["leave"]`` () =
    let actual =
        [ "leave"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Leave |> Some, actual)

[<Test>]
let ``Parse.It returns NoBet command when given ["no";"bet"]`` () =
    let actual =
        [ "no"; "bet" ]
        |> CommandSource.Parse
    Assert.AreEqual(None |> Command.Bet |> Some, actual)

[<Test>]
let ``Parse.It returns Gamble command when given ["gamble"]`` () =
    let actual =
        [ "gamble"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Gamble |> Some, actual)

[<Test>]
let ``Parse.It returns Rules command when given ["rules"]`` () =
    let actual =
        [ "rules"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Rules |> Some, actual)

[<Test>]
let ``Parse.It returns Bet 1.0 command when given ["bet";"1"]`` () =
    let actual =
        [ "bet"; "1" ]
        |> CommandSource.Parse
    Assert.AreEqual(1.0 |> Some |> Command.Bet |> Some, actual)

[<Test>]
let ``Parse.It returns Gamble command when given ["deal"]`` () =
    let actual =
        [ "deal"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Gamble |> Some, actual)

[<Test>]
let ``Parse.It returns Islands 0 command when given ["islands";"1"]`` () =
    let actual =
        [ "islands";"1"]
        |> CommandSource.Parse
    Assert.AreEqual(0u |> Command.Islands |> Some, actual)

[<Test>]
let ``Parse.It returns Islands 1 command when given ["islands";"2"]`` () =
    let actual =
        [ "islands";"2"]
        |> CommandSource.Parse
    Assert.AreEqual(1u |> Command.Islands |> Some, actual)

[<Test>]
let ``Parse.It returns Head For Foo command when given ["head";"for";"foo"]`` () =
    let actual =
        [ "head";"for";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual("foo" |> Command.HeadFor |> Some, actual)

[<Test>]
let ``Parse.It returns Head For Foo command when given ["distance";"to";"foo"]`` () =
    let actual =
        [ "distance";"to";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual("foo" |> Command.DistanceTo |> Some, actual)
   
[<Test>]
let ``Parse.It returns Accept Job 1 command when given ["accept";"job";"1"]`` () =
    let actual =
        [ "accept";"job";"1"]
        |> CommandSource.Parse
    Assert.AreEqual(1u |> Command.AcceptJob |> Some, actual)

[<Test>]
let ``Parse.It returns Buy (Specific 3,"foo") command when given ["buy";"3";"foo"]`` () =
    let actual =
        [ "buy";"3";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual((3UL |> Specific, "foo") |> Command.Buy |> Some, actual)

[<Test>]
let ``Parse.It returns Buy (Specific 3,"foo bar") command when given ["buy";"3";"foo";"bar"]`` () =
    let actual =
        [ "buy";"3";"foo bar"]
        |> CommandSource.Parse
    Assert.AreEqual((3UL |> Specific, "foo bar") |> Command.Buy |> Some, actual)

[<Test>]
let ``Parse.It returns Buy (Maximum,"foo") command when given ["buy";"maximum";"foo"]`` () =
    let actual =
        [ "buy";"maximum";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual((Maximum, "foo") |> Command.Buy |> Some, actual)

[<Test>]
let ``Parse.It returns Buy (Maximum,"foo bar") command when given ["buy";"maximum";"foo";"bar"]`` () =
    let actual =
        [ "buy";"maximum";"foo bar"]
        |> CommandSource.Parse
    Assert.AreEqual((Maximum, "foo bar") |> Command.Buy |> Some, actual)

[<Test>]
let ``Parse.It returns Sell (Maximum,"foo") command when given ["sell";"all";"foo"]`` () =
    let actual =
        [ "sell";"all";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual((Maximum, "foo") |> Command.Sell |> Some, actual)

[<Test>]
let ``Parse.It returns Sell (Specific 3u,"foo") command when given ["sell";"3";"foo"]`` () =
    let actual =
        [ "sell";"3";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual((Specific 3UL, "foo") |> Command.Sell |> Some, actual)

[<Test>]
let ``Parse.It returns Sell (Specific 3,"foo bar") command when given ["sell";"3";"foo";"bar"]`` () =
    let actual =
        [ "sell";"3";"foo bar"]
        |> CommandSource.Parse
    Assert.AreEqual((Specific 3UL, "foo bar") |> Command.Sell |> Some, actual)

[<Test>]
let ``Parse.It returns Careen (Port) command when given ["careen";"to";"port"]`` () =
    let actual =
        [ "careen";"to";"port"]
        |> CommandSource.Parse
    Assert.AreEqual(Port |> Command.Careen |> Some, actual)

[<Test>]
let ``Parse.It returns Careen (Starboard) command when given ["careen";"to";"starboard"]`` () =
    let actual =
        [ "careen";"to";"starboard"]
        |> CommandSource.Parse
    Assert.AreEqual(Starboard |> Command.Careen |> Some, actual)

[<Test>]
let ``Parse.It returns Weigh Anchor command when given ["weigh";"anchor"]`` () =
    let actual =
        [ "weigh";"anchor"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.WeighAnchor |> Some, actual)

[<Test>]
let ``Parse.It returns Clean Hull command when given ["clean";"hull"]`` () =
    let actual =
        [ "clean";"hull"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.CleanHull |> Some, actual)

[<Test>]
let ``Parse.It returns Clean Hull command when given ["clean";"the";"hull"]`` () =
    let actual =
        [ "clean";"the";"hull"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.CleanHull |> Some, actual)

[<Test>]
let ``Parse.It returns GoTo DarkAlley command when given ["enter";"alley"]`` () =
    let actual =
        [ "enter";"alley" ]
        |> CommandSource.Parse
    Assert.AreEqual(IslandFeatureIdentifier.DarkAlley |> Command.GoTo |> Some, actual)
    
[<Test>]
let ``Parse.It returns GoTo DarkAlley command when given ["enter";"the";"alley"]`` () =
    let actual =
        [ "enter";"the";"alley" ]
        |> CommandSource.Parse
    Assert.AreEqual(IslandFeatureIdentifier.DarkAlley |> Command.GoTo |> Some, actual)

[<Test>]
let ``Parse.It returns GoTo DarkAlley command when given ["enter";"dark";"alley"]`` () =
    let actual =
        [ "enter";"dark";"alley" ]
        |> CommandSource.Parse
    Assert.AreEqual(IslandFeatureIdentifier.DarkAlley |> Command.GoTo |> Some, actual)

[<Test>]
let ``Parse.It returns GoTo DarkAlley command when given ["enter";"the";"dark";"alley"]`` () =
    let actual =
        [ "enter";"the";"dark";"alley" ]
        |> CommandSource.Parse
    Assert.AreEqual(IslandFeatureIdentifier.DarkAlley |> Command.GoTo |> Some, actual)

[<Test>]
let ``Parse.It returns Save None command when given ["save"]`` () =
    let actual =
        [ "save" ]
        |> CommandSource.Parse
    Assert.AreEqual(None |> Command.Save |> Some, actual)

[<Test>]
let ``Parse.It returns Save "foo" command when given ["save"; "foo"]`` () =
    let actual =
        [ "save"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual("foo" |> Some |> Command.Save |> Some, actual)

[<Test>]
let ``Parse.It returns Save "foo bar" command when given ["save"; "foo"; "bar"]`` () =
    let actual =
        [ "save"; "foo"; "bar"]
        |> CommandSource.Parse
    Assert.AreEqual("foo bar" |> Some |> Command.Save |> Some, actual)
