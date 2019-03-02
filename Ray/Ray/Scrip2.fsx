#load "..\\references.fsx"
#load "Sampling.fs"

open FSharp.Charting
open System
open OpenTK
open Sampling

let random = Random()

Seq.init 10000 (fun _ ->
    let r1 = random.NextDouble()
    let r2 = random.NextDouble()
    r1, 2.0 * Math.PI * r2
    )
// |> Seq.map (fun (v : Vector3d) ->
//     v.X, v.Y)
|> Chart.Point 