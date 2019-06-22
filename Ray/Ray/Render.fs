module Render

open System
open OpenTK
open Scene
open Sampling

type Bitmap = System.Drawing.Bitmap

let setPixel (bitmap : Bitmap) x y (color : Vector3d) =
    let r = int (Math.Sqrt(color.X) * 255.0)
    let g = int (Math.Sqrt(color.Y) * 255.0)
    let b = int (Math.Sqrt(color.Z) * 255.0)
    // let r = int((color.X) * 255.0)
    // let g = int((color.Y) * 255.0)
    // let b = int((color.Z) * 255.0)
    let color = Drawing.Color.FromArgb(r, g, b)
    bitmap.SetPixel(x, y, color)

let clamp (color : Vector3d) =
    if color.X > 1.0 || color.Y > 1.0 || color.Z > 1.0 then
        let max = Math.Max(color.X, Math.Max(color.Y, color.Z))
        color / max
    else color

let maxDepth = 5
let isReachable source t direction nodes = failwith "should be reimplemented with new primitives"
// let shadowRay = makeRay source  direction
// match intersectNodes shadowRay nodes with
// | Some(hitInfo) ->
//     hitInfo.T > (t - 0.001)
//         && Vector3d.Dot(hitInfo.Normal, direction) > 0.0
//     // hitInfo.T >= t && Vector3d.Dot(hitInfo.Normal, direction) > 0.0
// // | Some(hitInfo) -> false
// | None ->
//     true
let getDirectLighting scene hitInfo lightNode =
    // rho * Le * v * A * cosThetaI * cosThetaL / distance^2
    failwith "should be reimplemented with new primitives"

// match samplePointAndNormOnNode lightNode with
// | Some (samplePoint, normalLight) ->
//     let direction = hitInfo.Point - samplePoint
//     let directionNorm = direction.Normalized()
//     let isReachable = isReachable hitInfo.Point direction.Length -directionNorm scene.NodesList
//     if isReachable then
//         let material = scene.Materials.[lightNode.Material]
//         let emitted = getEmitted material
//         let cosThetaI = Math.Abs(Vector3d.Dot(hitInfo.Normal, -directionNorm))
//         let cosThetaL = Math.Abs(Vector3d.Dot(normalLight, directionNorm))
//         let area : float = getAreaOfNode lightNode
//         emitted * cosThetaI * cosThetaL * area / direction.LengthSquared
//     else
//         Vector3d.Zero
// | _ ->
//     Vector3d.Zero
let rec pathTrace ray scene depth isEyeRay : Vector3d = failwith "should be reimplemented with new primitives"

// match intersectNodes ray scene.NodesList with
// | Some hitInfo ->
//     let material = scene.Materials.[hitInfo.Material]
//     let emitted = if isEyeRay  then getEmitted material else Vector3d.Zero
//     match scatter ray material hitInfo with
//     | Some(attenuation, scattered) when depth < 50 && emitted = Vector3d.Zero ->
//         let direct =
//             Seq.map (getDirectLighting scene hitInfo) scene.AreaLights
//             |> Seq.fold (+) Vector3d.Zero
//             // Vector3d.Zero
//         let indirect =
//             let dot = Math.Abs(Vector3d.Dot(hitInfo.Normal, -ray.Direction))
//             dot * pathTrace scattered scene (depth + 1) false
//         emitted
//             + attenuation * direct
//             + attenuation * indirect
//     | _ ->
//         emitted
// | _ ->
//     scene.Environment
let rec traceRay ray scene depth =
    let defaultRes = (Double.PositiveInfinity, Vector3d.Zero)
    if depth > maxDepth then defaultRes
    else
        match Node.intersect ray scene.Primitive with
        | Some hitInfo ->
            let color = (hitInfo.Normal.Normalized() + Vector3d.One) * 0.5
            (hitInfo.T, color)
        // let material = scene.Materials.[hitInfo.Material]
        // let shadedColor, scattered = shade ray material hitInfo scene.LightsList scene.NodesList
        // match scattered with
        // | Some scatRay ->
        //     let _, scatColor = traceRay scatRay scene (depth + 1)
        //     let color = shadedColor + scatColor
        //     Debug.Assert(not (color.X < 0.0 || color.Y < 0.0 || color.Z < 0.0))
        //     (hitInfo.T, color)
        // | _ ->
        //     (hitInfo.T, shadedColor)
        | _ -> defaultRes

let render (bitmap : Bitmap) (scene : Scene) integrator =
    let buf = Array2D.create bitmap.Height bitmap.Width Vector3d.Zero
    let w = bitmap.Width
    let h = bitmap.Height
    let samples = scene.Sampler.Count
    // System.Threading.Tasks.Parallel.For(0, bitmap.Height, fun r ->
    for r = 0 to bitmap.Height - 1 do
        for c = 0 to w - 1 do
            for s = 0 to samples - 1 do
                let sample = Vector2d(float c, float r) + (next2D scene.Sampler)
                let ray = scene.Camera.Ray sample
                let color = Integrator.estimateRadiance integrator scene ray
                buf.[r, c] <- buf.[r, c] + color
            buf.[r, c] <- buf.[r, c] / float samples
        // let percent = int (float r / float h * 1000.0)
        // if percent % 50 = 0 then printfn "%A%%" (percent / 10)
        ()
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
                if Double.IsInfinity z then 0.0
                else (max - z) / (max - min)
            setPixel bitmap column row <| Vector3d(f)) zbuffer
    bitmap
