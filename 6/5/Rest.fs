module Rest

open System
open OpenTK.Graphics.OpenGL
open OpenTK

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

let colorToVector (color : Drawing.Color) =
  Vector3d(float color.R / 255.0, float color.G / 255.0, float color.B / 255.0)