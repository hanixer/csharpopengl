module Render

open System
open System.Drawing
open OpenTK
open System.Diagnostics
open Camera

type Bitmap = System.Drawing.Bitmap
type Color = System.Drawing.Color
type Material =
    | Lambertian of Vector3d
    | Metal of Vector3d * float
    | Dielectric of float
type HitRecord =
    { T : float             // parameter used to determine point from ray
      Point : Vector3d
      Normal : Vector3d
      Material : Material }
type Hitable = 
    | Sphere of Vector3d * float * Material
    | HitableList of Hitable list

let nearZ = 0.1
let farZ = 1.0
let aperture = 0.001
let samples = 2
let lookFrom = Vector3d(50.0, 50.0, 50.0)
let lookAt = Vector3d(0.0, 0.0, 0.0)    
let up = Vector3d(0.0, 1.0, 0.0)

let rayPointAtParameter ray t =
    ray.Origin + ray.Direction * t

let setPixel (bitmap : Bitmap) x y (color : Vector3d) =    
    let r = int(Math.Sqrt(color.X) * 255.0)
    let g = int(Math.Sqrt(color.Y) * 255.0)
    let b = int(Math.Sqrt(color.Z) * 255.0)
    let color = Drawing.Color.FromArgb(r, g, b)
    bitmap.SetPixel(x, y, color)

let private random = new System.Random()

let randomInUnitSphere () =
    let points = Seq.initInfinite (fun _ -> 
        let x = random.NextDouble()
        let y = random.NextDouble()
        let z = random.NextDouble()
        2.0 * Vector3d(x, y, z) - Vector3d(1.0, 1.0, 1.0))
    Seq.find (fun (v : Vector3d) -> v.Length < 1.0) points

let rayDirection c r width (height : int) nearZ fieldOfView =
    let side = Math.Tan(fieldOfView) * nearZ
    let width = float width
    let height = float height
    let aspect = height / width
    let x = (float c / width - 0.5) * 2.0 * side
    let y = (float r / height - 0.5) * 2.0 * side * aspect
    let v = Vector3d(x, y, -nearZ)
    v.Normalize()
    v

let hitSphere ray (center, radius, material) tMin tMax =
    let computeHit t =
        let point = rayPointAtParameter ray t
        let normal = (point - center) / radius
        Some {T = t; Point = point; Normal = normal; Material = material}

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


let rec hit hitable ray tMin tMax =
    match hitable with
    | Sphere (center, radius, material) ->
        hitSphere ray (center, radius, material) tMin tMax
    | HitableList hitables ->
        hitList ray hitables tMin tMax

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

    List.fold fold (Double.PositiveInfinity, None) hitables
    |> snd

let refract (rayDir : Vector3d) (normal : Vector3d) niOverNt =
    let rayDir = rayDir.Normalized()
    let dot = Vector3d.Dot(rayDir, normal)
    Some((rayDir * niOverNt + (niOverNt * dot - Math.Sqrt(1.0 - dot*dot)) * normal).Normalized())

let reflect (rayDir : Vector3d) (normal : Vector3d) =
    let proj = normal * Vector3d.Dot(-rayDir, normal)
    (rayDir + 2.0 * proj).Normalized()

let refractiveRelation (rayDir : Vector3d) (normal : Vector3d) refrIndex =
    let dot = Vector3d.Dot(rayDir, normal)
    if dot > 0.0 then 
        Debug.Assert(false)
        let cosine = refrIndex * Vector3d.Dot(rayDir, normal) / rayDir.Length
        refrIndex, -normal, cosine
    else    
        let cosine = -Vector3d.Dot(rayDir, normal) / rayDir.Length
        1.0 / refrIndex, normal, cosine

let schlick cosine refrIndex = 
    let r0 = (1.0 - refrIndex) / (1.0 + refrIndex)
    let r0 = r0 * r0
    r0 + (1.0 - r0) * Math.Pow(1.0 - cosine, 5.0)

let scatter material rayIn hitRec =
    match material with
    | Lambertian(albedo) ->
        let randPoint = randomInUnitSphere()
        let target = hitRec.Point + hitRec.Normal + randPoint
        let scattered = {Origin = hitRec.Point; Direction = target - hitRec.Point}
        albedo, scattered
    | Metal(albedo, fuzzy) ->
        let fuzzy = if fuzzy < 1.0 then fuzzy else 1.0
        let reflected = reflect rayIn.Direction hitRec.Normal
        let randPoint = randomInUnitSphere() * fuzzy
        let scattered = {Origin = hitRec.Point; Direction = reflected + randPoint}
        albedo, scattered
    | Dielectric(index) ->
        let refrRelation, outwardNormal, cosine = refractiveRelation rayIn.Direction hitRec.Normal index
        let attenuation = Vector3d(1.0)
        let reflected = reflect rayIn.Direction hitRec.Normal
        match refract rayIn.Direction outwardNormal refrRelation with
        | Some refracted ->
            let reflectProb = schlick cosine index
            if random.NextDouble() < reflectProb then
                let scattered = {Origin = hitRec.Point; Direction = reflected}
                attenuation, scattered
            else
                let scattered = {Origin = hitRec.Point; Direction = refracted}
                attenuation, scattered
        | None ->
            let scattered = {Origin = hitRec.Point; Direction = reflected}
            attenuation, scattered

let rec colorIt ray hitable depth : Vector3d =
    match hit hitable ray 0.0001 Double.PositiveInfinity with
    | Some record ->
        if depth < 50 then
            let attenuation, scattered = scatter record.Material ray record
            attenuation * colorIt scattered hitable (depth + 1)
        else
            Vector3d(0.0)
    | None ->
        let dir = ray.Direction.Normalized()
        let t = (dir.Y + 1.0) / 2.0
        let c1 = (Rest.colorToVector Drawing.Color.White) 
        let c2 = Vector3d(0.5, 0.7, 1.0)
        t * c2 + (1.0 - t) * c1

let mainRender (bitmap : Bitmap) hitable fov =
    let camera = Camera.Camera(lookFrom, lookAt, up, fov, bitmap.Width, bitmap.Height, nearZ, farZ, aperture)

    let rec sampling c r s (color : Vector3d) =
        if s < samples then
            let ray = camera.Ray c r
            sampling c r (s + 1) (color + colorIt ray hitable 0)
        else
            color / (float samples)

    for r = 0 to bitmap.Height-1 do
        for c = 0 to bitmap.Width - 1 do
            let color = sampling c r 0 Vector3d.Zero
            setPixel bitmap c r color

let drawBitmap (source : Bitmap) (destination : Bitmap) (pixelSize : float) =
    use graphics = Graphics.FromImage(destination)
    use brush = new SolidBrush(Color.Black)    
    for r in 0..source.Height - 1 do        
        for c in 0..source.Width - 1 do
            let color = source.GetPixel(c, r)
            let x = (float c) * pixelSize
            let y = (float r) * pixelSize
            brush.Color <- color
            let rect = Drawing.RectangleF(float32 x, float32 y, float32 pixelSize, float32 pixelSize)
            graphics.FillRectangle(brush, rect)
