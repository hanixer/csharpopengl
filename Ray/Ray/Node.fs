module Node

open System
open OpenTK
open Object
open Transform
open Common
open Types

let makeGeometricPrimitive o m = GeometricPrimitive(o, m, None)
let makeTransformedPrimitive prim primToWorld = TransformedPrimitive(prim, primToWorld, inverted primToWorld)
let printVec (v : Vector3d) = printf "(%.2f, %.2f, %.2f) " v.X v.Y v.Z

let worldBoundsBVH bvh =
    match bvh with
    | BVHLeaf(_, bounds) -> bounds
    | BVHInterior(_, _, bounds) -> bounds

// returns bounding box of the primitive in world space
let rec worldBounds primitive : Bounds =
    match primitive with
    | GeometricPrimitive(object, _, _) -> Object.worldBounds object
    | TransformedPrimitive(prim, primToWorld, _) ->
        let box = worldBounds prim
        Transform.bounds primToWorld box
    | PrimitiveList(prims) -> primitivesToBoxes prims
    | OctreeAgregate(octree) -> octree.Bounds
    | BVHAccelerator(bvh) -> worldBoundsBVH bvh

and primitivesToBoxes (primitives : Primitive seq) = Bounds.unionMany (Seq.map worldBounds primitives)

let rec intersect ray primitive =
    match primitive with
    | GeometricPrimitive(object, material, areaLight) ->
        let hit = Object.intersect ray object
        Option.map (fun (hit : HitInfo) -> { hit with Prim = Some primitive }) hit
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

type PrimToBox = Collections.Generic.Dictionary<Primitive, Bounds>

let putPrimitivesInBox primitives (primToBox : PrimToBox) box =
    List.filter (fun primitive ->
        let b2 = primToBox.[primitive]
        let result = Bounds.intersects box b2
        result) primitives

let splitPrimsWithFullCover (box : Bounds) prims (primToBox : PrimToBox) =
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

let rec primitivesSet node =
    let res = Collections.Generic.HashSet<Primitive>()
    for c in node.Children do
        res.UnionWith(primitivesSet c)
    res.UnionWith(node.Primitives)
    res

let rec makeOctreeHelper maxDepth depth (box : Bounds) primitives primToBox =
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
    let m = Collections.Generic.Dictionary<Primitive, Bounds>()
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
type BVHPrimInfoMap = Collections.Generic.Dictionary<Primitive, Bounds * Vector3d>

type PrimInfo =
    { Prim : Primitive
      Bounds : Bounds
      Centroid : Vector3d }

let makeLeafBVH prim bounds = BVHLeaf(prim, bounds)
let makeInteriorBVH left right bounds = BVHInterior(left, right, bounds)

let centroidBounds (prims : PrimInfo []) =
    let centroids = Seq.map (fun p -> p.Centroid) prims
    Bounds.unionManyP centroids

let rec makeBHVHelper (prims : PrimInfo []) =
    let axisComparisons =
        [| fun (p : PrimInfo) -> p.Centroid.X
           fun (p : PrimInfo) -> p.Centroid.Y
           fun (p : PrimInfo) -> p.Centroid.Z |]

    let n = prims.Length
    let bounds = Bounds.unionMany (Seq.map (fun prim -> prim.Bounds) prims)
    if n = 1 then makeLeafBVH prims.[0].Prim bounds
    else
        let centroidBounds = centroidBounds prims
        let axis = Bounds.maximumExtent centroidBounds
        Array.sortInPlaceBy axisComparisons.[axis] prims
        let left, right = Array.splitAt (n / 2) prims
        makeInteriorBVH (makeBHVHelper left) (makeBHVHelper right) bounds

let rec setOfPrims bvh =
    match bvh with
    | BVHLeaf(i, _) -> [ i ]
    | BVHInterior(l, r, _) -> List.append (setOfPrims l) (setOfPrims r)

let makePrimInfos primitives =
    let handle prim =
        let bounds = worldBounds prim
        { Prim = prim
          Bounds = bounds
          Centroid = Bounds.centroid bounds }
    Seq.map handle primitives |> Seq.toArray

let makeBVH primitives =
    if Seq.length primitives = 1 then Seq.head primitives
    else
        let stopwatch = Diagnostics.Stopwatch.StartNew() //creates and start the instance of Stopwatch
        let primInfos = makePrimInfos primitives
        let bvh = makeBHVHelper primInfos
        let result = BVHAccelerator(bvh)
        stopwatch.Stop()
        Console.WriteLine("BVH construction: {0}", stopwatch.ElapsedMilliseconds)
        result

let emitted prim =
    match prim with
    | GeometricPrimitive(_, _, Some(light)) -> Light.emitted light
    | _ -> Vector3d.Zero

let getMaterial prim =
    match prim with
    | GeometricPrimitive(_, m, _) -> m
    | _ -> failwith "material is available only for geo prim"