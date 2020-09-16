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
        Assert.Fail("Stub.AvatarIslandFeatureSink")   
    let AvatarIslandFeatureSource (_) =
        Assert.Fail("Stub.AvatarIslandFeatureSource")
        None
    let CommandSource () : Command option =
        Assert.Fail("Stub.CommandSource")
        None
    
    

module Mock =
    let internal CommandSource 
            (command:Command option) 
            : unit -> Command option= 
        fun () -> command
