namespace Splorr.Seafarers.Persistence.Schema

module Tables =
    let Commodities = "CREATE TABLE IF NOT EXISTS [Commodities] (
	    [CommodityId] INTEGER,
	    [CommodityName] TEXT NOT NULL UNIQUE,
	    [BasePrice] REAL NOT NULL,
	    [SaleFactor] REAL NOT NULL,
	    [PurchaseFactor] REAL NOT NULL,
	    [Discount] REAL NOT NULL,
	    PRIMARY KEY([CommodityId]));"

    let Items : string = "CREATE TABLE IF NOT EXISTS [Items] (
		[ItemId] INTEGER,
		[ItemName] TEXT NOT NULL UNIQUE,
		[Occurrence] REAL NOT NULL,
		[Tonnage] REAL NOT NULL,
		PRIMARY KEY([ItemId]));"

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

    let RationItems : string = "CREATE TABLE IF NOT EXISTS [RationItems] (
        [ItemId] INTEGER,
		[DefaultOrder] INTEGER NOT NULL,
        PRIMARY KEY([ItemId]));"

    let Statistics : string = "CREATE TABLE IF NOT EXISTS [AvatarStatisticTemplates] (
		[StatisticId]	INTEGER,
		[StatisticName]	TEXT NOT NULL,
		[MinimumValue]	REAL NOT NULL,
		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
		PRIMARY KEY([StatisticId]));"

    let WorldConfiguration : string = "CREATE TABLE IF NOT EXISTS [WorldConfiguration] (
		[WorldConfigurationId]	INTEGER CHECK(WorldConfigurationId=1),
		[RewardMinimum]	REAL NOT NULL,
		[RewardMaximum]	REAL NOT NULL,
		[WorldWidth]	REAL NOT NULL,
		[WorldHeight]	REAL NOT NULL,
		[MaximumGenerationTries]	INTEGER NOT NULL,
		[MinimumIslandDistance]	REAL NOT NULL,
		[AvatarViewDistance]	REAL NOT NULL,
		[AvatarDockDistance]	REAL NOT NULL,
		PRIMARY KEY([WorldConfigurationId]));"
