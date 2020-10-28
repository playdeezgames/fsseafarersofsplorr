module WorldMoveTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``Move..`` () =
    let context = Contexts.TestContext()
    World.Move
        context
        1u
        Dummies.ValidAvatarId


