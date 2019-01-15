module Render

open System
open System.Drawing
open OpenTK

type Bitmap = System.Drawing.Bitmap
type Color = System.Drawing.Color
type Ray = {Origin : Vector3d; Direction : Vector3d}

let nearZ = 0.1
let fieldOfView = OpenTK.MathHelper.DegreesToRadians(45.0)

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

let computeColorFromLight (dirToSurface : Vector3d) lightDirection (sphereColor : Vector3d) =
    let coef = Vector3d.Dot(dirToSurface.Normalized(), lightDirection)    
    sphereColor * (1.0 - (coef / 2.0 + 0.5))

let pickNearestAndGetColor ray lightDirection a b discr sphereCenter sphereColor =
    let root = Math.Sqrt(discr)
    let t1 = (-b - root) / (2.0 * a)
    let point = ray.Origin + t1 * ray.Direction
    let dirToSurface = (point - sphereCenter).Normalized()
    let color = computeColorFromLight dirToSurface lightDirection sphereColor        
    Some color

let traceRay ray lightDirection sphereCenter sphereRadius (sphereColor : Vector3d) =
    let offset = ray.Origin - sphereCenter
    let a = Vector3d.Dot(ray.Direction, ray.Direction)
    let b = 2.0 * Vector3d.Dot(ray.Direction, offset)
    let c = Vector3d.Dot(offset, offset) - sphereRadius * sphereRadius
    let discr = b * b - 4.0 * a * c
    if discr >= 0.0 then
        pickNearestAndGetColor ray lightDirection a b discr sphereCenter sphereColor
    else
        None

let setPixel (bitmap : Bitmap) x y (color : Vector3d) =    
    let color = Drawing.Color.FromArgb(color.X * 255.0 |> int, 
                                        color.Y * 255.0 |> int,
                                        color.Z * 255.0 |> int)
    bitmap.SetPixel(x, y, color)

let renderSphereWithRays (bitmap : Bitmap) lightDirection sphereCenter sphereRadius sphereColor =
    for r = 0 to bitmap.Height-1 do
        for c = 0 to bitmap.Width - 1 do
            let origin = Vector3d.Zero
            let direction = rayDirection c r bitmap.Width bitmap.Height nearZ fieldOfView
            let ray = {Origin = origin; Direction = direction}
            match traceRay ray lightDirection sphereCenter sphereRadius sphereColor with
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

//  let drawCallback (e) =    
//      let bitmap = new Bitmap(5, 5)
//      bitmap.SetPixel(0, 0, Colors.Red)
//      bitmap.SetPixel(0, 2, Colors.DimGray)
//      bitmap.SetPixel(0, 4, Colors.Firebrick)
//      bitmap.SetPixel(1, 1, Colors.Fuchsia)
//      bitmap.SetPixel(1, 3, Colors.Gold)
//      drawBitmap e.Graphics bitmap 100.0f