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

let drawLine (g : Drawing.Graphics) (p1 : Vector2) (p2 : Vector2) =
    g.DrawLine(Drawing.Pens.White, Drawing.PointF(p1.X, p1.Y), Drawing.PointF(p2.X, p2.Y))

let drawnBitmap =
    let bmp = new Drawing.Bitmap(500, 500)
    let g = Drawing.Graphics.FromImage bmp
    // g.DrawLine(Drawing.Pens.Azure, 0, 0, 50, 50)
    let a = Vector2.Zero
    let b = Vector2(50.0f, 50.0f)
    let c = Vector2(100.0f, 0.0f)
    let n = Vector2(-50.0f, 50.0f)
    let ofs = Vector2(50.0f, 0.0f)
    let scale = 0.5f
    drawLine g a b
    drawLine g b c
    drawLine g a c
    drawLine g ofs (n + ofs)

    g.Flush()
    bmp

let measure task =
    let stopwatch = Diagnostics.Stopwatch.StartNew(); //creates and start the instance of Stopwatch
    Async.RunSynchronously task
    stopwatch.Stop();
    Console.WriteLine(stopwatch.ElapsedMilliseconds);

let file = @"scenes\old\oneSphere.xml"

let node =
    let transform = Vector3d(0.07, 3.0, 0.07) |> scale
    {Node.Name=""; Node.Children=[]; Node.Object = Some Object.Cylinder; Node.Transform = transform; Material = "white"}

type Window1(width, height) =
    inherit Window(width, height)

    let mutable bitmap : Drawing.Bitmap = null
    let mutable zbitmap : Drawing.Bitmap = null
    let mutable isZ = false

    member this.Update() =
        let scene = loadSceneFromFile file
        addNode scene node
        let zbuffer =  Array2D.create scene.Camera.Height scene.Camera.Width 0.0
        bitmap <- new Drawing.Bitmap(scene.Camera.Width, scene.Camera.Height)
        async { render bitmap zbuffer scene } |> measure
        isZ <- false
        this.DrawBitmapAndSave bitmap

    override this.OnUpdateFrame e =
        base.OnUpdateFrame e
        if base.Keyboard.[Key.Z] then
            this.DrawBitmapAndSave (if isZ then bitmap else zbitmap)
            isZ <- not isZ
        if base.Keyboard.[Key.F5] then
            this.Update()



[<EntryPoint>]
let main argv =
    // let win = new Window1(800, 600)
    // win.Update()
    // win.Run()

    let p0 = Vector3d(3., 0., -2.)
    let p1 = Vector3d(3., 5., -2.)
    let p2 = Vector3d(-1., 3., -3.)
    let pers = perspective 90. 1e-4 1e4

    let p0 = transformPoint pers p0
    let p1 = transformPoint pers p1
    let p2 = transformPoint pers p2

    printfn "%A %A %A" p0 p1 p2

    use img = new System.Drawing.Bitmap(800, 600)
    use g = System.Drawing.Graphics.FromImage(img)


    0 // return an integer exit code
