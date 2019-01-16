module Render

open System
open System.Drawing
open OpenTK

type Bitmap = System.Drawing.Bitmap
type Color = System.Drawing.Color
type Ray = {Origin : Vector3d; Direction : Vector3d}
type HitRecord =
    { T : float             // parameter used to determine point from ray
      Point : Vector3d
      Normal : Vector3d }
type Hitable = 
    | Sphere of Vector3d * float
    | HitableList of Hitable list

let nearZ = 0.1
let fieldOfView = OpenTK.MathHelper.DegreesToRadians(45.0)

let rayPointAtParameter ray t =
    ray.Origin + ray.Direction * t

let setPixel (bitmap : Bitmap) x y (color : Vector3d) =    
    let color = Drawing.Color.FromArgb(color.X * 255.0 |> int, 
                                        color.Y * 255.0 |> int,
                                        color.Z * 255.0 |> int)
    bitmap.SetPixel(x, y, color)

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

let computeColorFromLight ray point (sphereCenter : Vector3d) lightPosition (ambientIntensity : float, diffuseColor : Vector3d, specularColor : Vector3d, specularPower) =
    let viewVector = -ray.Direction.Normalized() 
    let normal = (point - sphereCenter).Normalized()
    let lightDir = (lightPosition - sphereCenter).Normalized()
    let reflectDir = viewVector + lightDir
    let reflectDir = 
        if Vector3d.Zero = reflectDir 
        then Vector3d.Zero
        else reflectDir.Normalized()
    let diffuse = Math.Max(0.0, Vector3d.Dot(lightDir, normal))
    if diffuse > 0.0 then
        let specularAngle = Math.Max(0.0, Vector3d.Dot(normal, reflectDir))
        let specular = Math.Pow(specularAngle, specularPower)
        let c = (ambientIntensity * diffuseColor + 0.5 * diffuse * diffuseColor + 0.5 * specular * specularColor)
        Vector3d(Math.Min(c.X, 1.0), Math.Min(c.Y, 1.0), Math.Min(c.Z, 1.0))
    else
        ambientIntensity * diffuseColor

let pickNearestAndGetColor ray lightPosition a b discr sphereCenter color  =
    let root = Math.Sqrt(discr)
    let t0 = (-b - root) / (2.0 * a)
    let t1 = (-b + root) / (2.0 * a)
    let t =
        if Math.Abs(t0) < Math.Abs(t1) 
        then t0
        else t1
    let point = ray.Origin + t * ray.Direction
    let color = computeColorFromLight ray point sphereCenter lightPosition color         
    Some color

let traceRay ray lightPosition sphereCenter sphereRadius color =
    let offset = ray.Origin - sphereCenter
    let a = Vector3d.Dot(ray.Direction, ray.Direction)
    let b = 2.0 * Vector3d.Dot(ray.Direction, offset)
    let c = Vector3d.Dot(offset, offset) - sphereRadius * sphereRadius
    let discr = b * b - 4.0 * a * c
    if discr >= 0.0 then
        pickNearestAndGetColor ray lightPosition a b discr sphereCenter color
    else
        None

let hitSphere ray (center, radius) tMin tMax =
    let computeHit t =
        let point = rayPointAtParameter ray t
        let normal = (point - center) / radius
        Some {T = t; Point = point; Normal = normal}

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
    | Sphere (center, radius) ->
        hitSphere ray (center, radius) tMin tMax
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

let colorIt ray hitable =
    match hit hitable ray 0.0 Double.PositiveInfinity with
    | Some record ->
        0.5 * Vector3d(record.Normal.X + 1.0, record.Normal.Y + 1.0, record.Normal.Z + 1.0)
    | None ->
        let dir = ray.Direction.Normalized()
        let t = (dir.Y + 1.0) / 2.0
        let c1 = (Rest.colorToVector Drawing.Color.White) 
        let c2 = (Rest.colorToVector Drawing.Color.Blue)
        t * c2 + (1.0 - t) * c1

let mainRender (bitmap : Bitmap) hitable =
    for r = 0 to bitmap.Height-1 do
        for c = 0 to bitmap.Width - 1 do
            let origin = Vector3d.Zero
            let direction = rayDirection c r bitmap.Width bitmap.Height nearZ fieldOfView
            let ray = {Origin = origin; Direction = direction}
            setPixel bitmap c r (colorIt ray hitable)

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
