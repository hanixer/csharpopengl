// Learn more about F# at http://fsharp.org

open System
open Rest
open OpenTK

let test (a, b, c) (d, e, f) (h, i, j) (k, l, m) =
    let box = Vector3d(a, b, c), Vector3d(d, e, f)
    let ray = {Origin = Vector3d(h, i, j); Direction = Vector3d(k, l, m)}
    hitBbox box ray Double.MinValue Double.MaxValue

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    test (2.0, 2.0, 2.0) (4.0, 4.0, 4.0) (0.0, 0.0, 0.0) (-1.0, -1.0, -1.0) |> ignore
    test (2.0, 2.0, 2.0) (4.0, 4.0, 4.0) (0.0, 0.0, 0.0) (1.0, 1.0, 1.0) |> ignore
    0 // return an integer exit code
