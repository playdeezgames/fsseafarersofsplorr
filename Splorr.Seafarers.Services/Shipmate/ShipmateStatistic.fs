namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module ShipmateStatistic =

    type ShipmateSingleStatisticSource = string * ShipmateIdentifier * ShipmateStatisticIdentifier -> Statistic option
    type GetContext =
        abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource ref
    let internal Get
            (context: CommonContext)
            (avatarId : string)
            (shipmateId : ShipmateIdentifier)
            (identifier : ShipmateStatisticIdentifier)
            : Statistic option =
        (context :?> GetContext).shipmateSingleStatisticSource.Value (avatarId, shipmateId, identifier)

    let internal GetHealth
            (context : CommonContext)
            (avatarId : string)
            (shipmateId : ShipmateIdentifier)
            : Statistic =
        Get context avatarId shipmateId ShipmateStatisticIdentifier.Health 
        |> Option.get

    let internal GetTurn
            (context : CommonContext)
            (avatarId : string)
            (shipmateId : ShipmateIdentifier)
            : Statistic =
        Get context avatarId shipmateId ShipmateStatisticIdentifier.Turn 
        |> Option.get


    type ShipmateSingleStatisticSink = string * ShipmateIdentifier * ShipmateStatisticIdentifier * Statistic option -> unit
    type PutContext =
        abstract member shipmateSingleStatisticSink : ShipmateSingleStatisticSink ref
    let internal Put
            (context : CommonContext)
            (avatarId:string)
            (shipmateId: ShipmateIdentifier)
            (identifier:ShipmateStatisticIdentifier)
            (statistic: Statistic option)
            : unit =
        (context :?> PutContext).shipmateSingleStatisticSink.Value (avatarId, shipmateId, identifier, statistic)

    let internal Transform 
            (context    : CommonContext)
            (identifier : ShipmateStatisticIdentifier) 
            (transform  : Statistic -> Statistic option) 
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier) 
            : unit =
        identifier
        |> Get context avatarId shipmateId
        |> Option.iter
            (transform >> Put context avatarId shipmateId identifier)
