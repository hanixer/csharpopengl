module Integrator

open System
open OpenTK
open Light
open Sampling
open Common
open Types

type Integrator =
    | Simple
    | AmbientOcclusion
    | Whitted

// 12345
// Pick one light
// Sample a point on a light
// Get emitted radiance
// Compute visibility

let isVisible (scene : Scene.Scene) origin target w n =
    let ray = makeRay origin w
    let epsilon = 0.0001
    match Node.intersect ray scene.Primitive with
    | Some(hit) ->
        let diff = hit.Point - target
        diff.Length < epsilon
    | _ -> false

let whitted (scene : Scene.Scene) ray =
    match Node.intersect ray scene.Primitive with
    | Some(hit) ->
        assert (hit.Prim.IsSome)
        let light = pickOne scene.AreaLights
        let direct = Option.defaultValue Vector3d.Zero <| Option.map Node.emitted hit.Prim
        let (lightPoint, lightNorm) = Light.sampleWithPoint light (Sampling.next2D scene.Sampler) hit.Point
        let lightPdf = Light.sampleWithPointPdf light hit.Point
        // let lightPdf = 0.05
        let emitted = light.Radiance
        let wi = lightPoint - hit.Point
        if wi = Vector3d.Zero then
            direct
        else
            let distanceSq = wi.LengthSquared
            wi.Normalize()
            let cosHit = Math.Max(0., Vector3d.Dot(hit.Normal, wi))
            let cosLight = Math.Max(0., Vector3d.Dot(lightNorm, -wi))
            let visibility = if isVisible scene hit.Point lightPoint wi lightNorm then 1. else 0.
            let geoTerm = visibility * cosHit * cosLight / distanceSq
            let material = Node.getMaterial hit.Prim.Value
            let bsdf = Material.computeBsdf material
            let attenuation = Material.evaluate bsdf -ray.Direction wi
            let reflected = attenuation * geoTerm * emitted / lightPdf
            let res = direct + reflected
            // printfn "%A" lightPdf
            assert (res.X >= 0.)
            res
            // (wi + Vector3d.One) * 0.5
            // (hit.Normal + Vector3d.One) * 0.5
            // (lightNorm + Vector3d.One) * 0.5
            // (Vector3d(cosHit) + Vector3d.One) * 0.5
            // (Vector3d(geoTerm) + Vector3d.One) * 0.5
            // Vector3d(visibility)
    | _ -> Vector3d.Zero

// Li = (F/4pi^2) * max(0, cos(theta)) / |x - p| ^ 2 * visibility
let simple (scene : Scene.Scene) ray =
    let light = (Seq.head scene.Lights).Value
    match Node.intersect ray scene.Primitive with
    | Some(hit) ->
        let lightPos = lightPos light
        let energy = energy light / (4. * Math.PI * Math.PI)
        let wi = lightPos - hit.Point
        let distanceSq = wi.LengthSquared
        wi.Normalize()
        // printfn "%A %A" dirToLight hit.Normal
        let cosTheta = Vector3d.Dot(hit.Normal.Normalized(), wi)
        let shadowRay = { ray with Direction = wi }
        match Node.intersect shadowRay scene.Primitive with
        | None ->
            let res = energy * Math.Max(0., cosTheta) / distanceSq
            // (hit.Normal + Vector3d.One)* 0.5
            res
        // (dirToLight + Vector3d.One)* 0.5
        | _ -> Vector3d.Zero
    | _ -> Vector3d.Zero

let ambientOcclusion (scene : Scene.Scene) ray =
    match Node.intersect ray scene.Primitive with
    | Some(hit) ->
        let sample2D = next2D scene.Sampler
        let sampleDir = squareToCosineHemisphere sample2D
        let uvw = buildOrthoNormalBasis hit.Normal
        let shadowRayDir = localOrthoNormalBasis sampleDir uvw

        let shadowRay =
            { ray with Direction = shadowRayDir
                       Origin = hit.Point }
        match Node.intersect shadowRay scene.Primitive with
        | None ->
            let cosTheta = Vector3d.Dot(hit.Normal.Normalized(), shadowRayDir)
            Vector3d.One * cosTheta / Math.PI
        | _ -> Vector3d.Zero
    | _ -> Vector3d.Zero

let estimateRadiance integrator (scene : Scene.Scene) ray =
    match integrator with
    | Simple -> simple scene ray
    | AmbientOcclusion -> ambientOcclusion scene ray
    | Whitted -> whitted scene ray
