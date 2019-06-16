module Integrator

open System
open OpenTK
open Light

type Integrator =
    | Simple

// Li = (F/4pi^2) * max(0, cos(theta)) / |x - p| ^ 2 * visibility
let lightAlongRay (scene : Scene.Scene) ray =
    let light = (Seq.head scene.Lights).Value
    match Node.intersect ray scene.Primitive with
    | Some(hitInfo) ->
        let lightPos = lightPos light
        let energy = energy light / (4. * Math.PI * Math.PI)
        let dirToLight = lightPos - hitInfo.Point
        let distanceSq = dirToLight.LengthSquared
        dirToLight.Normalize()
        let cosTheta = Vector3d.Dot(hitInfo.Normal.Normalized(), dirToLight)
        hitInfo.Point
        let shadowRay = { ray with Direction = dirToLight }
        match Node.intersect shadowRay scene.Primitive with
        | None ->
            energy * Math.Max(0., cosTheta) / distanceSq
        | _ -> Vector3d.Zero
     | _ -> Vector3d.Zero