module CommonTestFixtures

open Splorr.Seafarers.Models
open System.Data.SQLite

let private connectionString = "Data Source=:memory:;Version=3;New=True;"

let private setupCommodities (connection:SQLiteConnection) : unit =
    use command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS [Commodities] (
    	[CommodityId]	INTEGER,
    	[CommodityName]	TEXT NOT NULL UNIQUE,
    	[BasePrice]	REAL NOT NULL,
    	[SaleFactor]	REAL NOT NULL,
    	[PurchaseFactor]	REAL NOT NULL,
    	[Discount]	REAL NOT NULL,
    	PRIMARY KEY([CommodityId])
    );",connection)
    command.ExecuteNonQuery() |> ignore
    use command = new SQLiteCommand("REPLACE INTO [Commodities] 
        ([CommodityId],[CommodityName],[BasePrice],[SaleFactor],[PurchaseFactor],[Discount]) 
        VALUES 
        (1,'Commodity 1',1,0.01,0.01,0.5),
        (2,'Commodity 2',2,0.02,0.02,0.5),
        (3,'Commodity 3',3,0.03,0.03,0.5);",connection)
    command.ExecuteNonQuery() |> ignore

let private setupItems (connection:SQLiteConnection)  : unit =
    use command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS [Items] (
	    [ItemId]	INTEGER,
	    [ItemName]	TEXT NOT NULL UNIQUE,
	    [Occurrence]	REAL NOT NULL,
	    [Tonnage]	REAL NOT NULL,
	    PRIMARY KEY([ItemId])
    );",connection)
    command.ExecuteNonQuery() |> ignore
    use command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS [CommodityItems] (
	        [CommodityId]	INTEGER,
	        [ItemId]	INTEGER,
	        [Quantity]	REAL NOT NULL,
	        PRIMARY KEY([CommodityId],[ItemId])
    );",connection)
    command.ExecuteNonQuery() |> ignore
    use command = new SQLiteCommand("REPLACE INTO [Items] 
                ([ItemId], [ItemName], [Occurrence], [Tonnage])
                VALUES
                (1,'Item 1',1.0,1.0),
                (2,'Item 2',1.0,2.0),
                (3,'Item 3',1.0,3.0);",connection)
    command.ExecuteNonQuery() |> ignore
    use command = new SQLiteCommand("REPLACE INTO [CommodityItems] 
                ([CommodityId], [ItemId], [Quantity])
                VALUES
                (1,1,1.0),
                (1,2,2.0),
                (1,3,3.0),
                (2,2,1.0),
                (2,3,2.0),
                (3,3,1.0);",connection)
    command.ExecuteNonQuery() |> ignore

let internal SetupConnection() : SQLiteConnection = 
    let connection = new SQLiteConnection(connectionString)
    connection.Open()

    connection |> setupCommodities  

    connection |> setupItems

    connection
    
