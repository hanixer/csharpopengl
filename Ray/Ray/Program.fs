// Learn more about F# at http://fsharp.org
open System
open Scene
open Window
open Render
open OpenTK
open OpenTK.Input
open Node
open Parser
open Types
open Object

let measure task =
    let stopwatch = Diagnostics.Stopwatch.StartNew() //creates and start the instance of Stopwatch
    Async.RunSynchronously task
    stopwatch.Stop()
    Console.WriteLine(stopwatch.ElapsedMilliseconds)

let file = @"scenes\scene.1.xml"

let makeSphere m x y z =
    let off = Vector3d(float x,float y,float z)
    makeTransformedPrimitive (makeGeometricPrimitive Object.Sphere m) (Transform.translate off)

let makeSpheres m =
    makeSphere m 1 1 1 ::
    [ for x = 0 to 1 do
        for y = 0 to 1 do
            for z = 0 to 1 do
                yield makeSphere m (x * 3) (y * 3) (z * 3) ]

let makeScene (scene : Scene) =
    let m = Seq.head scene.Materials
    let mname = m.Key
    let numSamples = 10000
    let sampler = Sampling.makeSampler 1
    let sphere = makeGeometricPrimitive Object.Sphere (Blinn {Material.defaultBlinn with DiffuseColor = Vector3d.UnitX * 0.3})
    let spheres =
        Seq.init numSamples (fun i ->
            let sample = Sampling.next2D sampler
            let cone = Sampling.squareToCone sample (Math.Cos(Math.PI / 3.))
            let s = Transform.scale (Vector3d(0.01))
            let t = Transform.translate (Vector3d(cone.X, cone.Z, cone.Y))
            makeTransformedPrimitive sphere (Transform.compose t s))

    let bvh = makeBVH (Seq.append [scene.Primitive] spheres)
    { scene with Primitive = bvh }

type Window1(width, height) =
    inherit Window(width, height)
    let mutable bitmap : Drawing.Bitmap = null
    let mutable zbitmap : Drawing.Bitmap = null
    let mutable isZ = false

    member this.Update() =
        let scene, integrator = loadSceneAndIntegratorFromFile file
        let scene = makeScene scene
        let zbuffer = Array2D.create scene.Camera.Height scene.Camera.Width 0.0
        bitmap <- new Drawing.Bitmap(scene.Camera.Width, scene.Camera.Height)
        async { render bitmap scene integrator } |> measure
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
