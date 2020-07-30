module AngleTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

[<Test>]
let ``ToFloat.It returns 0 when given zeros.`` () =
    let dms = 0.0 
    let actual =
        dms
        |> Angle.ToRadians
    Assert.AreEqual(0.0, actual)
    
[<Test>]
let ``ToFloat.It returns half pi when given ninety degrees.`` () =
    let dms = 90.0
    let actual =
        dms
        |> Angle.ToRadians
    Assert.AreEqual(System.Math.PI/2.0, actual)


[<Test>]
let ``ToFloat.It returns pi when given one hundred eighty degrees.`` () =
    let dms = 180.0
    let actual =
        dms
        |> Angle.ToRadians
    Assert.AreEqual(System.Math.PI, actual)


[<Test>]
let ``ToFloat.It returns three halves pi when given two hundred seventy degrees.`` () =
    let dms = 270.0
    let actual =
        dms
        |> Angle.ToRadians
    Assert.AreEqual(3.0 * System.Math.PI/2.0, actual)
    
[<Test>]
let ``ToDms.It return all zero when given zero.`` () =
    let radians: float = 0.0
    let actual = radians |> Angle.ToDegrees
    Assert.AreEqual(0.0, actual)

[<Test>]
let ``ToDms.It returns ninety degrees when given half pi.`` () =
    let radians: float = System.Math.PI/2.0
    let actual = radians |> Angle.ToDegrees
    Assert.AreEqual(90.0, actual)

[<Test>]
let ``ToDms.It returns one hundred eighty degrees when given pi.`` () =
    let radians: float = -System.Math.PI
    let actual = radians |> Angle.ToDegrees
    Assert.AreEqual(180.0, actual)

[<Test>]
let ``ToDms.It return two hundred seventy degrees when give three halves pi.`` () =
    let radians: float = -System.Math.PI/2.0
    let actual = radians |> Angle.ToDegrees
    Assert.AreEqual(270.0, actual)

[<Test>]
let ``ToString.It formats degrees minutes and seconds.``()=
    let expected = "1.50\u00b0"
    let actual = 
        1.5
        |> Angle.ToString
    Assert.AreEqual(expected, actual)
