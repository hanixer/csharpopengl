#load "references.fsx"
// #load "Transform.fs"
// #load "Camera.fs"
// #load "Object.fs"
// #load "Node.fs"
// #load "Scene.fs"
open Transform
open OpenTK

let t = rotate (Vector3d(0.0, 0.0, 1.0)) 30.0
let v = transformVector (t.M) (Vector3d(1.0, 0.0, 0.0))
let v2 = transformVector (t.Inv) (Vector3d(1.0, 0.0, 0.0))
let v3 = transformVector (t.Inv) v
GlmNet.glm.rotate(30.0f, GlmNet.vec3(0.0f, 0.0f, 1.0f))
let gt = GlmNet.glm.rotate(OpenTK.MathHelper.DegreesToRadians 30.0f, GlmNet.vec3(0.0f, 0.0f, 1.0f))
let gt2 = GlmNet.glm.rotate(OpenTK.MathHelper.DegreesToRadians -30.0f, GlmNet.vec3(0.0f, 0.0f, 1.0f))
let gv = gt * (GlmNet.vec4(1.0f, 0.0f, 0.0f, 0.0f))
let gv2 = gt2 * (GlmNet.vec4(1.0f, 0.0f, 0.0f, 0.0f))
let ga = gt.to_array()
let gv3 = gt * gv2
gv2.x, gv2.y, gv2.z
gv3.x, gv3.y, gv3.z