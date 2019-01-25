module Noise

open OpenTK
open System
open System.Diagnostics

let private random = new Random()

let private randFloats =
    Array.init 256 (fun _ -> random.NextDouble())

let private gradients =
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

let lerp111 low high t =  (1.0 - t) * low + t * high
let smoothstep111 t = t * t * (3.0 - 2.0 * t)

let perlinInterp (lattice : Vector3d [,,]) u v w =
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
                let dot = Vector3d.Dot(lattice.[i, j, k], weigth)
                accum <- accum + (a * b * c * dot)
                Debug.Assert(accum > -1.0 && accum < 1.0)
    // let accum = accum + 1.0
    accum * 0.5 + 0.5
    // accum <- 0.0
    // for i = 0 to 1 do
    //     let ii = float i
    //     let weigth = u - ii
    //     let a = ii * uu + (1.0 - ii) * (1.0 - uu)      
    //     let dot = lattice.[i, 0, 0].X * weigth
    //     accum <- accum + (a * dot)
    // accum * 0.5 + 0.5
    let x0 = u
    let x1 = u - 1.0
    let y0 = v
    let y1 = v - 1.0
    let z0 = w
    let z1 = w - 1.0
    let p000 = Vector3d(x0, y0, z0)
    let p100 = Vector3d(x1, y0, z0)
    let p010 = Vector3d(x0, y1, z0)
    let p110 = Vector3d(x1, y1, z0)
    
    let p001 = Vector3d(x0, y0, z1)    
    let p101 = Vector3d(x1, y0, z1)
    let p011 = Vector3d(x0, y1, z1)
    let p111 = Vector3d(x1, y1, z1)

    let a = lerp111 (Vector3d.Dot(lattice.[0, 0, 0], p000)) (Vector3d.Dot(lattice.[1, 0, 0], p100)) uu
    let b = lerp111 (Vector3d.Dot(lattice.[0, 1, 0], p010)) (Vector3d.Dot(lattice.[1, 1, 0], p110)) uu
    let c = lerp111 (Vector3d.Dot(lattice.[0, 0, 1], p001)) (Vector3d.Dot(lattice.[1, 0, 1], p101)) uu
    let d = lerp111 (Vector3d.Dot(lattice.[0, 1, 1], p011)) (Vector3d.Dot(lattice.[1, 1, 1], p111)) uu

    let e = lerp111 a b v
    let f = lerp111 c d v

    lerp111 e f w * 0.5 + 0.5
    

let makeLattice (point : Vector3d) =
    let i = int (Math.Floor point.X)
    let j = int (Math.Floor point.Y)
    let k = int (Math.Floor point.Z)
    let c = Array3D.create 2 2 2 Vector3d.Zero
    for di = 0 to 1 do
        for dj = 0 to 1 do
            for dk = 0 to 1 do
                let permX = permutationX.[(i + di) &&& 255]
                let permY = permutationY.[(j + dj) &&& 255]
                let permZ = permutationZ.[(k + dk) &&& 255]
                let index =  permX ^^^ permY ^^^ permZ
                c.[di, dj, dk] <- gradients.[index]
    c            

let noise (point : Vector3d) =     
    let v = point.X - Math.Floor point.X
    let u = point.Y - Math.Floor point.Y
    let w = point.Z - Math.Floor point.Z
    let lattice = makeLattice point
    perlinInterp lattice u v w
    // lattice.[0, 0, 0].X * 0.5 + 0.5

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

let marble (point : Vector3d) depth scaleFactor =
    let rec loop i accum weight scale maxVal =
        if i < depth then
            let accum = (accum + (weight * noise (scale * point)))
            let maxVal = Math.Max(accum, maxVal)
            loop (i + 1) accum (weight * 0.5) (scale * 2.0) maxVal
        else
            accum, maxVal
    let accum, maxVal = loop 0 0.0 1.0 1.0 Double.MinValue
    let sin = Math.Sin(accum * 10.0 + scaleFactor * point.Z)
    (sin + 1.0) / 2.0
    