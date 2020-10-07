module UpsertTests

open System
open Xunit
open Splorr.DataStore

[<Fact>]
let ``Process.It returns an error message when given a upsert request and there is no upsertor functor.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (None,
            None,
            None,
            None)
    let request : Types.TestStoreRequest = Map.empty |> Upsert
    let expected : Result<Types.TestStoreResponse, string> =
        Error "No Upsertor"
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)

[<Fact>]
let ``Process.It returns empty set when given an empty table to upsert.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (None,
            None,
            None,
            Some (Fixtures.Dummy.Upsertor dummy))
    let request : Types.TestStoreRequest = Map.empty |> Upsert
    let expected : Result<Types.TestStoreResponse, string> =
        Set.empty |> Upserted |> Ok
    let originalRecordCount = dummy.Store.Count
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)
    Assert.Equal(originalRecordCount, dummy.Store.Count)


[<Fact>]
let ``Process.It returns a set of upserted when given an table to upsert.`` () =
    let dummy = Fixtures.Dummy()
    Fixtures.Dummy.Reset dummy
    let repository = 
        Types.TestRetrieverRepository
            (None,
            None,
            None,
            Some (Fixtures.Dummy.Upsertor dummy))
    let request : Types.TestStoreRequest = 
        Map.empty 
        |> Map.add Fixtures.Dummy.Id {Types.TestRecord.name="changed"}
        |> Map.add Fixtures.Dummy.UpsertId {Types.TestRecord.name="upserted"}
        |> Upsert
    let expected : Result<Types.TestStoreResponse, string> =
        Set.empty
        |> Set.add Fixtures.Dummy.Id
        |> Set.add Fixtures.Dummy.UpsertId
        |> Upserted
        |> Ok
    let originalRecordCount = dummy.Store.Count
    let actual : Result<Types.TestStoreResponse, string> = 
        Repository.Process request repository
    Assert.Equal(expected, actual)
    Assert.Equal(originalRecordCount + 1, dummy.Store.Count)

