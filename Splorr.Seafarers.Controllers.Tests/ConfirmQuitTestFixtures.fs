﻿module ConfirmQuitTestFixtures

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
let rationItemSourceStub () = [1UL]
let shipmateStatisticTemplateSourceStub () = Map.empty
let shipmateSingleStatisticSinkStub (_) (_) (_) = ()
let avatarJobSinkStub (_) (_) = ()
let avatarIslandSingleMetricSinkStub (_) (_) (_) (_) = ()
let islandSingleNameSinkStub (_) (_) = ()
let internal previousState = 
    World.Create
        avatarIslandSingleMetricSinkStub
        avatarJobSinkStub
        islandSingleNameSinkStub
        nameSourceStub
        worldSingleStatisticSourceStub
        shipmateStatisticTemplateSourceStub
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random
        avatarId
    |> Gamestate.AtSea


