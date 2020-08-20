module AvatarJobTests


open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open System.Data.SQLite
open Splorr.Seafarers.Models

[<Test>]
let ``GetForAvatar.It returns None for an avatar that does not have a job assigned.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        match connection |> AvatarJob.GetForAvatar inputAvatarId with
        | Ok actual     -> 
            Assert.True(actual.IsNone)
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()


[<Test>]
let ``GetForAvatar.It returns the assigned job for an avatar that has a job assigned.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        match connection |> AvatarJob.GetForAvatar inputAvatarId with
        | Ok (Some actual)     -> 
            Assert.AreEqual("flavor", actual.FlavorText)
            Assert.AreEqual(1.0, actual.Reward)
            Assert.AreEqual(2.0, actual.Destination |> fst)
            Assert.AreEqual(3.0, actual.Destination |> snd)
        | Ok None ->
            Assert.Fail "There was no job, but it should be there!"
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetForAvatar.It stores a job when given one for an avatar that does not have a job assigned.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let inputJob : Job option=
            {
                FlavorText = "flavor"
                Reward = 1.0
                Destination = (2.0, 3.0)
            }
            |> Some
        use command = new SQLiteCommand("SELECT COUNT(1) FROM [AvatarJobs] WHERE [AvatarId]=$avatarId", connection)
        command.Parameters.AddWithValue("$avatarId", inputAvatarId) |> ignore
        let initialCount = command.ExecuteScalar()
        Assert.AreEqual(0, initialCount)
        match connection |> AvatarJob.SetForAvatar inputAvatarId inputJob with
        | Ok ()     -> 
            let finalCount = command.ExecuteScalar()
            Assert.AreEqual(1, finalCount)
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetForAvatar.It eliminates a job when given None for an avatar that has a job assigned.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let inputJob : Job option=
            None
        use command = new SQLiteCommand("SELECT COUNT(1) FROM [AvatarJobs] WHERE [AvatarId]=$avatarId", connection)
        command.Parameters.AddWithValue("$avatarId", inputAvatarId) |> ignore
        let initialCount = command.ExecuteScalar()
        Assert.AreEqual(1, initialCount)
        match connection |> AvatarJob.SetForAvatar inputAvatarId inputJob with
        | Ok ()     -> 
            let finalCount = command.ExecuteScalar()
            Assert.AreEqual(0, finalCount)
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()
