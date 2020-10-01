namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type ShipmateSingleStatisticSink = string -> ShipmateIdentifier -> (ShipmateStatisticIdentifier * Statistic option) -> unit
type ShipmateSingleStatisticSource = string -> ShipmateIdentifier -> ShipmateStatisticIdentifier -> Statistic option

module ShipmateStatistic =
    type GetContext =
        inherit ServiceContext
        abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    let Get
            (context: ServiceContext)
            (avatarId : string)
            (shipmateId : ShipmateIdentifier)
            (identifier : ShipmateStatisticIdentifier)
            : Statistic option =
        (context :?> GetContext).shipmateSingleStatisticSource avatarId shipmateId identifier

    type PutContext =
        inherit ServiceContext
        abstract member shipmateSingleStatisticSink : ShipmateSingleStatisticSink
    let internal Put
            (context : ServiceContext)
            (avatarId:string)
            (shipmateId: ShipmateIdentifier)
            (identifier:ShipmateStatisticIdentifier)
            (statistic: Statistic option)
            : unit =
        (context :?> PutContext).shipmateSingleStatisticSink avatarId shipmateId (identifier, statistic)

    let Transform 
            (context    : ServiceContext)
            (identifier : ShipmateStatisticIdentifier) 
            (transform  : Statistic -> Statistic option) 
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier) 
            : unit =
        identifier
        |> Get context avatarId shipmateId
        |> Option.iter
            (transform >> Put context avatarId shipmateId identifier)
