module Node

open System
open OpenTK
open Object
open Transform
open Common

type Primitive =
    | GeometricPrimitive of Object * material : string // * AreaLight
    | TransformedPrimitive of prim : Primitive * primToWorld : Transform * worldToPrim : Transform
    | PrimitiveList of Primitive list

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
        let hitInfos = List.map (intersect ray) prims
        tryFindBestHitInfo hitInfos

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
