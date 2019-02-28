module Camera

open System
open OpenTK
open Common

type Camera(lookFrom : Vector3d, lookAt : Vector3d, upInput : Vector3d, fov : float, width, height) =
    let towardViwer = (lookFrom - lookAt).Normalized()
    let right = Vector3d.Cross(upInput.Normalized(), towardViwer)
    let up = Vector3d.Cross(towardViwer, right)
    let fov = OpenTK.MathHelper.DegreesToRadians (fov / 2.0)
    let widthf = float width
    let heightf = float height
    let aspect = heightf / widthf
    let scale = Math.Tan fov
    let mat3 = Matrix3d(right, up, towardViwer)
    let random = Random()

    let transformToWorld vec3 =
        let x = Vector3d.Dot(vec3, mat3.Column0)
        let y = Vector3d.Dot(vec3, mat3.Column1)
        let z = Vector3d.Dot(vec3, mat3.Column2)
        Vector3d(x, y, z)

    member this.Width = width
    member this.Height = height

    member this.Ray column row =
        let x = ((float column + random.NextDouble()) / widthf - 0.5) * scale
        let y = ((float row + random.NextDouble()) / heightf - 0.5) * scale * aspect
        let dir = Vector3d(x, y, -1.0).Normalized()
        let dirT = transformToWorld dir
        {Origin = lookFrom; Direction = dirT.Normalized()}

let defCameraPos = Vector3d.Zero
let defCameraTarget = Vector3d(0.0, 0.0, -1.0)
let defCameraUp = Vector3d(0.0, 1.0, 0.0)
let defCameraFov = 40.0
let defCameraWidth = 200.0
let defCameraHeight = 150.0