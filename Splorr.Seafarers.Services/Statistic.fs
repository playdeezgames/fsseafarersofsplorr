namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Statistic =
    let private SetCurrent (current:float) (statistic:Statistic) : Statistic =
        {statistic with
            CurrentValue = 
                current 
                |> min statistic.MaximumValue 
                |> max statistic.MinimumValue}


    let Create (minimum: float, maximum: float)  (current: float) : Statistic =
        {
            MinimumValue = minimum
            MaximumValue = maximum |> max minimum
            CurrentValue = minimum
        }
        |> SetCurrent current

    let ChangeBy (amount:float) (statistic:Statistic) : Statistic =
        statistic
        |> SetCurrent (amount + statistic.CurrentValue)
