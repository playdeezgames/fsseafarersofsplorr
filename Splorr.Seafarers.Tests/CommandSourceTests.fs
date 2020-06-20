module CommandSourceTests

open NUnit.Framework
open Splorr.Seafarers
open Splorr.Seafarers.Views
open Splorr.Seafarers.Models

[<Test>]
let ``Parse.It returns Quit command when given ["quit"]`` () =
    let actual =
        [ "quit" ]
        |> CommandSource.Parse
    Assert.AreEqual(Quit |> Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["quit";"foo"]`` () =
    let actual =
        [ "quit"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given []`` () =
    let actual =
        [ ]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Help command when given ["help"]`` () =
    let actual =
        [ "help" ]
        |> CommandSource.Parse
    Assert.AreEqual(Command.Help |> Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["help";"foo"]`` () =
    let actual =
        [ "help"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Yes command when given ["yes"]`` () =
    let actual =
        [ "yes" ]
        |> CommandSource.Parse
    Assert.AreEqual(Yes |> Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["yes";"foo"]`` () =
    let actual =
        [ "yes"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Yes command when given ["no"]`` () =
    let actual =
        [ "no" ]
        |> CommandSource.Parse
    Assert.AreEqual(No |> Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["no";"foo"]`` () =
    let actual =
        [ "no"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Yes command when given ["move"]`` () =
    let actual =
        [ "move" ]
        |> CommandSource.Parse
    Assert.AreEqual(Move |> Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["move";"foo"]`` () =
    let actual =
        [ "move"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set"]`` () =
    let actual =
        [ "set"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set";"foo"]`` () =
    let actual =
        [ "set"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set";"heading"]`` () =
    let actual =
        [ "set"; "heading"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set";"heading";"foo"]`` () =
    let actual =
        [ "set"; "heading"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1"]`` () =
    let actual =
        [ "set"; "heading"; "1"]
        |> CommandSource.Parse
    Assert.AreEqual({Degrees=1;Minutes=0;Seconds=0.0} |> Heading |> Set |> Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set";"heading";"1";"foo"]`` () =
    let actual =
        [ "set"; "heading"; "1";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1","2"]`` () =
    let actual =
        [ "set"; "heading"; "1"; "2"]
        |> CommandSource.Parse
    Assert.AreEqual({Degrees=1;Minutes=2;Seconds=0.0} |> Heading |> Set |> Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set";"heading";"1";"2";"foo"]`` () =
    let actual =
        [ "set"; "heading"; "1";"2";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Set Heading command when given ["set";"heading";"1","2","3"]`` () =
    let actual =
        [ "set"; "heading"; "1"; "2"; "3"]
        |> CommandSource.Parse
    Assert.AreEqual({Degrees=1;Minutes=2;Seconds=3.0} |> Heading |> Set |> Some, actual)


[<Test>]
let ``Parse.It returns invalid command when given ["set";"heading";"1";"2";"3";"foo"]`` () =
    let actual =
        [ "set"; "heading"; "1";"2";"3";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set";"speed"]`` () =
    let actual =
        [ "set"; "speed"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set";"speed";"foo"]`` () =
    let actual =
        [ "set"; "speed"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set";"speed";"1"]`` () =
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
let ``Parse.It returns invalid command when given ["start";"foo"]`` () =
    let actual =
        [ "start";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Menu command when given ["menu"]`` () =
    let actual =
        [ "menu"]
        |> CommandSource.Parse
    Assert.AreEqual(Menu|>Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["menu";"foo"]`` () =
    let actual =
        [ "menu";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Abandon command when given ["abandon"]`` () =
    let actual =
        [ "abandon"]
        |> CommandSource.Parse
    Assert.AreEqual(Abandon|>Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["abandon";"foo"]`` () =
    let actual =
        [ "abandon";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns Menu command when given ["resume"]`` () =
    let actual =
        [ "resume"]
        |> CommandSource.Parse
    Assert.AreEqual(Resume|>Some, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["resume";"foo"]`` () =
    let actual =
        [ "menu";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)
    