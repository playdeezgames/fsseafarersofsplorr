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


