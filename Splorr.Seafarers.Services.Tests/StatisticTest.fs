module StatisticTest

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let genericStatistic =
    {
        MinimumValue=0.0
        MaximumValue=1.0
        CurrentValue=0.5
    }

[<Test>]
let ``GetCurrentValue.It returns the current value of the given statistic.`` () =
    let inputMaximum = 100.0
    let inputMinimum = 0.0
    let inputCurrent = 50.0
    let input : Statistic =
        {
            MaximumValue = inputMaximum
            MinimumValue = inputMinimum
            CurrentValue = inputCurrent
        }
    let expected = inputCurrent
    let actual =
        input
        |> Statistic.GetCurrentValue
    Assert.AreEqual(expected, actual)


[<Test>]
let ``GetMaximumValue.It returns the maximum value of the given statistic.`` () =
    let inputMaximum = 100.0
    let inputMinimum = 0.0
    let inputCurrent = 50.0
    let input : Statistic =
        {
            MaximumValue = inputMaximum
            MinimumValue = inputMinimum
            CurrentValue = inputCurrent
        }
    let expected = inputMaximum
    let actual =
        input
        |> Statistic.GetMaximumValue
    Assert.AreEqual(expected, actual)

        