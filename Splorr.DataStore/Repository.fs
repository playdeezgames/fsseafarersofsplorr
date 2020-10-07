namespace Splorr.DataStore

open System

type StoreRetriever<'TIdentifier, 'TRecord, 'TRetrieveFilter when 'TIdentifier : comparison> = 'TRetrieveFilter -> Result<('TIdentifier * 'TRecord) list, string>
type StoreCreator<'TIdentifier, 'TRecord when 'TIdentifier : comparison> = 'TRecord -> Result<'TIdentifier, string>
type StoreDestroyer<'TIdentifier, 'TDestroyFilter when 'TIdentifier : comparison> = 'TDestroyFilter -> Result<Set<'TIdentifier>, string>
type StoreUpsertor<'TIdentifier, 'TRecord when 'TIdentifier : comparison> = Map<'TIdentifier, 'TRecord> -> Result<Set<'TIdentifier>, string>

type Repository<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter when 'TIdentifier : comparison> =
    abstract member Retriever : StoreRetriever<'TIdentifier, 'TRecord, 'TRetrieveFilter> option
    abstract member Creator : StoreCreator<'TIdentifier, 'TRecord> option
    abstract member Destroyer : StoreDestroyer<'TIdentifier, 'TDestroyFilter> option
    abstract member Upsertor : StoreUpsertor<'TIdentifier, 'TRecord> option
    
module Repository =
    let private ProcessRetrieve<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter when 'TIdentifier : comparison> 
            (filter : 'TRetrieveFilter)
            (repository : Repository<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter>)
            : Result<('TIdentifier * 'TRecord) list, string> =
        match repository.Retriever with
        | None ->
            "No Retriever" |> Error
        | Some retriever ->
            filter 
            |> retriever

    let private ProcessCreate<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter when 'TIdentifier : comparison> 
            (record : 'TRecord)
            (repository : Repository<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter>)
            : Result<'TIdentifier, string> =
        match repository.Creator with
        | None ->
            Error "No Creator"
        | Some creator ->
            record
            |> creator

    let private ProcessDestroy<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter when 'TIdentifier : comparison> 
            (filter : 'TDestroyFilter)
            (repository : Repository<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter>)
            : Result<Set<'TIdentifier>, string> =
        match repository.Destroyer with
        | None ->
            Error "No Destroyer"
        | Some destroyer ->
            filter
            |> destroyer

    let private ProcessUpsert<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter when 'TIdentifier : comparison> 
            (table : Map<'TIdentifier, 'TRecord>)
            (repository : Repository<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter>)
            : Result<Set<'TIdentifier>, string> =
        match repository.Upsertor with
        | None ->
            Error "No Upsertor"
        | Some upsertor ->
            table
            |> upsertor

    let Process<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter when 'TIdentifier : comparison> 
            (request    : StoreRequest<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter>) 
            (repository : Repository<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter>) 
            : Result<StoreResponse<'TIdentifier, 'TRecord>, string> =
        match request with
        | Retrieve filter ->
            ProcessRetrieve filter repository
            |> Result.map Retrieved
        | Create record ->
            ProcessCreate record repository
            |> Result.map Created
        | Destroy filter ->
            ProcessDestroy filter repository
            |> Result.map Destroyed
        | Upsert table ->
            ProcessUpsert table repository
            |> Result.map Upserted
