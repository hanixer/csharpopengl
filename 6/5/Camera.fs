module Camera

open OpenTK
open System

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
            fov : float, width, height, nearZ) =

    let mutable lookAt = Matrix4d()
    let lookFromMinus = -LookFrom
    let towardViwer = (LookFrom - LookAt).Normalized()
    let right = Vector3d.Cross(Up.Normalized(), towardViwer)
    let upVector = Vector3d.Cross(towardViwer, right)
    let fov = OpenTK.MathHelper.DegreesToRadians(fov)
    let side = Math.Tan(fov) * nearZ
    let width = float width
    let height = float height
    let aspect = height / width

    do    
        lookAt.Row0 <- Vector4d(right, Vector3d.Dot(right, lookFromMinus))
        lookAt.Row1 <- Vector4d(upVector, Vector3d.Dot(upVector, lookFromMinus))
        lookAt.Row2 <- Vector4d(towardViwer, Vector3d.Dot(towardViwer, lookFromMinus))
        lookAt.Row3 <- Vector4d(0.0, 0.0, 0.0, 1.0)

    member this.RayOrigin = LookFrom

    member this.RayDirection column row  = 
        let x = (float column / width - 0.5) * 2.0 * side
        let y = (float row / height - 0.5) * 2.0 * side * aspect
        let v = Vector4d(x, y, -nearZ, 1.0)
        let v = Vector4d.Transform(v, lookAt)
        let v = Vector3d(v.X / v.W, v.Y / v.W, v.Z / v.W)
        v

