module AvatarTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

[<Test>]
let ``Create.It creates an avatar.`` () =
    let actual =
        Avatar.Create()
    Assert.AreEqual((0.0,0.0), actual.Position)
    Assert.AreEqual(1.0, actual.Speed)
    Assert.AreEqual(0.0, actual.Heading)


[<Test>]
let ``SetSpeed.It sets all stop when given less than zero.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (-1.0)
    Assert.AreEqual(0.0, actual.Speed)

[<Test>]
let ``SetSpeed.It sets full speed when gives more than one.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (2.0)
    Assert.AreEqual(1.0, actual.Speed)

[<Test>]
let ``SetSpeed.It sets half speed when given half speed.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (0.5)
    Assert.AreEqual(0.5, actual.Speed)

[<Test>]
let ``SetSpeed.It sets full speed when given one.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (1.0)
    Assert.AreEqual(1.0, actual.Speed)

[<Test>]
let ``SetSpeed.It sets all stop when given zero.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.SetSpeed (0.0)
    Assert.AreEqual(0.0, actual.Speed)

[<Test>]
let ``SetHeading.It sets a given heading.`` () =
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

[<Test>]
let ``Move.It moves the avatar.`` () =
    let actual =
        Avatar.Create()
        |> Avatar.Move
    Assert.AreEqual((1.0,0.0), actual.Position)
