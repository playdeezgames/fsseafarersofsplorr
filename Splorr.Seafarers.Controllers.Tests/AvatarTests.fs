module Splorr.Seafarers.Controllers.Tests.Avatar

open NUnit.Framework
open Splorr.Seafarers.Controllers

[<Test>]
let ``Create function.Creates a blank avatar.`` () =
    let actual =
        Avatar.Create()
    Assert.AreEqual(0.0, actual.X)
    Assert.AreEqual(0.0, actual.Y)
    Assert.AreEqual(1.0, actual.Speed)
    Assert.AreEqual(0.0, actual.Heading)


[<Test>]
let ``SetSpeed function.Less than zero sets all stop.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (-1.0)
    Assert.AreEqual(0.0, actual.Speed)

[<Test>]
let ``SetSpeed function.Greater than one sets full speed.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (2.0)
    Assert.AreEqual(1.0, actual.Speed)

[<Test>]
let ``SetSpeed function.One half sets half speed.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (0.5)
    Assert.AreEqual(0.5, actual.Speed)

[<Test>]
let ``SetSpeed function.One sets full speed.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (1.0)
    Assert.AreEqual(1.0, actual.Speed)

[<Test>]
let ``SetSpeed function.Zero sets all stop.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (0.0)
    Assert.AreEqual(0.0, actual.Speed)
