module Node

open System
open OpenTK
open Object
open Transform
open Common

type Node = {
    Name : string
    Object : Object option
    Children : Node list
    Transform : Transform
    Material : string
}

let tryFindBestHitInfo hitInfos =
    let fold best hitInfo =
        match (best, hitInfo) with
        | Some b, Some h ->  if h.T < b.T then hitInfo else best
        | _, Some _ -> hitInfo       
        | _ -> best
    let result = Seq.fold fold None hitInfos
    result    

let hitInfoToWorld ray node hitInfo =
    let point = transformPoint node.Transform hitInfo.Point
    let normal = transformNormal node.Transform hitInfo.Normal
    let t = (point - ray.Origin).Length
    let mat = if hitInfo.Material.Length > 0 then hitInfo.Material else node.Material
    {hitInfo with Point = point; Normal = normal.Normalized(); T = t; Material = mat;}

let rec intersectNodes ray nodes tMin =
        Seq.map (fun node -> intersectNode ray node tMin) nodes
        |> tryFindBestHitInfo
    
and intersectNode ray node tMin =
    let rayLocal = {Origin = transformPointInv node.Transform ray.Origin; Direction = (transformVector node.Transform.Inv ray.Direction).Normalized()}
    let hitInfoChilds = intersectNodes rayLocal node.Children tMin
    let hitInfo =
        if Option.isSome node.Object then
            tryFindBestHitInfo [
                hitInfoChilds
                intersect rayLocal node.Object.Value tMin node.Material
            ]
        else
            hitInfoChilds
    Option.map (hitInfoToWorld ray node) hitInfo

let samplePointAndNormOnNode node =
    match node.Object with
    | Some(object) ->
        samplePointAndNormOnObject object
        |> Option.map (fun (point, norm) ->
            (transformPoint node.Transform point, transformNormal node.Transform norm))
    | _ ->
        None

let transformArea tm area =
    let diag = tm.M.Diagonal
    area * diag.X * diag.Y * diag.Z

let getAreaOfNode node =
    match node.Object with
    | Some(object) ->
        getAreaOfObject object
    | _ ->
        0.0


let getBoundingBox node =
    1