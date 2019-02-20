#load "../references.fsx"
#load "Common.fs"
#load "Object.fs"
#load "Node.fs"
#load "Light.fs"
#load "Material.fs"
open OpenTK
open Object
open Common
open Object
open Material
// debugFlag <- true
// isInsideDisk (Vector3d(0.1050, 0.9398, 0.0)) (Vector3d(0.15, 0.95, 0.0)) 0.1
let i = (Vector3d(1.0, -1.0, 0.0).Normalized())
let ii = -i
let v = refract i  (Vector3d(0.0, 1.0, 0.0)) 1.5
printfn "\n\n\n"
let vv = refract ii  (Vector3d(0.0, 1.0, 0.0)) 1.5

let v = randomInUnitSphere()
v.X * v.X + v.Y * v.Y + v.Z * v.Z 