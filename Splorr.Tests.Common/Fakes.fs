module Splorr.Tests.Common.Fakes

open NUnit.Framework

let Source<'TFilter,'TResult>(message:string,result:'TResult) (_:'TFilter) : 'TResult =
    Assert.Fail(message)
    result
let Sink<'TSunk>(message:string) (_:'TSunk) : unit =
    Assert.Fail(message)
    