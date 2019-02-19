module Light
open OpenTK
open Common
open Node
open System

type Light =
    | AmbientLight of Vector3d
    | DirectLight of intensity : Vector3d * direction : Vector3d
    | PointLight of intensity : Vector3d * position : Vector3d // intensity, position
    | AmbientOccluder of intensity : Vector3d * minAmount : float // intensity, min amount
    | AreaLight of object : string

let private random = Random()

let illuminate light hitPoint normal nodes =
    match light with
    | AmbientLight intens -> intens
    | DirectLight(intens, _) -> intens
    | PointLight(intens, _) -> intens
    | AmbientOccluder(intensity, minAmount) ->
        intensity

let lightDir light point =
    match light with
    | AmbientLight _ -> Vector3d.Zero
    | DirectLight(_, direction) -> direction
    | PointLight(_, position) -> (point - position).Normalized()
    | AmbientOccluder(_) -> randomInHemisphere()

let isAmbient l = 
    match l with
    | AmbientLight _ -> true
    | _ -> false

let isInShadow light hitInfo nodes =     
    let direction = -(lightDir light hitInfo.Point)
    let shadowRay = {Origin = hitInfo.Point ; Direction = direction}    
    let shadowHit = intersectNodes shadowRay nodes epsilon
    match (shadowHit, light) with
    | Some shadowHitInfo, PointLight(_, lightPos) ->
        (shadowHitInfo.Point - hitInfo.Point).Length < (lightPos - hitInfo.Point).Length
    | _ -> Option.isSome shadowHit
    