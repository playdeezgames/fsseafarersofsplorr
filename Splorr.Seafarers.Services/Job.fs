namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TermSource = unit -> string list
type TermSources = TermSource * TermSource * TermSource * TermSource * TermSource * TermSource
type WorldSingleStatisticSource = WorldStatisticIdentifier -> Statistic

type JobCreateContext =
    inherit UtilityPickRandomlyContext
    abstract member termSources                : TermSources
    abstract member worldSingleStatisticSource : WorldSingleStatisticSource

module Job =
    let Create 
            (context      : JobCreateContext)
            (destinations : Set<Location>) 
            : Job =

        let pickRandomly : string list -> string = 
            Utility.PickRandomly context

        let adverbSource, 
            adjectiveSource, 
            objectNameSource, 
            personNameSource, 
            personAdjectiveSource, 
            professionSource = 
                context.termSources

        let adverb = 
            adverbSource() 
            |> pickRandomly

        let adjective = 
            adjectiveSource() 
            |> pickRandomly

        let objectName = 
            objectNameSource() 
            |> pickRandomly

        let name = 
            personNameSource() 
            |> pickRandomly

        let personalAdjective = 
            personAdjectiveSource() 
            |> pickRandomly

        let profession = 
            professionSource() 
            |> pickRandomly

        let destination = 
            destinations 
            |> Set.toList 
            |> Utility.PickRandomly
                context

        let jobReward = 
            context.worldSingleStatisticSource 
                WorldStatisticIdentifier.JobReward

        let rewardMinimum, 
            rewardMaximum = 
                jobReward.MinimumValue, 
                jobReward.MaximumValue
        {
            FlavorText  = sprintf "please deliver this %s %s %s to %s the %s %s" adverb adjective objectName name personalAdjective profession
            Destination = destination
            Reward      = context.random.NextDouble() * (rewardMaximum - rewardMinimum) + rewardMinimum
        }
