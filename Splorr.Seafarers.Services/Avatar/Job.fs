namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

type AvatarJobSource = string -> Job option

module AvatarJob =
    type AbandonContext =
        inherit ServiceContext
        abstract member avatarJobSink : AvatarJobSink
        abstract member avatarJobSource : AvatarJobSource
    let Abandon 
            (context : ServiceContext)
            (avatarId: string)
            : unit =
        let context = context :?> AbandonContext
        let reputationCostForAbandoningAJob = -1.0
        avatarId
        |> context.avatarJobSource
        |> Option.iter
            (fun _ -> 
                avatarId
                |> AvatarShipmates.SetReputation 
                    context
                    ((AvatarShipmates.GetReputation 
                        context
                        avatarId) + 
                            reputationCostForAbandoningAJob) 
                avatarId
                |> Avatar.IncrementMetric 
                    context
                    Metric.AbandonedJob
                context.avatarJobSink avatarId None)

    type GetContext =
        inherit ServiceContext
        abstract member avatarJobSource : AvatarJobSource
    let Get
            (context : ServiceContext)
            (avatarId : string)
            : Job option =
        (context :?> GetContext).avatarJobSource avatarId

    type CompleteContext =
        inherit ServiceContext
        abstract member avatarJobSink : AvatarJobSink
        abstract member avatarJobSource : AvatarJobSource
    let Complete
            (context : ServiceContext)
            (avatarId:string)
            : unit =
        let context = context :?> CompleteContext
        match avatarId |> context.avatarJobSource with
        | Some job ->
            AvatarShipmates.SetReputation 
                context
                ((AvatarShipmates.GetReputation 
                    context
                    avatarId) + 
                        1.0)
                avatarId
            Shipmate.TransformStatistic 
                context
                ShipmateStatisticIdentifier.Money 
                (Statistic.ChangeCurrentBy job.Reward >> Some)
                avatarId
                Primary
            avatarId
            |> AvatarMetric.Add 
                context
                Metric.CompletedJob 
                1UL
            None
            |> context.avatarJobSink avatarId
        | _ -> ()
