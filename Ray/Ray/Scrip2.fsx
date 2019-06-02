#load @"..\references.fsx"

open Transform
open OpenTK

let width = 8
let height = 6
let aspect = 8./6.
let t1 = screenToRaster width height
let p1 = Vector3d(-1., 3./4., 0.)
let p2 = transformPoint t1 p1
let p3 = Vector3d.Zero
let p5 = Vector3d(1., -3./4., 0.)
let t3 = inverted t1 // rasterToScreen
let p10 = Vector3d(0.5,0.5,0.)
let p11 = transformPoint t3 p10
let p12 = transformPoint t3 Vector3d.Zero
let p13 = Vector3d(8.,6.,0.)
let p14 = transformPoint t3 p13