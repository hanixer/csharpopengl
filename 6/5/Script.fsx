#load "../references.fsx"
// #load "Render.fs"

// open FsCheck
// open Render

// let revRevIsOrig (xs:list<int>) = List.rev(List.rev xs) = xs



// Check.Quick revRevIsOrig

open OpenTK

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