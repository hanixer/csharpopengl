module Render

open System
open System.Drawing
open OpenTK
open System.Diagnostics
open Camera
open Common
open Noise
open Material
open Hit

type Bitmap = System.Drawing.Bitmap
type Color = System.Drawing.Color
type RenderSettings =
    { Samples : int
      LookFrom : Vector3d
      LookAt : Vector3d 
      Fov : float
      }
let nearZ = 0.1
let aperture = 0.05
let up = Vector3d(0.0, 1.0, 0.0)

let setPixel (bitmap : Bitmap) x y (color : Vector3d) =    
    let r = int(Math.Sqrt(color.X) * 255.0)
    let g = int(Math.Sqrt(color.Y) * 255.0)
    let b = int(Math.Sqrt(color.Z) * 255.0)
    let color = Drawing.Color.FromArgb(r, g, b)
    bitmap.SetPixel(x, y, color)

let rec colorIt ray hitable depth : Vector3d =
    match hit hitable ray 0.0001 Double.PositiveInfinity with
    | Some record ->
        let emitted = emitLight record.Material record.TexCoord record.Point
        match scatter record.Material ray record with
        | Some (attenuation, scattered) when depth < 50 ->
            emitted + attenuation * colorIt scattered hitable (depth + 1)
        | _ ->        
            emitted
    | None ->
        Vector3d.Zero

let mainRender (bitmap : Bitmap) settings hitable =
    let farZ = (settings.LookFrom - settings.LookAt).Length
    let camera = Camera.Camera( settings.LookFrom, settings.LookAt, 
                                up, settings.Fov, bitmap.Width, bitmap.Height, 
                                nearZ, farZ, aperture)

    let rec sampling c r s (color : Vector3d) =
        if s < settings.Samples then
            let ray = camera.Ray c r
            sampling c r (s + 1) (color + colorIt ray hitable 0)
        else
            color / (float settings.Samples)
            

    for r = 0 to bitmap.Height-1 do
        for c = 0 to bitmap.Width - 1 do
            let color = sampling c r 0 Vector3d.Zero
            let color = Vector3d(Math.Sqrt(color.X), Math.Sqrt(color.Y), Math.Sqrt(color.Z))
            let color = Vector3d(Math.Min(color.X, 1.0), Math.Min(color.Y, 1.0), Math.Min(color.Z, 1.0))

            setPixel bitmap c r color
