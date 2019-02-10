// Learn more about F# at http://fsharp.org

open System

open System.Xml
open Scene
open Window
open Render
open System
open OpenTK

type Window1(width, height) =
    inherit Window(width, height)

    override o.OnUpdateFrame e =
        base.OnUpdateFrame e

let drawLine (g : Drawing.Graphics) (p1 : Vector2) (p2 : Vector2) =
    g.DrawLine(Drawing.Pens.White, new Drawing.PointF(p1.X, p1.Y), new Drawing.PointF(p2.X, p2.Y))    

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

[<EntryPoint>]
let main argv =
    let scene = loadSceneFromFile "test3.xml"    
    let bitmap = new Drawing.Bitmap(scene.Camera.Width, scene.Camera.Height)
    let zbuffer = Array2D.create scene.Camera.Height scene.Camera.Width 0.0
    render bitmap zbuffer scene
    let zbitmap = drawZBuffer zbuffer
    let va = zbuffer.[300, 400]
    let win = new Window(800, 600)
    win.DrawBitmapAndSave bitmap
    win.Run()
    0 // return an integer exit code
