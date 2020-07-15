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
    ()

let internal SetupConnection() : SQLiteConnection = 
    let connection = new SQLiteConnection(connectionString)
    connection.Open()

    connection |> setupCommodities  

    connection
    
