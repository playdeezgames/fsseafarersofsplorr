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
        |> List.map (fun x -> (Flavor, x |> Line) |> Hued)
    input
    |> Utility.DumpMessages sinkFake
    Assert.AreEqual(expected, actual)

