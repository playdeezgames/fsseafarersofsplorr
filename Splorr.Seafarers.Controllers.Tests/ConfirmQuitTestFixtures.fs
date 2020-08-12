module ConfirmQuitTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open System

let internal worldSingleStatisticSourceStub (identfier: WorldStatisticIdentifier) : Statistic =
    match identfier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=30.0; MaximumValue=30.0; CurrentValue=30.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "soloIslandSingleStatisticSource")
let internal configuration: WorldConfiguration =
    {
        WorldSize              = (10.0, 10.0)
    }
let private vesselStatisticTemplateSourceStub () = 
    Map.empty

let private vesselStatisticSinkStub (_) (_) = ()
let private vesselSingleStatisticSourceStub (_) (identifier:VesselStatisticIdentifier) = 
    match identifier with
    | VesselStatisticIdentifier.PositionX
    | VesselStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; CurrentValue=100.0; MaximumValue=50.0} |> Some
    | VesselStatisticIdentifier.ViewDistance ->
        {MinimumValue=10.0; CurrentValue=10.0; MaximumValue=10.0} |> Some
    | _ -> None

let private random = Random()

let private avatarId = ""

let private nameSourceStub () = []
let internal shipmateRationItemSinkStub (_) (_) (_) = ()
let rationItemSourceStub () = [1UL]
let shipmateStatisticTemplateSourceStub () = Map.empty
let internal previousState = 
    World.Create
        nameSourceStub
        worldSingleStatisticSourceStub
        shipmateStatisticTemplateSourceStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        configuration 
        random
        avatarId
    |> Gamestate.AtSea


