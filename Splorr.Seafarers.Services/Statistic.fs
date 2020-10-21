namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Statistic =
    let internal SetCurrentValue 
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
            MaximumValue = 
                maximum 
                |> max statistic.MinimumValue}
        |> SetCurrentValue statistic.CurrentValue

    let internal Create 
            (minimum: float, maximum: float)  
            (current: float) 
            : Statistic =
        {
            MinimumValue = minimum
            MaximumValue = maximum |> max minimum
            CurrentValue = minimum
        }
        |> SetCurrentValue current

    let internal CreateFromTemplate 
            (template:StatisticTemplate) 
            : Statistic =
        Create 
            (template.MinimumValue, template.MaximumValue) 
            template.CurrentValue

    let internal ChangeCurrentBy 
            (amount    : float) 
            (statistic : Statistic) 
            : Statistic =
        statistic
        |> SetCurrentValue (amount + statistic.CurrentValue)

    let internal ChangeMaximumBy 
            (amount    : float) 
            (statistic : Statistic) 
            : Statistic =
        statistic
        |> SetMaximum (amount + statistic.MaximumValue)

    let GetCurrentValue
            (statistic : Statistic)
            : float =
        statistic.CurrentValue
    
    let GetMaximumValue
            (statistic : Statistic)
            : float =
        statistic.MaximumValue