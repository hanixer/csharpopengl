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
    Array3D.init tableSize tableSize tableSize (fun _ _ _ -> 
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
    let c00 = randoms.[y0, x0, 0]
    let c10 = randoms.[y0, x1, 0]
    let c01 = randoms.[y1, x0, 0]
    let c11 = randoms.[y1, x1, 0]
    let sx = smoothstep tx
    let sy = smoothstep ty
    let nx0 = lerp c00 c10 sx
    let nx1 = lerp c01 c11 sx
    lerp nx0 nx1 sy

let computeNoise3 (p : Vector3d) =
    let ix = int (Math.Floor p.X)
    let iy = int (Math.Floor p.Y)
    let iz = int (Math.Floor p.Z)
    let tx = p.X - float ix
    let ty = p.Y - float iy
    let tz = p.Z - float iz
    let x0 = ix &&& maxTableIndex
    let x1 = (x0 + 1) &&& maxTableIndex
    let y0 = iy &&& maxTableIndex
    let y1 = (y0 + 1) &&& maxTableIndex
    let z0 = iz &&& maxTableIndex
    let z1 = (z0 + 1) &&& maxTableIndex
    let c000 = randoms.[x0, y0, z0]
    let c001 = randoms.[x0, y0, z1]
    let c010 = randoms.[x0, y1, z0]
    let c011 = randoms.[x0, y1, z1]
    let c100 = randoms.[x1, y0, z0]
    let c101 = randoms.[x1, y0, z1]
    let c110 = randoms.[x1, y1, z0]
    let c111 = randoms.[x1, y1, z1]
    let sx = smoothstep tx
    let sy = smoothstep ty
    let sz = smoothstep tz
    let nx0 = lerp c000 c100 sx
    let nx1 = lerp c010 c110 sx
    let nx2 = lerp c001 c101 sx
    let nx3 = lerp c011 c111 sx
    let ny0 = lerp nx0 nx1 sy
    let ny1 = lerp nx2 nx3 sy
    lerp ny0 ny1 sz
    // c000
    // nx0
    // lerp c000 c100 sx

let computeMarble (p : Vector3d) (numLayers : int) = 
    let lacunarity = 2.0
    let gain = 0.5

    let rec loop (point : Vector3d) i accum amplitude =
        if i < numLayers then
            let noise = Math.Abs(2.0 * computeNoise3 (point) - 1.0)            
            let accum = (accum + (amplitude * noise))
            let point = point * lacunarity
            let amplitude = amplitude * gain
            loop point (i + 1) accum amplitude
        else
            accum

    let result = loop p 0 0.0 1.0
    let sin = Math.Sin(result * 2.0 + p.Z * 2.0)
    (sin + 1.0) / 2.0
    
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

let makeTurbulence width height (lacunarity : float) (gain : float) (numLayers : int) = 
    let mutable maxVal = Double.MinValue
    let rec loop (point : Vector2d) i accum amplitude =
        if i < numLayers then
            let noise = Math.Abs(2.0 * computeNoise (point) - 1.0)            
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

let makeMarble width height (lacunarity : float) (gain : float) (numLayers : int) = 
    let lacunarity = 2.0
    let gain = 0.5
    let mutable maxVal = Double.MinValue
    let rec loop (point : Vector2d) i accum amplitude =
        if i < numLayers then
            let noise = Math.Abs(2.0 * computeNoise (point) - 1.0)            
            let accum = (accum + (amplitude * noise))
            let point = point * lacunarity
            let amplitude = amplitude * gain
            loop point (i + 1) accum amplitude
        else
            accum

    Array2D.init height width <| fun r c ->
        let point = Vector2d(float c, float r) * frequency
        let result = loop point 0 0.0 1.0
        maxVal <- 1.0
        let sin = Math.Sin(result * 4.0 + point.X * 4.0)
        (sin + 1.0) / 2.0
    |> Array2D.map (fun t -> t / maxVal)

let subMainRender (bitmap : Bitmap) lacunarity (gain : float) =
    makeMarble bitmap.Width bitmap.Height lacunarity gain numLayers
    // makeTurbulence bitmap.Width bitmap.Height lacunarity gain numLayers
    // makeFractal bitmap.Width bitmap.Height lacunarity gain numLayers
    // generateNoiseMap bitmap.Width bitmap.Height frequency amplitude
    |> Array2D.iteri (fun c r t ->
        let tt = int (t * 255.0)
        bitmap.SetPixel(r, c, Color.FromArgb(tt, tt, tt)))