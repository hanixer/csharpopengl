#load @"..\references.fsx"

open Transform
open OpenTK
open Bounds

// (0.00, 0.00, 2.00) (0.03, -0.20, -0.98) (-1.00, -1.00, -1.00) (1.00, 1.00, 1.00)

let b1 = makeBounds (Vector3d(-1., -1., -1.)) (Vector3d(1., 1.,1.))
let r = Common.makeRay (Vector3d(0.,0.,2.)) (Vector3d(0.03,-0.2,-0.98).Normalized())
let res2 = hitBoundingBox r b1