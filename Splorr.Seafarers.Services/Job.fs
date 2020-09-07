namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TermSource = unit -> string list
type TermSources = TermSource * TermSource * TermSource * TermSource * TermSource * TermSource
type WorldSingleStatisticSource = WorldStatisticIdentifier -> Statistic

type JobCreationContext =
    inherit UtilityPickRandomlyContext
    abstract member termSources                : TermSources
    abstract member worldSingleStatisticSource : WorldSingleStatisticSource

module Job =
    let Create 
            (context      : JobCreationContext)
            (destinations : Set<Location>) 
            : Job =
        let adverbSource, adjectiveSource, objectNameSource, personNameSource, personAdjectiveSource, professionSource = context.termSources
        let adverb            = adverbSource() |> Utility.PickRandomly context
        let adjective         = adjectiveSource() |> Utility.PickRandomly context
        let objectName        = objectNameSource() |> Utility.PickRandomly context
        let name              = personNameSource() |> Utility.PickRandomly context
        let personalAdjective = personAdjectiveSource() |> Utility.PickRandomly context
        let profession        = professionSource() |> Utility.PickRandomly context
        let destination       = destinations |> Set.toList |> Utility.PickRandomly context 
        let jobReward         = context.worldSingleStatisticSource WorldStatisticIdentifier.JobReward
        let rewardMinimum, rewardMaximum = jobReward.MinimumValue, jobReward.MaximumValue
        {
            FlavorText  = sprintf "please deliver this %s %s %s to %s the %s %s" adverb adjective objectName name personalAdjective profession
            Destination = destination
            Reward      = context.random.NextDouble() * (rewardMaximum - rewardMinimum) + rewardMinimum
        }
