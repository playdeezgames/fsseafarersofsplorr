module Splorr.Tests.Common.Spies

open NUnit.Framework

let Source<'TFilter,'TResult>(flag:bool ref,result:'TResult) (_:'TFilter) : 'TResult =
    flag := true
    result

let SourceTable<'TFilter, 'TResult when 'TFilter:comparison>
        (counter:uint64 ref, resultTable: Map<'TFilter,'TResult>) 
        (filter:'TFilter) 
        : 'TResult =
    counter := counter.Value + 1UL
    Assert.IsTrue(resultTable.ContainsKey filter,filter |> sprintf "did not find '%A' in result table")
    resultTable.[filter]

let SourceHook<'TFilter, 'TResult>(counter : uint64 ref, hook:'TFilter->'TResult) (filter : 'TFilter) : 'TResult =
    counter := counter.Value + 1UL
    hook filter

let SinkHook<'TSunk>(counter:uint64 ref, hook : 'TSunk->unit) =
    SourceHook<'TSunk, unit>(counter, hook)

let Sink(flag:bool ref)(sunk:'TSunk) : unit = Source<'TSunk, unit> (flag, ()) sunk

let Expect(flag:bool ref, expected:'TSunk)(sunk:'TSunk) : unit =
    Assert.AreEqual(expected, sunk)
    Sink flag sunk

let SinkCounter(counter:uint64 ref)(_:'TSunk):unit =
    counter := counter.Value + 1UL

let ExpectSet<'TSunk when 'TSunk:comparison>(counter : uint64 ref, sunkSet : Set<'TSunk>) (sunk:'TSunk) : unit =
    counter := counter.Value + 1UL
    Assert.IsTrue(sunkSet.Contains(sunk),sunk |> sprintf "did not expect '%A' to be sunk")

let Validate<'TSunk>(counter:uint64 ref, validator: 'TSunk -> bool) (sunk:'TSunk) : unit =
    counter := counter.Value + 1UL
    Assert.IsTrue(validator sunk,sunk |> sprintf "could not validate '%A'")


