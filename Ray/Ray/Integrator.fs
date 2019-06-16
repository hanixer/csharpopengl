module Integrator

open System
open OpenTK
open Light
open Sampling
open Common

type Integrator =
    | Simple
    | AmbientOcclusion

// Li = (F/4pi^2) * max(0, cos(theta)) / |x - p| ^ 2 * visibility
let lightAlongRaySimple  (scene : Scene.Scene) ray =
    let light = (Seq.head scene.Lights).Value
    match Node.intersect ray scene.Primitive with
    | Some(hitInfo) ->
        let lightPos = lightPos light
        let energy = energy light / (4. * Math.PI * Math.PI)
        let dirToLight = lightPos - hitInfo.Point
        let distanceSq = dirToLight.LengthSquared
        dirToLight.Normalize()
        // printfn "%A %A" dirToLight hitInfo.Normal
        let cosTheta = Vector3d.Dot(hitInfo.Normal.Normalized(), dirToLight)
        let shadowRay = { ray with Direction = dirToLight }
        match Node.intersect shadowRay scene.Primitive with
        | None ->
            let res = energy * Math.Max(0., cosTheta) / distanceSq
            // (hitInfo.Normal + Vector3d.One)* 0.5
            res
            // (dirToLight + Vector3d.One)* 0.5

        | _ -> Vector3d.Zero
     | _ -> Vector3d.Zero

let ambientOcclusion (scene : Scene.Scene) ray =
    match Node.intersect ray scene.Primitive with
    | Some(hitInfo) ->
        let sample2D = next2D scene.Sampler
        let sampleDir = squareToCosineHemisphere sample2D
        let uvw = buildOrthoNormalBasis hitInfo.Normal
        let shadowRayDir = localOrthoNormalBasis sampleDir uvw
        let shadowRay = { ray with Direction = shadowRayDir ; Origin = hitInfo.Point }
        match Node.intersect shadowRay scene.Primitive with
        | None ->
            let cosTheta = Vector3d.Dot(hitInfo.Normal.Normalized(), shadowRayDir)
            Vector3d.One * cosTheta / Math.PI
        | _ -> Vector3d.Zero
    | _ -> Vector3d.Zero

let lightAlongRay integrator (scene : Scene.Scene) ray =
    match integrator with
    | Simple -> lightAlongRaySimple scene ray
    | AmbientOcclusion -> ambientOcclusion scene ray