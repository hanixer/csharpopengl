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

let maxDepth = 2

let tryFindBestHitInfo hitInfos =
    let fold best hitInfo =
        match (best, hitInfo) with
        | Some b, Some h ->  if h.T < b.T then hitInfo else best
        | _, Some _ -> hitInfo       
        | _ -> best
    let result = Seq.fold fold None hitInfos
    result    

let hitInfoToWorld ray node depth hitInfo =
    let point = transformPoint node.Transform hitInfo.Point
    let normal = transformNormal node.Transform hitInfo.Normal
    let t = (point - ray.Origin).Length
    let mat = if hitInfo.Material.Length > 0 then hitInfo.Material else node.Material
    {hitInfo with Point = point; Normal = normal.Normalized(); T = t; Material = mat; Depth = depth}

let rec intersectNodes ray nodes tMin depth =
    if depth > maxDepth then
        None
    else
        Seq.map (fun node -> intersectNode ray node tMin depth) nodes
        |> tryFindBestHitInfo
    
and intersectNode ray node tMin depth =
    let rayLocal = {Origin = transformPointInv node.Transform ray.Origin; Direction = (transformVector node.Transform.Inv ray.Direction).Normalized()}
    let hitInfoChilds = intersectNodes rayLocal node.Children tMin depth
    let hitInfo =
        if Option.isSome node.Object then
            tryFindBestHitInfo [
                hitInfoChilds
                intersect rayLocal node.Object.Value tMin node.Material
            ]
        else
            hitInfoChilds
    Option.map (hitInfoToWorld ray node depth) hitInfo
