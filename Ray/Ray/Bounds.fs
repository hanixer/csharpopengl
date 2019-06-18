module Bounds

open OpenTK
open Types
open System

let mutable counter = 0

let makeBounds a b =
    counter <- counter + 1
    // printfn "bounds counter %d" counter
    { PMin = a
      PMax = b }

let hitBoundingBox ray (box : Bounds) =
    let tmin = ray.TMin
    let tmax = ray.TMax

    let handle (tmin, tmax) (box1, box2, rayOrig, rayDir) =
        let inv = 1.0 / rayDir
        let t1 = (box1 - rayOrig) * inv
        let t2 = (box2 - rayOrig) * inv

        let t3, t4 =
            if rayDir < 0.0 then (t2, t1)
            else (t1, t2)

        let t5 = Math.Max(tmin, t3)
        let t6 = Math.Min(tmax, t4)
        (t5, t6)

    let box1 = box.PMin
    let box2 = box.PMax
    let t1, t2 = handle (tmin, tmax) (box1.X, box2.X, ray.Origin.X, ray.Direction.X)
    let t1, t2 = handle (t1, t2) (box1.Y, box2.Y, ray.Origin.Y, ray.Direction.Y)
    let t1, t2 = handle (t1, t2) (box1.Z, box2.Z, ray.Origin.Z, ray.Direction.Z)
    t1 <= t2

let union box1 box2 =
    let xmin = Math.Min(box1.PMin.X, box2.PMin.X)
    let ymin = Math.Min(box1.PMin.Y, box2.PMin.Y)
    let zmin = Math.Min(box1.PMin.Z, box2.PMin.Z)
    let xmax = Math.Max(box1.PMax.X, box2.PMax.X)
    let ymax = Math.Max(box1.PMax.Y, box2.PMax.Y)
    let zmax = Math.Max(box1.PMax.Z, box2.PMax.Z)
    { PMin = Vector3d(xmin, ymin, zmin)
      PMax = Vector3d(xmax, ymax, zmax) }

let inline getMin (p1 : Vector3d) (p2 : Vector3d) =
    let xmin = Math.Min(p1.X, p2.X)
    let ymin = Math.Min(p1.Y, p2.Y)
    let zmin = Math.Min(p1.Z, p2.Z)
    Vector3d(xmin, ymin, zmin)

let inline getMax (p1 : Vector3d) (p2 : Vector3d) =
    let xmax = Math.Max(p1.X, p2.X)
    let ymax = Math.Max(p1.Y, p2.Y)
    let zmax = Math.Max(p1.Z, p2.Z)
    Vector3d(xmax, ymax, zmax)

let unionMany boxes =
    let mutable pmin = Vector3d(Double.MaxValue)
    let mutable pmax = Vector3d(-Double.MaxValue)
    for b in boxes do
        pmin <- getMin pmin b.PMin
        pmax <- getMax pmax b.PMax
    { PMin = pmin
      PMax = pmax }

let unionManyP points =
    let mutable pmin = Vector3d(Double.MaxValue)
    let mutable pmax = Vector3d(-Double.MaxValue)
    for p in points do
        pmin <- getMin pmin p
        pmax <- getMax pmax p
    { PMin = pmin
      PMax = pmax }

let centroid box =
    0.5 * box.PMin + 0.5 * box.PMax

let maximumExtent box =
    let v = box.PMax - box.PMin
    if v.X > v.Y && v.X > v.Z then 0
    else if v.X > v.Z then 1
    else 2

let addPoint box point =
    union box { PMin = point
                PMax = point }

let isLess (v1 : Vector3d) (v2 : Vector3d) = v1.X < v2.X && v1.Y < v2.Y && v1.Z < v2.Z
let isLessOrEq (v1 : Vector3d) (v2 : Vector3d) = v1.X <= v2.X && v1.Y <= v2.Y && v1.Z <= v2.Z

let intersects box1 box2 =
    isLessOrEq box1.PMin box2.PMax && isLessOrEq box2.PMin box1.PMax

// returns true if box1 contains box2
let contains box1 box2 =
    isLessOrEq box1.PMin box2.PMin && isLessOrEq box2.PMax box1.PMax

let mutable counter2 = 0
let splitBox (box : Bounds) =
    counter2 <- counter2 + 1
    // printfn "splitBox counter %d" counter2
    let mid = box.PMin + (box.PMax - box.PMin) / 2.
    let off = mid - box.PMin
    let xoff = Vector3d(off.X, 0., 0.)
    let yoff = Vector3d(0., off.Y, 0.)
    let zoff = Vector3d(0., 0., off.Z)
    [| makeBounds box.PMin mid
       makeBounds (box.PMin + xoff) (mid + xoff)
       makeBounds (box.PMin + zoff) (mid + zoff)
       makeBounds (box.PMin + zoff + xoff) (mid + zoff + xoff)
       makeBounds (box.PMin + yoff) (mid + yoff)
       makeBounds (box.PMin + yoff + xoff) (mid + yoff + xoff)
       makeBounds (box.PMin + yoff + zoff) (mid + yoff + zoff)
       makeBounds (box.PMin + yoff + zoff + xoff) (mid + yoff + zoff + xoff) |]

let volume box =
    let diff = box.PMax - box.PMin
    diff.X * diff.Y * diff.Z
