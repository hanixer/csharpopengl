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
    if Bounds.hitBoundingBox ray octree.Bounds then
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
        List.fold (fun box prim ->
            Bounds.union box (worldBounds prim))
            (Bounds.makeBounds Vector3d.Zero Vector3d.Zero)
            prims
    | OctreeAgregate(octree) -> octree.Bounds

let putPrimitivesInBox primitives box =
    List.filter (fun primitive -> Bounds.intersects box (worldBounds primitive)) primitives

let rec constructOctree box primitives =
    if Seq.length primitives < 5 then
        { Children = Array.Empty()
          Primitives = primitives
          Bounds = box }
    else
        let boxes = Bounds.splitBox box
        let groupedPrims = Array.map (putPrimitivesInBox primitives) boxes
        let children = Array.mapi (fun i primitives -> constructOctree boxes.[i] primitives) groupedPrims
        { Children = children
          Primitives = primitives
          Bounds = box }