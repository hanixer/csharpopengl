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

type Node =
    { Name : string
      Object : Object option
      Children : Node list
      Transform : Transform
      Material : string }

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

let hitInfoToWorld ray node hitInfo =
    let point = transformPoint node.Transform hitInfo.Point
    let normal = transformNormal node.Transform hitInfo.Normal
    let t = (point - ray.Origin).Length

    let mat =
        if hitInfo.Material.Length > 0 then hitInfo.Material
        else node.Material
    { hitInfo with Point = point
                   Normal = normal.Normalized()
                   T = t
                   Material = mat }

let rec intersectNodes ray nodes =
    Seq.map (fun node -> intersectNode ray node) nodes |> tryFindBestHitInfo

and intersectNode ray node =
    let rayLocal =
        { ray with Origin = transformPointInv node.Transform ray.Origin
                   Direction =
                       (transformVector node.Transform.Inv ray.Direction)
                           .Normalized() }

    let hitInfoChilds = intersectNodes rayLocal node.Children

    let hitInfo =
        if Option.isSome node.Object then
            tryFindBestHitInfo [ hitInfoChilds
                                 Object.intersect rayLocal node.Object.Value ]
        else hitInfoChilds
    Option.map (hitInfoToWorld ray node) hitInfo

let samplePointAndNormOnNode node =
    match node.Object with
    | Some(object) ->
        samplePointAndNormOnObject object
        |> Option.map
               (fun (point, norm) ->
               (transformPoint node.Transform point,
                transformNormal node.Transform norm))
    | _ -> None

let transformArea tm area =
    let diag = tm.M.Diagonal
    area * diag.X * diag.Y * diag.Z

let getAreaOfNode node =
    match node.Object with
    | Some(object) -> getAreaOfObject object
    | _ -> 0.0

let getBoundingBox node = 1
