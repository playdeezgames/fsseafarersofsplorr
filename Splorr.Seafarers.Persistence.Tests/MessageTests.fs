module MessageTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``GetForAvatar.It returns a list of messages for an avatar that has messages.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let expected : Result<string list, string> =
            [
                "message1"
                "message2"
                "message3"
            ]
            |> Ok
        let actual =
            Message.GetForAvatar connection inputAvatarId
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()

[<Test>]
let ``GetForAvatar.It returns no messages for an avatar that has no messages.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let expected : Result<string list, string> =
            [ ]
            |> Ok
        let actual =
            Message.GetForAvatar connection inputAvatarId
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()

[<Test>]
let ``ClearForAvatar.It clears out messages for an avatar that has messages.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let expected : Result<unit, string> =
            ()
            |> Ok
        let actual =
            Message.ClearForAvatar connection inputAvatarId
        Assert.AreEqual(expected, actual)
        match Message.GetForAvatar connection inputAvatarId with
        | Ok [] ->
            ()
        | _ ->
            Assert.Fail("This is not a valid result.")
    finally
        connection.Close()

[<Test>]
let ``AddToAvatar.It adds messages for a given avatar id.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let inputMessage = "message"
        let expected : Result<unit, string> =
            ()
            |> Ok
        let actual =
            Message.AddForAvatar connection (inputAvatarId, inputMessage)
        Assert.AreEqual(expected, actual)
        match Message.GetForAvatar connection inputAvatarId with
        | Ok [ _ ] ->
            ()
        | _ ->
            Assert.Fail("This is not a valid result.")
    finally
        connection.Close()
