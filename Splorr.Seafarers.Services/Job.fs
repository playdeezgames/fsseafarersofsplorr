namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TermSource = unit -> string list
type TermSources = TermSource * TermSource * TermSource * TermSource * TermSource * TermSource
type JobRewardStatisticSource = unit -> Statistic

module Job =
    type CreateContext =
        inherit ServiceContext
        abstract member termSources              : TermSources
        abstract member jobRewardStatisticSource : JobRewardStatisticSource
        abstract member random                   : Random
    let Create 
            (context      : ServiceContext)
            (destinations : Set<Location>) 
            : Job =
        let context = context :?> CreateContext
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
            context.jobRewardStatisticSource() 

        let rewardMinimum, 
            rewardMaximum = 
                jobReward.MinimumValue, 
                jobReward.MaximumValue
        {
            FlavorText  = sprintf "please deliver this %s %s %s to %s the %s %s" adverb adjective objectName name personalAdjective profession
            Destination = destination
            Reward      = context.random.NextDouble() * (rewardMaximum - rewardMinimum) + rewardMinimum
        }
