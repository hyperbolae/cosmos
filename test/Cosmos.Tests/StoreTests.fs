namespace Cosmos.Tests.StoreTests

open Xunit
open Cosmos.Prelude.Result
open Cosmos.Domain
open Cosmos.Store

module ExpiringCacheTests =

    [<Fact>]
    let ``Can create and read item from expiring cache`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive 1<hr>
        cache.Create ttl "1" "test" |> ignore

        let item = cache.Read "1"

        let expected = Ok "test"

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Cannot read expired item from expiring cache`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive -1<hr>
        cache.Create ttl "1" "test" |> ignore

        let item = cache.Read "1"

        let expected = Error NotFoundError

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Cannot create to expiring cache when non-expired item exists`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive 1<hr>
        cache.Create ttl "1" "test" |> ignore

        let item = cache.Create ttl "1" "this already exists"

        let expected = Error AlreadyExistsError

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Can create to expiring cache when expired item exists`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive -1<hr>
        cache.Create ttl "1" "test" |> ignore

        let newTtl = TimeToLive 1<hr>
        let item = cache.Create newTtl "1" "new item"

        let expected = Ok "new item"

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Can read non-expired items from expiring cache`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive 1<hr>
        cache.Create ttl "1" "test1" |> ignore
        cache.Create ttl "2" "test2" |> ignore
        
        let oldTtl = TimeToLive -1<hr>
        cache.Create oldTtl "3" "expired item" |> ignore

        let items =
            cache.Values ()
            |> Seq.sort

        let expected =
            [ "test1"; "test2" ]
            |> List.sort

        Assert.Equal(expected, items)

    [<Fact>]
    let ``Can update item in expiring cache`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive 1<hr>
        cache.Create ttl "1" "test" |> ignore

        let newItem = "updated test"
        cache.Update "1" newItem |> ignore
        
        let item = cache.Read "1"
        let expected = Ok newItem

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Cannot update a non-existing item in expiring cache`` () =
        let cache = ExpiringCache()

        let newItem = "attempted updated test"
        let item = cache.Update "1" newItem

        let expected = Error NotFoundError

        Assert.Equal(expected, item)

    [<Fact>]
    let ``Cannot update an expired item in expiring cache`` () =
        let cache = ExpiringCache()
        let ttl = TimeToLive -1<hr>
        cache.Create ttl "1" "test" |> ignore

        let newItem = "updated test"
        let item = cache.Update "1" newItem

        let expected = Error NotFoundError

        Assert.Equal(expected, item)
