namespace Tarot

type Card = 
    | Minor of Suit * Rank
    | Major of Arcana

module Card =
    let IsFirstGreater (first : Card, second:Card) : bool = 
        match first, second with
        | Major _, Minor _ -> true
        | Major x, Major y -> Arcana.IsFirstGreater(x, y)
        | Minor (_,x), Minor (_,y) -> Rank.IsFirstGreater(x, y)
        | _ -> false