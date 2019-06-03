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


let makeScene (scene : Scene) =
    let me = TriangleMesh.loadFromFile @"scenes\teapot-low.obj"
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
    let prim = PrimitiveList listTriangles
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
