namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models
open Splorr.Common



module AvatarJob =
    type AvatarJobSource = string -> Job option

    type GetContext =
        abstract member avatarJobSource : AvatarJobSource ref
    let internal Get
            (context : CommonContext)
            (avatarId : string)
            : Job option =
        (context :?> GetContext).avatarJobSource.Value avatarId

    let internal Abandon 
            (context : CommonContext)
            (avatarId: string)
            : unit =
        let reputationCostForAbandoningAJob = -1.0
        avatarId
        |> Get context
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
                Avatar.SetJob context avatarId None)

    let internal Complete
            (context : CommonContext)
            (avatarId:string)
            : unit =
        match avatarId |> Get context with
        | Some job ->
            AvatarShipmates.SetReputation 
                context
                ((AvatarShipmates.GetReputation 
                    context
                    avatarId) + 
                        1.0)
                avatarId
            ShipmateStatistic.Transform 
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
            |> Avatar.SetJob context avatarId
        | _ -> ()
