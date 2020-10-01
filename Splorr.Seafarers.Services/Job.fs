namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TermSource = unit -> string list
type JobRewardStatisticSource = unit -> Statistic

module Job =
    type CreateContext =
        inherit ServiceContext
        abstract member termListSource : TermListSource
        abstract member jobRewardStatisticSource : JobRewardStatisticSource
    let Create 
            (context      : ServiceContext)
            (destinations : Set<Location>) 
            : Job =
        let context = context :?> CreateContext
        let pickRandomly : string list -> string = 
            Utility.PickRandomly context

        let adverbSource =
            context.termListSource "adverb"
        let adjectiveSource =
            context.termListSource "adjective"
        let objectNameSource =
            context.termListSource "object name"
        let personNameSource =
            context.termListSource "person name"
        let personAdjectiveSource =
            context.termListSource "person adjective"
        let professionSource =
            context.termListSource "profession"

        let adverb = 
            adverbSource
            |> pickRandomly

        let adjective = 
            adjectiveSource
            |> pickRandomly

        let objectName = 
            objectNameSource
            |> pickRandomly

        let name = 
            personNameSource
            |> pickRandomly

        let personalAdjective = 
            personAdjectiveSource
            |> pickRandomly

        let profession = 
            professionSource
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
            Reward      = Utility.RangeGenerator context (rewardMinimum, rewardMaximum)
        }
