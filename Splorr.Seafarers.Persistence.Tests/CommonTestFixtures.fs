module CommonTestFixtures

open Splorr.Seafarers.Models
open System.Data.SQLite
open Splorr.Seafarers.Persistence.Schema

let private connectionString = "Data Source=:memory:;Version=3;New=True;"

let private runCommands (connection:SQLiteConnection) (commandTexts:string list) : unit =
    commandTexts
    |> List.iter
        (fun text ->
            use command = new SQLiteCommand(text, connection)
            command.ExecuteNonQuery() |> ignore)
    

let private setupCommodities (connection:SQLiteConnection) : unit =
    [
        Tables.Commodities
        "REPLACE INTO [Commodities] 
        ([CommodityId],[CommodityName],[BasePrice],[SaleFactor],[PurchaseFactor],[Discount]) 
        VALUES 
        (1,'Commodity 1',1,0.01,0.01,0.5),
        (2,'Commodity 2',2,0.02,0.02,0.5),
        (3,'Commodity 3',3,0.03,0.03,0.5);"
    ]
    |> runCommands connection

let private setupItems (connection:SQLiteConnection)  : unit =
    [
        Tables.Items
        Tables.CommodityItems
        "REPLACE INTO [Items] 
        ([ItemId], [ItemName], [Occurrence], [Tonnage])
        VALUES
        (1,'Item 1',1.0,1.0),
        (2,'Item 2',1.0,2.0),
        (3,'Item 3',1.0,3.0);"
        "REPLACE INTO [CommodityItems] 
        ([CommodityId], [ItemId], [Quantity])
        VALUES
        (1,1,1.0),
        (1,2,2.0),
        (1,3,3.0),
        (2,2,1.0),
        (2,3,2.0),
        (3,3,1.0);"
    ] 
    |> runCommands connection

let private setupWorldConfiguration (connection:SQLiteConnection) : unit =
    [
        Tables.WorldConfiguration
        "REPLACE INTO [WorldConfiguration] 
        ([WorldConfigurationId],[RewardMinimum],[RewardMaximum],[WorldWidth],[WorldHeight],[MaximumGenerationTries],[MinimumIslandDistance]) 
        VALUES (1,1,10,100,100,500,10);"
    ]
    |> runCommands connection

let private setupStatistics (connection:SQLiteConnection) : unit =
    System.Enum.GetValues(typedefof<StatisticIdentifier>) 
    :?> StatisticIdentifier []
    |> Array.toList
    |> List.map
        (fun id ->
            sprintf "REPLACE INTO [Statistics] ([StatisticId], [StatisticName], [MinimumValue], [MaximumValue], [CurrentValue]) VALUES (%u, 'Statistic %u', 0.0, 100.0, 50.0);" (id |> uint) (id |> uint))
    |> List.append
        [
            Tables.Statistics
        ]
    |> runCommands connection

let private setupRationItems (connection:SQLiteConnection) : unit =
    [
        Tables.RationItems
        "REPLACE INTO [RationItems] ([ItemId]) VALUES (1), (2);"
    ]
    |> runCommands connection
let internal SetupConnection() : SQLiteConnection = 
    let connection = new SQLiteConnection(connectionString)
    connection.Open()

    connection |> setupCommodities  

    connection |> setupItems

    connection |> setupWorldConfiguration

    connection |> setupStatistics

    connection |> setupRationItems

    connection
    
