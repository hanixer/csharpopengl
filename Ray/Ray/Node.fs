module Node

open System
open OpenTK
open Object
open Transform
open Common

type OctreeNode =
    { Children : OctreeNode []
      Primitives : Primitive list
      Bounds : Bounds.Bounds }

and Primitive =
    | GeometricPrimitive of Object * material : string // * AreaLight
    | TransformedPrimitive of prim : Primitive * primToWorld : Transform * worldToPrim : Transform
    | PrimitiveList of Primitive list
    | OctreeAgregate of OctreeNode

let makeTransformedPrimitive prim primToWorld =
    TransformedPrimitive(prim, primToWorld, inverted primToWorld)

let printVec (v : Vector3d) =
    printf "(%.2f, %.2f, %.2f) " v.X v.Y v.Z

let rec intersect ray primitive =
    match primitive with
    | GeometricPrimitive(object, material) ->
        let hit = Object.intersect ray object
        Option.map (fun (hit : HitInfo) -> { hit with Material = material }) hit
    | TransformedPrimitive(child, primToWorld, worldToPrim) ->
        let rayPrim = Transform.ray worldToPrim ray
        let hit = intersect rayPrim child
        Option.map (Transform.hitInfo primToWorld) hit
    | PrimitiveList prims ->
        let hits = List.map (intersect ray) prims
        tryFindBestHitInfo hits
    | OctreeAgregate(octree) ->
        intersectOctree ray octree

and intersectOctree ray octree =
    // printVec ray.Origin
    // printVec ray.Direction
    // printVec octree.Bounds.PMin
    // printVec octree.Bounds.PMax
    // printfn ""
    if Bounds.hitBoundingBox ray octree.Bounds then
        // printfn "KEY SHOW!!!"
        if Seq.isEmpty octree.Children then
            let hits = List.map (intersect ray) octree.Primitives
            tryFindBestHitInfo hits
        else
            let hits = Seq.map (intersectOctree ray) octree.Children
            tryFindBestHitInfo hits
    else
        None

let samplePointAndNormOnNode node =
    failwith "should be reimplemented with new primitives"
//     match node.Object with
//     | Some(object) ->
//         samplePointAndNormOnObject object
//         |> Option.map
//                (fun (point, norm) ->
//                (transformPoint node.Transform point,
//                 transformNormal node.Transform norm))
//     | _ -> None

let getAreaOfNode node =
    failwith "should be reimplemented with new primitives"
    // match node.Object with
    // | Some(object) -> getAreaOfObject object
    // | _ -> 0.0



let rec worldBounds primitive : Bounds.Bounds =
    match primitive with
    | GeometricPrimitive(object, _) ->
        Object.worldBounds object
    | TransformedPrimitive(prim, primToWorld, _) ->
        let box = worldBounds prim
        Transform.bounds primToWorld box
    | PrimitiveList(prims) ->
        primitivesToBoxes prims
    | OctreeAgregate(octree) -> octree.Bounds

and primitivesToBoxes primitives =
    match primitives with
    | p :: ps ->
        let box = worldBounds p
        let boxes = Seq.map worldBounds ps
        Seq.fold Bounds.union box boxes
    | _ ->
        Bounds.makeBounds Vector3d.Zero Vector3d.Zero

let putPrimitivesInBox primitives box =
    List.filter (fun primitive ->
        let b2 = worldBounds primitive
        let result = Bounds.intersects box b2
        result)
        primitives

let rec makeOctreeHelper box primitives =
    if Seq.length primitives < 10 then
        { Children = Array.Empty()
          Primitives = primitives
          Bounds = box }
    else
        let boxes = Bounds.splitBox box
        let groupedPrims = Array.map (putPrimitivesInBox primitives) boxes
        let makeChild i primitives =
            makeOctreeHelper boxes.[i] primitives
        let children = Array.mapi makeChild groupedPrims
        { Children = children
          Primitives = primitives
          Bounds = box }

let makeOctree primitives =
    let box = primitivesToBoxes primitives
    makeOctreeHelper box primitives
