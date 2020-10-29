module WorldSetHeadingTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``SetHeading.It sets a vessel heading for the given avatar and reports the new status with a message.`` () =
    let context = Contexts.TestContext()
    World.SetHeading
        context
        1.0
        Dummies.ValidAvatarId

