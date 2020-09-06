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

    let private rankText =
        Map.empty
        |> Map.add Rank.Ace    "|   Ace   |"
        |> Map.add Rank.Deuce  "|  Deuce  |"
        |> Map.add Rank.Three  "|  Three  |"
        |> Map.add Rank.Four   "|   Four  |"
        |> Map.add Rank.Five   "|   Five  |"
        |> Map.add Rank.Six    "|   Six   |"
        |> Map.add Rank.Seven  "|  Seven  |"
        |> Map.add Rank.Eight  "|  Eight  |"
        |> Map.add Rank.Nine   "|   Nine  |"
        |> Map.add Rank.Ten    "|   Ten   |"
        |> Map.add Rank.Page   "|   Page  |"
        |> Map.add Rank.Knight "|  Knight |"
        |> Map.add Rank.Queen  "|  Queen  |"
        |> Map.add Rank.King   "|   King  |"

    let private suitText =
        Map.empty
        |> Map.add Wands     "|  Wands  |"
        |> Map.add Swords    "| Swords  |"
        |> Map.add Cups      "|   Cups  |"
        |> Map.add Pentacles "|Pentacles|"

    let ToText (card:Card) : string list =
        match card with
        | Major Arcana.Fool ->
            [
                "+---------+"
                "|  %-/   0|"
                "|  |/     |"
                "|  / ^    |"
                "|   / \   |"
                "|  /   \  |"
                "| /     \ |"
                "|/_______\|"
                "+---------+"
            ]
        | Major Arcana.Magician ->
            [
                "+---------+"
                "|   ___  1|"
                "|   \ /   |"
                "|    V    |"
                "|         |"
                "| /\   /\ |"
                "|/__\ /__\|"
                "|         |"
                "+---------+"
            ]
        | Major Arcana.HighPriestess ->
            [
                "+---------+"
                "|   \--/ 2|"
                "|    \/   |"
                "|   / \   |"
                "|  /   \  |"
                "| /   _ \ |"
                "|/___\ /_\|"
                "|     V   |"
                "+---------+"
            ]
        | Major Arcana.Empress ->
            [
                "+---------+"
                "|        3|"
                "| _     _ |"
                "| V  _  V |"
                "| ^  V  ^ |"
                "|/_\ ^ /_\|"
                "|   /_\   |"
                "|         |"
                "+---------+"
            ]
        | Major Arcana.Emperor ->
            [
                "+---------+"
                "|        4|"
                "|         |"
                "|    ^    |"
                "| /\/ \/\ |"
                "|/_/   \_\|"
                "| /_____\ |"
                "|         |"
                "+---------+"
            ]
        | Major Arcana.Hierophant ->
            [
                "+---------+"
                "|  \---/ 5|"
                "|   \ /   |"
                "|    /    |"
                "|   / \   |"
                "|  /   \  |"
                "| /     \ |"
                "|/_______\|"
                "+---------+"
            ]
        | Major Arcana.Lovers -> 
            [
                "+---------+"
                "|    ^   6|"
                "|   / \   |"
                "|  / ^ \  |"
                "| / /=\ \ |"
                "|/_/===\_\|"
                "| /=====\ |"
                "|/=======\|"
                "+---------+"
            ]
        | Major Arcana.Chariot ->
            [
                "+---------+"
                "|        7|"
                "|\-------/|"
                "| \     / |"
                "|  \   /  |"
                "|   \ /   |"
                "| /\ v /\ |"
                "|/__\ /__\|"
                "+---------+"
            ]
        | Major Arcana.Strength ->
            [
                "+---------+"
                "|    ^   8|"
                "|   /=\   |"
                "|  /===\  |"
                "| /=====\ |"
                "|/=======\|"
                "|    ^    |"
                "|   /_\   |"
                "+---------+"
            ]
        | Major Arcana.Hermit ->
            [
                "+---------+"
                "|        9|"
                "|         |"
                "|    ^    |"
                "|   / \   |"
                "|  /___\  |"
                "|         |"
                "|         |"
                "+---------+"
            ]
        | Major Arcana.WheelOfFortune ->
            [
                "+---------+"
                "|       10|"
                "|   .-.   |"
                "|  /   \  |"
                "| |     | |"
                "| |     | |"
                "|  \   /  |"
                "|   `-`   |"
                "+---------+"
            ]
        | Major Arcana.Justice ->
            [
                "+---------+"
                "|       11|"
                "|    /\   |"
                "|   /  \  |"
                "|  /___-- |"
                "| --    ^ |"
                "| ^    /_\|"
                "|/=\      |"
                "+---------+"
            ]
        | Major Arcana.HangedMan ->
            [
                "+---------+"
                "|    ^  12|"
                "|   / \   |"
                "|  /   \  |"
                "| /     \ |"
                "|/____/|_\|"
                "|    < |  |"
                "|     \|  |"
                "+---------+"
            ]
        | Major Arcana.Death ->
            [
                "+---------+"
                "|       13|"
                "|  _____  |"
                "|  \===/  |"
                "|   \=/   |"
                "|    V    |"
                "|         |"
                "|         |"
                "+---------+"
            ]
        | Major Arcana.Temperance ->
            [
                "+---------+"
                "|  _____14|"
                "|  \   /  |"
                "|   \ /   |"
                "|    X    |"
                "|   / \   |"
                "|  /___\  |"
                "|         |"
                "+---------+"

            ]
        | Major Arcana.Devil ->
            [
                "+---------+"
                "|       15|"
                "|__/___\__|"
                "|\/     \/|"
                "|  \   /  |"
                "|   \ /   |"
                "|    V    |"
                "|         |"
                "+---------+"
            ]
        | Major Arcana.Tower ->
            [
                "+---------+"
                "|  /    16|"
                "| /|___*  |"
                "|/-%  / * |"
                "|  \ / * *|"
                "|   % \   |"
                "|  /   \  |"
                "| /_____\ |"
                "+---------+"
            ]
        | Major Arcana.Star ->
            [
                "+---------+"
                "|       17|"
                "|  *-^-*  |"
                "|  |/*\|  |"
                "| <|***|> |"
                "|  |\*/|  |"
                "|  *-v-*  |"
                "|         |"
                "+---------+"
            ]
        | Major Arcana.Moon ->
            [
                "+---------+"
                "|       18|"
                "|    ^    |"
                "|   /-----|"
                "|  /**\ / |"
                "| /****v  |"
                "|/*******\|"
                "|         |"
                "+---------+"
            ]
        | Major Arcana.Sun ->
            [
                "+---------+"
                "|       19|"
                "|    ^    |"
                "|   /-----|"
                "|  /  \*/ |"
                "| /    v  |"
                "|/_______\|"
                "|         |"
                "+---------+"
            ]
        | Major Arcana.Judgement ->
            [
                "+---------+"
                "|_______20|"
                "|\=======/|"
                "| \=====/ |"
                "|  \===/  |"
                "|   \=/   |"
                "| /\ v /\ |"
                "|/__\ /__\|"
                "+---------+"
            ]
        | Major Arcana.World ->
            [
                "+---------+"
                "|       21|"
                "|    ^    |"
                "|   / \   |"
                "|  /___\  |"
                "| /\===/\ |"
                "|/  \=/  \|"
                "|____v____|"
                "+---------+"

            ]
        | Minor (suit, rank) ->
            [
                "+---------+"
                "|         |"
                rankText.[rank]
                "|         |"
                "|   of    |"
                "|         |"
                suitText.[suit]
                "|         |"
                "+---------+"
            ]
        | _ ->
            [
                "+---------+"
                "|         |"
                "|         |"
                "|         |"
                "|         |"
                "|         |"
                "|         |"
                "|         |"
                "+---------+"
            ]