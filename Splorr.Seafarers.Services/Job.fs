namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TermSource = unit -> string list

module Job =
    let private adjectives = 
        [
            "valuable"
            "ugly"
            "well-crafted"
            "shiny"
            "enchanted"
            "tatty"
            "glowing"
            "shrunken"
            "embalmed"
            "moist"
            "mouldy"
            "bloody"
            "gnarly"
            "oaken"
            "turgid"
            "swollen"
            "incredible"
            "cheesy"
            "soapy"
            "decadent"
            "worn"
        ]

    let private objectNames = 
        [
            "'stuff'"
            "widget"
            "orb"
            "macguffin"
            "flask"
            "arrow"
            "knickers"
            "body part"
            "staff"
            "bow"
            "sword"
            "shield"
            "goblet"
            "statue"
            "idol"
            "marital aid"
            "book"
            "doorknob"
            "trinket"
            "necklace"
            "ring"
            "bodice"
            "chastity belt"
            "seedling"
            "tiara"
            "streamdeck"
            "jug"
            "key"
            "scenegraph"
            "cheese wedge"
            "copy of Turbo Boom"
            "soap"
            "wrench"
            "pie"
            "face shield"
            "panda milk"
            "map"
            "gravy"
        ]

    let private names = 
        [
            "Robert"
            "Terence"
            "Gareth"
            "Julian"
            "Ivan"
            "Timothy"
            "Joshua"
            "Bob"
            "Jim"
            "Jill"
            "Kelly"
            "Percival"
            "Matthew"
            "Sally"
            "Susan"
            "Muffin"
            "Beardy"
            "Jimmy"
            "Montgomery"
            "James"
            "Davin"
            "Ryan"
            "'Grumpy'"
        ]

    let private personalAdjectives =
        [
            "happy"
            "joyful"
            "magnificent"
            "enthusiastic"
            "submissive"
            "dominant"
            "dedicated"
            "bald"
            "well-endowed"
            "evil"
            "naked"
            "unscrupulous"
            "mysogynistic"
            "gluttonous"
            "smelly"
            "surly"
            "shy"
            "contemptuous"
            "self-righteous"
        ]

    let private professions = 
        [
            "tanner"
            "tailor"
            "blacksmith"
            "noble"
            "peasant"
            "squire"
            "vagrant"
            "harlot"
            "pimp"
            "miller"
            "abbot"
            "grump"
            "hamster"
            "streamer"
            "barkeep"
            "silversmith"
            "glazier"
            "calligrapher"
            "influencer"
            "milkman"
            "baker"
            "greengrocer"
            "nomad"
            "traveller"
            "moderator"
            "arbiter"
            "'thing-doer'"
            "shepherd"
        ]

    let Create 
            (adverbSource : TermSource)
            (random       : Random) 
            (rewardRange  : float * float) 
            (destinations : Set<Location>) 
            : Job =
        let adverb            = adverbSource() |> Utility.PickRandomly random
        let adjective         = Utility.PickRandomly random adjectives
        let objectName        = Utility.PickRandomly random objectNames
        let name              = Utility.PickRandomly random names
        let personalAdjective = Utility.PickRandomly random personalAdjectives
        let profession        = Utility.PickRandomly random professions
        let destination       = destinations |> Set.toList |> Utility.PickRandomly random 
        let rewardMinimum, rewardMaximum = rewardRange
        {
            FlavorText  = sprintf "please deliver this %s %s %s to %s the %s %s" adverb adjective objectName name personalAdjective profession
            Destination = destination
            Reward      = random.NextDouble() * (rewardMaximum - rewardMinimum) + rewardMinimum
        }
