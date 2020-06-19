module AvatarTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

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

[<Test>]
let ``SetHeading function.Sets a new heading.`` () =
    let heading = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
    let actual =
        Avatar.Create()
        |> Avatar.SetHeading heading
    Assert.AreEqual(heading |> Dms.ToFloat, actual.Heading)
