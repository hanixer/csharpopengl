#load @"..\references.fsx"

open Transform
open OpenTK

let t1 = translate (Vector3d(-1.))
let t2 = scale (Vector3d(3.))
let t3 = compose t1 t2
let t4 = compose t2 t1
let p = Vector3d(1.,2.,3.)
let p1 = (transformPoint t3 p) = (transformPoint t1 (transformPoint t2 p))
let p2 = (transformPoint t4 p) = (transformPoint t2 (transformPoint t1 p))
let p3 = (transformPoint t4 p) <> (transformPoint t3 p)