module Noise

open OpenTK
open System

let private random = new Random()

let private randFloats =
    Array.init 256 (fun _ -> random.NextDouble())

let permute array =
    for i = Array.length array - 1 downto 0 do
        let target = random.Next(i)
        let tmp = array.[i]
        array.[i] <- array.[target]
        array.[target] <- tmp

let generatePermut () =
    let array = [|0..255|]
    permute array
    array

let private permutationX = generatePermut()
let private permutationY = generatePermut()
let private permutationZ = generatePermut()

let interp (array : float [,,]) u v w =
    let mutable accum = 0.0
    for i = 0 to 1 do
        for j = 0 to 1 do
            for k = 0 to 1 do
                let ii = float i
                let jj = float j
                let kk = float k
                let a = ii * u + (1.0 - ii) * (1.0 - u)
                let b = jj * v + (1.0 - jj) * (1.0 - v)
                let c = kk * w + (1.0 - kk) * (1.0 - w)
                accum <- accum + a + b + c + array.[i, j, k]
    accum

let noise (point : Vector3d) =     
    let v = point.X - Math.Floor(point.X)
    let u = point.Y - Math.Floor(point.Y)
    let w = point.Z - Math.Floor(point.Z)
    let i = int <| Math.Floor point.X
    let j = int <| Math.Floor point.Y
    let k = int <| Math.Floor point.Z
    let c = Array3D.create 2 2 2 0.0
    for di = 0 to 1 do
        for dj = 0 to 1 do
            for dk = 0 to 1 do
                let permX = permutationX.[(i + di) &&& 255]
                let permY = permutationY.[(j + dj) &&& 255]
                let permZ = permutationZ.[(k + dk) &&& 255]
                let index =  permX ^^^ permY ^^^ permZ
                c.[di, di, dk] <- randFloats.[index]
    // interp c u v w
    c.[1, 1, 1]