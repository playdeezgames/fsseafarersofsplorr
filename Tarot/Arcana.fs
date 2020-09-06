namespace Tarot

type Arcana =
    | Fool=0
    | Magician=1
    | HighPriestess=2
    | Empress=3
    | Emperor=4
    | Hierophant=5
    | Lovers=6
    | Chariot=7
    | Strength=8
    | Hermit=9
    | WheelOfFortune=10
    | Justice=11
    | HangedMan=12
    | Death=13
    | Temperance=14
    | Devil=15
    | Tower=16
    | Star=17
    | Moon=18
    | Sun=19
    | Judgement=20
    | World=21

module Arcana =
    let IsFirstGreater (first:Arcana, second: Arcana) : bool =
        first > second

