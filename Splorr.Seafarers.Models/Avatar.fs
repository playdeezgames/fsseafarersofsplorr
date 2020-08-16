namespace Splorr.Seafarers.Models

type ShipmateIdentifier = //TODO: needs a conversion to string so that shipmateids may be stored in the database
    | Primary

type Avatar =
    {
        Job          : Job option                        //TODO: associate directly to avatarId
        Inventory    : Map<uint64, uint32>               //TODO: gets its own table
        Metrics      : Map<Metric, uint>                 //TODO: gets its own table associate to avatarId
    }
