namespace Tarot

type Rank =
    | Ace=1
    | Deuce=2
    | Three=3
    | Four=4
    | Five=5
    | Six=6
    | Seven=7
    | Eight=8
    | Nine=9
    | Ten=10
    | Page=11
    | Knight=12
    | Queen=13
    | King=14

module Rank =
    let IsFirstGreater (first:Rank, second:Rank) : bool =
        first > second
