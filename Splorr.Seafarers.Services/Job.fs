namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Job =
    let private adverbs = 
        [
            "extremely"
            "woefully"
            "majestically"
            "surprisingly"
            "tenderly"
            "carelessly"
            "greatly"
            "reassuringly"
            "interestingly"
            "officially"
            "accidentally"
            "jaggedly"
            "carefully"
            "thoughtfully"
            "shodilly"
            "amazingly"
            "recklessly"
            "unashamedly"
        ]

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
            "incridible"
            "cheesy"
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
            "Sally"
            "Susan"
            "Muffin"
            "Beardy"
            "Jimmy"
            "Montgomery"
            "James"
            "Davin"
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
        ]
    let private ChooseRandomTerm (random:System.Random) (terms:string list) : string =
        terms
        |> List.sortBy (fun _ -> random.Next())
        |> List.head

    let Create (random:System.Random) (rewardMinimum:float, rewardMaximum: float) (destinations:Set<Location>) : Job =
        let adverb = ChooseRandomTerm random adverbs
        let adjective = ChooseRandomTerm random adjectives
        let objectName = ChooseRandomTerm random objectNames
        let name = ChooseRandomTerm random names
        let personalAdjective = ChooseRandomTerm random personalAdjectives
        let profession = ChooseRandomTerm random professions
        {
            FlavorText = sprintf "please deliver this %s %s %s to %s the %s %s" adverb adjective objectName name personalAdjective profession
            Destination = 
                destinations
                |> Set.toList
                |> List.sortBy (fun _ -> random.Next())
                |> List.head
            Reward = random.NextDouble() * (rewardMaximum - rewardMinimum) + rewardMinimum
        }
