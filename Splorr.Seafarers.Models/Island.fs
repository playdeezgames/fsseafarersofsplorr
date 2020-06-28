namespace Splorr.Seafarers.Models

type Island =
    {
        Name: string
        VisitCount: uint32 option
        LastVisit: uint32 option
        Jobs: Job list
        Markets: Map<Commodity, Market>
    }

