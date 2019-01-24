module Noise

open OpenTK
open System
open System.Diagnostics

let private random = new Random()

let private randFloats =
    Array.init 256 (fun _ -> random.NextDouble())

let private randVectors =
    Array.init 256 (fun _ -> 
        let x = random.NextDouble()
        let y = random.NextDouble()
        let z = random.NextDouble()
        (Vector3d(x, y, z) * 2.0 - Vector3d.One).Normalized()
        )

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
                accum <- accum + (a * b * c * array.[i, j, k])
    accum

let perlinInterp (array : Vector3d [,,]) u v w =
    let uu = u * u * (3.0 - 2.0 * u)
    let vv = v * v * (3.0 - 2.0 * v)
    let ww = w * w * (3.0 - 2.0 * w)
    let mutable accum = 0.0
    for i = 0 to 1 do
        for j = 0 to 1 do
            for k = 0 to 1 do
                let ii = float i
                let jj = float j
                let kk = float k
                let weigth = Vector3d(u - ii, v - jj, w - kk)
                let a = ii * uu + (1.0 - ii) * (1.0 - uu)
                let b = jj * vv + (1.0 - jj) * (1.0 - vv)
                let c = kk * ww + (1.0 - kk) * (1.0 - ww)                
                let dot = Vector3d.Dot(array.[i, j, k], weigth)
                accum <- accum + (a * b * c * dot)
                Debug.Assert(accum > -1.0 && accum < 1.0)
    // let accum = accum + 1.0
    accum * 0.5 + 0.5
    // Math.Abs(accum)

let noise (point : Vector3d) =     
    let v = point.X - Math.Floor(point.X)
    let u = point.Y - Math.Floor(point.Y)
    let w = point.Z - Math.Floor(point.Z)
    let i = int <| Math.Floor point.X
    let j = int <| Math.Floor point.Y
    let k = int <| Math.Floor point.Z
    let c = Array3D.create 2 2 2 Vector3d.Zero
    for di = 0 to 1 do
        for dj = 0 to 1 do
            for dk = 0 to 1 do
                let permX = permutationX.[(i + di) &&& 255]
                let permY = permutationY.[(j + dj) &&& 255]
                let permZ = permutationZ.[(k + dk) &&& 255]
                let index =  permX ^^^ permY ^^^ permZ
                c.[di, dj, dk] <- randVectors.[index]
    perlinInterp c u v w
    // c.[1, 1, 1]

let turbulence (point : Vector3d) depth =
    let rec loop i accum weight scale maxVal =
        if i < depth then
            let accum = (accum + (weight * noise (scale * point)))
            let maxVal = Math.Max(accum, maxVal)
            loop (i + 1) accum (weight * 0.5) (scale * 2.0) maxVal
        else
            accum, maxVal
    let accum, maxVal = loop 0 0.0 1.0 1.0 Double.MinValue
    Debug.Assert(accum / maxVal <= 1.0)
    accum / maxVal
    