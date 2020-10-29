module WorldSetSpeedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``SetSpeed.It sets a new speed for the given avatar and reports the status to a message.`` () =
    let context = Contexts.TestContext()
    World.SetSpeed
        context
        0.5
        Dummies.ValidAvatarId


