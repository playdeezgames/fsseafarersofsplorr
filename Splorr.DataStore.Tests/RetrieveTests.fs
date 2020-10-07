module RetrieveTests

open System
open Xunit
open Splorr.DataStore

[<Fact>]
let ``Process.It returns error message when given a retrieve request and there is no retriever for the repository.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (None,
            None,
            None,
            None)
    let request : Types.TestStoreRequest = Fixtures.Dummy.Id |> Types.ById |> Retrieve
    let expected : Result<Types.TestStoreResponse, string> =
        Error "No Retriever"
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)

[<Fact>]
let ``Process.It returns empty list when given a retrieve request and there are no records that match the filter.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (Some (Fixtures.Dummy.Retriever dummy),
            None,
            None,
            None)
    let request : Types.TestStoreRequest = Fixtures.Dummy.InvalidId |> Types.ById |> Retrieve
    let expected : Result<Types.TestStoreResponse, string> =
        [
        ]
        |> Retrieved
        |> Ok
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)


[<Fact>]
let ``Process.It returns filtered list when given a retrieve request and there are records that match the filter.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (Some (Fixtures.Dummy.Retriever dummy),
            None,
            None,
            None)
    let request : Types.TestStoreRequest = Fixtures.Dummy.Id |> Types.ById |> Retrieve
    let expected : Result<Types.TestStoreResponse, string> =
        [
            (Fixtures.Dummy.Id, dummy.Store.[Fixtures.Dummy.Id])
        ]
        |> Retrieved
        |> Ok
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)
        