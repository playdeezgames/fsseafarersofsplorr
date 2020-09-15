namespace Tarot

module Deck =
    let private ranks =
        System.Enum.GetValues(typedefof<Rank>)
        :?> Rank []
        |> Array.toList
    let private makeSuitOf (suit:Suit) =
        List.map (fun x -> Minor (suit, x))
    let private wands = ranks |> makeSuitOf Wands
    let private cups =ranks |> makeSuitOf Cups
    let private swords =ranks |> makeSuitOf Swords
    let private pentacles =ranks |> makeSuitOf Pentacles
    let private arcanaCards =
        System.Enum.GetValues(typedefof<Arcana>)
        :?> Arcana []
        |> Array.toList
        |> List.map Major
    let Create () : Card list =
        [
            arcanaCards
            wands
            cups
            swords
            pentacles
        ]
        |> List.reduce List.append

