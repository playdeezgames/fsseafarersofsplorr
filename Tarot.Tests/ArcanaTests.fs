module ArcanaTests

open Tarot
open Xunit

[<Theory>]
[<InlineData(Arcana.World, Arcana.Fool, true)>]
[<InlineData(Arcana.Fool, Arcana.World, false)>]
let ``IsGreaterThan.It indicates whether or not the first given arcana is greater than the second given arcana.`` (first : Arcana, second: Arcana, expected : bool) =
    let actual = Arcana.IsFirstGreater (first, second)
    Assert.Equal(expected, actual)
    


