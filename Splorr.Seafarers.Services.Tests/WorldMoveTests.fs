module WorldMoveTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``Move.When given a zero turn move does nothing.`` () =
    let context = Contexts.TestContext()
    World.Move
        context
        1u
        Dummies.ValidAvatarId

[<Test>]
let ``Move.When given a one turn move it moves once.`` () =
    let context = Contexts.TestContext()
    World.Move
        context
        1u
        Dummies.ValidAvatarId

[<Test>]
let ``Move.When given a two turn move it moves twice.`` () =
    let context = Contexts.TestContext()
    World.Move
        context
        1u
        Dummies.ValidAvatarId


[<Test>]
let ``Move.When a move ages out the avatar's primary shipmate, the game is over.`` () =
    let context = Contexts.TestContext()
    World.Move
        context
        1u
        Dummies.ValidAvatarId
