﻿// Learn more about F# at http://fsharp.org

open System
open OpenTK
open OpenTK.Graphics.OpenGL
open OpenTK.Input
open OpenTK.Graphics.OpenGL
open Render
open System.Threading.Tasks


type Game() =
    /// <summary>Creates a 800x600 window with the specified title.</summary>
    inherit GameWindow(800, 600)

    let canvas = new System.Drawing.Bitmap(800, 600, Drawing.Imaging.PixelFormat.Format32bppArgb)
    let mutable bytes = Array.create 1 (byte 0)
    let width = 400
    let height = 200
    let zoom = 1.0
    let noiseScale = 2.0
    let hitable = 
                    [Sphere(Vector3d(0.0, 2.0, 0.0), 2.0, Lambertian(NoiseTexture(noiseScale)))
                     Sphere(Vector3d(0.0, -1000.0, 0.0), 1000.0, Lambertian(NoiseTexture(noiseScale)))
                    ]
                    |> Seq.ofList
                    |> HitableList 
    let randomScene() =
        let n = 3
        let random = Random()
        [ for a = -11 to 10 do
            for b = -11 to 10 do
                let choose = random.NextDouble()
                let z = (float b) + 0.9 * (random.NextDouble())
                let center = Vector3d(float a + 0.9 * random.NextDouble(), 0.2, z)
                if ((center - Vector3d(4.0, 0.2, 0.0)).Length > 0.9) then
                    if choose < 0.8 then
                        let r = random.NextDouble() * random.NextDouble()
                        let g = random.NextDouble() * random.NextDouble()
                        let b = random.NextDouble() * random.NextDouble()
                        yield Sphere(center, 0.2, Lambertian(ConstantTexture(Vector3d(r, g, b))))
                    else if choose < 0.95 then
                        let r = 0.5 *(1.0 + random.NextDouble())
                        let g = 0.5 *(1.0 + random.NextDouble())
                        let b = 0.5 *(1.0 + random.NextDouble())
                        let fuzzy = 0.5 *(1.0 + random.NextDouble())
                        yield Sphere(center, 0.2, Metal(ConstantTexture(Vector3d(r, g, b)), fuzzy))
                    else
                        yield Sphere(center, 0.2, Dielectric(1.5)) ]
        @ [ Sphere(Vector3d(0.0, -1000.0, 0.0), 1000.0, Lambertian(CheckerTexture((ConstantTexture(Vector3d(0.0, 0.0, 0.0))), ConstantTexture(Vector3d(0.5, 0.5, 0.5)))))
            Sphere(Vector3d(0.0, 1.0, 0.0), 1.0, Dielectric(1.5))
            Sphere(Vector3d(-4.0, 1.0, 0.0), 1.0, Lambertian(ConstantTexture(Vector3d(0.4, 0.2, 0.1))))
            Sphere(Vector3d(4.0, 1.0, 0.0), 1.0, Metal(ConstantTexture(Vector3d(0.7, 0.6, 0.5)), 0.0)) ]
        |> Seq.ofList
    
    // let hitable = makeBvh hitable

    do 
        base.VSync <- VSyncMode.On

     /// <summary>Load resources here.</summary>
     /// <param name="e">Not used.</param>
    override o.OnLoad e =
        base.OnLoad(e)
        GL.ClearColor(0.f, 0.f, 0.f, 0.0f)
        GL.Enable(EnableCap.DepthTest)
        let bitmap = new Drawing.Bitmap(width, height)

        let stopwatch = Diagnostics.Stopwatch.StartNew(); //creates and start the instance of Stopwatch

        Render.mainRender bitmap hitable 90.0
        // subMainRender bitmap

        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);

        Render.drawBitmap bitmap canvas zoom
        bytes <-Rest.getBytesFromBitmap canvas
        bitmap.RotateFlip(Drawing.RotateFlipType.RotateNoneFlipY)
        bitmap.Save("test-images/output.png")

    /// <summary>
    /// Called when your window is resized. Set your viewport here. It is also
    /// a good place to set up your projection matrix (which probably changes
    /// along when the aspect ratio of your window).
    /// </summary>
    /// <param name="e">Not used.</param>
    override o.OnResize e =
        base.OnResize e
        GL.Viewport(base.ClientRectangle.X, base.ClientRectangle.Y, base.ClientRectangle.Width, base.ClientRectangle.Height)
        let mutable projection = Matrix4.CreatePerspectiveFieldOfView(float32 (Math.PI / 4.), float32 base.Width / float32 base.Height, 1.f, 64.f)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(&projection)


    /// <summary>
    /// Called when it is time to setup the next frame. Add you game logic here.
    /// </summary>
    /// <param name="e">Contains timing information for framerate independent logic.</param>
    override o.OnUpdateFrame e =
        base.OnUpdateFrame e       
        if base.Keyboard.[Key.Escape] then base.Close()

    /// <summary>
    /// Called when it is time to render the next frame. Add your rendering code here.
    /// </summary>
    /// <param name="e">Contains timing information.</param>
    override o.OnRenderFrame(e) =
        base.OnRenderFrame e
        GL.Clear(ClearBufferMask.ColorBufferBit ||| ClearBufferMask.DepthBufferBit)
        let mutable modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadMatrix(&modelview)

        GL.DrawPixels(canvas.Width, canvas.Height, PixelFormat.Bgra, PixelType.UnsignedByte, bytes)

        base.SwapBuffers()  

    

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let win = new Game()
    win.Run()
    
    0 // return an integer exit code
