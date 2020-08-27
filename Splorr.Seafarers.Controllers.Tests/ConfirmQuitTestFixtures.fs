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
    | WorldStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=10.0; CurrentValue=5.0}
    | WorldStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=10.0; CurrentValue=5.0}
    | _ ->
        raise (System.NotImplementedException "worldSingleStatisticSourceStub")
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
let internal rationItemSourceStub () = [1UL]
let internal shipmateStatisticTemplateSourceStub () = Map.empty
let internal shipmateSingleStatisticSinkStub (_) (_) (_) = ()
let internal avatarJobSinkStub (_) (_) = ()
let internal avatarIslandSingleMetricSinkStub (_) (_) (_) (_) = ()
let internal islandSingleNameSinkStub (_) (_) = ()
let internal islandSingleStatisticSinkStub (_) (_) = ()
let internal islandStatisticTemplateSourceStub () = Map.empty
let internal previousState = 
    avatarId
    |> Gamestate.AtSea


