namespace Fixtures.Common

open NUnit.Framework

module Dummy =
    let IslandLocation = (0.0, 0.0)
    let AvatarId = "avatar"

module Stub =
    let AvatarIslandFeatureSink (_,_) = 
        Assert.Fail("Stub.AvatarIslandFeatureSink")   
        ()

