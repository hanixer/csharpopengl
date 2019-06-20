module Object

open System
open OpenTK
open Common
open Sampling
open Types

let quadratic a b c =
    let discrim = b * b - 4.0 * a * c
    if discrim < 0.0 then
        None
    else
        let discrimRoot = Math.Sqrt(discrim)
        let t0 = (-b - discrimRoot) / (2.0 * a)
        let t1 = (-b + discrimRoot) / (2.0 * a)
        if t0 > t1 then
            Some(t1, t0)
        else
            Some(t0, t1)

let isInsideDisk (point : Vector3d) (center : Vector3d) radius =
    (point - center).Length * (point - center).Length < radius * radius

let defaultHitInfo = {
    T = 0.0
    Point = Vector3d.Zero
    Normal = Vector3d.Zero
    Prim = None
}

// (1 - beta - gamma) * a + beta * b + gamma * c = o + t * d
// a + beta * (b - a) + gamma * (c - gamma) = o + t * d
// beta * (b - a) + gamma * (c - a) - t * d = o - a
// | a b c | | beta  |   | d |
// | e f g | | gamma | = | h |
// | i j k | | t     |   | l |
// a = (b.x - a.x)
// b = ()
// [a e i] = b - a
// [b f j] = c - a
// [c g k] = -d
// [d h l] = o - a
let intersectTriangle ray (p0 : Vector3d) (p1 : Vector3d) (p2 : Vector3d) =
    let aei = p1 - p0
    let bfj = p2 - p0
    let cgk = -ray.Direction
    let dhl = ray.Origin - p0
    let m : Matrix3d = Matrix3d(aei, bfj, cgk)
    m.Transpose()
    let det = m.Determinant
    if Math.Abs(det) < epsilon then
        None
    else
        let m1 = Matrix3d(dhl, bfj, cgk)
        m1.Transpose()
        let beta = m1.Determinant / det
        let m2 = Matrix3d(aei, dhl, cgk)
        m2.Transpose()
        let gamma = m2.Determinant / det
        let m3 = Matrix3d(aei, bfj, dhl)
        m3.Transpose()
        let t = m3.Determinant / det
        let normal = Vector3d.Cross(p1 - p0, p2 - p0)
        normal.Normalize()
        let alpha = 1.0 - beta - gamma
        let inline inRange r = r > 0.0 && r < 1.0
        if inRange alpha && inRange beta && inRange gamma && t > epsilon then
            let point = pointOnRay ray t
            Some {defaultHitInfo with T = t; Point = point; Normal = normal}
        else
            None

let intersectTriangleObj ray (data : TriangleMesh.Data) =
    Seq.map (fun (face : TriangleMesh.Face) ->
        let p0 = data.Vertex(face.[0])
        let p1 = data.Vertex(face.[1])
        let p2 = data.Vertex(face.[2])
        intersectTriangle ray p0 p1 p2 ) data.Faces
    |> tryFindBestHitInfo

let intersectRectangle ray (p0 : Vector3d) (p1 : Vector3d) (p2 : Vector3d) =
    let v1 = p1 - p0
    let v2 = p2 - p0
    let normal = Vector3d.Cross(v1, v2)
    normal.Normalize()
    let t = Vector3d.Dot(p0 - ray.Origin, normal) / Vector3d.Dot(ray.Direction, normal)
    if t > epsilon then
        let point = pointOnRay ray t
        let d = point - p0
        let dDotV2 = Vector3d.Dot(d, v2)
        let dDotV1 = Vector3d.Dot(d, v1)
        if dDotV2 < 0.0 || dDotV2 > v2.LengthSquared then
            None
        else if dDotV1 < 0.0 || dDotV1 > v1.LengthSquared then
            None
        else
            Some {defaultHitInfo with T = t; Point = point; Normal = normal}
    else
        None

let intersectPlane ray =
    intersectRectangle ray (Vector3d(-1.0, 1.0, 0.0)) (Vector3d(1.0, 1.0, 0.0)) (Vector3d(-1.0, -1.0, 0.0))

let intersectDisk ray =
    let t = (- ray.Origin.Z) / ray.Direction.Z
    let radius = 1.0
    if t > epsilon then
        let point = pointOnRay ray t
        if point.X * point.X + point.Y * point.Y < radius then
            let normal = Vector3d(0.0, 0.0, 1.0)
            Some {defaultHitInfo with T = t; Point = point; Normal = normal}
        else
            None
    else
        None

let intersectRectWithHoles ray width radius =
    let t = (- ray.Origin.Z) / ray.Direction.Z
    if t < ray.TMin then
        None
    else
        let point = pointOnRay ray t
        let x0 = 0.0
        let x1 = width
        let y0 = 0.0
        let y1 = width
        if point.X >= x0 && point.X <= x1 && point.Y >= y0 && point.Y <= y1 then
            let ration = width / (2.0 * radius)
            let r = int ((point.Y / width) * ration)
            let c = int ((point.X / width) * ration)
            let holeEnabled = (r % 2 = 0 && c % 2 = 0 || r % 2 <> 0 && c % 2 <> 0)
            if not holeEnabled then
                let normal = Vector3d(0.0, 0.0, 1.0)
                Some {defaultHitInfo with T = t; Point = point; Normal = normal}
            else
                let center = Vector3d((float c + 0.5) / ration, (float r + 0.5) / ration, 0.0)
                if isInsideDisk point center radius then
                    None
                else
                    let normal = Vector3d(0.0, 0.0, 1.0)
                    Some {defaultHitInfo with T = t; Point = point; Normal = normal}

        else
            None

let rec intersect ray object =
    let computeHit (t : float) =
        let point = pointOnRay ray t
        let normal = point.Normalized()
        Some {defaultHitInfo with T = t; Point = point; Normal = normal}

    match object with
    | Triangle(a, b, c) ->
        intersectTriangle ray a b c
    | TriangleObjPart(a, b, c, data) ->        
        let p0 = data.Vertex(a)
        let p1 = data.Vertex(b)
        let p2 = data.Vertex(c)
        intersectTriangle ray p0 p1 p2
    | Sphere ->
        let offset = ray.Origin
        let a = Vector3d.Dot(ray.Direction, ray.Direction)
        let b = 2.0 * Vector3d.Dot(ray.Direction, offset)
        let c = Vector3d.Dot(offset, offset) - 1.0
        match quadratic a b c with
        | Some(t0, _) when t0 > ray.TMin ->
            computeHit t0
        | Some(_, t1) when t1 > ray.TMin ->
            computeHit t1
        | _ ->
            None
    | Cylinder ->
        let computeHit (t : float) =
            let point = pointOnRay ray t
            let normal = Vector3d(point.X, 0.0, point.Z).Normalized()
            if point.Y <= 1.0 && point.Y >= -1.0 then
                Some {defaultHitInfo with T = t; Point = point; Normal = normal}
            else
                None

        let a = ray.Direction.X * ray.Direction.X + ray.Direction.Z * ray.Direction.Z
        let b = 2.0 * (ray.Origin.X * ray.Direction.X + ray.Origin.Z * ray.Direction.Z)
        let c = ray.Origin.X * ray.Origin.X + ray.Origin.Z * ray.Origin.Z - 1.0
        if debugFlag then
            printfn "a = %A; b = %A; c = %A" a b c
            printfn "%A " <| quadratic a b c
        match quadratic a b c with
        | Some(t0, _) when t0 > ray.TMin ->
            computeHit t0
        | Some(_, t1) when t1 > ray.TMin ->
            computeHit t1
        | _ ->
            None
    | RectXYWithHoles(width, radius) ->
        intersectRectWithHoles ray width radius
    | Disk ->
        intersectDisk ray
    | Rectangle(p0, p1, p2) ->
        intersectRectangle ray p0 p1 p2
    | Plane ->
        intersectPlane ray
    | ObjectList objs ->
        Seq.map (fun object -> intersect ray object) objs
        |> Seq.minBy (fun h ->
            if h.IsSome then h.Value.T
            else Double.MaxValue)
    | TriangleObj data ->
        intersectTriangleObj ray data

let samplePointRectangle (p0 : Vector3d) (p1 : Vector3d) (p2 : Vector3d)=
    let r0, r1 = randomTwo()
    let d1 = p1 - p0
    let d2 = p2 - p0
    let width = d1.Length
    let height = d2.Length
    let v1 = r0 * width * d1
    let v2 = r1 * height * d2
    let norm = Vector3d.Cross(d1, d2)
    Some(p0 + v1 + v2, norm.Normalized())

let samplePointAndNormOnObject object =
    match object with
    | Disk ->
        let norm = Vector3d(0.0, 0.0, 1.0)
        Some(randomInDisk(), norm)
    | Sphere ->
        let point = randomInHemisphere2()
        let norm = point.Normalized()
        Some(randomInHemisphere2(), norm)
    | Rectangle(p0, p1, p2) ->
        samplePointRectangle p0 p1 p2
    | Plane ->
        samplePointRectangle (Vector3d(-1.0, 0.0, 1.0)) (Vector3d(1.0, 0.0, 1.0)) (Vector3d(-1.0, 0.0, -1.0))
    | _ -> None

let sample object (sample : Vector2d) =
    match object with
    | Disk ->
        let norm = Vector3d(0.0, 0.0, 1.0)
        let p = squareToCircle sample
        Some(randomInDisk(), norm)
    | Sphere ->
        let point = randomInHemisphere2()
        let norm = point.Normalized()
        Some(randomInHemisphere2(), norm)
    | Rectangle(p0, p1, p2) ->
        samplePointRectangle p0 p1 p2
    | Plane ->
        samplePointRectangle (Vector3d(-1.0, 0.0, 1.0)) (Vector3d(1.0, 0.0, 1.0)) (Vector3d(-1.0, 0.0, -1.0))
    | _ -> None

let getAreaOfObject object =
    match object with
    | Disk -> 2.0 * Math.PI
    | Sphere -> 4.0 * Math.PI
    | Rectangle(p0, p1, p2) ->
        let d1 = p1 - p0
        let d2 = p2 - p0
        d1.Length * d2.Length
    | Plane -> 4.0
    | _ -> 0.0

let worldBounds object =
    match object with
    | Triangle(p0, p1, p2) ->
        let b0 = Bounds.makeBounds p0 p0
        let b1 = Bounds.addPoint b0 p1
        Bounds.addPoint b1 p2
    | TriangleObjPart(a, b, c, data) ->        
        let p0 = data.Vertex(a)
        let p1 = data.Vertex(b)
        let p2 = data.Vertex(c)
        Bounds.unionManyP [ p0; p1; p2 ]
    | _ ->
        Bounds.makeBounds (Vector3d.One * -1.) Vector3d.One

let makeBox (p0 : Vector3d) (p1 : Vector3d) =
    let v0 = p0
    let v1 = Vector3d(p1.X, p0.Y, p0.Z)
    let v2 = Vector3d(p1.X, p0.Y, p1.Z)
    let v3 = Vector3d(p0.X, p0.Y, p1.Z)
    let v4 = Vector3d(p0.X, p1.Y, p0.Z)
    let v5 = Vector3d(p1.X, p1.Y, p0.Z)
    let v6 = p1
    let v7 = Vector3d(p0.X, p1.Y, p1.Z)
    let f0 = Rectangle(v0, v1, v4)
    let f1 = Rectangle(v1, v2, v5)
    let f2 = Rectangle(v2, v3, v6)
    let f3 = Rectangle(v3, v0, v7)
    let f4 = Rectangle(v4, v5, v7)
    let f5 = Rectangle(v0, v1, v3)
    ObjectList [f0; f1; f2; f3; f4; f5]

let makeTriangleShapes (data : TriangleMesh.Data) =
    Array.init data.FacesCount (fun faceIndex ->
        let a = data.Faces.[faceIndex].[0]
        let b = data.Faces.[faceIndex].[1]
        let c = data.Faces.[faceIndex].[2]
        TriangleObjPart(a, b, c, data))