module Camera

open OpenTK
open System

type Ray = {Origin : Vector3d; Direction : Vector3d}

let rayDirection123 c r width (height : int) nearZ fieldOfView =
    let side = Math.Tan(fieldOfView) * nearZ
    let width = float width
    let height = float height
    let aspect = height / width
    let x = (float c / width - 0.5) * 2.0 * side
    let y = (float r / height - 0.5) * 2.0 * side * aspect
    let v = Vector3d(x, y, -nearZ)
    v.Normalize()
    v

let private random = new Random()

let randomInUnitDisk () =
    let points = Seq.initInfinite (fun _ -> 
        let x = random.NextDouble()
        let y = random.NextDouble()
        let z = 0.0
        2.0 * Vector3d(x, y, z) - Vector3d(1.0, 1.0, 0.0))
    Seq.find (fun (v : Vector3d) -> v.Length < 1.0) points

type Camera(LookFrom : Vector3d, LookAt : Vector3d, Up : Vector3d, 
            fov : float, width, height, nearZ, far : float, aperture) =

    let mutable lookAt = Matrix4d()
    let lookFromMinus = -LookFrom
    let towardViwer = (LookFrom - LookAt).Normalized()
    let right = Vector3d.Cross(Up.Normalized(), towardViwer)
    let upVector = Vector3d.Cross(towardViwer, right)
    let fov = OpenTK.MathHelper.DegreesToRadians(fov / 2.0)
    let side = Math.Tan(fov) * nearZ
    let width = float width
    let height = float height
    let aspect = height / width
    let lensRadius = aperture / 2.0

    let transform x y z = 
        let v = Vector4d.Transform(Vector4d(x, y, z, 1.0), lookAt)
        Vector3d(v.X / v.W, v.Y / v.W, v.Z / v.W)

    do  
        lookAt <- Matrix4d.LookAt(LookFrom, LookAt, Up)
        printfn "look2 = %A" lookAt
        lookAt.Column0 <- Vector4d(right, 0.0)
        lookAt.Column1 <- Vector4d(upVector, 0.0)
        lookAt.Column2 <- Vector4d(towardViwer, 0.0)
        lookAt.Column3 <- Vector4d(lookFromMinus, 1.0)
        printfn "look1 = %A" lookAt
        printfn "row2 = %A" lookAt.Row2

    member this.TransformV (v : Vector3d) =
        (v.X * right + v.Y * upVector + v.Z * towardViwer) + lookFromMinus

    member this.Ray column row =
        let x = ((float column + random.NextDouble()) / width - 0.5) * 2.0 * side
        let y = ((float row + random.NextDouble()) / height - 0.5) * 2.0 * side * aspect
        let pointNear = Vector3d(x, y, -nearZ)
        let pointFar = pointNear * -far / -nearZ |> this.TransformV
        let pointLens = randomInUnitDisk() * lensRadius + LookFrom
        let direction = (pointFar - pointLens).Normalized()
        let direction = (this.TransformV pointNear - LookFrom).Normalized()
        {Origin = pointLens; Direction = direction}