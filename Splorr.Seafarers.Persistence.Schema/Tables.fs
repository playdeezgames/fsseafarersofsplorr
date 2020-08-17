namespace Splorr.Seafarers.Persistence.Schema

module Tables =
    let ShipmateStatisticTemplates : string = "CREATE TABLE IF NOT EXISTS [ShipmateStatisticTemplates] (
		[StatisticId]	INTEGER,
		[StatisticName]	TEXT NOT NULL,
		[MinimumValue]	REAL NOT NULL,
		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
		PRIMARY KEY([StatisticId]));"

    let Commodities = "CREATE TABLE IF NOT EXISTS [Commodities] (
	    [CommodityId] INTEGER,
	    [CommodityName] TEXT NOT NULL UNIQUE,
	    [BasePrice] REAL NOT NULL,
	    [SaleFactor] REAL NOT NULL,
	    [PurchaseFactor] REAL NOT NULL,
	    [Discount] REAL NOT NULL,
	    PRIMARY KEY([CommodityId]));"

    let CommodityItems : string = "CREATE TABLE IF NOT EXISTS [CommodityItems] (
	    [CommodityId] INTEGER,
	    [ItemId] INTEGER,
	    [Quantity] REAL NOT NULL,
	    PRIMARY KEY([CommodityId],[ItemId]));"

    let IslandItems : string = "CREATE TABLE IF NOT EXISTS [IslandItems] (
		[IslandX]	REAL NOT NULL,
		[IslandY]	REAL NOT NULL,
		[ItemId]	INTEGER NOT NULL,
		PRIMARY KEY([IslandX],[IslandY],[ItemId]));"

    let IslandMarkets : string = "CREATE TABLE IF NOT EXISTS [IslandMarkets] (
    	[IslandX]	REAL NOT NULL,
    	[IslandY]	REAL NOT NULL,
    	[CommodityId]	INTEGER NOT NULL,
    	[Supply]	REAL NOT NULL,
    	[Demand]	REAL NOT NULL,
    	PRIMARY KEY([IslandX],[IslandY],[CommodityId]));"
    
    let Items : string = "CREATE TABLE IF NOT EXISTS [Items] (
		[ItemId] INTEGER,
		[ItemName] TEXT NOT NULL UNIQUE,
		[Occurrence] REAL NOT NULL,
		[Tonnage] REAL NOT NULL,
		PRIMARY KEY([ItemId]));"

    let Messages : string = "CREATE TABLE IF NOT EXISTS [Messages] (
    	[MessageId]	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    	[AvatarId]	TEXT NOT NULL,
    	[Message]	TEXT NOT NULL);"

    let RationItems : string = "CREATE TABLE IF NOT EXISTS [RationItems] (
        [ItemId] INTEGER,
		[DefaultOrder] INTEGER NOT NULL,
        PRIMARY KEY([ItemId]));"

    let ShipmateStatistics : string ="CREATE TABLE IF NOT EXISTS [ShipmateStatistics] (
		[AvatarId]	TEXT NOT NULL,
		[ShipmateId]	TEXT NOT NULL,
		[StatisticId]	INTEGER NOT NULL,
		[MinimumValue]	REAL NOT NULL,
		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
		PRIMARY KEY([AvatarId],[ShipmateId],[StatisticId]));"

    let ShipmateRationItems : string = "CREATE TABLE IF NOT EXISTS [ShipmateRationItems] (
    	[AvatarId]	TEXT NOT NULL,
    	[ShipmateId]	TEXT NOT NULL,
    	[ItemId]	INTEGER NOT NULL,
    	[Order]	INTEGER NOT NULL,
    	PRIMARY KEY([AvatarId],[ShipmateId],[ItemId]));"

    let Terms : string = "CREATE TABLE [Terms] (
		[TermType]	TEXT NOT NULL,
		[Term]	TEXT NOT NULL,
		PRIMARY KEY([Term],[TermType]));"

    let VesselStatisticTemplates : string = "CREATE TABLE IF NOT EXISTS [VesselStatisticTemplates] (
		[StatisticId]	INTEGER,
		[StatisticName]	TEXT NOT NULL,
		[MinimumValue]	REAL NOT NULL,
		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
		PRIMARY KEY([StatisticId]));"

    let VesselStatistics : string = "CREATE TABLE IF NOT EXISTS [VesselStatistics] (
		[AvatarId]      TEXT NOT NULL,
		[StatisticId]	INTEGER NOT NULL,
		[MinimumValue]	REAL NOT NULL,
		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
		PRIMARY KEY([AvatarId], [StatisticId]));"

    let WorldStatistics : string = "CREATE TABLE IF NOT EXISTS [WorldStatistics] (
		[StatisticId]	INTEGER NOT NULL,
		[MinimumValue]	REAL NOT NULL,
		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
		PRIMARY KEY([StatisticId]));"

    let AvatarInventories : string ="CREATE TABLE IF NOT EXISTS [AvatarInventories] (
    	[AvatarId]	TEXT NOT NULL,
    	[ItemId]	INTEGER NOT NULL,
    	[ItemCount]	INTEGER NOT NULL CHECK(ItemCount>0),
    	PRIMARY KEY([AvatarId],[ItemId]));"

