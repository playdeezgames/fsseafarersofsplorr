module Splorr.Seafarers.Views.Tests.UtilityTests

open NUnit.Framework
open Splorr.Seafarers.Views

let mutable private sunkMessages: string list = []
let private sink(message:string) : unit =
    sunkMessages <- List.append sunkMessages [message]

[<Test>]
let ``DumpMessages function.Sends all messages to the sink.`` () =
    sunkMessages<-[]
    let messages = ["one"; "two"; "three"]
    Utility.DumpMessages sink messages
    Assert.AreEqual(messages, sunkMessages)

