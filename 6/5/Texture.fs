module Texture

open System
open OpenTK.Graphics.OpenGL
open OpenTK

type Bmp = Drawing.Bitmap

type Texture = 
    | ConstantTexture of Vector3d
    | CheckerTexture of Texture * Texture
    | NoiseTexture of float
    | ImageTexture of byte[] * int * int

let textureFromBitmap (bitmap : Bmp) =
    let bytes = Common.getBytesFromBitmapRgb bitmap
    ImageTexture (bytes, bitmap.Width, bitmap.Height)
    
let rec textureValue texture (texCoord : Vector2d) (p : Vector3d) =
    match texture with
    | ConstantTexture(color) -> color
    | CheckerTexture(one, two) ->
        let sin = Math.Sin(p.X * 10.0) * Math.Sin(p.Y * 10.0) * Math.Sin(p.Z * 10.0)
        if sin < 0.0 then
            textureValue one texCoord p
        else        
            textureValue two texCoord p
    | NoiseTexture(scale) ->
        let ppp = Vector2d(scale * p.X, scale * p.Y)
        // NoiseTrain.computeNoise3 (scale * p) * Vector3d.One
        NoiseTrain.computeMarble (scale * p) 5 * Vector3d.One
        // (noise (scale * p)) * Vector3d.One
    | ImageTexture(bytes, width, height) ->
        let w = float width
        let h = float height
        let i = int (texCoord.X * (w - 1.0))
        let j = int (texCoord.Y * (h - 1.0))
        let index = j * width * 3 + i * 3
        let r = float bytes.[index ] / 255.0
        let g = float bytes.[j * width * 3 + i * 3 + 1 ] / 255.0
        let b = float bytes.[j * width * 3 + i * 3 + 2 ] / 255.0
        // (u * Vector3d(1.0, 0.0, 0.0) + v * Vector3d(0.0, 1.0, 0.0)) / 2.0
        Vector3d(b, g, r)
        // u * Vector3d.One