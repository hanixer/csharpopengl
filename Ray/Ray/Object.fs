module Object

open System
open OpenTK
open Common

type Object = 
    | Sphere
    | Cylinder
    | RectXYWithHoles of float * float // width, radius
    | Triangle of Vector3d * Vector3d * Vector3d
    | Disk
    | Rectangle of Vector3d * Vector3d * Vector3d

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
    Material = ""
    Depth = 0
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

let intersectRectangle ray (p0 : Vector3d) (p1 : Vector3d) (p2 : Vector3d) =
    let v1 = p1 - p0
    let v2 = p2 - p0
    let normal = Vector3d.Cross(v2, v1)
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
            Some {defaultHitInfo with T = t; Point = pointOnRay ray t; Normal = normal}
    else
        None

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

let intersectRectWithHoles ray width radius tMin =
    let t = (- ray.Origin.Z) / ray.Direction.Z
    if t < tMin then
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

let intersect ray object tMin material =
    let computeHit (t : float) =
        let point = pointOnRay ray t
        let normal = point.Normalized()
        Some {defaultHitInfo with T = t; Point = point; Normal = normal}

    match object with
    | Triangle(a, b, c) ->
        intersectTriangle ray a b c
    | Sphere ->
        let offset = ray.Origin
        let a = Vector3d.Dot(ray.Direction, ray.Direction)
        let b = 2.0 * Vector3d.Dot(ray.Direction, offset)
        let c = Vector3d.Dot(offset, offset) - 1.0
        match quadratic a b c with
        | Some(t0, _) when t0 > tMin ->
            computeHit t0
        | Some(_, t1) when t1 > tMin ->
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
        | Some(t0, _) when t0 > tMin ->
            computeHit t0
        | Some(_, t1) when t1 > tMin ->
            computeHit t1
        | _ -> 
            None
    | RectXYWithHoles(width, radius) ->    
        intersectRectWithHoles ray width radius tMin
    | Disk ->
        intersectDisk ray
    | Rectangle(p0, p1, p2) ->
        intersectRectangle ray p0 p1 p2
let samplePointOnObject object =
    match object with
    | Disk ->
        Some(randomInDisk())
        // Some(randomInHemisphere())
    | Sphere ->
        Some(randomInHemisphere2())
    | _ -> None

let getAreaOfObject object =
    match object with
    | Disk -> 2.0 * Math.PI
    | Sphere -> 4.0 * Math.PI
    | _ -> 0.0