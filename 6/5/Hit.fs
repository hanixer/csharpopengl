module Hit

open System
open OpenTK
open Common
open Material

type Rotate = RX | RY | RZ

type Hitable = 
    | Sphere of Vector3d * float * Material
    | HitableList of Hitable seq
    | BvhNode of Hitable * Hitable * Bbox
    | XyRect of float * float * float * float * float * Material
    | XzRect of float * float * float * float * float * Material
    | YzRect of float * float * float * float * float * Material
    | FlipNormals of Hitable
    | BoxHitable of Vector3d * Vector3d * Hitable
    | Translate of Hitable * Vector3d

let makeBox (p0 : Vector3d) (p1 : Vector3d) material =
    let list = [
        XyRect(p0.X, p1.X, p0.Y, p1.Y, p1.Z, material)
        FlipNormals(XyRect(p0.X, p1.X, p0.Y, p1.Y, p0.Z, material))
        XzRect(p0.X, p1.X, p0.Z, p1.Z, p1.Y, material)
        FlipNormals(XzRect(p0.X, p1.X, p0.Z, p1.Z, p0.Y, material))
        YzRect(p0.Y, p1.Y, p0.Z, p1.Z, p1.X, material)
        FlipNormals(YzRect(p0.Y, p1.Y, p0.Z, p1.Z, p0.X, material))
    ]
    BoxHitable(p0, p1, HitableList(list))

let hitSphere ray (center, radius, material) tMin tMax =
    let computeHit (t : float) =
        let point = rayPointAtParameter ray t
        let normal = (point - center) / radius
        let tex = getSphericalTexCoord normal
        Some {T = t; Point = point; Normal = normal; Material = material; TexCoord = tex}

    let offset = ray.Origin - center
    let a = Vector3d.Dot(ray.Direction, ray.Direction)
    let b = 2.0 * Vector3d.Dot(ray.Direction, offset)
    let c = Vector3d.Dot(offset, offset) - radius * radius
    let discr = b * b - 4.0 * a * c
    if discr >= 0.0 then
        let t = (-b - Math.Sqrt(discr)) / (2.0 * a)
        if t < tMax && t > tMin then
            computeHit t
        else
            let t = (-b - Math.Sqrt(discr)) / (2.0 * a)
            if t < tMax && t > tMin then
                computeHit t
            else None        
    else
        None

let hitXyRect ray (x0, x1, y0, y1, k, material) tMin tMax =
    let t = (k - ray.Origin.Z) / ray.Direction.Z
    if t < tMin || t > tMax then
        None
    else
        let point = rayPointAtParameter ray t
        if point.X >= x0 && point.X <= x1 && point.Y >= y0 && point.Y <= y1 then
            let u = (point.X - x0) / (x1 - x0)
            let v = (point.Y - y0) / (y1 - y0)
            let tex = Vector2d(u, v)
            let normal = Vector3d(0.0, 0.0, 1.0)      
            Some {T = t; Point = point; Normal = normal; Material = material; TexCoord = tex}
        else
            None

let hitXzRect ray (x0, x1, z0, z1, k, material) tMin tMax =
    let t = (k - ray.Origin.Y) / ray.Direction.Y
    if t < tMin || t > tMax then
        None
    else
        let point = rayPointAtParameter ray t
        if point.X >= x0 && point.X <= x1 && point.Z >= z0 && point.Z <= z1 then
            let u = (point.X - x0) / (x1 - x0)
            let v = (point.Z - z0) / (z1 - z0)
            let tex = Vector2d(u, v)
            let normal = Vector3d(0.0, 1.0, 0.0)      
            Some {T = t; Point = point; Normal = normal; Material = material; TexCoord = tex}
        else
            None

let hitYzRect ray (y0, y1, z0, z1, k, material) tMin tMax =
    let t = (k - ray.Origin.X) / ray.Direction.X
    if t < tMin || t > tMax then
        None
    else
        let point = rayPointAtParameter ray t
        if point.Y >= y0 && point.Y <= y1 && point.Z >= z0 && point.Z <= z1 then
            let u = (point.Y - y0) / (y1 - y0)
            let v = (point.Z - z0) / (z1 - z0)
            let tex = Vector2d(u, v)
            let normal = Vector3d(1.0, 0.0, 0.0)      
            Some {T = t; Point = point; Normal = normal; Material = material; TexCoord = tex}
        else
            None

let hitRotateY (hitable, angle) ray tMin tMax =
    failwith ""

let rec hit hitable ray tMin tMax =
    match hitable with
    | Sphere (center, radius, material) ->
        hitSphere ray (center, radius, material) tMin tMax
    | HitableList hitables ->
        hitList ray hitables tMin tMax
    | BvhNode(left, right, box) ->
        hitBvhNode ray (left, right, box) tMin tMax
    | XyRect(x0, x1, y0, y1, k, material) ->
        hitXyRect ray (x0, x1, y0, y1, k, material) tMin tMax
    | XzRect(x0, x1, z0, z1, k, material) ->
        hitXzRect ray (x0, x1, z0, z1, k, material) tMin tMax
    | YzRect(y0, y1, z0, z1, k, material) ->
        hitYzRect ray (y0, y1, z0, z1, k, material) tMin tMax
    | FlipNormals(hitable) ->
        hit hitable ray tMin tMax
        |> Option.map (fun record -> {record with Normal = -record.Normal})
    | BoxHitable(pmin, pmax, list) ->
        hit list ray tMin tMax
    | Translate(hitable, offset) ->
        let ray = {ray with Origin = ray.Origin - offset}
        hit hitable ray tMin tMax
        |> Option.map (fun record -> {record with Point = record.Point + offset})

and hitList ray hitables tMin tMax =
    let fold (closest, result : HitRecord option) hitable =
        match hit hitable ray tMin tMax with
        | Some record ->
            if record.T < closest then
                (record.T, Some record)
            else
                closest, result
        | None ->
            closest, result

    Seq.fold fold (Double.PositiveInfinity, None) hitables
    |> snd

and hitBvhNode ray (left, right, box) tMin tMax =
    if hitBbox box ray tMin tMax then
        let lResult, rResult = hit left ray tMin tMax, hit right ray tMin tMax
        match lResult, rResult  with
        | Some leftRec, Some rightRec ->
            if leftRec.T < rightRec.T then
                Some leftRec
            else
                Some rightRec
        | Some _, _ -> lResult
        | _, Some _ -> rResult
        | _ -> None
    else    
        None

let surroundingBox (box1 : Bbox) (box2 : Bbox) =
    let (box1Min, box1Max) = box1
    let (box2Min, box2Max) = box2
    Vector3d(Math.Min(box1Min.X, box2Min.X), Math.Min(box1Min.Y, box2Min.Y), Math.Min(box1Min.Z, box2Min.Z)),
    Vector3d(Math.Max(box1Max.X, box2Max.X), Math.Max(box1Max.Y, box2Max.Y), Math.Max(box1Max.Z, box2Max.Z))

let littleNum = 0.00001

let rec boundingBox = function
    | Sphere(center, radius, _) ->
        center - Vector3d(radius), center + Vector3d(radius)
    | HitableList(hitables) ->
        let boxInit = Vector3d(Double.MaxValue), Vector3d(Double.MinValue)
        let fold box hitable =
            surroundingBox box <| boundingBox hitable
        Seq.fold fold boxInit hitables
    | BvhNode(_, _, box) -> box
    | XyRect(x0, x1, y0, y1, k, material) ->
        Vector3d(x0, y0, k - littleNum), Vector3d(x1, y1, k + littleNum)
    | XzRect(x0, x1, z0, z1, k, material) ->
        Vector3d(x0, k - littleNum, z0), Vector3d(x1, k + littleNum, z1)
    | YzRect(y0, y1, z0, z1, k, material) ->
        Vector3d(k - littleNum, y0, z0), Vector3d(k + littleNum, y1, z1)
    | FlipNormals(hitable) -> boundingBox hitable
    | BoxHitable(pmin, pmax, _) -> pmin, pmax
    // | Translate(h, _) -> boundingBox h
    // | RotateY(h, _) -> boundingBox h
    | _ -> 
        failwith "Not implemented BBx"