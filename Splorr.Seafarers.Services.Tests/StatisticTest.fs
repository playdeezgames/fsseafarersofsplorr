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
let ``ChangeCurrentBy.It changes the current value by the given delta.`` () =
    let input = genericStatistic
    let inputDelta = 0.25
    let expected =
        {input with
            CurrentValue = 0.75}
    let actual =
        input
        |> Statistic.ChangeCurrentBy inputDelta
    Assert.AreEqual(expected, actual)

[<Test>]
let ``ChangeCurrentBy.It will not change the current value below the minimum.`` () =
    let input = genericStatistic
    let inputDelta = -1.0
    let expected =
        {input with
            CurrentValue = 0.0}
    let actual =
        input
        |> Statistic.ChangeCurrentBy inputDelta
    Assert.AreEqual(expected, actual)

[<Test>]
let ``ChangeCurrentBy.It will not change the current value above the maximum.`` () =
    let input = genericStatistic
    let inputDelta = 01.0
    let expected =
        {input with
            CurrentValue = 1.0}
    let actual =
        input
        |> Statistic.ChangeCurrentBy inputDelta
    Assert.AreEqual(expected, actual)


[<Test>]
let ``ChangeMaximumBy.It changes the maximum value by the given delta.`` () =
    let input = genericStatistic
    let inputDelta = 0.25
    let expected =
        {input with
            MaximumValue = 1.25}
    let actual =
        input
        |> Statistic.ChangeMaximumBy inputDelta
    Assert.AreEqual(expected, actual)

[<Test>]
let ``ChangeMaximumBy.It will not change the maximum value below the minimum.`` () =
    let input = genericStatistic
    let inputDelta = -2.0
    let expected =
        {input with
            MaximumValue = 0.0
            CurrentValue = 0.0}
    let actual =
        input
        |> Statistic.ChangeMaximumBy inputDelta
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetCurrentValue.It returns the current value of the given statistic.`` () =
    let inputMaximum = 100.0
    let inputMinimum = 0.0
    let inputCurrent = 50.0
    let input =
        Statistic.Create (inputMinimum, inputMaximum) inputCurrent
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
    let input =
        Statistic.Create (inputMinimum, inputMaximum) inputCurrent
    let expected = inputMaximum
    let actual =
        input
        |> Statistic.GetMaximumValue
    Assert.AreEqual(expected, actual)

      
[<Test>]
let ``SetCurrentValue.It sets the current value of the given statistic.`` () =
    let inputMaximum    = 100.0
    let inputMinimum    = 0.0
    let originalCurrent = 50.0
    let inputCurrent    = 75.0
    let input =
        Statistic.Create (inputMinimum, inputMaximum) originalCurrent
    let expected = 
        {input with CurrentValue = inputCurrent}
    let actual =
        input
        |> Statistic.SetCurrentValue inputCurrent
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CreateFromTemplate.It creates a statistic from a statistic template.`` () =
        let inputMinimum = 1.0
        let inputCurrent = 2.0
        let inputMaximum = 3.0
        let input :StatisticTemplate = 
            {
                StatisticName = ""
                MinimumValue = inputMinimum
                CurrentValue = inputCurrent
                MaximumValue = inputMaximum
            }
        let expected : Statistic =
            {
                MinimumValue = inputMinimum
                CurrentValue = inputCurrent
                MaximumValue = inputMaximum
            }
        let actual =
            input
            |> Statistic.CreateFromTemplate
        Assert.AreEqual(expected, actual)

        