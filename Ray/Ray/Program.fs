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

let file = @"scenes\old\test3.xml"

let node =
    let transform = Vector3d(0.07, 3.0, 0.07) |> scale
    { Node.Name = ""
      Node.Children = []
      Node.Object = Some Object.Cylinder
      Node.Transform = transform
      Material = "white" }

type Window1(width, height) =
    inherit Window(width, height)
    let mutable bitmap : Drawing.Bitmap = null
    let mutable zbitmap : Drawing.Bitmap = null
    let mutable isZ = false

    member this.Update() =
        let scene = loadSceneFromFile file
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
