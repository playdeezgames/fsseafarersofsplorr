namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module Job =
    type JobRewardStatisticSource = unit -> Statistic

    type CreateContext =
        abstract member jobRewardStatisticSource : JobRewardStatisticSource ref
    let private GetJobRewardStatistic
            (context : CommonContext)
            : Statistic =
        (context :?> CreateContext).jobRewardStatisticSource.Value()

    let internal Create 
            (context      : CommonContext)
            (destinations : Set<Location>) 
            : Job =
        let adverb =
            Utility.GenerateFromTermList context "adverb"
        let adjective =
            Utility.GenerateFromTermList context "adjective"
        let objectName =
            Utility.GenerateFromTermList context "object name"
        let personName =
            Utility.GenerateFromTermList context "person name"
        let personalAdjective =
            Utility.GenerateFromTermList context "person adjective"
        let profession =
            Utility.GenerateFromTermList context "profession"

        let destination = 
            destinations 
            |> Set.toList 
            |> Utility.PickFromListRandomly
                context

        let jobReward = 
            GetJobRewardStatistic context

        let rewardMinimum, 
            rewardMaximum = 
                jobReward.MinimumValue, 
                jobReward.MaximumValue
        {
            FlavorText  = sprintf "please deliver this %s %s %s to %s the %s %s" adverb adjective objectName personName personalAdjective profession
            Destination = destination
            Reward      = Utility.GenerateFromRange context (rewardMinimum, rewardMaximum)
        }
