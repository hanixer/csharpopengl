// Learn more about F# at http://fsharp.org
open System
open System.Xml
open Scene
open Window
open Render
open System
open OpenTK
open OpenTK.Input
open Object
open Transform
open Light
open Common
open Material
open Node
open ObjLoader.Loader.Loaders
open System.IO

let measure task =
    let stopwatch = Diagnostics.Stopwatch.StartNew() //creates and start the instance of Stopwatch
    Async.RunSynchronously task
    stopwatch.Stop()
    Console.WriteLine(stopwatch.ElapsedMilliseconds)

let file = @"scenes\old\oneSphere.xml"

let makeSphere m x y z =
    let off = Vector3d(float x,float y,float z)
    makeTransformedPrimitive (GeometricPrimitive(Object.Sphere, m)) (Transform.translate off)

let makeSpheres m =
    makeSphere m 1 1 1 ::
    [ for x = 0 to 1 do
        for y = 0 to 1 do
            for z = 0 to 1 do
                yield makeSphere m (x * 3) (y * 3) (z * 3) ]

let makeScene (scene : Scene) =
    let me = TriangleMesh.loadFromFile @"scenes\sphere.obj"
    printfn "mesh faces = %d" me.FacesCount
    let m = Seq.head scene.Materials
    let mname = m.Key
    let listTriangles =
        (Seq.map (Object.Triangle >> (fun i -> GeometricPrimitive(i, mname))) (TriangleMesh.meshToList me))
        |> Seq.toList
    let p0 = GeometricPrimitive(Object.Sphere, mname)
    let p1 = makeTransformedPrimitive p0 (scale (Vector3d(0.25)))
    let p2 = makeTransformedPrimitive p0 (translate (Vector3d(2., 0., 0.)))
    let p3 = makeTransformedPrimitive p1 (translate (Vector3d(2., 2., 0.)))
    let p4 = makeTransformedPrimitive p1 (translate (Vector3d(0., 2., 0.)))
    let tri = Object.Triangle(Vector3d.Zero, Vector3d(1., 0., 0.), Vector3d(1., 1., 0.))
    let p5 = GeometricPrimitive(tri, mname)
    let p6 = makeTransformedPrimitive p5 (translate (Vector3d(0., 0., 1.)))
    let p7 = makeTransformedPrimitive p5 (translate (Vector3d(0., 0., 0.5)))
    let prim = OctreeAgregate(Node.makeOctree [p5; p6; p7])
    // let prim = PrimitiveList listTriangles
    let prim = OctreeAgregate(Node.makeOctree listTriangles)
    let sphs = makeSpheres mname
    let tris = [
        Object.Triangle(Vector3d(1.378399968, -1.378399968, 0.01090000011),Vector3d(1.152999997, -1.571099997, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        Object.Triangle(Vector3d(1.152999997, -1.571099997, 0.01090000011),Vector3d(0.8988000154, -1.727499962, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        Object.Triangle(Vector3d(0.8988000154, -1.727499962, 0.01090000011),Vector3d(0.6194000244, -1.843799949, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        // Object.Triangle(Vector3d(0.6194000244, -1.843799949, 0.01090000011),Vector3d(0.3185000122, -1.916399956, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        // Object.Triangle(Vector3d(0.3185000122, -1.916399956, 0.01090000011),Vector3d(0.0, -1.941400051, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        // Object.Triangle(Vector3d(0.0, -1.941400051, 0.01090000011),Vector3d(-0.3185000122, -1.916399956, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        // Object.Triangle(Vector3d(-0.3185000122, -1.916399956, 0.01090000011),Vector3d(-0.6194000244, -1.843799949, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        // Object.Triangle(Vector3d(-0.6194000244, -1.843799949, 0.01090000011),Vector3d(-0.8988000154, -1.727499962, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        // Object.Triangle(Vector3d(-0.8988000154, -1.727499962, 0.01090000011),Vector3d(-1.152999997, -1.571099997, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        // Object.Triangle(Vector3d(-1.152999997, -1.571099997, 0.01090000011),Vector3d(-1.378399968, -1.378399968, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
        // Object.Triangle(Vector3d(-1.378399968, -1.378399968, 0.01090000011),Vector3d(-1.571099997, -1.152999997, 0.01090000011),Vector3d(0.0, 0.0, 0.0))
    ]
    // let prims = List.map (fun j -> GeometricPrimitive(j, mname)) tris
    // let prim = OctreeAgregate(Node.makeOctree prims)
    // let sphs = [makeSphere mname 0 0 0]
    // let prim = OctreeAgregate(Node.makeOctree sphs)
    // let prim = PrimitiveList sphs
    { scene with Primitive = prim }

type Window1(width, height) =
    inherit Window(width, height)
    let mutable bitmap : Drawing.Bitmap = null
    let mutable zbitmap : Drawing.Bitmap = null
    let mutable isZ = false

    member this.Update() =
        let scene = loadSceneFromFile file
        let scene = makeScene scene
        let zbuffer = Array2D.create scene.Camera.Height scene.Camera.Width 0.0
        bitmap <- new Drawing.Bitmap(scene.Camera.Width, scene.Camera.Height)
        async { render bitmap zbuffer scene } |> measure
        isZ <- false
        this.DrawBitmapAndSave bitmap
        this.Close()

    override this.OnUpdateFrame e =
        base.OnUpdateFrame e
        if base.Keyboard.[Key.Z] then
            this.DrawBitmapAndSave(if isZ then bitmap
                                   else zbitmap)
            isZ <- not isZ
        if base.Keyboard.[Key.F5] then this.Update()

[<EntryPoint>]
let main argv =
    let win = new Window1(800, 600)
    win.Update()
    win.Run()
    0 // return an integer exit code
