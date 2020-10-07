namespace Splorr.Seafarers.DataStore

open System

module Message =
    type Identifier = int64

    type Record =
        {
            avatarId : string
            text : string
            foregroundColor : ConsoleColor
            backgroundColor : ConsoleColor
        }

    type IRepository = IRepository<Identifier, Record, string, string>


