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
open Light

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

let maxDepth = 5

let isReachable source target t direction nodes =
    let shadowRay = {Origin = source ; Direction = direction}
    match intersectNodes shadowRay nodes epsilon with
    | Some(hitInfo) -> hitInfo.T >= t
    // | Some(hitInfo) -> false
    | None -> true

let getReflectedForLightSource ray light hitInfo nodes nodesList material =
    let samples = 10
    let sum = 
        Seq.init samples (fun _ ->
        match samplePointOnLight light nodes with
        | Some(point) ->
            let direction = point - hitInfo.Point
            direction.Normalize()
            let t = (point - hitInfo.Point).Length / direction.Length
            let isReachable = isReachable hitInfo.Point point t direction nodesList
            if isReachable then
                let area = getAreaOfLight light nodes
                let c = illuminate light point direction nodesList
                let dot = Math.Max(Vector3d.Dot(hitInfo.Normal.Normalized(), direction), 0.0)
                let attenuation = getAttenuation material
                c * area * dot * attenuation
                // Vector3d.One
                // Vector3d(0.0, 0.0, 1.0)
            else
                Vector3d(0.0, 1.0, 0.0)
                Vector3d.Zero
        | _ -> 
            Vector3d.Zero)
        |> Seq.fold (+) Vector3d.Zero
    sum / float samples

let getReflectedTotal ray scene hitInfo material =
    scene.LightsList
    |> Seq.map (fun light -> getReflectedForLightSource ray light hitInfo scene.Nodes scene.NodesList material) 
    |> Seq.fold (+) Vector3d.Zero

let areaLightTrace ray scene =
    match intersectNodes ray scene.NodesList epsilon with
    | Some hitInfo ->
        let material = scene.Materials.[hitInfo.Material]
        let emitted = getEmitted material
        let reflected = getReflectedTotal ray scene hitInfo material    
        // printfn "%A; %A\n" emitted reflected
        emitted + reflected
    | _ -> Vector3d(0.1, 0.0, 0.2)

let rec traceRay ray scene depth = 
    let defaultRes = (Double.PositiveInfinity, Vector3d.Zero)
    if depth > maxDepth then
        defaultRes
    else
        match intersectNodes ray scene.NodesList epsilon with
        | Some hitInfo ->
            let material = scene.Materials.[hitInfo.Material]
            let shadedColor, scattered = shade ray material hitInfo scene.LightsList scene.NodesList
            match scattered with
            | Some scatRay ->
                let _, scatColor = traceRay scatRay scene (depth + 1)
                let color = shadedColor + scatColor
                Debug.Assert(not (color.X < 0.0 || color.Y < 0.0 || color.Z < 0.0))
                (hitInfo.T, color)
            | _ ->
                (hitInfo.T, shadedColor)
        | _ -> defaultRes

let render (bitmap : Bitmap) (zbuffer : float [,]) (scene : Scene) =
    let buf = Array2D.create bitmap.Height bitmap.Width Vector3d.Zero
    let w = bitmap.Width
    let h = bitmap.Height
    // Parallel.For(0, bitmap.Height, fun r ->
    for r = 0 to bitmap.Height - 1 do
        for c = 0 to w - 1 do
            let ray = scene.Camera.Ray c r
            let t, color = (0.0, areaLightTrace ray scene)
            zbuffer.[r, c] <- t
            buf.[r, c] <- color
    // ) |> ignore
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
    