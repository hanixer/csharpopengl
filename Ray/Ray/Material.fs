module Material
open OpenTK
open Light
open Common
open System
open Node

type Material =
    | Blinn of Vector3d * Vector3d * float // diffuse, specular, glossy
    
let shade ray material hitInfo lights nodes =
    match material with
    | Blinn(diffuseColor, specularColor, glossiness) ->
        let ambComponent = 
            Seq.tryFind isAmbient lights
            |> Option.map (fun light -> 
                illuminate light hitInfo.Point hitInfo.Normal nodes)
            |> Option.defaultValue Vector3d.Zero
        let lights = Seq.filter (isAmbient >> not) lights
        let initial = ambComponent * diffuseColor
        lights
        |> Seq.fold (fun result light ->
            if isInShadow light hitInfo nodes then
                result
            else
                let wi = -(lightDir light hitInfo.Point)
                let lightColor = illuminate light hitInfo.Point hitInfo.Normal nodes
                let nDotWi = Math.Max(Vector3d.Dot(hitInfo.Normal, wi), 0.0)
                let lambertian =
                    nDotWi * lightColor * diffuseColor
                let specularCoef = 
                    if nDotWi > 0.0 then
                        let viewDir = - ray.Direction
                        let halfDir = (wi + viewDir).Normalized()
                        let specAngle = Math.Max(Vector3d.Dot(halfDir, hitInfo.Normal), 0.0)
                        Math.Pow(specAngle, glossiness)
                    else
                        0.0
                result + lambertian + specularColor * specularCoef * lightColor * nDotWi
            ) initial

let defBlinnDiffuse = Vector3d(0.5)
let defBlinnSpecular = Vector3d(0.7)
let defBlinnGlossiness = 20.0