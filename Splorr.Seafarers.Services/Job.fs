﻿namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TermSource = unit -> string list
type TermSources = TermSource * TermSource * TermSource * TermSource * TermSource * TermSource
type WorldSingleStatisticSource = WorldStatisticIdentifier -> Statistic

type JobCreationContext =
    abstract member termSources                : TermSources
    abstract member worldSingleStatisticSource : WorldSingleStatisticSource

module Job =
    let Create 
            (context      : JobCreationContext)
            (random       : Random) 
            (destinations : Set<Location>) 
            : Job =
        let adverbSource, adjectiveSource, objectNameSource, personNameSource, personAdjectiveSource, professionSource = context.termSources
        let adverb            = adverbSource() |> Utility.PickRandomly random
        let adjective         = adjectiveSource() |> Utility.PickRandomly random
        let objectName        = objectNameSource() |> Utility.PickRandomly random
        let name              = personNameSource() |> Utility.PickRandomly random
        let personalAdjective = personAdjectiveSource() |> Utility.PickRandomly random
        let profession        = professionSource() |> Utility.PickRandomly random
        let destination       = destinations |> Set.toList |> Utility.PickRandomly random 
        let jobReward         = context.worldSingleStatisticSource WorldStatisticIdentifier.JobReward
        let rewardMinimum, rewardMaximum = jobReward.MinimumValue, jobReward.MaximumValue
        {
            FlavorText  = sprintf "please deliver this %s %s %s to %s the %s %s" adverb adjective objectName name personalAdjective profession
            Destination = destination
            Reward      = random.NextDouble() * (rewardMaximum - rewardMinimum) + rewardMinimum
        }
