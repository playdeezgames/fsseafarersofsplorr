module TermTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``GetForTermType.It returns a list of terms that apply to a given term type.`` () =
    use connection = SetupConnection()
    try
        let inputTermType = "adverb"
        match connection |> Term.GetForTermType inputTermType with
        | Ok actual     -> 
            Assert.AreEqual(1, actual.Length)
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()


[<Test>]
let ``GetForTermType.It returns an empty list of terms for a non-existent term type.`` () =
    use connection = SetupConnection()
    try
        let inputTermType = "split infinitives"
        match connection |> Term.GetForTermType inputTermType with
        | Ok actual     -> 
            Assert.True(actual.IsEmpty)
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()
