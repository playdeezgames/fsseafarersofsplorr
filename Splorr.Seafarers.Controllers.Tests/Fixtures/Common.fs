namespace Fixtures.Common

open NUnit.Framework
open System
open Splorr.Seafarers.Controllers

module Dummy =
    let IslandLocation = (0.0, 0.0)
    let AvatarId = "avatar"
    let Random = Random()

module Fake =
    let AvatarIslandFeatureSink (_,_) = 
        Assert.Fail("Fake.AvatarIslandFeatureSink")   
    let AvatarIslandFeatureSource (_) =
        Assert.Fail("Fake.AvatarIslandFeatureSource")
        None
    let CommandSource () : Command option =
        Assert.Fail("Fake.CommandSource")
        None
    let AvatarMessagePurger (_) =
        Assert.Fail("Fake.AvatarMessagePurger")
    let ShipmateSingleStatisticSink (_) (_) (_) =
        Assert.Fail("Fake.ShipmateSingleStatisticSink")
    
    

module Mock =
    let internal CommandSource 
            (command:Command option) 
            : unit -> Command option= 
        fun () -> command
