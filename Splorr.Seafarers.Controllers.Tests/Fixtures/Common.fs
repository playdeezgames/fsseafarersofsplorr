namespace Fixtures.Common

open NUnit.Framework

module Stub =
    let AvatarIslandFeatureSink (_,_) = 
        Assert.Fail("Stub.AvatarIslandFeatureSink")   
        ()

