﻿module CommonTestFixtures

open Splorr.Seafarers.Models
open System.Data.SQLite
open Splorr.Seafarers.Persistence.Schema

let private connectionString = "Data Source=:memory:;Version=3;New=True;"

let private runCommands 
        (connection   : SQLiteConnection) 
        (commandTexts : string list) 
        : unit =
    commandTexts
    |> List.iter
        (fun text ->
            use command = new SQLiteCommand(text, connection)
            command.ExecuteNonQuery() |> ignore)
    

let private setupCommodities 
        (connection : SQLiteConnection) 
        : unit =
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

let private setupItems 
        (connection : SQLiteConnection)
        : unit =
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

let private setupShipmateStatisticTemplates 
        (connection : SQLiteConnection) 
        : unit =
    System.Enum.GetValues(typedefof<ShipmateStatisticIdentifier>) 
    :?> ShipmateStatisticIdentifier []
    |> Array.toList
    |> List.map
        (fun id ->
            sprintf "REPLACE INTO [ShipmateStatisticTemplates] ([StatisticId], [StatisticName], [MinimumValue], [MaximumValue], [CurrentValue]) VALUES (%u, 'Statistic %u', 0.0, 100.0, 50.0);" (id |> uint) (id |> uint))
    |> List.append
        [
            Tables.ShipmateStatisticTemplates
        ]
    |> runCommands connection

let private setupVesselStatisticTemplates 
        (connection:SQLiteConnection) 
        : unit =
    System.Enum.GetValues(typedefof<VesselStatisticIdentifier>) 
    :?> VesselStatisticIdentifier []
    |> Array.toList
    |> List.map
        (fun id ->
            sprintf "REPLACE INTO [VesselStatisticTemplates] ([StatisticId], [StatisticName], [MinimumValue], [MaximumValue], [CurrentValue]) VALUES (%u, 'Statistic %u', 0.0, 100.0, 50.0);" (id |> uint) (id |> uint))
    |> List.append
        [
            Tables.VesselStatisticTemplates
        ]
    |> runCommands connection

let private setupRationItems 
        (connection : SQLiteConnection) 
        : unit =
    [
        Tables.RationItems
        "REPLACE INTO [RationItems] ([ItemId],[DefaultOrder]) VALUES (1,2), (2,1);"
    ]
    |> runCommands connection

let private setupIslandItems 
        (connection : SQLiteConnection) 
        : unit =
    [
        Tables.IslandItems
        "REPLACE INTO [IslandItems] ([IslandX], [IslandY], [ItemId]) VALUES (0.0, 0.0, 1), (0.0, 0.0, 2), (0.0, 0.0, 3), (10.0, 0.0, 1), (10.0, 0.0, 2), (0.0, 10.0, 1);"
    ]
    |> runCommands connection

let private setupIslandMarkets 
        (connection : SQLiteConnection) 
        : unit =
    [
        Tables.IslandMarkets
        "REPLACE INTO [IslandMarkets] ([IslandX], [IslandY], [CommodityId], [Supply], [Demand]) VALUES (0.0, 0.0, 1, 1.0, 1.0), (0.0, 0.0, 2, 2.0, 2.0), (0.0, 0.0, 3, 3.0, 3.0);"
    ]
    |> runCommands connection

let private setupVesselStatistics 
        (avatarId   : string) 
        (connection : SQLiteConnection) 
        : unit =
    System.Enum.GetValues(typedefof<VesselStatisticIdentifier>) 
    :?> VesselStatisticIdentifier []
    |> Array.toList
    |> List.map
        (fun id ->
            sprintf "REPLACE INTO [VesselStatistics] ([AvatarId], [StatisticId], [MinimumValue], [MaximumValue], [CurrentValue]) VALUES ('%s', %u, 0.0, 100.0, 50.0);" (avatarId) (id |> uint))
    |> List.append
        [
            Tables.VesselStatistics
        ]
    |> runCommands connection

let private setupWorldStatistics 
        (connection : SQLiteConnection) 
        : unit =
    System.Enum.GetValues(typedefof<WorldStatisticIdentifier>) 
    :?> WorldStatisticIdentifier []
    |> Array.toList
    |> List.map
        (fun id ->
            sprintf "REPLACE INTO [WorldStatistics] ([StatisticId], [MinimumValue], [MaximumValue], [CurrentValue]) VALUES (%u, 0.0, 100.0, 50.0);" (id |> uint))
    |> List.append
        [
            Tables.WorldStatistics
        ]
    |> runCommands connection

let private setupTerms
        (connection : SQLiteConnection) 
        : unit =
    [
        Tables.Terms
        "REPLACE INTO [Terms] ([TermType], [Term]) VALUES ('adverb', 'woefully');"
        "REPLACE INTO [Terms] ([TermType], [Term]) VALUES ('adjective', 'tatty');"
        "REPLACE INTO [Terms] ([TermType], [Term]) VALUES ('object name', 'thing');"
        "REPLACE INTO [Terms] ([TermType], [Term]) VALUES ('person name', 'bob');"
        "REPLACE INTO [Terms] ([TermType], [Term]) VALUES ('person adjective', 'smelly');"
        "REPLACE INTO [Terms] ([TermType], [Term]) VALUES ('profession', 'monk');"
    ]
    |> runCommands connection

let internal ExistingAvatarId = "avatar"

let private setupMessages
        (connection:SQLiteConnection)
        : unit =
    [
        Tables.Messages
        ExistingAvatarId |> sprintf "REPLACE INTO [Messages] ([AvatarId], [Message]) VALUES ('%s', 'message1');"
        ExistingAvatarId |> sprintf "REPLACE INTO [Messages] ([AvatarId], [Message]) VALUES ('%s', 'message2');"
        ExistingAvatarId |> sprintf "REPLACE INTO [Messages] ([AvatarId], [Message]) VALUES ('%s', 'message3');"
    ]
    |> runCommands connection

let internal PrimaryShipmateId = "primary"

let private setupShipmateRationItems
        (connection : SQLiteConnection)
        : unit = 
    [
        Tables.ShipmateRationItems
        (ExistingAvatarId, PrimaryShipmateId) ||> sprintf "REPLACE INTO [ShipmateRationItems] ([AvatarId], [ShipmateId], [ItemId], [Order]) VALUES ('%s','%s',1,1);"
        (ExistingAvatarId, PrimaryShipmateId) ||> sprintf "REPLACE INTO [ShipmateRationItems] ([AvatarId], [ShipmateId], [ItemId], [Order]) VALUES ('%s','%s',2,2);"
    ]
    |> runCommands connection

let private setupShipmateStatistics
        (avatarId   : string)
        (shipmateId : string)
        (connection : SQLiteConnection)
        : unit =
    System.Enum.GetValues(typedefof<ShipmateStatisticIdentifier>) 
    :?> ShipmateStatisticIdentifier []
    |> Array.toList
    |> List.map
        (fun id ->
            sprintf "REPLACE INTO [ShipmateStatistics] ([AvatarId], [ShipmateId], [StatisticId], [MinimumValue], [MaximumValue], [CurrentValue]) VALUES ('%s','%s',%u, 0.0, 100.0, 50.0);" avatarId shipmateId (id |> uint))
    |> List.append
        [
            Tables.ShipmateStatistics
        ]
    |> runCommands connection


let internal NewAvatarId = "newavatar"

let internal SetupConnection() : SQLiteConnection = 
    let connection = new SQLiteConnection(connectionString)
    connection.Open()

    [
        setupCommodities  
        setupItems
        setupShipmateStatisticTemplates
        setupVesselStatisticTemplates
        setupVesselStatistics ExistingAvatarId
        setupRationItems
        setupIslandItems
        setupIslandMarkets
        setupTerms
        setupMessages
        setupShipmateRationItems
        setupWorldStatistics
        setupShipmateStatistics ExistingAvatarId PrimaryShipmateId
    ]
    |> List.iter (fun f -> f connection)

    connection
    
