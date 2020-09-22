module DeckTests

open Tarot
open Xunit

[<Fact>]
let ``Create.It creates a new deck of cards.`` () =
    let actual =
        Deck.Create()
    Assert.Equal(78, actual |> List.length)