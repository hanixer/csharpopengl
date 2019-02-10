module Render

open System
open OpenTK
open Scene
open Common
open Object
open Node
open Transform
open System.Diagnostics
open Material
open System.Threading.Tasks

type Bitmap = System.Drawing.Bitmap

let setPixel (bitmap : Bitmap) x y (color : Vector3d) =    
    let r = int(Math.Sqrt(color.X) * 255.0)
    let g = int(Math.Sqrt(color.Y) * 255.0)
    let b = int(Math.Sqrt(color.Z) * 255.0)
    let r = int((color.X) * 255.0)
    let g = int((color.Y) * 255.0)
    let b = int((color.Z) * 255.0)
    let color = Drawing.Color.FromArgb(r, g, b)
    bitmap.SetPixel(x, y, color)

let clamp (color : Vector3d) =
    if color.X > 1.0 || color.Y > 1.0 || color.Z > 1.0 then
        let max = Math.Max(color.X, Math.Max(color.Y, color.Z))
        color / max
    else
        color

let render (bitmap : Bitmap) (zbuffer : float [,]) (scene : Scene) =
    let buf = Array2D.create bitmap.Height bitmap.Width Vector3d.Zero
    let w = bitmap.Width
    Parallel.For(0, bitmap.Height, fun r ->
    // for r = 0 to bitmap.Height - 1 do
        for c = 0 to w - 1 do
            let ray = scene.Camera.Ray c r
            let t, color = 
                match intersectNodes ray scene.Nodes epsilon with
                | Some hitInfo ->
                    let material = scene.Materials.[hitInfo.Material]
                    // Debug.Assert(c <> 364 || r <> 159)
                    let color = shade ray material hitInfo (scene.Lights |> Map.toSeq |> Seq.map snd) scene.Nodes (c = 364 && r = 159 || c = 280  && r = 156)
                    if not ((c <> 364 || r <> 159)) then
                        printfn "the color is %A" color
                    Debug.Assert(not (color.X < 0.0 || color.Y < 0.0 || color.Z < 0.0))
                    (hitInfo.T, color)
                | _ -> 
                    (Double.PositiveInfinity, Vector3d.Zero)
            zbuffer.[r, c] <- t
            buf.[r, c] <- color
    ) |> ignore
    Array2D.iteri (fun r c x -> setPixel bitmap c r (clamp x)) buf

let drawZBuffer zbuffer =
    let h = Array2D.length1 zbuffer
    let w = Array2D.length2 zbuffer
    let bitmap = new Bitmap(w, h)
    let seq = Seq.cast<float> zbuffer |> Seq.filter (Double.IsInfinity >> not)
    if not (Seq.isEmpty seq) then
        let min = Seq.min seq
        let max = Seq.max seq
        Array2D.iteri (fun row column z ->
            let f = 
                if Double.IsInfinity z
                then 0.0
                else (max - z) / (max - min)
                // else 1.0 - (z - min) / (max - min)
            setPixel bitmap column row <| Vector3d(f)) 
            zbuffer
    bitmap
    