module UtilityTests

open NUnit.Framework
open Splorr.Seafarers.Controllers

[<Test>]
let ``DumpMessages.It sends all messages to the sink.`` () =
    let mutable actual: Message list = []
    let sinkFake(message:Message) : unit =
        actual <- List.append actual [message]
    let input = 
        [
            "one"
            "two"
            "three"
        ]
    let expected = 
        input
        |> List.map (fun x -> (Hue.Flavor, x |> Line) |> Hued)
    input
    |> Utility.DumpMessages sinkFake
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Lesser.It returns the first given item if the first item is the lesser.`` () =
    let first = 0u
    let second = 1u
    let expected = first
    let actual = Utility.Lesser first second
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Lesser.It returns the second given item if the second item is the lesser.`` () =
    let first = 1u
    let second = 0u
    let expected = second
    let actual = Utility.Lesser first second
    Assert.AreEqual(expected, actual)