#load "../references.fsx"
#load "Common.fs"
#load "Object.fs"
// #load "Node.fs"
// #load "Light.fs"
// #load "Material.fs"
open OpenTK
open Object
open Common
open Object
debugFlag <- true
isInsideDisk (Vector3d(0.1050, 0.9398, 0.0)) (Vector3d(0.15, 0.95, 0.0)) 0.1