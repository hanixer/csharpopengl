#load @"..\references.fsx"

open Transform
open OpenTK
open Bounds

let t1 = scale (Vector3d(-2.,-2.,1.))
let r = Common.makeRay (Vector3d.Zero) (Vector3d(3.,3.,3.).Normalized())
let r1 = Transform.ray t1 r