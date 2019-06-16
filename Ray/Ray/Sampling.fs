module Sampling

open System
open OpenTK

let private random = Random()

type Sampler =
    { Generator : Random
      Count : int }

let makeSampler n : Sampler =
    { Generator = Random()
      Count = n }

let next2D (sampler : Sampler) =
    Vector2d(sampler.Generator.NextDouble(), sampler.Generator.NextDouble())

let randomInUnitSphere () =
    let points = Seq.initInfinite (fun _ -> 
        let x = random.NextDouble()
        let y = random.NextDouble()
        let z = random.NextDouble()
        2.0 * Vector3d(x, y, z) - Vector3d(1.0, 1.0, 1.0))
    Seq.find (fun (v : Vector3d) -> v.Length < 1.0) points

let randomInDisk () =
    let points = Seq.initInfinite (fun _ -> 
        let x = random.NextDouble()
        let y = random.NextDouble()
        2.0 * Vector3d(x, y, 0.0) - Vector3d(1.0, 1.0, 0.0))
    Seq.find (fun (v : Vector3d) -> v.Length < 1.0) points

let randomInHemisphere () =
    let r1 = random.NextDouble()
    let r2 = random.NextDouble()
    let phi = 2.0 * Math.PI * r1
    let theta = Math.Acos(1.0 - r2)
    let x  = Math.Sin theta * Math.Cos phi
    let y = Math.Sin theta * Math.Sin phi
    let z = Math.Cos theta
    Vector3d(x, z, y)

let randomInHemisphere2 () =
    let r1 = random.NextDouble()
    let r2 = random.NextDouble()
    let phi = 2.0 * Math.PI * r1
    let theta = Math.Acos(2.0 * r2 - 1.0)
    let x  = Math.Sin theta * Math.Cos phi
    let y = Math.Sin theta * Math.Sin phi
    let z = Math.Cos theta
    Vector3d(x, z, y)

let randomCosineDirection () =
    let r1 = random.NextDouble()
    let r2 = random.NextDouble()
    let z = Math.Sqrt(1.0 - r2)
    let phi = 2.0 * Math.PI * r1
    let x = Math.Cos phi * 2.0 * Math.Sqrt r2
    let y = Math.Sin phi * 2.0 * Math.Sqrt r2
    Vector3d(x, y, z)

let randomTwo() =
    (random.NextDouble(), random.NextDouble())

let private tent xi =
    if xi < 0.5 then
        -1. + Math.Sqrt(2. * xi)
    else
        1. - Math.Sqrt(2. - 2.*xi)

let squareToTent (p : Vector2d) =
    Vector2d(tent p.X, tent p.Y)

let squareToTentPdf (p : Vector2d) =
    let prob (a : float) = 1. - Math.Abs(a)
    prob p.X * prob p.Y

let squareToCosineHemisphere (sample : Vector2d) =
    let r1 = sample.X
    let r2 = sample.Y
    let z = Math.Sqrt(1.0 - r2)
    let phi = 2.0 * Math.PI * r1
    let x = Math.Cos phi * 2.0 * Math.Sqrt r2
    let y = Math.Sin phi * 2.0 * Math.Sqrt r2
    Vector3d(x, y, z)