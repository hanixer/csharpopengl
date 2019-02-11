#load "../references.fsx"
#load "Common.fs"
#load "Object.fs"
#load "Node.fs"
#load "Light.fs"
#load "Material.fs"
open OpenTK
open Object
open Common
open Material 
debugFlag <- true
intersect {Origin = Vector3d(0.0, 0.0, 100.0); Direction = Vector3d(0.0, 0.0, -1.0)} Cylinder 0.00001 "thing"
quadratic 1.0 (-2.0) 0.5