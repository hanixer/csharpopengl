module Bvh

open OpenTK
open System
open Hit
open Common 

let private random = new Random()

let surroundingBox (box1 : Bbox) (box2 : Bbox) =
    let (box1Min, box1Max) = box1
    let (box2Min, box2Max) = box2
    Vector3d(Math.Min(box1Min.X, box2Min.X), Math.Min(box1Min.Y, box2Min.Y), Math.Min(box1Min.Z, box2Min.Z)),
    Vector3d(Math.Max(box1Max.X, box2Max.X), Math.Max(box1Max.Y, box2Max.Y), Math.Max(box1Max.Z, box2Max.Z))

let rec boundingBox = function
    | Sphere(center, radius, _) ->
        center - Vector3d(radius), center + Vector3d(radius)
    | HitableList(hitables) ->
        let boxInit = Vector3d(Double.MaxValue), Vector3d(Double.MinValue)
        let fold box hitable =
            surroundingBox box <| boundingBox hitable
        Seq.fold fold boxInit hitables
    | BvhNode(_, _, box) -> box

let rec makeBvh hitables =
    let sort axis hitable1 hitable2 =
        let (box1, _) = boundingBox hitable1
        let (box2, _) = boundingBox hitable2
        match axis with
        | 0 -> box1.X - box2.X
        | 1 -> box1.Y - box2.Y
        | _ -> box1.Z - box2.Z
        |> int
    
    let finishConstruction left right =
        let box = surroundingBox (boundingBox left) (boundingBox right)
        BvhNode(left, right, box)

    let axis = random.Next(3)
    let hitables =
        hitables
        |> Seq.sortWith (sort axis)
    match Seq.length hitables with
    | 2 ->
        let left = Seq.item 0 hitables
        let right = Seq.item 1 hitables
        finishConstruction left right
    | 1 ->
        let left = Seq.item 0 hitables
        let right = Seq.item 0 hitables
        finishConstruction left right
    | length ->
        let left = makeBvh (Seq.take (length / 2) hitables)
        let right = makeBvh (Seq.skip (length / 2) hitables)
        finishConstruction left right