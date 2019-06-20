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
//
let whitted (scene : Scene.Scene) ray =
    match Node.intersect ray scene.Primitive with
    | Some(hitInfo) ->
        assert (hitInfo.Prim.IsSome)
        let light = pickOne scene.AreaLights
        let direct = Option.defaultValue Vector3d.Zero <| Option.map Node.emitted hitInfo.Prim
        let (lightPoint, lightNorm) = Light.sample light (Sampling.next2D scene.Sampler)
        let lightPdf = 1. / Light.area light
        let emitted = light.Radiance
        let wi = lightPoint - hitInfo.Point
        let distanceSq = wi.LengthSquared
        wi.Normalize()
        let cosHit = Vector3d.Dot(hitInfo.Normal, wi)
        let cosLight = Vector3d.Dot(lightNorm, -wi)
        let geoTerm = Math.Max(0., cosHit * cosLight) / distanceSq
        let material = Node.getMaterial hitInfo.Prim.Value
        let bsdf = Material.computeBsdf material
        let attenuation = Material.evaluate bsdf -ray.Direction wi
        let reflected = attenuation * geoTerm * emitted / lightPdf
        let res = direct + reflected
        assert (res.X >= 0.)
        // printfn "%A" res
        res
    | _ -> Vector3d.Zero

// Li = (F/4pi^2) * max(0, cos(theta)) / |x - p| ^ 2 * visibility
let simple (scene : Scene.Scene) ray =
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

        let shadowRay =
            { ray with Direction = shadowRayDir
                       Origin = hitInfo.Point }
        match Node.intersect shadowRay scene.Primitive with
        | None ->
            let cosTheta = Vector3d.Dot(hitInfo.Normal.Normalized(), shadowRayDir)
            Vector3d.One * cosTheta / Math.PI
        | _ -> Vector3d.Zero
    | _ -> Vector3d.Zero

let estimateRadiance integrator (scene : Scene.Scene) ray =
    match integrator with
    | Simple -> simple scene ray
    | AmbientOcclusion -> ambientOcclusion scene ray
    | Whitted -> whitted scene ray
