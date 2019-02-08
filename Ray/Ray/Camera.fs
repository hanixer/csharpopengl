module Camera

open OpenTK

type Camera(lookFrom : Vector3d, lookAt : Vector3d, up : Vector3d, fov, width, height) =
    let dir = (lookAt - lookFrom).Normalized()
    member this.Nothing x = 0

let defCameraPos = Vector3d.Zero
let defCameraTarget = Vector3d(0.0, 0.0, -1.0)
let defCameraUp = Vector3d(0.0, 1.0, 0.0)
let defCameraFov = 40.0
let defCameraWidth = 200.0
let defCameraHeight = 150.0