#load "../references.fsx"
#load "Rest.fs"


open OpenTK
open System
open Rest

let box = Vector3d(2.0, 2.0, 2.0), Vector3d(4.0, 4.0, 4.0)
let ray = {Origin = Vector3d(5.0, 5.0, 5.0); Direction = Vector3d(-1.0, -1.0, -1.0)}

hitBbox box ray Double.MinValue Double.MaxValue

let test (a, b, c) (d, e, f) (h, i, j) (k, l, m) =
    let box = Vector3d(a, b, c), Vector3d(d, e, f)
    let ray = {Origin = Vector3d(h, i, j); Direction = Vector3d(k, l, m)}
    hitBbox box ray Double.MinValue Double.MaxValue

test (2.0, 2.0, 2.0) (4.0, 4.0, 4.0) (0.0, 0.0, 0.0) (1.0, 1.0, 1.0)
test (2.0, 2.0, 2.0) (4.0, 4.0, 4.0) (5.0, 5.0, 5.0) (-1.0, -1.0, -1.0)
test (2.0, 2.0, 2.0) (4.0, 4.0, 4.0) (0.0, 0.0, 0.0) (-1.0, -1.0, -1.0)
test (2.0, 2.0, 2.0) (4.0, 4.0, 4.0) (5.0, 1.0, 2.0) (-1.0, -1.0, -1.0)
test (2.0, 2.0, 2.0) (4.0, 4.0, 4.0) (5.0, 2.0, 5.0) (-1.0, 1.0, -1.0)
test (2.0, 2.0, 2.0) (4.0, 4.0, 4.0) (2.0, 0.0, 0.0) (0.0, 1.0, -1.0)
