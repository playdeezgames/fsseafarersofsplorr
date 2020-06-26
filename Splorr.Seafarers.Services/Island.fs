namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Island =
    let Create() : Island =
        {
            Name = ""
            LastVisit = None
            VisitCount = None
            Jobs = []
        }

    let SetName (name:string) (island:Island) : Island =
        {island with Name = name}

    let GetDisplayName (island:Island) : string =
        match island.VisitCount with
        | Some _ ->
            island.Name
        | None ->
            "????"
    
    let AddVisit (turn: uint32) (island:Island) : Island =
        match island.VisitCount, island.LastVisit with
        | None, None ->
            {island with 
                VisitCount = Some 1u
                LastVisit = Some turn}
        | Some x, None ->
            {island with
                VisitCount = Some (x+1u)
                LastVisit = Some turn}
        | Some x, Some t when t<turn ->
            {island with
                VisitCount = Some (x+1u)
                LastVisit = Some turn}
        | _ -> island

    let GenerateJobs (random:System.Random) (rewardRange:float*float) (destinations:Set<Location>) (island:Island) : Island =
        if island.Jobs.IsEmpty && not destinations.IsEmpty then
            {island with Jobs = [Job.Create random rewardRange destinations] |> List.append island.Jobs}
        else
            island

    let RemoveJob (index:uint32) (island:Island) : Island * (Job option) =
        let taken, left =
            island.Jobs
            |> List.zip [1u..(island.Jobs.Length |> uint32)]
            |> List.partition 
                (fun (idx, _)->idx=index)
        {island with Jobs = left |> List.map snd}, taken |> List.map snd |> List.tryHead
