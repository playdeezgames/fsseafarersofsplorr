namespace Splorr.Seafarers.Persistence.Schema

module Tables =
    let AvatarGamblingHands : string = "CREATE TABLE IF NOT EXISTS [AvatarGamblingHands]
		(
			[AvatarId] TEXT NOT NULL
				CONSTRAINT AvatarGamblingHands_pk
					PRIMARY KEY,
			[FirstCard] INT NOT NULL,
			[SecondCard] INT NOT NULL,
			[FinalCard] INT NOT NULL
		);"
    
    let AvatarInventories : string ="CREATE TABLE IF NOT EXISTS [AvatarInventories] (
		[AvatarId]	TEXT NOT NULL,
		[ItemId]	INTEGER NOT NULL,
		[ItemCount]	INTEGER NOT NULL CHECK(ItemCount>0),
		PRIMARY KEY([AvatarId],[ItemId]));"

    let AvatarIslandFeatures : string = "CREATE TABLE IF NOT EXISTS [AvatarIslandFeatures] (
    	[AvatarId]	TEXT NOT NULL,
    	[FeatureId]	INTEGER NOT NULL,
        [IslandX] REAL NOT NULL,
        [IslandY] REAL NOT NULL,
    	PRIMARY KEY([AvatarId]));"

    let AvatarIslandMetrics : string = "CREATE TABLE IF NOT EXISTS [AvatarIslandMetrics] (
    	[AvatarId]	TEXT NOT NULL,
    	[IslandX]	REAL NOT NULL,
    	[IslandY]	REAL NOT NULL,
    	[MetricId]	INTEGER NOT NULL,
    	[MetricValue]	INTEGER NOT NULL,
    	PRIMARY KEY([AvatarId],[IslandX],[IslandY],[MetricId]));"

    let AvatarJobs : string = "CREATE TABLE IF NOT EXISTS [AvatarJobs] (
    	[AvatarId]	TEXT NOT NULL,
    	[DestinationX]	REAL NOT NULL,
    	[DestinationY]	REAL NOT NULL,
    	[Reward]	REAL NOT NULL,
    	[Description]	TEXT NOT NULL,
    	PRIMARY KEY([AvatarId]));"

    let AvatarMetrics : string = "CREATE TABLE IF NOT EXISTS [AvatarMetrics] (
    	[AvatarId]	TEXT NOT NULL,
    	[MetricId]	INTEGER NOT NULL,
    	[MetricValue]	INTEGER NOT NULL CHECK([MetricValue]>0),
    	PRIMARY KEY([AvatarId],[MetricId]));"

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

    let IslandFeatureGenerators : string = "CREATE TABLE IF NOT EXISTS [IslandFeatureGenerators] (
    	[FeatureId]	INTEGER NOT NULL,
    	[FeatureWeight]	REAL NOT NULL,
    	[FeaturelessWeight]	REAL NOT NULL,
    	PRIMARY KEY([FeatureId]));"

    let IslandFeatures : string = "CREATE TABLE IF NOT EXISTS [IslandFeatures] (
    	[IslandX]	REAL NOT NULL,
    	[IslandY]	REAL NOT NULL,
    	[FeatureId]	INTEGER NOT NULL,
    	PRIMARY KEY([IslandX],[IslandY],[FeatureId]));";

    let IslandItems : string = "CREATE TABLE IF NOT EXISTS [IslandItems] (
		[IslandX]	REAL NOT NULL,
		[IslandY]	REAL NOT NULL,
		[ItemId]	INTEGER NOT NULL,
		PRIMARY KEY([IslandX],[IslandY],[ItemId]));"

    let IslandJobs : string = "CREATE TABLE IF NOT EXISTS [IslandJobs] (
    	[IslandX]	REAL NOT NULL,
    	[IslandY]	REAL NOT NULL,
    	[Order]	INTEGER NOT NULL,
    	[DestinationX]	REAL NOT NULL,
    	[DestinationY]	REAL NOT NULL,
    	[Reward]	REAL NOT NULL,
    	[Description]	TEXT NOT NULL,
    	PRIMARY KEY([IslandX],[IslandY],[Order]));"

    let IslandMarkets : string = "CREATE TABLE IF NOT EXISTS [IslandMarkets] (
    	[IslandX]	REAL NOT NULL,
    	[IslandY]	REAL NOT NULL,
    	[CommodityId]	INTEGER NOT NULL,
    	[Supply]	REAL NOT NULL,
    	[Demand]	REAL NOT NULL,
    	PRIMARY KEY([IslandX],[IslandY],[CommodityId]));"

    let Islands : string = "CREATE TABLE IF NOT EXISTS [Islands] (
    	[IslandX]	REAL NOT NULL,
    	[IslandY]	REAL NOT NULL,
    	[IslandName]	TEXT NOT NULL,
    	PRIMARY KEY([IslandX],[IslandY]));"
    
    let Items : string = "CREATE TABLE IF NOT EXISTS [Items] (
		[ItemId] INTEGER,
		[ItemName] TEXT NOT NULL UNIQUE,
		[Occurrence] REAL NOT NULL,
		[Tonnage] REAL NOT NULL,
		PRIMARY KEY([ItemId]));"

    let IslandStatistics : string ="CREATE TABLE IF NOT EXISTS [IslandStatistics] (
    		[IslandX]	REAL NOT NULL,
    		[IslandY]	REAL NOT NULL,
    		[StatisticId]	INTEGER NOT NULL,
    		[MinimumValue]	REAL NOT NULL,
    		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
    		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
    		PRIMARY KEY([IslandX],[IslandY],[StatisticId]));"

    let IslandStatisticTemplates : string = "CREATE TABLE IF NOT EXISTS [IslandStatisticTemplates] (
    		[StatisticId]	INTEGER,
    		[StatisticName]	TEXT NOT NULL,
    		[MinimumValue]	REAL NOT NULL,
    		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
    		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
    		PRIMARY KEY([StatisticId]));"

    let Messages : string = "CREATE TABLE IF NOT EXISTS [Messages] (
    	[MessageId]	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    	[AvatarId]	TEXT NOT NULL,
    	[Message]	TEXT NOT NULL);"

    let RationItems : string = "CREATE TABLE IF NOT EXISTS [RationItems] (
        [ItemId] INTEGER,
		[DefaultOrder] INTEGER NOT NULL,
        PRIMARY KEY([ItemId]));"

    let ShipmateRationItems : string = "CREATE TABLE IF NOT EXISTS [ShipmateRationItems] (
		[AvatarId]	TEXT NOT NULL,
		[ShipmateId]	TEXT NOT NULL,
		[ItemId]	INTEGER NOT NULL,
		[Order]	INTEGER NOT NULL,
		PRIMARY KEY([AvatarId],[ShipmateId],[ItemId]));"

    let ShipmateStatisticTemplates : string = "CREATE TABLE IF NOT EXISTS [ShipmateStatisticTemplates] (
		[StatisticId]	INTEGER,
		[StatisticName]	TEXT NOT NULL,
		[MinimumValue]	REAL NOT NULL,
		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
		PRIMARY KEY([StatisticId]));"

    let ShipmateStatistics : string ="CREATE TABLE IF NOT EXISTS [ShipmateStatistics] (
		[AvatarId]	TEXT NOT NULL,
		[ShipmateId]	TEXT NOT NULL,
		[StatisticId]	INTEGER NOT NULL,
		[MinimumValue]	REAL NOT NULL,
		[MaximumValue]	REAL NOT NULL CHECK(MaximumValue>=MinimumValue),
		[CurrentValue]	REAL NOT NULL CHECK(CurrentValue>=MinimumValue AND CurrentValue<=MaximumValue),
		PRIMARY KEY([AvatarId],[ShipmateId],[StatisticId]));"

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


