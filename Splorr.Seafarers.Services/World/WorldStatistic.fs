namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module WorldStatistic =
    type WorldSingleStatisticSource = WorldStatisticIdentifier -> Statistic
    type GetStatisticContext =
        abstract member worldSingleStatisticSource : WorldSingleStatisticSource ref
    let internal GetStatistic
            (context : CommonContext)
            (identifier : WorldStatisticIdentifier)
            : Statistic =
        (context :?> GetStatisticContext).worldSingleStatisticSource.Value identifier

    let private GetCurrentStatisticValue
            (context: CommonContext) =
        GetStatistic context
        >> Statistic.GetCurrentValue

    let internal GetIslandGenerationRetries
            (context : CommonContext)
            : uint =
        WorldStatisticIdentifier.IslandGenerationRetries
        |> GetCurrentStatisticValue context
        |> uint

    let internal GetMinimumIslandDistance
            (context : CommonContext)
            : float =
        WorldStatisticIdentifier.IslandDistance
        |> GetCurrentStatisticValue context 

    let private GetMaximumStatisticValue
            (context: CommonContext) =
        GetStatistic context
        >> Statistic.GetMaximumValue

    let private GetMaximumPositionX
            (context : CommonContext)
            : float =
        WorldStatisticIdentifier.PositionX
        |> GetMaximumStatisticValue context 

    let private GetMaximumPositionY
            (context : CommonContext)
            : float =
        WorldStatisticIdentifier.PositionY
        |> GetMaximumStatisticValue context 

    let internal GetWorldSize
            (context : CommonContext)
            : float * float =
        (GetMaximumPositionX context,
            GetMaximumPositionY context)



