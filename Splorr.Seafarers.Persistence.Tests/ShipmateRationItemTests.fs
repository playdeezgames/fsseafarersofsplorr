module ShipmateRationItemTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``GetForShipmate.It gets the list of ration items for an existing shipmate.`` () =
    let connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let inputShipmateId = PrimaryShipmateId
        let expected : Result<uint64 list, string> =
            [ 1UL; 2UL ] |> Ok
        let actual =
            ShipmateRationItem.GetForShipmate connection inputAvatarId inputShipmateId
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()
    
[<Test>]
let ``GetForShipmate.It an empty list for a non-existing shipmate.`` () =
    let connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let inputShipmateId = PrimaryShipmateId
        let expected : Result<uint64 list, string> =
            [ ] |> Ok
        let actual =
            ShipmateRationItem.GetForShipmate connection inputAvatarId inputShipmateId
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()

[<Test>]
let ``SetForShipmate.It stores a new list of items for a non-existing shipmate.`` () =
    let connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let inputShipmateId = PrimaryShipmateId
        let expectedInitial : Result<uint64 list, string> = [] |> Ok
        let actualInitial = ShipmateRationItem.GetForShipmate connection inputAvatarId inputShipmateId 
        Assert.AreEqual(expectedInitial, actualInitial)
        let inputRationItems = 
            [ 1UL; 2UL ]
        let expected : Result<unit, string> =
            () |> Ok
        let actual =
            ShipmateRationItem.SetForShipmate connection inputAvatarId inputShipmateId inputRationItems
        Assert.AreEqual(expected, actual)
        let expectedFinal : Result<uint64 list, string> = inputRationItems |> Ok
        let actualFinal = ShipmateRationItem.GetForShipmate connection inputAvatarId inputShipmateId
        Assert.AreEqual(expectedFinal, actualFinal)
    finally
        connection.Close()

[<Test>]
let ``SetForShipmate.It stores a new list of items for an existing shipmate.`` () =
    let connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let inputShipmateId = PrimaryShipmateId
        let expectedInitial : Result<uint64 list, string> = [ 1UL; 2UL ] |> Ok
        let actualInitial = ShipmateRationItem.GetForShipmate connection inputAvatarId inputShipmateId
        Assert.AreEqual(expectedInitial, actualInitial)
        let inputRationItems = 
            [ 2UL; 1UL ]
        let expected : Result<unit, string> =
            () |> Ok
        let actual =
            ShipmateRationItem.SetForShipmate connection inputAvatarId inputShipmateId inputRationItems
        Assert.AreEqual(expected, actual)
        let expectedFinal : Result<uint64 list, string> = inputRationItems |> Ok
        let actualFinal = ShipmateRationItem.GetForShipmate connection inputAvatarId inputShipmateId
        Assert.AreEqual(expectedFinal, actualFinal)
    finally
        connection.Close()
