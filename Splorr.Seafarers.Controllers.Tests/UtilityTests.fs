module UtilityTests

open NUnit.Framework
open Splorr.Seafarers.Controllers


[<Test>]
let ``DumpMessages.It sends all messages to the sink.`` () =
    let mutable actual: string list = []
    let sinkFake(message:string) : unit =
        actual <- List.append actual [message]
    let input = 
        [
            "one"
            "two"
            "three"
        ]
    let expected = input
    input
    |> Utility.DumpMessages sinkFake
    Assert.AreEqual(expected, actual)

