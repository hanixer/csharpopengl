#load @"..\references.fsx"

open Transform
open OpenTK
open Bounds

let pw = scale (Vector3d(1.,-0.5,1.))
let wp = inverted pw
let n = Vector3d(0., -1.0, 0.)
let n2 = transformNormal pw n