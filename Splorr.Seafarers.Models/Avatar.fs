namespace Splorr.Seafarers.Models

type ShipmateIdentifier = //TODO: needs a conversion to string so that shipmateids may be stored in the database
    | Primary

type Avatar =
    {
        Job          : Job option                        //TODO: associate directly to avatarId
        Metrics      : Map<Metric, uint64>               //TODO: gets its own table associate to avatarId
    }
