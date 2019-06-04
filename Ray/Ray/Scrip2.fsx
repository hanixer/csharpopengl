#load @"..\references.fsx"

open Transform
open OpenTK
open Bounds

let a = [| 0 .. 4 |]
let k = Array.splitAt (a.Length / 2) a


let p0 = Vector3d(-10.0000, 10.0000, 0.0000)
let p1 = Vector3d(-10.0000 ,-10.0000, 0.0000)
let p2 = Vector3d(10.0000 ,-10.0000, 0.0000)
let p3 = Vector3d(10.0000, 10.0000, 0.0000)
let bb0 = makeBounds p0 p0
let bb1 = addPoint(addPoint bb0 p1)p2
let bb2 = addPoint(addPoint bb0 p2)p3