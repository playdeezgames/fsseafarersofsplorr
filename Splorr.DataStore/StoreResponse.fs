namespace Splorr.DataStore

type StoreResponse<'TIdentifier, 'TRecord when 'TIdentifier : comparison> =
    | Created of 'TIdentifier
    | Retrieved of ('TIdentifier * 'TRecord) list
    | Upserted of Set<'TIdentifier>
    | Destroyed of Set<'TIdentifier>

