#load "../references.fsx"
// #load "Render.fs"

// open FsCheck
// open Render

// let revRevIsOrig (xs:list<int>) = List.rev(List.rev xs) = xs



// Check.Quick revRevIsOrig

open OpenTK
open System

let sequence = Seq.initInfinite (fun _ -> 
    let random = new System.Random()
    random.NextDouble(),random.NextDouble(),random.NextDouble())


sequence |> Seq.head
sequence |> Seq.tail |> Seq.head 
sequence |> Seq.find (fun (x, y, z) -> System.Math.Sqrt(x * x + y * y + z * z) < 1.0)
|>  (fun (x, y, z as xxx)  -> System.Math.Sqrt(x * x + y * y + z * z), xxx)

let randomInUnitSphere () =
    let random = new System.Random()
    let points = Seq.initInfinite (fun _ -> 
        let x = random.NextDouble()
        let y = random.NextDouble()
        let z = random.NextDouble()
        2.0 * Vector3d(x, y, z) - Vector3d(1.0, 1.0, 1.0))
    Seq.find (fun (v : Vector3d) -> v.Length < 1.0) points

randomInUnitSphere()

let reflected (rayDir : Vector3d) (normal : Vector3d) =
    let proj = normal * Vector3d.Dot(-rayDir, normal)
    (rayDir + 2.0 * proj).Normalized()


let refract (rayDir : Vector3d) (normal : Vector3d) niOverNt =
    let rayDir = rayDir.Normalized()
    let dot = Vector3d.Dot(rayDir, normal)
    let discr = 4.0 * (1.0 - niOverNt * niOverNt * (1.0 - dot * dot))
    if discr >= 0.0 then
        let cos = -Math.Sqrt(discr) / 2.0
        let projection = normal * dot
        let component1 = rayDir * niOverNt - projection * niOverNt
        let component2 = normal * cos
        let result = component1 + component2
        Some(result.Normalized())
    else
        None
    Some((rayDir * niOverNt + (niOverNt * dot - Math.Sqrt(1.0 - dot*dot)) * normal).Normalized())

// let v = refract (Vector3d(1.0, -1.0, 0.0).Normalized()) (Vector3d(0.0, 1.0, 0.0)) 0.5
let v = refract (Vector3d(1.0, -0.5, 0.0).Normalized()) (Vector3d(0.0, 1.0, 0.0)) 0.5
// let v = refract (Vector3d(0.25, -1.0, 0.0).Normalized()) (Vector3d(0.0, 1.0, 0.0)) 1.5
// v, Math.Asin(v.Value.Normalized().Y)
// printfn "%A" v
// val v : Vector3d option = Some (0,316227766016838; -0,948683298050514; 0)
// Some (0,447213595499958; -0,894427190999916; 0)