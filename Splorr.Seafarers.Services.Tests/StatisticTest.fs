module StatisticTest

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

[<Test>]
let ``Create.It will not allow maximum to be less than minimum.`` () =
    let inputMinimum = 0.0
    let inputMaximum = -1.0
    let inputCurrent = 0.0
    let expected:Statistic =
        {
            MinimumValue = 0.0
            MaximumValue = 0.0
            CurrentValue = 0.0
        }
    let actual =
        Statistic.Create (inputMinimum, inputMaximum) inputCurrent
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Create.It will not allow current to be less than minimum.`` () =
    let inputMinimum = 0.0
    let inputMaximum = 1.0
    let inputCurrent = -1.0
    let expected:Statistic =
        {
            MinimumValue = 0.0
            MaximumValue = 1.0
            CurrentValue = 0.0
        }
    let actual =
        Statistic.Create (inputMinimum, inputMaximum) inputCurrent
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Create.It will not allow current to be greater than maximum.`` () =
    let inputMinimum = 0.0
    let inputMaximum = 1.0
    let inputCurrent = 2.0
    let expected:Statistic =
        {
            MinimumValue = 0.0
            MaximumValue = 1.0
            CurrentValue = 1.0
        }
    let actual =
        Statistic.Create (inputMinimum, inputMaximum) inputCurrent
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Create.It allows current to be between minimum and maximum.`` () =
    let inputMinimum = 0.0
    let inputMaximum = 1.0
    let inputCurrent = 0.5
    let expected:Statistic =
        {
            MinimumValue = 0.0
            MaximumValue = 1.0
            CurrentValue = 0.5
        }
    let actual =
        Statistic.Create (inputMinimum, inputMaximum) inputCurrent
    Assert.AreEqual(expected, actual)

let genericStatistic =
    {
        MinimumValue=0.0
        MaximumValue=1.0
        CurrentValue=0.5
    }

[<Test>]
let ``ChangeBy.It changes the current value by the given delta.`` () =
    let input = genericStatistic
    let inputDelta = 0.25
    let expected =
        {input with
            CurrentValue = 0.75}
    let actual =
        input
        |> Statistic.ChangeBy inputDelta
    Assert.AreEqual(expected, actual)

[<Test>]
let ``ChangeBy.It will not change the current value below the minimum.`` () =
    let input = genericStatistic
    let inputDelta = -1.0
    let expected =
        {input with
            CurrentValue = 0.0}
    let actual =
        input
        |> Statistic.ChangeBy inputDelta
    Assert.AreEqual(expected, actual)

[<Test>]
let ``ChangeBy.It will not change the current value above the maximum.`` () =
    let input = genericStatistic
    let inputDelta = 01.0
    let expected =
        {input with
            CurrentValue = 1.0}
    let actual =
        input
        |> Statistic.ChangeBy inputDelta
    Assert.AreEqual(expected, actual)
