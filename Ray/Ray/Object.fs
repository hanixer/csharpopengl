module Object

open System
open OpenTK
open Common

type Object = 
    | Sphere
    | Cylinder

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

let intersect ray object tMin material =
    let computeHit (t : float) =
        let point = pointOnRay ray t
        let normal = point.Normalized()
        Some {T = t; Point = point; Normal = normal; Material = material}

    match object with
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
                Some {T = t; Point = point; Normal = normal; Material = material}
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
