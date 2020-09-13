﻿module AvatarTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures
open System

let internal random = Random()
let internal rewardRange = (1.0,10.0)
let internal singleLocation = [(0.0, 0.0)] |> Set.ofList
let internal vesselSingleStatisticSource (_) (identifier) = 
    match identifier with
    | VesselStatisticIdentifier.FoulRate ->
        {MinimumValue = 0.001; CurrentValue=0.001; MaximumValue=0.001} |> Some
    | VesselStatisticIdentifier.Speed ->
        {MinimumValue = 0.0; CurrentValue=1.0; MaximumValue=1.0} |> Some
    | VesselStatisticIdentifier.Heading ->
        {MinimumValue = 0.0; CurrentValue=0.0; MaximumValue=6.3} |> Some
    | VesselStatisticIdentifier.PositionX ->
        {MinimumValue = 0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
    | VesselStatisticIdentifier.PositionY ->
        {MinimumValue = 0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
    | _ ->
        None
let internal vesselSingleStatisticSink (_) (_) = ()

type TestJobCreationContext
        (
            termSources                : TermSources, 
            worldSingleStatisticSource : WorldSingleStatisticSource
        ) =
    interface UtilitySortListRandomlyContext with
        member _.random : Random = random

    interface JobCreateContext with
        member _.termSources : TermSources = termSources
        member _.worldSingleStatisticSource : WorldSingleStatisticSource = worldSingleStatisticSource

let internal jobCreationContextStub =
    TestJobCreationContext
        (termSourcesStub, 
        worldSingleStatisticSourceStub)

let internal avatarId = "avatar"
let internal job =
    Job.Create 
        jobCreationContextStub
        singleLocation
