namespace Cosmos.Store

open System
open System.Collections.Concurrent
open System.Collections.Generic
open Cosmos.Domain
open Cosmos.Prelude.Result

type ExpiringCache<'a>() =
    let cache = ConcurrentDictionary<string, DateTime * 'a>()

    member _.Create (TimeToLive ttl) id item =
        let validUntil = DateTime.UtcNow.AddHours(float ttl)

        let addToCache () =
            match cache.TryAdd(id, (validUntil, item)) with
            | true -> Ok cache.[id]
            | false -> Error StoreError
            |> Result.map snd

        let handleExisting expiry =
            if expiry < DateTime.UtcNow then
                cache.Remove id |> ignore
                addToCache ()
            else
                Error AlreadyExistsError

        match cache.TryGetValue id with
        | true, (expiry, _) -> handleExisting expiry
        | false, _ -> addToCache ()

    member _.Read id =
        let handleExisting expiry value =
            if expiry < DateTime.UtcNow then
                cache.Remove id |> ignore
                Error NotFoundError
            else
                Ok value

        match cache.TryGetValue id with
        | true, (expiry, value) -> handleExisting expiry value
        | false, _ -> Error NotFoundError

    member _.Update id item =
        let handleExisting expiry existingItem =
            if expiry < DateTime.UtcNow then
                cache.Remove id |> ignore
                Error NotFoundError
            else
                match cache.TryUpdate (id, (expiry, item), (expiry, existingItem)) with
                | true -> Ok item
                | false -> Error StoreError

        match cache.TryGetValue id with
        | true, (expiry, value) -> handleExisting expiry value
        | false, _ -> Error NotFoundError

    member _.Values() =
        for KeyValue(id, (expiry, _)) in cache do
            if expiry < DateTime.UtcNow then
                cache.Remove id |> ignore
        
        cache.Values
        |> Seq.map snd
