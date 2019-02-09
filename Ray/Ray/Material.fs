module Material
open OpenTK
open Light
open Common

type Material =
    | Blinn of Vector3d * Vector3d * float // diffuse, specular, glossy
    
let shade material hitInfo lights =
    match material with
    | Blinn(diffuse, specular, glossiness) ->
        let ambient = Seq.tryFind isAmbient lights
        let lights = Seq.filter (isAmbient >> not) lights
        let initial = 
            ambient
            |> Option.map (fun light -> illuminate light hitInfo.Point hitInfo.Normal)
            |> Option.defaultValue Vector3d.Zero
        lights
        |> Seq.fold (fun result light ->
            let wi = -(lightDir light hitInfo.Point)
            let lightColor = illuminate light hitInfo.Point hitInfo.Normal
            let nDotWi = Vector3d.Dot(hitInfo.Normal, wi)
            if nDotWi > 0.0 then
                result + nDotWi * lightColor * diffuse
            else
            // (hitInfo.Normal + Vector3d(1.0)) * 0.5
                result
            ) (initial * diffuse)

let defBlinnDiffuse = Vector3d(0.5)
let defBlinnSpecular = Vector3d(0.7)
let defBlinnGlossiness = 20.0