using BitMiracle.LibJpeg;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using OpenTK.Graphics.OpenGL4;

namespace Utilities
{
    public class Images
    {
        public static byte[] ImageToByte2(System.Drawing.Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }
        public static byte[] ImageToByte(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
               bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);
            return rgbValues;
        }

        public static void LoadImage(string path)
        {
            //using (var stream = File.OpenRead(path))
            //{
            //    var image = new JpegImage(stream);
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        image.WriteBitmap(memoryStream);
            //        byte[] bytes = memoryStream.ToArray();
            //        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.Byte, bytes);
            //    }
            //}
            var image = new Bitmap(path);
            var bytes = ImageToByte(image);
            for (int i = 0; i < bytes.Length; i += 3)
            {
                var B = bytes[i];
                bytes[i] = bytes[i + 2];
                bytes[i + 2] = B;
            }
            var product = image.Width * image.Height;
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.Byte, bytes);
            Console.WriteLine("image Length = {0}, product = {1}, x3 = {2}", bytes.Length, product, product * 3);

            bytes = new byte[]
            {
                0xff, 0x00, 0x00,   0x00, 0x00, 0x00,
                0x00, 0xff, 0x00,   0x00, 0x00, 0xff
            };
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, 2, 2, 0, PixelFormat.Rgb, PixelType.Byte, bytes);
        }

        public static void LoadTexture(string filename, out int textureId)
        {
            using (var bitmap = new Bitmap(filename))
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                        bitmap.Width, bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte,
                        bitmapData.Scan0);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            }
        }
        public static void LoadTexture(string filename, int textureId, bool flipY = true)
        {
            using (var bitmap = new Bitmap(filename))
            {
                if (flipY)
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                int width = bitmap.Width;
                int height = bitmap.Height;
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                        bitmap.Width, bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte,
                        bitmapData.Scan0);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            }
        }
    }
}
