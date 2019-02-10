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

let intersect ray object tMin material =
    let computeHit (t : float) =
        let point = pointOnRay ray t
        let normal = point.Normalized()
        Some {T = t; Point = point; Normal = normal; Material = material}

    match object with
    | Sphere ->
        let offset = ray.Origin
        let a = Vector3d.Dot(ray.Direction, ray.Direction)
        let b = 2.0 * Vector3d.Dot(ray.Direction, offset)
        let c = Vector3d.Dot(offset, offset) - 1.0
        let discr = b * b - 4.0 * a * c
        if discr >= 0.0 then
            let t = (-b - Math.Sqrt(discr)) / (2.0 * a)
            if t > tMin then
                computeHit t
            else
                let t = (-b - Math.Sqrt(discr)) / (2.0 * a)
                if t > tMin then
                    computeHit t
                else None        
        else
            None

let tryFindBestHitInfo hitInfos =
    let fold best hitInfo =
        match (best, hitInfo) with
        | Some b, Some h ->  if h.T < b.T then hitInfo else best
        | _, Some _ -> hitInfo       
        | _ -> best
    let result = Seq.fold fold None hitInfos
    result    

let rec intersectNodes ray nodes tMin =
    Seq.map (fun node -> intersectNode ray node tMin) nodes
    |> tryFindBestHitInfo
    
and intersectNode ray node tMin =
    let rayLocal = {Origin = transformPointInv node.Transform ray.Origin; Direction = (transformVector node.Transform.Inv ray.Direction).Normalized()}
    tryFindBestHitInfo [
        intersectNodes rayLocal node.Children tMin
        intersect rayLocal node.Object tMin node.Material ]
    |> Option.map (fun hitInfo ->
        let point = transformPoint node.Transform hitInfo.Point
        let normal = transformNormal node.Transform hitInfo.Normal
        // let normal = hitInfo.Normal
        // let normal = transformVector node.Transform.M hitInfo.Normal
        let t = (point - ray.Origin).Length
        {hitInfo with Point = point; Normal = normal.Normalized(); T = t})    

let render (bitmap : Bitmap) (zbuffer : float [,]) (scene : Scene) =
    for r = 0 to bitmap.Height - 1 do
        for c = 0 to bitmap.Width - 1 do
            let ray = scene.Camera.Ray c r
            let t, color = 
                match intersectNodes ray scene.Nodes 0.00001 with
                | Some hitInfo ->
                    let material = scene.Materials.[hitInfo.Material]
                    let color = shade ray material hitInfo (scene.Lights |> Map.toSeq |> Seq.map snd) 
                    // let color = hitInfo.Normal * 0.5 + Vector3d(0.5)
                    Debug.Assert(not (color.X < 0.0 || color.Y < 0.0 || color.Z < 0.0))
                    (hitInfo.T, color)
                | _ -> 
                    (Double.PositiveInfinity, Vector3d.Zero)
            zbuffer.[r, c] <- t
            setPixel bitmap c r (clamp color)

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
    