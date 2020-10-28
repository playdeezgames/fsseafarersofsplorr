module WorldSetSpeedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``SetSpeed..`` () =
    let context = Contexts.TestContext()
    World.SetSpeed
        context
        0.5
        Dummies.ValidAvatarId


