#load "../references.fsx"
#load "Transform.fs"


open OpenTK
open System
open Transform

let point = Vector3d(1.0, 0.0, 0.0)
let th = MathHelper.DegreesToRadians 30.0
let t = rotateY th
let p1 = transformPoint t point
let p2 = transformPointInv t p1
// Math.Sin(Open)
// Math.Cos()