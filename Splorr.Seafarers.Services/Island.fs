namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Island =
    let Create() : Island =
        {
            Name = ""
            LastVisit = None
            VisitCount = None
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
