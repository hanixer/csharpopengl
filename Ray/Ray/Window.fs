module Window

open System
open OpenTK
open OpenTK.Input
open OpenTK.Graphics.OpenGL

type Window(width, height) =
    inherit GameWindow(width, height)

    let canvas = new Drawing.Bitmap(width, height, Drawing.Imaging.PixelFormat.Format32bppArgb)
    let mutable bytes = Array.create (width * height * 4) (byte 0)

    let update() =
        let stopwatch = Diagnostics.Stopwatch.StartNew(); //creates and start the instance of Stopwatch

        // Render.mainRender bitmap settings hitable

        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);

    do 
        base.VSync <- VSyncMode.On

    member this.DrawBitmapAndSave bitmap =
        let zoom = 1.0
        Common.drawBitmapOnBitmap bitmap canvas zoom
        bytes <- Common.getBytesFromBitmap canvas
        bitmap.RotateFlip(Drawing.RotateFlipType.RotateNoneFlipY)        
        if System.IO.Directory.Exists "test-images" |> not then
            System.IO.Directory.CreateDirectory "test-images" |> ignore
        bitmap.Save("test-images/output.png")


     /// <summary>Load resources here.</summary>
     /// <param name="e">Not used.</param>
    override o.OnLoad e =
        base.OnLoad(e)
        GL.ClearColor(0.f, 0.f, 0.f, 0.0f)
        GL.Enable(EnableCap.DepthTest)
        update()

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
