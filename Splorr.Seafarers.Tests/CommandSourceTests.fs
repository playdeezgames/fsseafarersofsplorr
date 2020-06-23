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
    Assert.AreEqual(Quit |> Some, actual)

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
    Assert.AreEqual(Yes |> Some, actual)

[<Test>]
let ``Parse.It returns Yes command when given ["no"]`` () =
    let actual =
        [ "no" ]
        |> CommandSource.Parse
    Assert.AreEqual(No |> Some, actual)

[<Test>]
let ``Parse.It returns Move command when given ["move"]`` () =
    let actual =
        [ "move" ]
        |> CommandSource.Parse
    Assert.AreEqual(Move |> Some, actual)

[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1"]`` () =
    let actual =
        [ "set"; "heading"; "1"]
        |> CommandSource.Parse
    Assert.AreEqual({Degrees=1;Minutes=0;Seconds=0.0} |> Heading |> Set |> Some, actual)

[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1","2"]`` () =
    let actual =
        [ "set"; "heading"; "1"; "2"]
        |> CommandSource.Parse
    Assert.AreEqual({Degrees=1;Minutes=2;Seconds=0.0} |> Heading |> Set |> Some, actual)

[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1","2","3"]`` () =
    let actual =
        [ "set"; "heading"; "1"; "2"; "3"]
        |> CommandSource.Parse
    Assert.AreEqual({Degrees=1;Minutes=2;Seconds=3.0} |> Heading |> Set |> Some, actual)

[<Test>]
let ``Parse.It returns Set Speed 1 command when given ["set";"speed";"1"]`` () =
    let actual =
        [ "set"; "speed"; "1"]
        |> CommandSource.Parse
    Assert.AreEqual(1.0|>Speed|>Set|>Some, actual)

[<Test>]
let ``Parse.It returns Start command when given ["start"]`` () =
    let actual =
        [ "start"]
        |> CommandSource.Parse
    Assert.AreEqual(Start|>Some, actual)

[<Test>]
let ``Parse.It returns Menu command when given ["menu"]`` () =
    let actual =
        [ "menu"]
        |> CommandSource.Parse
    Assert.AreEqual(Menu|>Some, actual)

[<Test>]
let ``Parse.It returns Abandon command when given ["abandon"]`` () =
    let actual =
        [ "abandon"]
        |> CommandSource.Parse
    Assert.AreEqual(Abandon|>Some, actual)

[<Test>]
let ``Parse.It returns Resume command when given ["resume"]`` () =
    let actual =
        [ "resume"]
        |> CommandSource.Parse
    Assert.AreEqual(Resume|>Some, actual)

[<Test>]
let ``Parse.It returns Dock command when given ["dock"]`` () =
    let actual =
        [ "dock"]
        |> CommandSource.Parse
    Assert.AreEqual(Dock|>Some, actual)

[<Test>]
let ``Parse.It returns Undock command when given ["undock"]`` () =
    let actual =
        [ "undock"]
        |> CommandSource.Parse
    Assert.AreEqual(Undock|>Some, actual)

[<Test>]
let ``Parse.It returns Status command when given ["status"]`` () =
    let actual =
        [ "status"]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Status|>Some, actual)

[<Test>]
let ``Parse.It returns Islands 0 command when given ["islands"]`` () =
    let actual =
        [ "islands"]
        |> CommandSource.Parse
    Assert.AreEqual(0u |> Islands |> Some, actual)

[<Test>]
let ``Parse.It returns Islands 0 command when given ["islands";"1"]`` () =
    let actual =
        [ "islands";"1"]
        |> CommandSource.Parse
    Assert.AreEqual(0u |> Islands |> Some, actual)

[<Test>]
let ``Parse.It returns Islands 1 command when given ["islands";"2"]`` () =
    let actual =
        [ "islands";"2"]
        |> CommandSource.Parse
    Assert.AreEqual(1u |> Islands |> Some, actual)

[<Test>]
let ``Parse.It returns Head For Foo command when given ["head";"for";"foo"]`` () =
    let actual =
        [ "head";"for";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual("foo" |> HeadFor |> Some, actual)
    