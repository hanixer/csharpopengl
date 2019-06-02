#load @"..\references.fsx"

open Transform
open OpenTK
open Bounds

let b1 = {PMin=Vector3d.Zero; PMax=Vector3d(5.)}
let b2 = {PMin=Vector3d.One * -1.; PMax=Vector3d(2.)}
let b3 = union b1 b2
let t1 = scale (Vector3d(1., -4., 1.))
let b4 = bounds t1 b1