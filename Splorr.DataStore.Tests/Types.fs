module Types

open Splorr.DataStore

type TestIdentifier = int
type TestRecord =
    {
        name : string
    }
type TestFilter =
    | ById of TestIdentifier

type TestDestroyFilter = Set<TestIdentifier>

type TestRetrieverRepository
        (retriever,
        creator,
        destroyer,
        upsertor) =
    interface Repository<TestIdentifier, TestRecord, TestFilter, TestDestroyFilter> with
        member this.Upsertor: StoreUpsertor<TestIdentifier,TestRecord> option = upsertor
        member this.Destroyer: StoreDestroyer<TestIdentifier, TestDestroyFilter> option = destroyer
        member this.Creator: StoreCreator<TestIdentifier,TestRecord> option = creator
        member this.Retriever: StoreRetriever<TestIdentifier, TestRecord, TestFilter> option = retriever

type TestStoreRequest = StoreRequest<TestIdentifier, TestRecord, TestFilter, TestDestroyFilter>
type TestStoreResponse = StoreResponse<TestIdentifier, TestRecord>



