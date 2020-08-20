module AvatarInventoryTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``GetForAvatar.It retrieves the inventory for an existing avatar.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        match AvatarInventory.GetForAvatar connection inputAvatarId with
        | Ok actual     -> 
            Assert.AreEqual(3, actual.Count)
            Assert.AreEqual(2UL, actual.[1UL])
            Assert.AreEqual(3UL, actual.[2UL])
            Assert.AreEqual(1UL, actual.[3UL])
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetForAvatar.It removes the inventory for an existing avatar when given an empty inventory.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        match AvatarInventory.SetForAvatar connection inputAvatarId Map.empty with
        | Ok ()     -> 
            match AvatarInventory.GetForAvatar connection inputAvatarId with
            | Ok result ->
                Assert.True(result |> Map.isEmpty)
            | _ -> Assert.Fail("Didn't retrieve the inventory.")
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()
    
[<Test>]
let ``SetForAvatar.It sets the inventory for a non-existing avatar when given an inventory.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let inputInventory = 
            Map.empty
            |> Map.add 1UL 1UL
        match AvatarInventory.SetForAvatar connection inputAvatarId inputInventory with
        | Ok ()     -> 
            match AvatarInventory.GetForAvatar connection inputAvatarId with
            | Ok result ->
                Assert.AreEqual(1, result.Count)
                Assert.AreEqual(1UL, result.[1UL])
            | _ -> Assert.Fail("Didn't retrieve the inventory.")
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()
