module Common

open System
open OpenTK.Graphics.OpenGL
open OpenTK

type Ray = {Origin : Vector3d; Direction : Vector3d}

type HitInfo = {
    T : float
    Point : Vector3d
    Normal : Vector3d
    Material : string
    Depth : int
}

type BoundingBox = {
    PMin : Vector3d
    PMax : Vector3d
}

let tryFindBestHitInfo hitInfos =
    let fold best hitInfo =
        match (best, hitInfo) with
        | Some b, Some h ->  if h.T < b.T then hitInfo else best
        | _, Some _ -> hitInfo
        | _ -> best
    let result = Seq.fold fold None hitInfos
    result

let hitBoundingBox (box : BoundingBox) ray tmin tmax = 
   let handle (tmin, tmax) (box1, box2, rayOrig, rayDir) =
      let inv = 1.0 / rayDir
      let t1 = (box1 - rayOrig) * inv
      let t2 = (box2 - rayOrig) * inv
      let t3, t4 =  if rayDir < 0.0 then (t2, t1) else (t1, t2)
      let t5 = Math.Max(tmin, t3)
      let t6 = Math.Min(tmax, t4)
      (t5, t6)
   
   let box1 = box.PMin
   let box2 = box.PMax
   let t1, t2 = handle (tmin, tmax) (box1.X, box2.X, ray.Origin.X, ray.Direction.X) 
   let t1, t2 = handle (t1, t2) (box1.Y, box2.Y, ray.Origin.Y, ray.Direction.Y) 
   let t1, t2 = handle (t1, t2) (box1.Y, box2.Y, ray.Origin.Y, ray.Direction.Y) 
   t1 < t2 && t2 > 0.0

let mutable debugFlag = false

let epsilon = 0.00001


let pointOnRay ray (t : float) =
    ray.Origin + ray.Direction * t

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

let getSphericalTexCoord (p : Vector3d) =
   let theta = Math.Acos(p.Y) // [0; pi]
   let phi = Math.Atan2(-p.Z, p.X) // [-pi; pi]
   let v = theta / Math.PI
   let u = (phi + Math.PI) / (2.0 * Math.PI)
   Vector2d(u, v)

let buildOrthoNormalBasis (normal : Vector3d) =
    let w = normal.Normalized()
    let help =
        if Math.Abs(w.X) > 0.9 then
            Vector3d(0.0, 1.0, 0.0)
        else
            Vector3d(1.0, 0.0, 0.0)
    let v = Vector3d.Cross(w, help)
    v.Normalize()
    let u = Vector3d.Cross(w, v)
    (u, v, w)

type V3 = Vector3d

let localOrthoNormalBasis (a : V3) (u : V3, v : V3, w : V3) =
    a.X * u + a.Y * v + a.Z * w

let colorToVector (color : Drawing.Color) =
  Vector3d(float color.R / 255.0, float color.G / 255.0, float color.B / 255.0)

let loadTexture (bitmap : System.Drawing.Bitmap) =
   let id = GL.GenTexture()
   GL.BindTexture(TextureTarget.Texture2D, id)
   let data = bitmap.LockBits(Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), Drawing.Imaging.ImageLockMode.ReadOnly, Drawing.Imaging.PixelFormat.Format32bppArgb)
   GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
   bitmap.UnlockBits(data)
   GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, int TextureMinFilter.Linear);
   GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, int TextureMagFilter.Linear);
   id

let getBytesFromBitmap (bitmap: System.Drawing.Bitmap) =
  let data = bitmap.LockBits(System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat)
  let size = data.Stride * data.Height
  let bytes = Array.create size (byte 0)
  System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bytes, 0, size)
  bitmap.UnlockBits(data)
  bytes

let getBytesFromBitmapRgb (bitmap: System.Drawing.Bitmap) =
  let rect = System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height)
  let newBitmap = new System.Drawing.Bitmap(bitmap.Width, bitmap.Height, Drawing.Imaging.PixelFormat.Format24bppRgb)
  use graph = Drawing.Graphics.FromImage(newBitmap)
  graph.DrawImage(bitmap, rect)
  graph.Flush()
  getBytesFromBitmap newBitmap

let drawBitmapOnBitmap (source : System.Drawing.Bitmap) (destination : System.Drawing.Bitmap) (pixelSize : float) =
    use graphics = System.Drawing.Graphics.FromImage(destination)
    use brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black)    
    for r in 0..source.Height - 1 do        
        for c in 0..source.Width - 1 do
            let color = source.GetPixel(c, r)
            let x = (float c) * pixelSize
            let y = (float r) * pixelSize
            brush.Color <- color
            let rect = Drawing.RectangleF(float32 x, float32 y, float32 pixelSize, float32 pixelSize)
            graphics.FillRectangle(brush, rect)
