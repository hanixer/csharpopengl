module Light
open OpenTK
open Common
open Node
open System
open Sampling

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
    | AreaLight _ -> Vector3d.One

let lightDir light point =
    match light with
    | AmbientLight _ -> Vector3d.Zero
    | DirectLight(_, direction) -> direction
    | PointLight(_, position) -> (point - position).Normalized()
    | AmbientOccluder(_) -> randomInHemisphere()
    | _ -> failwith "lightDir: not implemented"

let isAmbient l = 
    match l with
    | AmbientLight _ -> true
    | _ -> false

let isInShadow light point nodes =     
    let direction = -(lightDir light point)
    let shadowRay = makeRay point direction
    let shadowHit = intersectNodes shadowRay nodes
    match (shadowHit, light) with
    | Some shadowHitInfo, PointLight(_, lightPos) ->
        (shadowHitInfo.Point - point).Length < (lightPos - point).Length
    | _ -> Option.isSome shadowHit
    
let samplePointOnLight light nodes =
    match light with
    | AreaLight(nodeName) ->
        let node = Map.find nodeName nodes
        samplePointAndNormOnNode node
    | _ -> None    

let getAreaOfLight light nodes =
    match light with
    | AreaLight(nodeName) ->
        let node = Map.find nodeName nodes
        getAreaOfNode node
    | _ -> 0.0