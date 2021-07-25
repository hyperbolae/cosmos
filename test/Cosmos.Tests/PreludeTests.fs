module Cosmos.Tests.PreludeTests

open Xunit
open Cosmos.Prelude.Result

[<Fact>]
let ``Can create AggregateError with two non-aggregate errors`` () =
    let error1 = ValidationError "Error 1"
    let error2 = ValidationError "Error 2"

    let aggregateError = createAggregateError error1 error2

    let expected =
        AggregateError [ ValidationError "Error 1"
                         ValidationError "Error 2" ]

    Assert.Equal(expected, aggregateError)

[<Fact>]
let ``Can create AggregateError with first aggregate error`` () =
    let error1 =
        AggregateError [ ValidationError "Error 2"
                         ValidationError "Error 3" ]

    let error2 = ValidationError "Error 1"

    let aggregateError = createAggregateError error1 error2

    let expected =
        AggregateError [ ValidationError "Error 1"
                         ValidationError "Error 2"
                         ValidationError "Error 3" ]

    Assert.Equal(expected, aggregateError)

[<Fact>]
let ``Can create AggregateError with second aggregate error`` () =
    let error1 = ValidationError "Error 1"

    let error2 =
        AggregateError [ ValidationError "Error 2"
                         ValidationError "Error 3" ]

    let aggregateError = createAggregateError error1 error2

    let expected =
        AggregateError [ ValidationError "Error 1"
                         ValidationError "Error 2"
                         ValidationError "Error 3" ]

    Assert.Equal(expected, aggregateError)

[<Fact>]
let ``Can create AggregateError with two aggregate errors`` () =
    let error1 =
        AggregateError [ ValidationError "Error 1"
                         ValidationError "Error 2" ]

    let error2 =
        AggregateError [ ValidationError "Error 3"
                         ValidationError "Error 4" ]

    let aggregateError = createAggregateError error1 error2

    let expected =
        AggregateError [ ValidationError "Error 1"
                         ValidationError "Error 2"
                         ValidationError "Error 3"
                         ValidationError "Error 4" ]

    Assert.Equal(expected, aggregateError)
