module ParseInvalidCommandTests

open NUnit.Framework
open Splorr.Seafarers

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
let ``Parse.It returns invalid command when given ["bet"]`` () =
    let actual =
        [ "bet" ]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["bet";"foo"]`` () =
    let actual =
        [ "bet"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["bet";"-1"]`` () =
    let actual =
        [ "bet"; "-1"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["bet";"0"]`` () =
    let actual =
        [ "bet"; "0"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["bet";"0.0"]`` () =
    let actual =
        [ "bet"; "0.0"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["help";"foo"]`` () =
    let actual =
        [ "help"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["yes";"foo"]`` () =
    let actual =
        [ "yes"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["items";"foo"]`` () =
    let actual =
        [ "items"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["no";"foo"]`` () =
    let actual =
        [ "no"; "foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

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
let ``Parse.It returns invalid command when given ["leave";"foo"]`` () =
    let actual =
        [ "leave"; "foo"]
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
let ``Parse.It returns invalid command when given ["set";"heading";"1";"foo"]`` () =
    let actual =
        [ "set"; "heading"; "1";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["set";"heading";"1";"2";"foo"]`` () =
    let actual =
        [ "set"; "heading"; "1";"2";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

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
let ``Parse.It returns invalid command when given ["start";"foo"]`` () =
    let actual =
        [ "start";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["menu";"foo"]`` () =
    let actual =
        [ "menu";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["metrics";"foo"]`` () =
    let actual =
        [ "metrics";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["abandon";"foo"]`` () =
    let actual =
        [ "abandon";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["resume";"foo"]`` () =
    let actual =
        [ "resume";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["dock";"foo"]`` () =
    let actual =
        [ "dock";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["undock";"foo"]`` () =
    let actual =
        [ "undock";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["islands";"foo"]`` () =
    let actual =
        [ "islands";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["head"]`` () =
    let actual =
        [ "head"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["head";"for"]`` () =
    let actual =
        [ "head";"for"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["head";"for";"foo";"foo"]`` () =
    let actual =
        [ "head";"for";"foo";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["status";"foo"]`` () =
    let actual =
        [ "status";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["jobs";"foo"]`` () =
    let actual =
        [ "jobs";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["accept"]`` () =
    let actual =
        [ "accept"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["accept";"job"]`` () =
    let actual =
        [ "accept";"job"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["accept";"job";"foo"]`` () =
    let actual =
        [ "accept";"job";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["inventory";"foo"]`` () =
    let actual =
        [ "inventory";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["buy"]`` () =
    let actual =
        [ "buy"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["buy";"maximum"]`` () =
    let actual =
        [ "buy";"maximum"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["save"]`` () =
    let actual =
        [ "save"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["buy";"foo"]`` () =
    let actual =
        [ "buy";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["buy";"0"]`` () =
    let actual =
        [ "buy";"0"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["buy";"-3";"foo"]`` () =
    let actual =
        [ "buy";"-3";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)


[<Test>]
let ``Parse.It returns invalid command when given ["sell"]`` () =
    let actual =
        [ "sell"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["sell";"foo"]`` () =
    let actual =
        [ "sell";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["sell";"all"]`` () =
    let actual =
        [ "sell";"all"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["sell";"0"]`` () =
    let actual =
        [ "sell";"0"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["sell";"-3";"foo"]`` () =
    let actual =
        [ "sell";"-3";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["careen"]`` () =
    let actual =
        [ "careen" ]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["careen";"to"]`` () =
    let actual =
        [ "careen";"to" ]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)
    
[<Test>]
let ``Parse.It returns invalid command when given ["careen";"to";"port";"foo"]`` () =
    let actual =
        [ "careen";"to";"port";"foo" ]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["weigh"]`` () =
    let actual =
        [ "weigh" ]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["weigh";"foo"]`` () =
    let actual =
        [ "weigh"; "foo" ]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["clean"]`` () =
    let actual =
        [ "clean" ]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["distance"]`` () =
    let actual =
        [ "distance"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["distance";"to"]`` () =
    let actual =
        [ "distance";"to"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)

[<Test>]
let ``Parse.It returns invalid command when given ["distance";"to";"foo";"foo"]`` () =
    let actual =
        [ "distance";"to";"foo";"foo"]
        |> CommandSource.Parse
    Assert.AreEqual(None, actual)
