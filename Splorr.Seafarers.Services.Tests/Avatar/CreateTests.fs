module AvatarCreateTests

open Splorr.Seafarers.Services
open NUnit.Framework

type TestAvatarCreateContext
        (avatarJobSink,
        rationItemSource,
        shipmateRationItemSink,
        shipmateSingleStatisticSink,
        shipmateStatisticTemplateSource,
        vesselStatisticSink, 
        vesselStatisticTemplateSource) =
    interface Avatar.CreateContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
    interface Vessel.CreateContext with
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource
    interface Shipmate.CreateContext with
        member _.rationItemSource: RationItemSource = rationItemSource
        member _.shipmateRationItemSink: ShipmateRationItemSink = shipmateRationItemSink
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource

[<Test>]
let ``Create.It creates an avatar.`` () =
    let avatarJobSink (_) (actual) = 
        Assert.AreEqual(None, actual)
    let rationItemSource () = 
        []
    let shipmateRationItemSink (_) (_) (actual) = 
        Assert.AreEqual([], actual)
    let shipmateSingleStatisticSink (_) (_) (_) = 
        Assert.Fail("shipmateSingleStatisticSink")
    let shipmateStatisticTemplateSource () = 
        Map.empty
    let vesselStatisticSink (_) (actual) = 
        Assert.AreEqual(Map.empty, actual)
    let vesselStatisticTemplateSource () = 
        Map.empty
    let context = 
        TestAvatarCreateContext
            (avatarJobSink,
            rationItemSource,
            shipmateRationItemSink,
            shipmateSingleStatisticSink,
            shipmateStatisticTemplateSource,
            vesselStatisticSink, 
            vesselStatisticTemplateSource)
    Avatar.Create
        context
        Fixtures.Common.Dummy.AvatarId                        

