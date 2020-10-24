namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module WorldExport = 
    type GameDataSink = string -> string option
    type SaveContext =
        abstract member gameDataSink : GameDataSink ref
    let internal Save
            (context : CommonContext)
            (filename : string)
            (avatarId: string)
            : unit =
        match (context :?> SaveContext).gameDataSink.Value filename with
        | Some s ->
            AvatarMessages.Add context [ s |> sprintf "Saved game to '%s'." ] avatarId
        | _ ->
            AvatarMessages.Add context [ "Could not save game!" ] avatarId


