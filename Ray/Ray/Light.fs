module Light
open OpenTK
open Common
open Node

type Light =
    | AmbientLight of Vector3d
    | DirectLight of Vector3d * Vector3d // intensity, direction
    | PointLight of Vector3d * Vector3d // intensity, position

// virtual Color	Illuminate(const Point3 &p, const Point3 &N) const=0;
// virtual Point3	Direction (const Point3 &p) const=0;
// virtual bool	IsAmbient () const { return false; }

let illuminate light point normal =
    match light with
    | AmbientLight intens -> intens
    | DirectLight(intens, _) -> intens
    | PointLight(intens, _) -> intens

let lightDir light point =
    match light with
    | AmbientLight _ -> Vector3d.Zero
    | DirectLight(_, direction) -> direction
    | PointLight(_, position) -> (point - position).Normalized()

let isAmbient l = 
    match l with
    | AmbientLight _ -> true
    | _ -> false

let isInShadow light hitInfo nodes shouldOutput =     
    let direction = -(lightDir light hitInfo.Point)
    let shadowRay = {Origin = hitInfo.Point ; Direction = direction}    
    let shadowHit = intersectNodes shadowRay nodes epsilon
    if shouldOutput then 
        printfn "isinshadow"
        printfn "direction %A " direction
        printfn "shadow ray %A " shadowRay
        printfn "shadowhit %A" shadowHit
    match (shadowHit, light) with
    | Some shadowHitInfo, PointLight(_, lightPos) ->
        if shouldOutput then
            printfn "1 length %A, 2 leength %A" (shadowHitInfo.Point - hitInfo.Point).Length (lightPos - hitInfo.Point).Length
        (shadowHitInfo.Point - hitInfo.Point).Length < (lightPos - hitInfo.Point).Length
    | _ -> Option.isSome shadowHit

let isInShadowDebug light hitInfo nodes shouldOutput =     
    let direction = -(lightDir light hitInfo.Point)
    let shadowRay = {Origin = hitInfo.Point ; Direction = direction}    
    let shadowHit = intersectNodes shadowRay nodes epsilon
    if shouldOutput then 
        printfn "isinshadow"
        printfn "direction %A " direction
        printfn "shadow ray %A " shadowRay
        printfn "shadowhit %A" shadowHit
    match (shadowHit, light) with
    | Some shadowHitInfo, PointLight(_, lightPos) ->
        if shouldOutput then
            printfn "1 length %A, 2 leength %A" (shadowHitInfo.Point - hitInfo.Point).Length (lightPos - hitInfo.Point).Length
        (shadowHitInfo.Point - hitInfo.Point).Length < (lightPos - hitInfo.Point).Length
    | _ -> Option.isSome shadowHit
    
// let illuminate light point normal =
//     match light with
//     | AmbientLight intens -> intens
//     | DirectLight(intens, direction) -> 
//         Vector3d.Dot(normal, direction) * intens
//     | PointLight(intens, lightPos) -> 
//         let direction = (lightPos - point).Normalized()
//         Vector3d.Dot(normal, direction) * intens
