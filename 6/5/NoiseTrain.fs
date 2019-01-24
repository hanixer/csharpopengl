module NoiseTrain

open System
open OpenTK
open System.Drawing
open System.Diagnostics

let private random = new Random()
let private tableSize = 256
let private maxTableIndex = 256 - 1
let private frequency = 0.02
let private amplitude = 1.0
let private numLayers = 5
let private randoms =
    Array2D.init tableSize tableSize (fun _ _ -> 
        random.NextDouble())

let lerp low high t =  (1.0 - t) * low + t * high
let smoothstep t = t * t * (3.0 - 2.0 * t)

let computeNoise (p : Vector2d) =
    let ix = int (Math.Floor p.X)
    let iy = int (Math.Floor p.Y)
    let tx = p.X - float ix
    let ty = p.Y - float iy
    let x0 = ix &&& maxTableIndex
    let x1 = (x0 + 1) &&& maxTableIndex
    let y0 = iy &&& maxTableIndex
    let y1 = (y0 + 1) &&& maxTableIndex
    let c00 = randoms.[y0, x0]
    let c10 = randoms.[y0, x1]
    let c01 = randoms.[y1, x0]
    let c11 = randoms.[y1, x1]
    let sx = smoothstep tx
    let sy = smoothstep ty
    let nx0 = lerp c00 c10 sx
    let nx1 = lerp c01 c11 sx
    lerp nx0 nx1 sy

let generateNoiseMap width height (frequency : float) (amplitude : float) = 
    Array2D.init height width <| fun r c ->
        computeNoise (Vector2d(float c, float r) * frequency)

let makeFractal width height (lacunarity : float) (gain : float) (numLayers : int) = 
    let mutable maxVal = Double.MinValue
    let rec loop (point : Vector2d) i accum amplitude =
        if i < numLayers then
            let noise = computeNoise (point)
            let accum = (accum + (amplitude * noise))
            let point = point * lacunarity
            let amplitude = amplitude * gain
            loop point (i + 1) accum amplitude
        else
            accum

    Array2D.init height width <| fun r c ->
        let point = Vector2d(float c, float r) * frequency
        let result = loop point 0 0.0 1.0
        maxVal <- Math.Max(result, maxVal)
        result
    |> Array2D.map (fun t -> t / maxVal)

let subMainRender (bitmap : Bitmap) lacunarity (gain : float) =
    makeFractal bitmap.Width bitmap.Height lacunarity gain numLayers
    // generateNoiseMap bitmap.Width bitmap.Height frequency amplitude
    |> Array2D.iteri (fun c r t ->
        let tt = int (t * 255.0)
        bitmap.SetPixel(r, c, Color.FromArgb(tt, tt, tt)))