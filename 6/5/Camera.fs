module Camera

open OpenTK
open System
open Common

let private random = new Random()

let randomInUnitDisk () =
    let points = Seq.initInfinite (fun _ -> 
        let x = random.NextDouble()
        let y = random.NextDouble()
        let z = 0.0
        2.0 * Vector3d(x, y, z) - Vector3d(1.0, 1.0, 0.0))
    Seq.find (fun (v : Vector3d) -> v.Length < 1.0) points

type Camera(lookFrom : Vector3d, lookAt : Vector3d, up : Vector3d, 
            fov : float, width, height, nearZ, far : float, aperture) =

    let towardViwer = (lookFrom - lookAt).Normalized()
    let right = Vector3d.Cross(up.Normalized(), towardViwer)
    let upVector = Vector3d.Cross(towardViwer, right)
    let fov = OpenTK.MathHelper.DegreesToRadians(fov / 2.0)
    let side = Math.Tan(fov) * nearZ
    let width = float width
    let height = float height
    let aspect = height / width
    let lensRadius = aperture / 2.0
    let mat3 = Matrix3d(right, upVector, towardViwer)

    let transform vec3 =
        Vector3d(Vector3d.Dot(vec3, mat3.Row0),Vector3d.Dot(vec3 , mat3.Row1),Vector3d.Dot(vec3 , mat3.Row2)).Normalized()

    do  
        mat3.Transpose()

    member this.Ray column row =
        let x = ((float column + random.NextDouble()) / width - 0.5) * 2.0 * side
        let y = ((float row + random.NextDouble()) / height - 0.5) * 2.0 * side * aspect
        let pointNear = Vector3d(x, y, -nearZ)
        let pointFar = pointNear * -far / -nearZ
        let pointLens = randomInUnitDisk() * lensRadius
        let pointLens = Vector3d.Zero
        let direction = (pointFar - pointLens).Normalized() |> transform
        {Origin = pointLens + lookFrom; Direction = direction}