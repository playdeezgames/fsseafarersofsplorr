module UtilityTests

open System
open NUnit.Framework
open Splorr.Seafarers.Services

type TestUtilityRandomContext(random) =
    interface ServiceContext
    interface Utility.RandomContext with
        member this.random: Random = random
[<Test>]
let ``WeightedGenerator.It generates a random result based on weights.`` () =
    let seed = 0
    let table =
        Map.empty
        |> Map.add true 1.0
        |> Map.add false 1.0
    let expected = true |> Some
    let context = 
        TestUtilityRandomContext
            (Random(seed))
    let actual =
        Utility.WeightedGenerator
            context
            table
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SupplyDemandGenerator.It chooses a random value between 3 and 18.`` () =
    let seed = 0
    let context = 
        TestUtilityRandomContext
            (Random(seed))
    let expected = 16.869547913721554
    let actual =
        Utility.SupplyDemandGenerator
            context
    Assert.AreEqual(expected, actual)

[<Test>]
let ``PickRandomly.It chooses a random value from a list.`` () =
    let seed = 0
    let context = 
        TestUtilityRandomContext
            (Random(seed))
    let expected = 5
    let values = [1;2;3;4;5]
    let actual =
        Utility.PickRandomly
            context
            values
    Assert.AreEqual(expected, actual)
    

[<Test>]
let ``SortListRandomly.It random sorts a list.`` () =
    let seed = 0
    let context = 
        TestUtilityRandomContext
            (Random(seed))
    let expected = [5;4;1;3;2]
    let values = [1;2;3;4;5]
    let actual =
        Utility.SortListRandomly
            context
            values
    List.zip expected actual
    |> List.iter
        (fun (e, a) -> Assert.AreEqual(e, a))

[<Test>]
let ``RangeGenerator.It generates a value within a given float range.`` () =
    let seed = 0
    let context = 
        TestUtilityRandomContext
            (Random(seed))
    let expected = 4.6312163498397991
    let minimum = 1.0
    let maximum = 6.0
    let actual =
        Utility.RangeGenerator
            context
            (minimum, maximum)
    Assert.AreEqual(expected, actual)

