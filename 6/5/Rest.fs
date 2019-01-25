module Rest

open System
open OpenTK.Graphics.OpenGL
open OpenTK

type Bbox = Vector3d * Vector3d
type Ray = {Origin : Vector3d; Direction : Vector3d}

let loadTexture (bitmap : System.Drawing.Bitmap) =
   let id = GL.GenTexture()
   GL.BindTexture(TextureTarget.Texture2D, id)
   let data = bitmap.LockBits(Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), Drawing.Imaging.ImageLockMode.ReadOnly, Drawing.Imaging.PixelFormat.Format32bppArgb)
   GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
      OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
   bitmap.UnlockBits(data)
   GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, int TextureMinFilter.Linear);
   GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, int TextureMagFilter.Linear);
   id

let getBytesFromBitmap (bitmap: System.Drawing.Bitmap) =
  let data = bitmap.LockBits(System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), 
                             System.Drawing.Imaging.ImageLockMode.ReadWrite, 
                             bitmap.PixelFormat)
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

let colorToVector (color : Drawing.Color) =
  Vector3d(float color.R / 255.0, float color.G / 255.0, float color.B / 255.0)

let hitBbox ((box1, box2) : Bbox) ray tmin tmax = 
   let handle (tmin, tmax) (box1, box2, rayOrig, rayDir) =
      let inv = 1.0 / rayDir
      let t1 = (box1 - rayOrig) * inv
      let t2 = (box2 - rayOrig) * inv
      let t3, t4 =  if rayDir < 0.0 then t2, t1 else t1, t2
      let t5 = Math.Max(tmin, t3)
      let t6 = Math.Min(tmax, t4)
      (t5, t6)

   let t1, t2 = handle (tmin, tmax) (box1.X, box2.X, ray.Origin.X, ray.Direction.X) 
   let t1, t2 = handle (t1, t2) (box1.Y, box2.Y, ray.Origin.Y, ray.Direction.Y) 
   let t1, t2 = handle (t1, t2) (box1.Y, box2.Y, ray.Origin.Y, ray.Direction.Y) 
   t1 < t2 && t2 > 0.0

let getSphericalTexCoord (p : Vector3d) =
   let theta = Math.Acos(p.Y) // [0; pi]
   let phi = Math.Atan2(-p.Z, p.X) // [-pi; pi]
   let v = theta / Math.PI
   let u = (phi + Math.PI) / (2.0 * Math.PI)
   Vector2d(u, v)
Math.Acos(3.0)
Math.Abs(Math.Atan2(1.0, 1.0) - (Math.PI / 4.0)) < 0.0001
Math.Atan2(1.0, 1.0) // 0.7853981634 Math.PI / 4.0
Math.Abs(Math.Atan2(1.0, -1.0) - (3.0 * Math.PI / 4.0)) < 0.0001
Math.Abs(Math.Atan2(-1.0, -1.0) - (-3.0 * Math.PI / 4.0)) < 0.0001
