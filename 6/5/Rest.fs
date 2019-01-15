module Rest

open System
open OpenTK.Graphics.OpenGL

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