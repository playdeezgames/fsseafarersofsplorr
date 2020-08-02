module CommonTestFixtures

open Splorr.Seafarers.Controllers
open System.Data.SQLite
open Splorr.Seafarers.Models


let internal connectionString = "Data Source=:memory:;Version=3;New=True;"
let internal random = System.Random()
let internal sinkStub (_:Message) : unit = ()
let internal toSource (command:Command option) = fun () -> command
let internal createConnection() :SQLiteConnection =
    new SQLiteConnection(connectionString)
let internal avatarId:string = ""
let internal statisticDescriptors =
    [
        {StatisticId = AvatarStatisticIdentifier.Satiety; StatisticName="satiety"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        {StatisticId = AvatarStatisticIdentifier.Health; StatisticName="health"; MinimumValue=0.0; CurrentValue=100.0;MaximumValue=100.0}
        {StatisticId = AvatarStatisticIdentifier.Turn; StatisticName="turn"; MinimumValue=0.0; CurrentValue=0.0;MaximumValue=50000.0}
    ]
let internal vesselStatisticTemplateSourceStub() :Map<VesselStatisticIdentifier, VesselStatisticTemplate>= 
    Map.empty
let internal vesselStatisticSinkStub (_) (_) = ()
let internal vesselSingleStatisticSourceStub (_) (identifier) = 
    match identifier with 
    | VesselStatisticIdentifier.FoulRate ->
        {MinimumValue=0.001; MaximumValue=0.001; CurrentValue=0.001} |> Some
    | VesselStatisticIdentifier.Tonnage ->
        {MinimumValue=100.0; MaximumValue=100.0; CurrentValue=100.0} |> Some
    | _ ->
        None
let internal vesselSingleStatisticSinkStub (_) (_) = ()

