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
    let towardViwer = (lookFrom - lookAt).Normalized()
    let right = Vector3d.Cross(upInput.Normalized(), towardViwer)
    let up = Vector3d.Cross(towardViwer, right)
    let fov2 = OpenTK.MathHelper.DegreesToRadians(fov)
    let widthf = float width
    let heightf = float height
    let aspect2 = heightf / widthf
    let aspect = widthf / heightf
    let scale = Math.Tan fov2
    let mat3 = Matrix3d(right, up, towardViwer)
    let random = Random()
    let cameraToScreen = Transform.perspective fov 1e-4 1e4
    let screenToCamera = Transform.inverted cameraToScreen
    let screenToRaster = Transform.screenToRaster width height
    let rasterToScreen = Transform.inverted screenToRaster
    let rasterToCamera = Transform.compose screenToCamera rasterToScreen
    let toWorldMatrix = Transform.transpose (Matrix4d.LookAt(lookFrom, lookAt, upInput))
    let cameraToWorld = Transform.lookAt lookFrom lookAt upInput
    let transformToWorld vec3 =
        let x = Vector3d.Dot(vec3, mat3.Column0)
        let y = Vector3d.Dot(vec3, mat3.Column1)
        let z = Vector3d.Dot(vec3, mat3.Column2)
        Vector3d(x, y, z)

    member this.Width = width
    member this.Height = height

    member this.Ray2 (rasterSample : Vector2d) =
        let rasterP = Vector3d(rasterSample)
        let p1 = Transform.transformPoint rasterToScreen rasterP
        let p2 = Transform.transformPoint screenToCamera p1
        let cameraP = Transform.transformPoint rasterToCamera rasterP
        let worldP = Transform.transformPoint cameraToWorld cameraP
        let direction = worldP - lookFrom
        direction.Normalize()
        // printfn "rs = %A p1 = %A p2 = %A d = %A" rasterSample p1 p2 direction
        { Origin = lookFrom
          Direction = direction }

    member this.Ray column row =
        let x = ((float column + random.NextDouble()) / widthf - 0.5) * scale
        let y = ((float row + random.NextDouble()) / heightf - 0.5) * scale * aspect2
        let dir = Vector3d(x, y, -1.0).Normalized()
        let dirT = transformToWorld dir
        {Origin = lookFrom; Direction = dirT.Normalized()}

let defCameraPos = Vector3d.Zero
let defCameraTarget = Vector3d(0.0, 0.0, -1.0)
let defCameraUp = Vector3d(0.0, 1.0, 0.0)
let defCameraFov = 40.0
let defCameraWidth = 200.0
let defCameraHeight = 150.0