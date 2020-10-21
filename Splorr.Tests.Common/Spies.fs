module Splorr.Tests.Common.Spies

open NUnit.Framework

let Source<'TFilter,'TResult>(flag:bool ref,result:'TResult) (_:'TFilter) : 'TResult =
    flag := true
    result

let Sink(flag:bool ref)(sunk:'TSunk) : unit = Source<'TSunk, unit> (flag, ()) sunk

let Expect(flag:bool ref, expected:'TSunk)(sunk:'TSunk) : unit =
    Assert.AreEqual(expected, sunk)
    Sink flag sunk

