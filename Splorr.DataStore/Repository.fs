namespace Splorr.Seafarers.DataStore

type IRepository<'TIdentifier, 'TRecord, 'TRetrieveFilter, 'TDestroyFilter when 'TIdentifier : comparison> =
    abstract member Create: 'TRecord -> 'TIdentifier
    abstract member Retrieve: 'TRetrieveFilter -> ('TIdentifier * 'TRecord) list
    abstract member Update: Map<'TIdentifier, 'TRecord> -> Set<'TIdentifier>
    abstract member Destroy : 'TDestroyFilter -> Set<'TIdentifier>
