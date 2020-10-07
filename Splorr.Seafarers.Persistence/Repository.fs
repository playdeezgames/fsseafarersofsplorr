namespace Splorr.Seafarers.Persistence

open System
open Splorr.Seafarers.DataStore

type StoreRetriever<'TIdentifier, 'TRecord, 'TRetrieveFilter when 'TIdentifier : comparison> = 'TRetrieveFilter -> Result<('TIdentifier * 'TRecord) list, string>
type StoreCreator<'TIdentifier, 'TRecord when 'TIdentifier : comparison> = 'TRecord -> Result<'TIdentifier, string>
type StoreDestroyer<'TIdentifier, 'TDestroyFilter when 'TIdentifier : comparison> = 'TDestroyFilter -> Result<Set<'TIdentifier>, string>
type StoreUpsertor<'TIdentifier, 'TRecord when 'TIdentifier : comparison> = Map<'TIdentifier, 'TRecord> -> Result<Set<'TIdentifier>, string>

type Repository<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter when 'TIdentifier : comparison>
        (creator, retriever, upsertor, destroyer) =
    member _.Creator : StoreCreator<'TIdentifier, 'TRecord> option = creator
    member _.Retriever : StoreRetriever<'TIdentifier, 'TRecord, 'TRetrieveFilter> option = retriever
    member _.Upsertor : StoreUpsertor<'TIdentifier, 'TRecord> option = upsertor
    member _.Destroyer : StoreDestroyer<'TIdentifier, 'TDestroyFilter> option = destroyer
    interface IRepository<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter> with
        member this.Destroy(filter: 'TDestroyFilter): Set<'TIdentifier> = 
            match this.Destroyer with
            | None ->
                raise (Exception "No Destroyer")
            | Some destroyer ->
                match filter |> destroyer with
                | Ok x -> x
                | Error m ->
                    raise (Exception m)

        member this.Update(table: Map<'TIdentifier,'TRecord>): Set<'TIdentifier> = 
            match this.Upsertor with
            | None ->
                raise (Exception "No Upsertor")
            | Some upsertor ->
                match table |> upsertor with
                | Ok x -> x
                | Error m ->
                    raise (Exception m)

        member this.Retrieve(filter: 'TRetrieveFilter): ('TIdentifier * 'TRecord) list = 
            match this.Retriever with
            | None ->
                raise (Exception "No Retriever")
            | Some retriever ->
                match filter |> retriever with
                | Ok x -> x
                | Error m ->
                    raise (Exception m)

        member this.Create(record: 'TRecord): 'TIdentifier = 
            match this.Creator with
            | None ->
                raise (Exception "No Creator")
            | Some creator ->
                match record |> creator with
                | Ok x -> x
                | Error m -> raise (Exception m)

