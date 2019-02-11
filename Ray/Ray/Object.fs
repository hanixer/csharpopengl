module Object

open System
open OpenTK
open Common

type Object = 
    | Sphere
    | Cylinder
    | RectXYWithHoles of float * float * float // width, height, radius

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
    | RectXYWithHoles(width, height, radius) ->    
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
                    Some {T = t; Point = point; Normal = normal; Material = material;}
                else
                    let center = Vector3d((float c + 0.5) / ration, (float r + 0.5) / ration, 0.0)
                    if isInsideDisk point center radius then
                        printfn "c = %A; r = %A; point = %A; center = %A; radius = %A => inside" c r point center radius
                        printfn ""
                        None
                    else
                        printfn "c = %A; r = %A; point = %A; center = %A; radius = %A => outside" c r point center radius
                        printfn ""
                        let normal = Vector3d(0.0, 0.0, 1.0)
                        Some {T = t; Point = point; Normal = normal; Material = material;}

            else
                None