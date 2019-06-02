module Bounds

open OpenTK
open Common
open System

type Bounds =
    { PMin : Vector3d
      PMax : Vector3d }

let hitBoundingBox (box : Bounds) ray tmin tmax =
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
    let t1, t2 =
        handle (tmin, tmax) (box1.X, box2.X, ray.Origin.X, ray.Direction.X)
    let t1, t2 = handle (t1, t2) (box1.Y, box2.Y, ray.Origin.Y, ray.Direction.Y)
    let t1, t2 = handle (t1, t2) (box1.Y, box2.Y, ray.Origin.Y, ray.Direction.Y)
    t1 < t2 && t2 > 0.0

let union box1 box2 =
    let xmin = Math.Min(box1.PMin.X, box2.PMin.X)
    let ymin = Math.Min(box1.PMin.Y, box2.PMin.Y)
    let zmin = Math.Min(box1.PMin.Z, box2.PMin.Z)
    let xmax = Math.Max(box1.PMax.X, box2.PMax.X)
    let ymax = Math.Max(box1.PMax.Y, box2.PMax.Y)
    let zmax = Math.Max(box1.PMax.Z, box2.PMax.Z)
    { PMin = Vector3d(xmin, ymin, zmin)
      PMax = Vector3d(xmax, ymax, zmax) }

let addPoint box point =
    union box {PMin = point; PMax = point}
