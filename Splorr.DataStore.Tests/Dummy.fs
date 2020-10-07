namespace Fixtures

open System

type Dummy() =
    member val Store : Map<Types.TestIdentifier, Types.TestRecord> = Map.empty with get, set
    member x.Apply (store:Map<Types.TestIdentifier, Types.TestRecord>) =
        x.Store <- store
    

module Dummy = 
    let Id : Types.TestIdentifier = 1
    let UpsertId : Types.TestIdentifier = 2
    let InvalidId : Types.TestIdentifier = 0
    let private StoreBoilerplate : Map<Types.TestIdentifier, Types.TestRecord> =
        Map.empty
        |> Map.add Id { name = "one" }
    let Retriever (dummy:Dummy) (filter:Types.TestFilter) : Result<(Types.TestIdentifier * Types.TestRecord) list, string> =
        match filter with
        | Types.ById identifier ->
            dummy.Store
            |> Map.filter 
                (fun k _ -> k = identifier)
            |> Map.toList
            |> Ok
    let Reset (dummy:Dummy) =
        dummy.Apply StoreBoilerplate
    let Creator 
            (dummy:Dummy)
            (record:Types.TestRecord)
            : Result<Types.TestIdentifier, string> =
        let identifier =
            dummy.Store
            |> Map.toList
            |> List.map fst
            |> List.max
            |> (+) 1
        dummy.Store 
        |> Map.add identifier record 
        |> dummy.Apply
        identifier
        |> Ok

    let Destroyer
            (dummy:Dummy)
            (identifiers : Set<Types.TestIdentifier>)
            : Result<Set<Types.TestIdentifier>, string> =
        identifiers
        |> Set.fold
            (fun result identifier->
                if dummy.Store.ContainsKey identifier then
                    dummy.Store |> Map.remove identifier |> dummy.Apply
                    result
                    |> Set.add identifier
                else
                    result) Set.empty
        |> Ok

    let Upsertor
            (dummy : Dummy)
            (table : Map<Types.TestIdentifier, Types.TestRecord>)
            : Result<Set<Types.TestIdentifier>, string> =
        table
        |> Map.fold
            (fun result identifier record ->
                dummy.Store |> Map.add identifier record |> dummy.Apply
                result
                |> Set.add identifier) Set.empty
        |> Ok
