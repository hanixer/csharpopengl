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

and BVHNode =
    | BVHLeaf of prim : Primitive * bounds : Bounds.Bounds
    | BVHInterior of left : BVHNode * right : BVHNode * bounds : Bounds.Bounds

and Primitive =
    | GeometricPrimitive of Object * material : string // * AreaLight
    | TransformedPrimitive of prim : Primitive * primToWorld : Transform * worldToPrim : Transform
    | PrimitiveList of Primitive list
    | OctreeAgregate of OctreeNode
    | BVHAccelerator of BVHNode

let makeTransformedPrimitive prim primToWorld = TransformedPrimitive(prim, primToWorld, inverted primToWorld)
let printVec (v : Vector3d) = printf "(%.2f, %.2f, %.2f) " v.X v.Y v.Z

let worldBoundsBVH bvh =
    match bvh with
    | BVHLeaf(_, bounds) -> bounds
    | BVHInterior(_, _, bounds) -> bounds

// returns bounding box of the primitive in world space
let rec worldBounds primitive : Bounds.Bounds =
    match primitive with
    | GeometricPrimitive(object, _) -> Object.worldBounds object
    | TransformedPrimitive(prim, primToWorld, _) ->
        let box = worldBounds prim
        Transform.bounds primToWorld box
    | PrimitiveList(prims) -> primitivesToBoxes prims
    | OctreeAgregate(octree) -> octree.Bounds
    | BVHAccelerator(bvh) -> worldBoundsBVH bvh

and primitivesToBoxes (primitives : Primitive seq) =
    Bounds.unionMany (Seq.map worldBounds primitives)

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
    | OctreeAgregate(octree) -> intersectOctree ray octree
    | BVHAccelerator(bvh) -> intersectBVH ray bvh

and intersectOctree ray octree =
    if Bounds.hitBoundingBox ray octree.Bounds then
        let hits1 = Seq.map (intersectOctree ray) octree.Children
        let hits2 = List.map (intersect ray) octree.Primitives
        tryFindBestHitInfo [ tryFindBestHitInfo hits1
                             tryFindBestHitInfo hits2 ]
    else None

and intersectBVH ray bvh =
    if Bounds.hitBoundingBox ray (worldBoundsBVH bvh) then
        match bvh with
        | BVHLeaf(prim, _) -> intersect ray prim
        | BVHInterior(left, right, _) ->
            tryFindBestHitInfo [ intersectBVH ray left
                                 intersectBVH ray right ]
    else None

let samplePointAndNormOnNode node = failwith "should be reimplemented with new primitives"
//     match node.Object with
//     | Some(object) ->
//         samplePointAndNormOnObject object
//         |> Option.map
//                (fun (point, norm) ->
//                (transformPoint node.Transform point,
//                 transformNormal node.Transform norm))
//     | _ -> None

let getAreaOfNode node = failwith "should be reimplemented with new primitives"

type PrimToBox = Collections.Generic.Dictionary<Primitive, Bounds.Bounds>

let putPrimitivesInBox primitives (primToBox : PrimToBox) box =
    List.filter (fun primitive ->
        let b2 = primToBox.[primitive]
        let result = Bounds.intersects box b2
        result) primitives

let splitPrimsWithFullCover (box : Bounds.Bounds) prims (primToBox : PrimToBox) =
    let fold (full, notFull) prim =
        let boxPrim = primToBox.[prim]
        // printf "boxPrim ="
        // printVec boxPrim.PMin
        // printVec boxPrim.PMax
        // printf "; box ="
        // printVec box.PMin
        // printVec box.PMax
        // printfn ""
        if Bounds.contains boxPrim box then (prim :: full, notFull)
        else (full, prim :: notFull)
    List.fold fold ([], []) prims

let rec printTriangle p =
    match p with
    | GeometricPrimitive(Object.Triangle(p0, p1, p2), _) ->
        printf "Object.Triangle("
        printf "Vector3d(%A, %A, %A)," p0.X p0.Y p0.Z
        printf "Vector3d(%A, %A, %A)," p1.X p1.Y p1.Z
        printf "Vector3d(%A, %A, %A)" p2.X p2.Y p2.Z
        printfn ")"
    | _ -> ()

let rec primitivesSet node =
    let res = Collections.Generic.HashSet<Primitive>()
    for c in node.Children do
        res.UnionWith(primitivesSet c)
    res.UnionWith(node.Primitives)
    res

let rec makeOctreeHelper maxDepth depth (box : Bounds.Bounds) primitives primToBox =
    let vol = Bounds.volume box
    // printf "makeOctreeHelper %d " (Seq.length primitives)
    // printVec box.PMin
    // printf " %A" vol
    if Seq.length primitives < 10 || depth >= maxDepth then
        { // printfn " less!"
          Children = Array.Empty()
          Primitives = primitives
          Bounds = box }
    else
        // let (fullCover, notFullCover) = splitPrimsWithFullCover box primitives primToBox
        // printfn " full = %d; not = %d " fullCover.Length notFullCover.Length
        let boxes = Bounds.splitBox box
        let groupedPrims = Array.map (putPrimitivesInBox primitives primToBox) boxes
        let hs = Collections.Generic.HashSet<Primitive>()
        for gr in groupedPrims do
            hs.UnionWith(gr)
        System.Diagnostics.Debug.Assert(hs.Count = primitives.Length)
        let makeChild i primitives = makeOctreeHelper maxDepth (depth + 1) boxes.[i] primitives primToBox
        let children = Array.mapi makeChild groupedPrims
        { Children = children
          Primitives = []
          Bounds = box }

let makeOctree primitives =
    let m = Collections.Generic.Dictionary<Primitive, Bounds.Bounds>()
    for p in primitives do
        m.Add(p, worldBounds p)
    let box = primitivesToBoxes primitives
    let maxDepth = 8 + (int (Math.Log(float (Seq.length primitives))))
    let result = makeOctreeHelper maxDepth 0 box primitives m
    let primset = primitivesSet result
    printfn "set %d origin %d" primset.Count primitives.Length
    result

////////////////////////////////////////////////////////////////////////////
/// BVH
type BVHPrimInfoMap = Collections.Generic.Dictionary<Primitive, Bounds.Bounds * Vector3d>

let makeLeafBVH prim bounds = BVHLeaf(prim, bounds)

let makeInteriorBVH left right bounds = BVHInterior(left, right, bounds)

let centroidBounds (primInfos : BVHPrimInfoMap) (prims : Primitive []) =
    let fold bounds prim = Bounds.union bounds (fst primInfos.[prim])
    Seq.fold fold (fst primInfos.[prims.[0]]) prims

let rec makeBHVHelper (primInfos : BVHPrimInfoMap) (prims : Primitive []) =
    let axisComparisons =
        [| fun (p : Primitive) -> (snd primInfos.[p]).X
           fun (p : Primitive) -> (snd primInfos.[p]).Y
           fun (p : Primitive) -> (snd primInfos.[p]).Z |]
    let n = prims.Length
    let bounds = Bounds.unionMany (Seq.map (fun prim -> fst primInfos.[prim]) prims)
    if n = 1 then makeLeafBVH prims.[0] bounds
    else
        let centroidBounds = centroidBounds primInfos prims
        let axis = Bounds.maximumExtent centroidBounds
        Array.sortInPlaceBy axisComparisons.[axis] prims
        let left, right = Array.splitAt (n / 2) prims
        makeInteriorBVH (makeBHVHelper primInfos left) (makeBHVHelper primInfos right) bounds

let rec setOfPrims bvh =
    match bvh with
    | BVHLeaf(i, _) -> [i]
    | BVHInterior(l, r, _) -> List.append (setOfPrims l) (setOfPrims r)

let makeBVH primitives =
    let primInfos = BVHPrimInfoMap()
    for prim in primitives do
        let bounds = worldBounds prim
        let centroid = Bounds.centroid bounds
        primInfos.Add(prim, (bounds, centroid))
    let bvh = makeBHVHelper primInfos (Array.ofSeq primitives)
    let result = BVHAccelerator(bvh)
    result
