namespace Splorr.DataStore

type StoreRequest<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter when 'TIdentifier : comparison> =
    | Create of 'TRecord
    | Retrieve of 'TRetrieveFilter
    | Upsert of Map<'TIdentifier, 'TRecord>
    | Destroy of 'TDestroyFilter