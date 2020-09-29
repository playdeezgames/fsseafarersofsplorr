namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type IslandJobSource = Location -> Job list
type IslandJobSink = Location -> Job -> unit
type IslandJobPurger = Location -> uint32 -> unit

module IslandJob =
    type GetContext =
        inherit ServiceContext
        abstract member islandJobSource : IslandJobSource
    let Get
            (context : ServiceContext)
            (location: Location)
            : Job list =
        (context :?> GetContext).islandJobSource location

    type AddContext =
        inherit ServiceContext
        abstract member islandJobSink : IslandJobSink
    let private Add
            (context : ServiceContext)
            (location : Location)
            (job : Job)
            : unit =
        (context :?> AddContext).islandJobSink location job

    let Generate 
            (context      : ServiceContext)
            (destinations : Set<Location>) 
            (location     : Location)
            : unit =
        if (Get context location).IsEmpty && not destinations.IsEmpty then
            Job.Create 
                context 
                destinations
            |> Add context location

    type PurgeContext =
        abstract member islandJobPurger : IslandJobPurger
    let Purge
            (context  : ServiceContext)
            (location : Location)
            (index    : uint32)
            : unit =
        (context :?> PurgeContext).islandJobPurger location index
