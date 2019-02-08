// Learn more about F# at http://fsharp.org

open System

open System.Xml
open Scene
open Window
open Render

type Window1(width, height) =
    inherit Window(width, height)

    override o.OnUpdateFrame e =
        base.OnUpdateFrame e

[<EntryPoint>]
let main argv =
    let scene = loadSceneFromFile "test2.xml"    
    let bitmap = new Drawing.Bitmap(scene.Camera.Width, scene.Camera.Height)
    let zbuffer = Array2D.create scene.Camera.Height scene.Camera.Width 0.0
    render bitmap zbuffer scene
    let zbitmap = drawZBuffer zbuffer
    let va = zbuffer.[300, 400]
    let win = new Window(800, 600)
    win.DrawBitmapAndSave bitmap
    win.Run()
    0 // return an integer exit code
