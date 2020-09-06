module CardTests

open Tarot
open Xunit

[<Fact>]
let ``IsFirstGreater.It indicates whether or the first given card is greater than the second given card.`` () =
    [
        ((Wands, Rank.King)|>Minor,(Swords, Rank.King)|>Minor,false)
        ((Wands, Rank.King)|>Minor,(Swords, Rank.Ace)|>Minor,true)
        ((Wands, Rank.Ace)|>Minor,(Swords, Rank.King)|>Minor,false)
        (Arcana.Fool |> Major,(Swords, Rank.King)|>Minor,true)
        ((Wands, Rank.King)|>Minor,Arcana.Tower |> Major,false)
        ((Wands, Rank.Ace)|>Minor,Arcana.Fool |> Major,false)
    ]
    |> List.iter
        (fun (first, second, expected) ->
            let actual = Card.IsFirstGreater (first, second)
            Assert.Equal(expected, actual))