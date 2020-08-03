module ConfirmQuitTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open System

let internal configuration: WorldConfiguration =
    {
        AvatarDistances        = (10.0, 1.0)
        MaximumGenerationTries = 10u
        MinimumIslandDistance  = 30.0
        RationItems            = [ 1UL ]
        RewardRange            = (1.0, 10.0)
        StatisticDescriptors   = []
        WorldSize              = (10.0, 10.0)
    }
let private vesselStatisticTemplateSourceStub () = 
    Map.empty

let private vesselStatisticSinkStub (_) (_) = ()

let private random = Random()

let private avatarId = ""

let private nameSourceStub () = []

let internal previousState = 
    World.Create
        nameSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        configuration 
        random
        avatarId
    |> Gamestate.AtSea


