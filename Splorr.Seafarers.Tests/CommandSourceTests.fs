module CommandSourceTests

open NUnit.Framework
open Splorr.Seafarers
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

[<Test>]
let ``Parse.It returns Quit command when given ["quit"]`` () =
    let actual =
        [ "quit" ]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Quit |> Some, actual)

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
    Assert.AreEqual({Degrees=1;Minutes=0;Seconds=0.0} |> Heading |> Command.Set |> Some, actual)

[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1","2"]`` () =
    let actual =
        [ "set"; "heading"; "1"; "2"]
        |> CommandSource.Parse
    Assert.AreEqual({Degrees=1;Minutes=2;Seconds=0.0} |> Heading |> Command.Set |> Some, actual)

[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1","2","3"]`` () =
    let actual =
        [ "set"; "heading"; "1"; "2"; "3"]
        |> CommandSource.Parse
    Assert.AreEqual({Degrees=1;Minutes=2;Seconds=3.0} |> Heading |> Command.Set |> Some, actual)

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
    Assert.AreEqual(Command.Start|>Some, actual)

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
let ``Parse.It returns Prices command when given ["prices"]`` () =
    let actual =
        [ "prices"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Prices |> Some, actual)

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
let ``Parse.It returns Accept Job 1 command when given ["accept";"job";"1"]`` () =
    let actual =
        [ "accept";"job";"1"]
        |> CommandSource.Parse
    Assert.AreEqual(1u |> Command.AcceptJob |> Some, actual)
 