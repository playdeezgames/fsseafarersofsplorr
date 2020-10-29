module LocationTests

open NUnit.Framework
open Splorr.Seafarers.Services

[<Test>]
let ``DistanceTo.It returns the distance between two given locations.`` () =
    let first = (0.0,0.0)
    let second = (3.0, 4.0)
    let actual = first |> Location.DistanceTo second
    Assert.AreEqual(5.0, actual)

[<Test>]
let ``HeadingTo.It returns the direction in radians from a given location to another given location.`` () =
    let first = (0.0,0.0)
    let second = (2.0, 2.0)
    let actual = Location.HeadingTo first second
    Assert.AreEqual(System.Math.PI/4.0, actual)

