// Learn more about F# at http://fsharp.org

open System
open OpenTK
open OpenTK.Graphics.OpenGL
open OpenTK.Input
open Render
open Hit
open Material
open Texture

let cornellBox =
    let red = Lambertian(ConstantTexture(Vector3d(0.65, 0.05, 0.05)))
    let white = Lambertian(ConstantTexture(Vector3d(0.73)))
    let green = Lambertian(ConstantTexture(Vector3d(0.12, 0.45, 0.15)))
    let light = DiffuseLight(ConstantTexture(Vector3d(15.0)))
    let box1 = makeBox Vector3d.Zero (Vector3d(165.0)) white
    let box2 = makeBox Vector3d.Zero (Vector3d(165.0, 330.0, -165.0)) white
    [
        YzRect(0.0, 555.0, -555.0, 0.0, 0.0, red)
        FlipNormals(YzRect(0.0, 555.0, -555.0, 0.0, 555.0, green))
        XzRect(213.0, 343.0, -332.0, -227.0, 554.0, light)
        FlipNormals(XzRect(0.0, 555.0, -555.0, 0.0, 555.0, white))
        XzRect(0.0, 555.0, -555.0, 0.0, 0.0, white)
        XyRect(0.0, 555.0, 0.0, 555.0, -555.0, white)
        // makeBox Vector3d.Zero (Vector3d(165.0)) white
        Translate(box1, Vector3d(265.0, 0.0, -260.0))
        makeRotate box2 RY 30.0
    ]

let standardScene = 
    let texBitmap = new Bitmap("earth.jpg")
    let materialBitmap = Lambertian(textureFromBitmap texBitmap)
    let simpleMat = Lambertian(ConstantTexture(Vector3d(1.0, 0.5, 0.0)))
    let iii = [
        for x = 0 to 5 do
                yield Sphere(Vector3d(float x, 1.0, 0.0), 0.5, simpleMat)
        for z = 0 to 5 do
                yield Sphere(Vector3d(0.0, 1.0, float z), 0.5, simpleMat)
    ]
    let noiseScale = 1.0
    let noiseMat = Lambertian(NoiseTexture(noiseScale))
    [
        Sphere(Vector3d(0.0, 2.0, 0.0), 2.0, noiseMat)
        Sphere(Vector3d(0.0, 7.0, 0.0), 1.0, DiffuseLight(ConstantTexture(Vector3d(4.0))))
        Sphere(Vector3d(2.0, 0.5, 2.0), 0.5, Lambertian(ConstantTexture(Vector3d(1.0, 0.0, 0.0))))
        Translate(Sphere(Vector3d(2.0, 0.5, 2.0), 0.5, Lambertian(ConstantTexture(Vector3d(0.0, 1.0, 0.0)))), Vector3d(1.0))
        Translate(Sphere(Vector3d(2.0, 0.5, 2.0), 0.5, Lambertian(ConstantTexture(Vector3d(0.0, 0.0, 1.0)))), Vector3d(1.0, 1.0, -1.0))
    ] |> Seq.ofList

let oneBox =
    let simpleMat = Lambertian(ConstantTexture(Vector3d(1.0, 0.5, 0.0)))
    let light = DiffuseLight(ConstantTexture(Vector3d(15.0)))
    let box = makeBox (Vector3d(-2.0, 0.0, 2.0)) (Vector3d(2.0, 2.0, -2.0)) (Lambertian(ConstantTexture(Vector3d(0.0, 0.5, 0.2))))
    let iii = [
        for x = 0 to 5 do
                yield Sphere(Vector3d(float x, 1.0, 0.0), 0.5, simpleMat)
        for z = 0 to 5 do
                yield Sphere(Vector3d(0.0, 1.0, float z), 0.5, simpleMat)
    ]
    [
        Sphere(Vector3d(0.0, 507.0, 0.0), 100.0, light)
        XzRect(-100.0, 100.0, -100.0, 100.0, -1.0, Lambertian(ConstantTexture(Vector3d(0.0, 0.2, 0.0))))
        (XyRect(0.0, 3.0, 0.0, 3.0, 0.0, Lambertian(ConstantTexture(Vector3d(0.2, 0.2, 0.8)))))
        
        // makeRotate (XyRect(0.0, 3.0, 0.0, 3.0, 0.0, Lambertian(ConstantTexture(Vector3d(0.2, 0.2, 0.8))))) RY 30.0
        // makeRotate box RY 30.0
    ]
    // @ iii

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

type Game() =
    /// <summary>Creates a 800x600 window with the specified title.</summary>
    inherit GameWindow(800, 600)

    let canvas = new System.Drawing.Bitmap(800, 600, Drawing.Imaging.PixelFormat.Format32bppArgb)
    let mutable bytes = Array.create 1 (byte 0)
    let width = 200
    let height = 200
    let settings = { 
        Samples = 100
        // LookFrom = Vector3d(3.0, 5.0, 13.0) * 0.5
        LookFrom = Vector3d(3.0, 5.0, 13.0) * 0.5
        // LookFrom = Vector3d(278.0, 278.0, 800.0)
        LookAt = Vector3d(0.0)
        // LookAt = Vector3d(278.0, 278.0, 0.0)
        // Fov = 40.0
        Fov = 90.0
    }
    let zoom = 1.0
    let mutable lacunarity = 1.4
    let mutable gain = 0.2
    
    // let hitable : Hitable = Bvh.makeBvh hitableSeq
    // let hitable = HitableList hitableSeq
    // let hitable = HitableList cornellBox
    // let hitable = Bvh.makeBvh cornellBox
    // let hitable = HitableList standardScene
    let hitable = HitableList oneBox

    let update() =
        let bitmap = new Drawing.Bitmap(width, height)

        let stopwatch = Diagnostics.Stopwatch.StartNew(); //creates and start the instance of Stopwatch

        Render.mainRender bitmap settings hitable
        // NoiseTrain.subMainRender bitmap lacunarity gain

        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);

        Common.drawBitmapOnBitmap bitmap canvas zoom
        bytes <-Common.getBytesFromBitmap canvas
        bitmap.RotateFlip(Drawing.RotateFlipType.RotateNoneFlipY)
        
        bitmap.Save("test-images/output.png")

    do 
        base.VSync <- VSyncMode.On

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
        elif base.Keyboard.[Key.Number1] then
            lacunarity <- lacunarity * 2.0
            update()
        elif base.Keyboard.[Key.Number2] then
            lacunarity <- lacunarity * 0.5
            update()
        elif base.Keyboard.[Key.Number3] then
            gain <- gain * 2.0
            update()
        elif base.Keyboard.[Key.Number4] then
            gain <- gain * 0.5
            update()
        elif base.Keyboard.[Key.R] then
            lacunarity <- 2.0
            gain <- 0.5
            update()

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
    let win = new Game()
    win.Run()
    
    0 // return an integer exit code
