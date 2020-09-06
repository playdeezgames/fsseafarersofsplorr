module RankTests

open Tarot
open Xunit

[<Theory>]
[<InlineData(Rank.King, Rank.Ace, true)>]
[<InlineData(Rank.King, Rank.Queen, true)>]
[<InlineData(Rank.Knight, Rank.Queen, false)>]
[<InlineData(Rank.Seven, Rank.Seven, false)>]
[<InlineData(Rank.Ace, Rank.King, false)>]
let ``IsFirstGreater.It indicates if the first given rank is higher than the second given rank.`` (first:Rank, second:Rank, expected:bool) =
    let actual = Rank.IsFirstGreater (first, second)
    Assert.Equal(expected, actual)
    
