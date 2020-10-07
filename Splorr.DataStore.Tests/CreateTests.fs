module CreateTests

open System
open Xunit
open Splorr.DataStore

[<Fact>]
let ``Process.It returns an error message when given a create request and there is no creator functor.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (None,
            None,
            None,
            None)
    let request : Types.TestStoreRequest = {Types.TestRecord.name = "new"}  |> Create 
    let expected : Result<Types.TestStoreResponse, string> =
        Error "No Creator"
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)


[<Fact>]
let ``Process.It returns the new record's identifier when given a create request and there is a creator functor.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (None,
            Some (Fixtures.Dummy.Creator dummy),
            None,
            None)
    let request : Types.TestStoreRequest = {Types.TestRecord.name = "new"}  |> Create 
    let expected : Result<Types.TestStoreResponse, string> =
        2 |> Created |> Ok
    let originalRecordCount = dummy.Store.Count
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)
    Assert.Equal(originalRecordCount + 1, dummy.Store.Count)

    