namespace Cosmos.Tests.StoreTests

open Xunit
open Cosmos.Prelude.Result
open Cosmos.Domain
open Cosmos.Store

module ExpiringCacheTests =
    [<Fact>]
    let ``Can store and retrieve item from expiring cache`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive 1<hr>
        cache.Add ttl "1" "test" |> ignore

        let item = cache.Get "1"

        let expected = Ok "test"

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Cannot retrieve expired item from expiring cache`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive -1<hr>
        cache.Add ttl "1" "test" |> ignore

        let item = cache.Get "1"

        let expected = Error ExpiredError

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Cannot add to expiring cache when non-expired item exists`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive 1<hr>
        cache.Add ttl "1" "test" |> ignore

        let item = cache.Add ttl "1" "this already exists"

        let expected = Error AlreadyExistsError

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Can add to expiring cache when expired item exists`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive -1<hr>
        cache.Add ttl "1" "test" |> ignore

        let newTtl = TimeToLive 1<hr>
        let item = cache.Add newTtl "1" "new item"

        let expected = Ok "new item"

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Can get non-expired items from expiring cache`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive 1<hr>
        cache.Add ttl "1" "test1" |> ignore
        cache.Add ttl "2" "test2" |> ignore
        
        let oldTtl = TimeToLive -1<hr>
        cache.Add oldTtl "3" "expired item" |> ignore

        let items = cache.Values ()

        let expected = [ "test1"; "test2" ]

        Assert.Equal(expected, items)
