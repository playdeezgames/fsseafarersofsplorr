module AvatarTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

open AvatarTestFixtures

[<Test>]
let ``Create.It creates an avatar.`` () =
    let actual =
        avatar
    Assert.AreEqual((0.0,0.0), actual.Position)
    Assert.AreEqual(1.0, actual.Speed)
    Assert.AreEqual(0.0, actual.Heading)


[<Test>]
let ``SetSpeed.It sets all stop when given less than zero.`` () =
    let actual =
        avatar
        |> Avatar.SetSpeed (-1.0)
    Assert.AreEqual(0.0, actual.Speed)

[<Test>]
let ``SetSpeed.It sets full speed when gives more than one.`` () =
    let actual =
        avatar
        |> Avatar.SetSpeed (2.0)
    Assert.AreEqual(1.0, actual.Speed)

[<Test>]
let ``SetSpeed.It sets half speed when given half speed.`` () =
    let actual =
        avatar
        |> Avatar.SetSpeed (0.5)
    Assert.AreEqual(0.5, actual.Speed)

[<Test>]
let ``SetSpeed.It sets full speed when given one.`` () =
    let actual =
        avatar
        |> Avatar.SetSpeed (1.0)
    Assert.AreEqual(1.0, actual.Speed)

[<Test>]
let ``SetSpeed.It sets all stop when given zero.`` () =
    let actual =
        avatar
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
        avatar
        |> Avatar.SetHeading heading
    Assert.AreEqual(heading |> Dms.ToFloat, actual.Heading)

[<Test>]
let ``Move.It moves the avatar.`` () =
    let actual =
        avatar
        |> Avatar.Move
    Assert.AreEqual((1.0,0.0), actual.Position)

[<Test>]
let ``SetJob.It sets the job of the given avatar.`` () =
    let actual =
        avatar
        |> Avatar.SetJob job
    Assert.AreEqual(job |> Some, actual.Job)

[<Test>]
let ``AbandonJob.It does nothing when the given avatar has no job.`` () =
    let subject = avatar
    let expected = subject
    let actual =
        subject
        |> Avatar.AbandonJob
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AbandonJob.It set job to None when the given avatar has a job.`` () =
    let subject = employedAvatar
    let expected = {subject with Job=None; Reputation = subject.Reputation - 1.0}
    let actual =
        subject
        |> Avatar.AbandonJob
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``CompleteJob.It does nothing when the given avatar has no job.`` () =
    let subject = avatar
    let expected = subject
    let actual = 
        subject
        |> Avatar.CompleteJob
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CompleteJob.It sets job to None, adds reward money, adds reputation when the given avatar has a job.`` () =
    let subject = employedAvatar
    let subjectJob = employedAvatar.Job.Value
    let expected = {subject with Job = None; Money = subject.Money + subjectJob.Reward; Reputation = subject.Reputation + 1.0}
    let actual = 
        subject
        |> Avatar.CompleteJob
    Assert.AreEqual(expected, actual)
