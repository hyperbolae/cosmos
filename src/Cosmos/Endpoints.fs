namespace Cosmos.Endpoints

open Giraffe

module Warbler =
    let warbler f = warbler (fun _ -> f ())

module Threads =

    let handler : HttpHandler =
        choose [ route "" >=> text "benga threads" ]

module Updates =

    let handler : HttpHandler =
        choose [ route "" >=> text "benga updates" ]
