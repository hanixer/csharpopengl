module Material

open System
open OpenTK.Graphics.OpenGL
open OpenTK
open Texture
open Common

type Material =
    | Lambertian of Texture
    | Metal of Texture * float
    | Dielectric of float
    | DiffuseLight of Texture

type HitRecord =
    { T : float             // parameter used to determine point from ray
      Point : Vector3d
      Normal : Vector3d
      Material : Material
      TexCoord : Vector2d }

let private random = new Random()

let refract (rayDir : Vector3d) (normal : Vector3d) niOverNt =
    let rayDir = rayDir.Normalized()
    let dot = Vector3d.Dot(rayDir, normal)
    Some((rayDir * niOverNt + (niOverNt * dot - Math.Sqrt(1.0 - dot*dot)) * normal).Normalized())

let reflect (rayDir : Vector3d) (normal : Vector3d) =
    let proj = normal * Vector3d.Dot(-rayDir, normal)
    (rayDir + 2.0 * proj).Normalized()

let refractiveRelation (rayDir : Vector3d) (normal : Vector3d) refrIndex =
    let dot = Vector3d.Dot(rayDir, normal)
    if dot > 0.0 then 
        let cosine = refrIndex * Vector3d.Dot(rayDir, normal) / rayDir.Length
        refrIndex, -normal, cosine
    else    
        let cosine = -Vector3d.Dot(rayDir, normal) / rayDir.Length
        1.0 / refrIndex, normal, cosine

let schlick cosine refrIndex = 
    let r0 = (1.0 - refrIndex) / (1.0 + refrIndex)
    let r0 = r0 * r0
    r0 + (1.0 - r0) * Math.Pow(1.0 - cosine, 5.0)

let scatter material rayIn hitRec =
    match material with
    | Lambertian(albedo) ->
        let randPoint = randomInUnitSphere()
        let target = hitRec.Point + hitRec.Normal + randPoint
        let scattered = {Origin = hitRec.Point; Direction = target - hitRec.Point}
        let attenuation = textureValue albedo hitRec.TexCoord hitRec.Point
        Some (attenuation, scattered)
    | Metal(albedo, fuzzy) ->
        let fuzzy = if fuzzy < 1.0 then fuzzy else 1.0
        let reflected = reflect rayIn.Direction hitRec.Normal
        let randPoint = randomInUnitSphere() * fuzzy
        let scattered = {Origin = hitRec.Point; Direction = reflected + randPoint}
        let attenuation = textureValue albedo hitRec.TexCoord hitRec.Point
        Some(attenuation, scattered)
    | Dielectric(index) ->
        let refrRelation, outwardNormal, cosine = refractiveRelation rayIn.Direction hitRec.Normal index
        let attenuation = Vector3d(1.0)
        let reflected = reflect rayIn.Direction hitRec.Normal
        match refract rayIn.Direction outwardNormal refrRelation with
        | Some refracted ->
            let reflectProb = schlick cosine index
            if random.NextDouble() < reflectProb then
                let scattered = {Origin = hitRec.Point; Direction = reflected}
                Some(attenuation, scattered)
            else
                let scattered = {Origin = hitRec.Point; Direction = refracted}
                Some(attenuation, scattered)
        | None ->
            let scattered = {Origin = hitRec.Point; Direction = reflected}
            Some(attenuation, scattered)
    | DiffuseLight _ -> None

let emitLight material texCoord p =
    match material with
    | DiffuseLight texture ->
        textureValue texture texCoord p
    | _ -> Vector3d.Zero