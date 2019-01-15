// Learn more about F# at http://fsharp.org

open System
open OpenTK
open OpenTK.Graphics.OpenGL
open OpenTK.Input
open OpenTK.Graphics.OpenGL

module Helper =
    let getBytesFromBitmap (bitmap: System.Drawing.Bitmap) =
        let data = bitmap.LockBits(System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), 
                                   System.Drawing.Imaging.ImageLockMode.ReadWrite, 
                                   bitmap.PixelFormat)
        let size = data.Stride * data.Height
        let bytes = Array.create size (byte 0)
        for i = 0 to 2 do
            for j = 0 to 3 do
                printf "%x " bytes.[i * 4 + j]
            printfn ""
        System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bytes, 0, size)
        for i = 0 to 2 do
            for j = 0 to 3 do
                printf "%x " bytes.[i * 4 + j]
            printfn ""
        bitmap.UnlockBits(data)
        bytes

    let colorToVector (color : Drawing.Color) =
        Vector3d(float color.R / 255.0, float color.G / 255.0, float color.B / 255.0)

type Game() =
    /// <summary>Creates a 800x600 window with the specified title.</summary>
    inherit GameWindow(800, 600)

    let canvas = new System.Drawing.Bitmap(800, 600, Drawing.Imaging.PixelFormat.Format32bppArgb)
    let mutable bytes = Array.create 1 (byte 0)
    let sphereCenter = Vector3d(0.0, 0.0, -5.0)
    let sphereRadius = 0.5
    let sphereColor = Helper.colorToVector Drawing.Color.White

    do 
        base.VSync <- VSyncMode.On

     /// <summary>Load resources here.</summary>
     /// <param name="e">Not used.</param>
    override o.OnLoad e =
        base.OnLoad(e)
        GL.ClearColor(0.f, 0.f, 0.f, 0.0f)
        GL.Enable(EnableCap.DepthTest)
        let bitmap = new Drawing.Bitmap(100, 100)
        
        Render.renderSphereWithRays bitmap sphereCenter sphereRadius sphereColor

        Render.drawBitmap bitmap canvas 5.0
        bytes <-Helper.getBytesFromBitmap canvas

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
