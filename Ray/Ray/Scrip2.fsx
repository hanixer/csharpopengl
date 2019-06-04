#load @"..\references.fsx"

open Transform
open OpenTK
open Bounds

let a = [| 0 .. 4 |]
let k = Array.splitAt (a.Length / 2) a