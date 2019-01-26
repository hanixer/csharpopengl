module Bvh

open OpenTK
open System
open Hit
open Common 

let private random = new Random()

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