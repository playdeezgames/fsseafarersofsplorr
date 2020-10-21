module LocationTests

open NUnit.Framework
open Splorr.Seafarers.Services

[<Test>]
let ``DistanceTo.It returns the distance between two given locations.`` () =
    let first = (0.0,0.0)
    let second = (3.0, 4.0)
    let actual = first |> Location.DistanceTo second
    Assert.AreEqual(5.0, actual)

//[<Test>]
//let ``ScaleBy.It returns the product of a given location by a given scale.`` () =
//    let actual = 
//        (1.0,2.0)
//        |> Location.ScaleBy 3.0
//    Assert.AreEqual(3.0, actual |> fst)
//    Assert.AreEqual(6.0, actual |> snd)

[<Test>]
let ``HeadingTo.It returns the direction in radians from a given location to another given location.`` () =
    let first = (0.0,0.0)
    let second = (2.0, 2.0)
    let actual = Location.HeadingTo first second
    Assert.AreEqual(System.Math.PI/4.0, actual)

