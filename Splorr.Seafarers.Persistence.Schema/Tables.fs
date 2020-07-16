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

    let WorldConfiguration : string = "CREATE TABLE IF NOT EXISTS [WorldConfiguration] (
			[WorldConfigurationId]	INTEGER CHECK(WorldConfigurationId=1),
			[RewardMinimum]	REAL NOT NULL,
			[RewardMaximum]	REAL NOT NULL,
			[WorldWidth]	REAL NOT NULL,
			[WorldHeight]	REAL NOT NULL,
			[MaximumGenerationTries]	INTEGER NOT NULL,
			[MinimumIslandDistance]	REAL NOT NULL,
			PRIMARY KEY([WorldConfigurationId])
		);"
