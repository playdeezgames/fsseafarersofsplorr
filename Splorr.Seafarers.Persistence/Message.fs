namespace Splorr.Seafarers.Persistence

open System

module Message =
    type Repository = Repository<Splorr.Seafarers.DataStore.Message.Identifier, Splorr.Seafarers.DataStore.Message.Record, string, string>

    let CreateRepository () : Splorr.Seafarers.DataStore.Message.IRepository =
        Repository(None, None, None, None) :> Splorr.Seafarers.DataStore.Message.IRepository


