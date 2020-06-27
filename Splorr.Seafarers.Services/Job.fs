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
        ]
    let private objectNames = 
        [
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
        let profession = ChooseRandomTerm random professions
        {
            FlavorText = sprintf "please deliver this %s %s %s to %s the %s" adverb adjective objectName name profession
            Destination = 
                destinations
                |> Set.toList
                |> List.sortBy (fun _ -> random.Next())
                |> List.head
            Reward = random.NextDouble() * (rewardMaximum - rewardMinimum) + rewardMinimum
        }
