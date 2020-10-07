module DestroyTests

open System
open Xunit
open Splorr.DataStore

[<Fact>]
let ``Process.It returns an error message when given a destroy request and there is no destroyer functor.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (None,
            None,
            None,
            None)
    let request : Types.TestStoreRequest = Set.empty |> Set.add Fixtures.Dummy.InvalidId |> Destroy
    let expected : Result<Types.TestStoreResponse, string> =
        Error "No Destroyer"
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)

[<Fact>]
let ``Process.It returns an empty set when given a destroy request and there no records were destroyed.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (None,
            None,
            Some (Fixtures.Dummy.Destroyer dummy),
            None)
    let request : Types.TestStoreRequest = Set.empty |> Set.add Fixtures.Dummy.InvalidId |> Destroy
    let expected : Result<Types.TestStoreResponse, string> =
        Set.empty |> Destroyed |> Ok
    let originalRecordCount = dummy.Store.Count
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)
    Assert.Equal(originalRecordCount, dummy.Store.Count)


[<Fact>]
let ``Process.It returns a set of destroyed record identifiers when given a destroy request and some records were destroyed.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (None,
            None,
            Some (Fixtures.Dummy.Destroyer dummy),
            None)
    let request : Types.TestStoreRequest = Set.empty |> Set.add Fixtures.Dummy.InvalidId |> Set.add Fixtures.Dummy.Id |> Destroy
    let expected : Result<Types.TestStoreResponse, string> =
        Set.empty |> Set.add Fixtures.Dummy.Id |> Destroyed |> Ok
    let originalRecordCount = dummy.Store.Count
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)
    Assert.Equal(originalRecordCount - 1, dummy.Store.Count)

