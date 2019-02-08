module Transform

open OpenTK
open System

type Transform = {
    M : Matrix4d
    Inv : Matrix4d
}


let transpose (m : Matrix4d) =
    Matrix4d (
        m.Column0, m.Column1, m.Column2, m.Column3
    )

let rotateY theta =
    let cosTheta = Math.Cos(theta)
    let sinTheta = Math.Sin(theta)    
    let r0 = Vector4d(cosTheta, 0.0, sinTheta, 0.0)
    let r1 = Vector4d(0.0, 1.0, 0.0, 0.0)
    let r2 = Vector4d(-sinTheta, 0.0, cosTheta, 0.0)
    let r3 = Vector4d(0.0, 0.0, 0.0, 1.0)
    let m = Matrix4d(r0, r1, r2, r3)
    let inv = transpose m
    {M = m; Inv = inv}

let translate delta =
    let mutable m = Matrix4d.Identity
    let mutable inv = Matrix4d.Identity
    m.Column3 <- Vector4d(delta, 1.0)
    inv.Column3 <- Vector4d(-delta, 1.0)
    {M = m; Inv = inv}

let scale (v : Vector3d) =
    let mutable m = Matrix4d.Identity
    let mutable inv = Matrix4d.Identity
    m.M11 <- v.X
    m.M22 <- v.Y
    m.M33 <- v.Z
    inv.M11 <- 1.0 / v.X
    inv.M22 <- 1.0 / v.Y
    inv.M33 <- 1.0 / v.Z
    {M = m; Inv = inv}

let private transformPointHelper (m : Matrix4d) (point : Vector3d) =
    let point4 = Vector4d(point, 1.0)
    let x = Vector4d.Dot(point4, m.Row0)
    let y = Vector4d.Dot(point4, m.Row1)
    let z = Vector4d.Dot(point4, m.Row2)
    let w = Vector4d.Dot(point4, m.Row3)
    Vector3d(x / w, y / w, z / w)

let transformPoint (transform : Transform) (point : Vector3d) =
    transformPointHelper transform.M point

let transformPointInv (transform : Transform) (point : Vector3d) =
    transformPointHelper transform.Inv point

// let transformNormal (transform : Transform) (n : Vector3d) =
//     let m = transform.Inv
//     Vector3d (
//         m.M11 * n.X + m.M21 * n.Y + m.M31 + n.Z,
//         m.M12 * n.X + m.M22 * n.Y + m.M32 + n.Z,
//         m.M13 * n.X + m.M23 * n.Y + m.M33 + n.Z
//     )

let t = scale (Vector3d(2.0))

transformPoint t (Vector3d(5.0, 6.0, 7.0))