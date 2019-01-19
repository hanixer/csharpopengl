#load "../references.fsx"
// #load "Render.fs"
#load "Camera.fs"

// open FsCheck
// open Render

// let revRevIsOrig (xs:list<int>) = List.rev(List.rev xs) = xs



// Check.Quick revRevIsOrig

open OpenTK
open System
open Camera

let cam = Camera(Vector3d(0.0, 0.0, 3.0), Vector3d(0.0, 0.0, -1.0), Vector3d(0.0, 1.0, 0.0), 90.0, 200, 100, 0.1, 100.0, 0.2)
cam.TransformV (Vector3d(0.0))
cam.Ray 100 50
// cam.Ray 199 50