module Material
open OpenTK
open Light
open Common
open System
open Node
open System.Diagnostics

type BlinnData = {
    DiffuseColor : Vector3d
    SpecularColor : Vector3d
    Glossiness : float
    Reflection : Vector3d
    Refraction : Vector3d
    Ior : float
    Absorption : Vector3d
}

type Material =
    | Blinn of BlinnData

let reflect (rayDir : Vector3d) (normal : Vector3d) =
    let proj = normal * Vector3d.Dot(-rayDir, normal)
    (rayDir + 2.0 * proj).Normalized()
    
let rec shade ray material hitInfo lights nodes =
        match material with
        | Blinn b ->
            let ambComponent = 
                Seq.tryFind isAmbient lights
                |> Option.map (fun light -> 
                    illuminate light hitInfo.Point hitInfo.Normal nodes)
                |> Option.defaultValue Vector3d.Zero
            let lights = Seq.filter (isAmbient >> not) lights
            let initial = ambComponent * b.DiffuseColor
            let directColor =
                lights
                |> Seq.fold (fun result light ->
                    if isInShadow light hitInfo nodes then
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
            let reflectedColor = 
                if b.Reflection.X > epsilon && b.Reflection.Y > epsilon && b.Reflection.Z > epsilon then
                    let reflectedDir = reflect ray.Direction hitInfo.Normal
                    let reflectedRay = {Origin = hitInfo.Point; Direction = reflectedDir}
                    match intersectNodes reflectedRay nodes epsilon (hitInfo.Depth + 1) with
                    | Some hitInfoRefl when hitInfoRefl.Depth < maxDepth ->
                        let reshade = shade reflectedRay material hitInfo lights nodes
                        reshade * b.Reflection
                    | _ -> Vector3d.Zero
                else Vector3d.Zero
            directColor + reflectedColor                

let defaultBlinn = {
    DiffuseColor = Vector3d(0.5)
    SpecularColor = Vector3d(0.7)
    Glossiness = 20.0
    Reflection = Vector3d.Zero
    Refraction = Vector3d.Zero
    Ior = 0.0
    Absorption = Vector3d.Zero
}