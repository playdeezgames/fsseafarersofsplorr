namespace Splorr.Seafarers.Persistence

open System
open Splorr.DataStore

module Message =
    type Identifier = int64

    type Record =
        {
            avatarId : string
            text : string
            foregroundColor : ConsoleColor
            backgroundColor : ConsoleColor
        }

    type Repository =
        inherit Repository<Identifier, Record, string, string>

    let ClearForAvatar 
            (repository : Repository)
            (avatarId   : string)
            : unit = 
        match Splorr.DataStore.Repository.Process (avatarId |> Destroy) repository with
        | Ok (Destroyed _) ->
            ()
        | Error m ->
            raise (Exception m)
        | _ ->
            raise (Exception "Unexpected Process Result")

    let private AddOneForAvatar
            (repository : Repository)
            (message : Record) 
            : unit =
        match Splorr.DataStore.Repository.Process (message |> Create) repository with
        | Ok (Created _) ->
            ()
        | Error m ->
            raise (Exception m)
        | _ ->
            raise (Exception "Unexpected Process Result")


    let AddForAvatar
            (repository : Repository)
            (messages : Record list)
            : unit =
        messages
        |> List.iter (AddOneForAvatar repository)

    let GetForAvatar
            (repository : Repository)
            (avatarId   : string)
            : Record list =
        match Splorr.DataStore.Repository.Process (avatarId |> Retrieve) repository with
        | Ok (Retrieved x) ->
            x
            |> List.map snd
        | Error m ->
            raise (Exception m)
        | _ ->
            raise (Exception "Unexpected Process Result")


            


