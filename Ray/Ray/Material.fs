module Material
open OpenTK
open Light
open Common
open System

type Material =
    | Blinn of Vector3d * Vector3d * float // diffuse, specular, glossy
    
let shade ray material hitInfo lights nodes shouldOutput =
    match material with
    | Blinn(diffuseColor, specularColor, glossiness) ->
        let ambient = Seq.tryFind isAmbient lights
        let lights = Seq.filter (isAmbient >> not) lights
        let ambComponent = 
            ambient
            |> Option.map (fun light -> illuminate light hitInfo.Point hitInfo.Normal)
            |> Option.defaultValue Vector3d.Zero
        let initial = ambComponent * diffuseColor
        lights
        |> Seq.fold (fun result light ->
            if isInShadow light hitInfo nodes shouldOutput then
                if shouldOutput then printfn "in shadow result = %A" result
                result
            else
                let wi = -(lightDir light hitInfo.Point)
                let lightColor = illuminate light hitInfo.Point hitInfo.Normal
                let nDotWi = Math.Max(Vector3d.Dot(hitInfo.Normal, wi), 0.0)
                let lambertian =
                    nDotWi * lightColor * diffuseColor
                let specularCoef = 
                    // if nDotWi > 0.0 then
                        let halfDir = (wi - ray.Direction).Normalized()
                        let specAngle = Math.Max(Vector3d.Dot(halfDir, hitInfo.Normal), 0.0)
                        // let specAngle = Math.Abs(Vector3d.Dot(halfDir, hitInfo.Normal))
                        Math.Pow(specAngle, glossiness)
                    // else
                        // 0.0
                if  shouldOutput then
                    printfn "result %A " result
                    printfn "wi %A" wi
                    printfn "lightColor %A" lightColor
                    printfn "ndotwi %A " nDotWi
                    printfn "lasmbertian %A"                 lambertian
                    printfn "spcCoef %A" specularCoef
                result + lambertian + specularColor * specularCoef * lightColor
            ) initial

let defBlinnDiffuse = Vector3d(0.5)
let defBlinnSpecular = Vector3d(0.7)
let defBlinnGlossiness = 20.0