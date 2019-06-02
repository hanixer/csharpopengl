module Camera

open System
open OpenTK
open Common

// World space - commons space for all objects and ray
// Camera space - space in which camera is placed to origin, up direction is y, look direction is -z
// Screen space - space of the image, top-left is -1, aspect, bottom right is 1, -aspect, where aspect = width / height
// NDC - top-left is 0,0, bottom-right is 1,1
// Raster space - top-left is 0,0, bottom-right is width,height

// Camera is used to generate rays
type Camera(lookFrom : Vector3d, lookAt : Vector3d, upInput : Vector3d, fov : float, width, height) =
    let widthf = float width
    let heightf = float height
    let aspect = widthf / heightf
    let fovHorizontal = fov * aspect
    let cameraToScreen = Transform.perspective fovHorizontal 1e-4 1e4
    let screenToCamera = Transform.inverted cameraToScreen
    let screenToRaster = Transform.screenToRaster width height
    let rasterToScreen = Transform.inverted screenToRaster
    let rasterToCamera = Transform.compose screenToCamera rasterToScreen
    let cameraToWorld = Transform.lookAt lookFrom lookAt upInput

    member __.Width = width
    member __.Height = height

    member __.Ray2 (rasterSample : Vector2d) =
        let rasterP = Vector3d(rasterSample)
        let cameraP = Transform.transformPoint rasterToCamera rasterP
        let worldP = Transform.transformPoint cameraToWorld cameraP
        let direction = worldP - lookFrom
        direction.Normalize()
        { Origin = lookFrom
          Direction = direction }

let defCameraPos = Vector3d.Zero
let defCameraTarget = Vector3d(0.0, 0.0, -1.0)
let defCameraUp = Vector3d(0.0, 1.0, 0.0)
let defCameraFov = 40.0
let defCameraWidth = 200.0
let defCameraHeight = 150.0