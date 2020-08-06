namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TermSource = unit -> string list

module Job =
    let Create 
            (termSources  : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (random       : Random) 
            (rewardRange  : float * float) 
            (destinations : Set<Location>) 
            : Job =
        let adverbSource, adjectiveSource, objectNameSource, personNameSource, personAdjectiveSource, professionSource = termSources
        let adverb            = adverbSource() |> Utility.PickRandomly random
        let adjective         = adjectiveSource() |> Utility.PickRandomly random
        let objectName        = objectNameSource() |> Utility.PickRandomly random
        let name              = personNameSource() |> Utility.PickRandomly random
        let personalAdjective = personAdjectiveSource() |> Utility.PickRandomly random
        let profession        = professionSource() |> Utility.PickRandomly random
        let destination       = destinations |> Set.toList |> Utility.PickRandomly random 
        let rewardMinimum, rewardMaximum = rewardRange
        {
            FlavorText  = sprintf "please deliver this %s %s %s to %s the %s %s" adverb adjective objectName name personalAdjective profession
            Destination = destination
            Reward      = random.NextDouble() * (rewardMaximum - rewardMinimum) + rewardMinimum
        }
