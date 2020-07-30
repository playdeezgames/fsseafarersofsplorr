namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Statistic =
    let private SetCurrent 
            (current   : float) 
            (statistic : Statistic) 
            : Statistic =
        {statistic with
            CurrentValue = 
                current 
                |> min statistic.MaximumValue 
                |> max statistic.MinimumValue}

    let private SetMaximum 
            (maximum   : float) 
            (statistic : Statistic) 
            : Statistic =
        {statistic with
            MaximumValue = maximum |> max statistic.MinimumValue}
        |> SetCurrent statistic.CurrentValue

    let Create 
            (minimum: float, maximum: float)  
            (current: float) 
            : Statistic =
        {
            MinimumValue = minimum
            MaximumValue = maximum |> max minimum
            CurrentValue = minimum
        }
        |> SetCurrent current

    let ChangeCurrentBy 
            (amount    : float) 
            (statistic : Statistic) 
            : Statistic =
        statistic
        |> SetCurrent (amount + statistic.CurrentValue)


    let ChangeMaximumBy 
            (amount    : float) 
            (statistic : Statistic) 
            : Statistic =
        statistic
        |> SetMaximum (amount + statistic.MaximumValue)
