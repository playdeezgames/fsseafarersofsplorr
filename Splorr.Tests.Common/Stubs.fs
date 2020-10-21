module Splorr.Tests.Common.Stubs

let Source<'TFilter,'TResult>(result:'TResult) (_:'TFilter) : 'TResult =
    result
let Sink(sunk:'TSunk) : unit = Source<'TSunk, unit> (()) sunk

