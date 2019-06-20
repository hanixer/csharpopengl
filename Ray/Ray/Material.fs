module Material
open OpenTK
open Light
open Common
open System
open Sampling
open Types

let getAttenuation material =
    match material with
    | Blinn(blinn) -> blinn.DiffuseColor
    | _ -> Vector3d.Zero

let getEmitted material =
    match material with
    | Emissive(color) -> color
    | _ -> Vector3d.Zero

// Return attenuation and scattered ray
let scatter ray material hitInfo =
    match material with
    | Blinn(blinn) ->
        let samplePoint = randomInUnitSphere()
        let target = hitInfo.Point + hitInfo.Normal + samplePoint
        let dir = target - hitInfo.Point
        dir.Normalize()
        let scattered = {ray with Origin = hitInfo.Point; Direction = dir}        
        let attenuation = blinn.DiffuseColor
        Some (attenuation, scattered)
    | _ -> None

let reflect (rayDir : Vector3d) (normal : Vector3d) =
    let proj = normal * Vector3d.Dot(-rayDir, normal)
    (rayDir + 2.0 * proj).Normalized()
    
let refract (rayDir : Vector3d) (normal : Vector3d) ior =
    let rayDir = rayDir.Normalized()
    let dot = Vector3d.Dot(rayDir, normal)
    let ior, cosThetaI, n = if dot < 0.0 then (ior, -dot, normal) else (1.0 / ior, dot, -normal)
    let projected = cosThetaI * n
    let vecDiff = rayDir + projected    
    let component1 = vecDiff / ior
    let sinThetaI = vecDiff.Length
    let sinThetaT = sinThetaI / ior 
    let sinThetaT2 = sinThetaT * sinThetaT
    if sinThetaT2 < 0.0 then 
        Vector3d.Zero
    else
        let cosThetaT = Math.Sqrt(1.0 - sinThetaT2)
        let component2 = -n * cosThetaT
        let result = component1 + component2
        result.Normalize()
        result

let rec shade ray material hitInfo lights nodes =
    match material with
    | Blinn b ->
        let ambComponent = 
            Seq.tryFind isAmbient lights
            |> Option.map (fun light -> 
                illuminate light hitInfo.Point hitInfo.Normal nodes)
            |> Option.defaultValue Vector3d.Zero
        let lightsOther = Seq.filter (isAmbient >> not) lights
        let initial = ambComponent * b.DiffuseColor
        let directColor =
            lightsOther
            |> Seq.fold (fun result light ->
                if isInShadow light hitInfo.Point nodes then
                    result
                else
                    let wi = -(lightDir light hitInfo.Point)
                    let lightColor = illuminate light hitInfo.Point hitInfo.Normal nodes
                    let nDotWi = Math.Max(Vector3d.Dot(hitInfo.Normal, wi), 0.0)
                    let lambertian =
                        nDotWi * lightColor * b.DiffuseColor
                    let specularCoef = 
                        if nDotWi > 0.0 then
                            let viewDir = - ray.Direction
                            let halfDir = (wi + viewDir).Normalized()
                            let specAngle = Math.Max(Vector3d.Dot(halfDir, hitInfo.Normal), 0.0)
                            Math.Pow(specAngle, b.Glossiness)
                        else
                            0.0
                    result + lambertian + b.SpecularColor * specularCoef * lightColor * nDotWi
                ) initial
        let isReflectionPresent = b.Reflection.X > epsilon && b.Reflection.Y > epsilon && b.Reflection.Z > epsilon
        let isRefractionPresent = b.Refraction.X > epsilon && b.Refraction.Y > epsilon && b.Refraction.Z > epsilon
        let scattered = 
            if isReflectionPresent then
                let reflectedDir = reflect ray.Direction hitInfo.Normal
                Some {ray with Origin = hitInfo.Point; Direction = reflectedDir}
            else if isRefractionPresent then
                let refractedDir = refract ray.Direction hitInfo.Normal b.Ior
                Some {ray with Origin = hitInfo.Point; Direction = refractedDir}
            else None
        (directColor, scattered)
    | ReflectMaterial(reflectColor) ->
        let scattered = 
            let reflectedDir = reflect ray.Direction hitInfo.Normal
            Some {ray with Origin = hitInfo.Point; Direction = reflectedDir}
        (reflectColor, scattered)
    | e -> failwithf "shade: not implemented material %A" e

let defaultBlinn = {
    DiffuseColor = Vector3d(0.5)
    SpecularColor = Vector3d(0.7)
    Glossiness = 20.0
    Reflection = Vector3d.Zero
    Refraction = Vector3d.Zero
    Ior = 0.0
    Absorption = Vector3d.Zero
}

let computeBsdf material =
    match material with
    | Blinn(data) -> Diffuse(data.DiffuseColor)
    | _ -> failwith "computeBsdf: not implemented"

let evaluate bsdf (wo : Vector3d) (wi : Vector3d) =
    let eps = 0.00000001
    assert (Math.Abs(wo.Length - 1.) < eps)
    assert (Math.Abs(wi.Length - 1.) < eps)
    match bsdf with
    | Diffuse color ->
        color / Math.PI