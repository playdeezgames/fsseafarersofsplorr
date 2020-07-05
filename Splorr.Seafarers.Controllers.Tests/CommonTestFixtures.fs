module CommonTestFixtures

open Splorr.Seafarers.Controllers

let internal random = System.Random()
let internal sinkStub (_:Message) : unit = ()
let internal toSource (command:Command option) = fun () -> command

