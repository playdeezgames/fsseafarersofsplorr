module AvatarGamblingHandTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Tarot

[<Test>]
let ``GetForAvatar.It retrieve the hand for a given avatar.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = WinningGamblingAvatarId
        match AvatarGamblingHand.GetForAvatar connection inputAvatarId with
        | Ok (Some (first, second, final)) ->
            Assert.AreEqual(Minor (Wands, Rank.Ace), first)
            Assert.AreEqual(Minor (Wands, Rank.King), second)
            Assert.AreEqual(Minor (Wands, Rank.Seven), final)
        | Error message ->
            Assert.Fail message
        | _ ->
            Assert.Fail "unexpected result"
    finally
        connection.Close()
        
[<Test>]
let ``GetForAvatar.It retrieves nothing for a given avatar who does not have a gambling hand.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        match AvatarGamblingHand.GetForAvatar connection inputAvatarId with
        | Ok x ->
            Assert.AreEqual(None, x)
        | Error message ->
            Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetForAvatar.It sets a gambling hand for a given avatar that does not already have a hand.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let givenHand = (Minor (Cups, Rank.Ace),Minor (Cups, Rank.Deuce),Minor (Cups, Rank.Three)) |> Some
        match AvatarGamblingHand.SetForAvatar connection inputAvatarId givenHand with
        | Ok () ->
            let expectedResult : Result<(Card*Card*Card) option, string> = givenHand |> Ok
            let actualResult = AvatarGamblingHand.GetForAvatar connection inputAvatarId
            Assert.AreEqual(expectedResult, actualResult)
        | Error message ->
            Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetForAvatar.It removes a gambling hand for a given avatar that does already have a hand.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = WinningGamblingAvatarId
        let givenHand = None
        match AvatarGamblingHand.SetForAvatar connection inputAvatarId givenHand with
        | Ok () ->
            let expectedResult : Result<(Card*Card*Card) option, string> = givenHand |> Ok
            let actualResult = AvatarGamblingHand.GetForAvatar connection inputAvatarId
            Assert.AreEqual(expectedResult, actualResult)
        | Error message ->
            Assert.Fail message
    finally
        connection.Close()
