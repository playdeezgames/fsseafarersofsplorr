namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module IslandJob =

    type IslandJobSource = Location -> Job list
    type GetContext =
        abstract member islandJobSource : IslandJobSource ref
    let internal Get
            (context : CommonContext)
            (location: Location)
            : Job list =
        (context :?> GetContext).islandJobSource.Value location

    type IslandJobSink = Location * Job -> unit
    type AddContext =
        abstract member islandJobSink : IslandJobSink ref
    let private Add
            (context : CommonContext)
            (location : Location)
            (job : Job)
            : unit =
        (context :?> AddContext).islandJobSink.Value (location, job)

    let internal Generate 
            (context      : CommonContext)
            (destinations : Set<Location>) 
            (location     : Location)
            : unit =
        if (Get context location).IsEmpty && not destinations.IsEmpty then
            Job.Create 
                context 
                destinations
            |> Add context location

    type IslandJobPurger = Location * uint32 -> unit
    type PurgeContext =
        abstract member islandJobPurger : IslandJobPurger ref
    let internal Purge
            (context  : CommonContext)
            (location : Location)
            (index    : uint32)
            : unit =
        (context :?> PurgeContext).islandJobPurger.Value (location, index)
