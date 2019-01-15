module Render

open System
open System.Drawing
open OpenTK

type Bitmap = System.Drawing.Bitmap
type Color = System.Drawing.Color
type Ray = {Origin : Vector3d; Direction : Vector3d}

let nearZ = 0.1
let fieldOfView = OpenTK.MathHelper.DegreesToRadians(45.0)

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

let computeColorFromLight ray point (sphereCenter : Vector3d) lightPosition (diffuseColor : Vector3d, specularColor : Vector3d, specularPower) =
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
        (diffuse * diffuseColor + specular * specularColor) / 2.0
    else
        Vector3d(0.0)
    // let coef = Vector3d.Dot(normal, lightDirection)
    // sphereColor * (1.0 - (coef / 2.0 + 0.5))
    // let dbgval = 1.0 - (Vector3d.Dot(normal, half) / 2.0 + 0.5)
    // printfn "%A %A" viewVector lightDirection
    // Vector3d(dbgval)

let pickNearestAndGetColor ray lightPosition a b discr sphereCenter color  =
    let root = Math.Sqrt(discr)
    let t1 = (-b - root) / (2.0 * a)
    let point = ray.Origin + t1 * ray.Direction
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

let renderSphereWithRays (bitmap : Bitmap) lightPosition sphereCenter sphereRadius color =
    for r = 0 to bitmap.Height-1 do
        for c = 0 to bitmap.Width - 1 do
            let origin = Vector3d.Zero
            let direction = rayDirection c r bitmap.Width bitmap.Height nearZ fieldOfView
            let ray = {Origin = origin; Direction = direction}
            match traceRay ray lightPosition sphereCenter sphereRadius color with
            | Some color ->
                setPixel bitmap c r color
            | _ ->
                setPixel bitmap c r <| Vector3d(0.3, 0.3, 0.3)

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
