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
        // let ambComponent = 
        //     Seq.tryFind isAmbient lights
        //     |> Option.map (fun light -> 
        //         illuminate light hitInfo.Point hitInfo.Normal nodes)
        //     |> Option.defaultValue Vector3d.Zero
        // let lights = Seq.filter (isAmbient >> not) lights
        // let initial = ambComponent * diffuseColor
        // lights
        // |> Seq.fold (fun result light ->
        //     if isInShadow light hitInfo nodes then
        //         result
        //     else
        //         let wi = -(lightDir light hitInfo.Point)
        //         let lightColor = illuminate light hitInfo.Point hitInfo.Normal nodes
        //         let nDotWi = Math.Max(Vector3d.Dot(hitInfo.Normal, wi), 0.0)
        //         let lambertian =
        //             nDotWi * lightColor * diffuseColor
        //         let specularCoef = 
        //             if nDotWi > 0.0 then
        //                 let viewDir = - ray.Direction
        //                 let halfDir = (wi + viewDir).Normalized()
        //                 let specAngle = Math.Max(Vector3d.Dot(halfDir, hitInfo.Normal), 0.0)
        //                 Math.Pow(specAngle, glossiness)
        //             else
        //                 0.0
        //         result + lambertian + specularColor * specularCoef * lightColor * nDotWi
        //     ) initial
        let light = Seq.head lights
        let lightColor = illuminate light hitInfo.Point hitInfo.Normal nodes
        let v = hitInfo.Normal
        let u = Vector3d.Cross(v, Vector3d(v.X, 0.0, v.Z))
        let w = Vector3d.Cross(u, v)
        let sample = randomInHemisphere()
        let direction = Vector3d(Vector3d.Dot(u, sample), Vector3d.Dot(v, sample), Vector3d.Dot(w, sample))
        let newRay = {Origin = hitInfo.Point; Direction = direction.Normalized()}
        match intersectNodes newRay nodes epsilon with
        | Some hitInfo2 ->
            lightColor * diffuseColor
        | None ->
            0.1 * lightColor * diffuseColor

let defBlinnDiffuse = Vector3d(0.5)
let defBlinnSpecular = Vector3d(0.7)
let defBlinnGlossiness = 20.0