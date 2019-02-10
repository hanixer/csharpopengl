module Object

open System
open OpenTK
open Common

type Object = Sphere

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
        let discr = b * b - 4.0 * a * c
        if discr >= 0.0 then
            let t = (-b - Math.Sqrt(discr)) / (2.0 * a)
            if t > tMin then
                computeHit t
            else
                let t = (-b + Math.Sqrt(discr)) / (2.0 * a)
                if t > tMin then
                    computeHit t
                else None        
        else
            None