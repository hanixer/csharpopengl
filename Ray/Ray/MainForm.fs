namespace Ray

open System
open Eto.Forms
open Eto.Drawing

type Vec3 = 
    {x : float; y : float; z : float}
    static member (/) (v: Vec3, a: float) =
        {x = v.x / a; y = v.y / a; z = v.z / a}
    static member (+) (v1: Vec3, v2: Vec3) =
        {x = v1.x + v2.x; y = v1.y + v2.y; z = v1.z + v2.z}
    static member (-) (v1: Vec3, v2: Vec3) =
        {x = v1.x - v2.x; y = v1.y - v2.y; z = v1.z - v2.z}


type Ray = {Origin : Vec3; Direction : Vec3}


module Render =

    let makeVec3 v = {x = v; y = v; z = v}

    let length v =
        Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z)

    let normalize v =
        v / (length v)

    let nearZ = 0.1
    let fieldOfView = 180.0 / 45.0 * Math.PI

    let rayDirection c r width height nearZ fieldOfView =
        let side = Math.Tan(fieldOfView) * nearZ
        let aspect = height / width
        let x = (float c / float width - 0.5) * 2.0 * side
        let y = (float r / float height - 0.5) * 2.0 * side * aspect
        normalize {x = x; y = y; z = -nearZ}

    let traceRay ray sphereCenter sphereRadius sphereColor =
        let

    let render (bitmap : Bitmap) sphereCenter sphereRadius sphereColor =
        for r in bitmap.Width-1 do
            for c in bitmap.Height - 1 do
                let origin = makeVec3 0.0
                let direction = rayDirection c r bitmap.Width bitmap.Height nearZ fieldOfView
                let ray = {Origin = origin; Direction = direction}


    let drawBitmap (graphics : Graphics) (bitmap : Bitmap) (pixelSize : float32) =
        for r in 0..bitmap.Height - 1 do        
            for c in 0..bitmap.Width - 1 do
                let color = bitmap.GetPixel(c, r)
                let x = (float32 c) * pixelSize
                let y = (float32 r) * pixelSize
                graphics.FillRectangle(color, x, y, pixelSize, pixelSize) 

    let drawCallback (e : PaintEventArgs) =    
        let bitmap = new Bitmap(5, 5, e.Graphics)
        bitmap.SetPixel(0, 0, Colors.Red)
        bitmap.SetPixel(0, 2, Colors.DimGray)
        bitmap.SetPixel(0, 4, Colors.Firebrick)
        bitmap.SetPixel(1, 1, Colors.Fuchsia)
        bitmap.SetPixel(1, 3, Colors.Gold)
        drawBitmap e.Graphics bitmap 100.0f

type MainForm () as this =
    inherit Form()
    do
        base.Title <- "My Eto Form"
        base.ClientSize <- new Size(400, 350)


        let drawable = new Drawable()
        drawable.Paint.Add Render.drawCallback

        base.Content <- drawable

        // table with three rows
        // let layout = new StackLayout()
        // layout.Items.Add(new StackLayoutItem(new Label(Text = "Hello World!")))
        // Add more controls here

        // base.Content <- layout;

        // create a few commands that can be used for the menu and toolbar
        // let clickMe = new Command(MenuText = "Click Me!", ToolBarText = "Click Me!")
        // clickMe.Executed.Add(fun e -> MessageBox.Show(this, "I was clicked!") |> ignore)

        // let quitCommand = new Command(MenuText = "Quit")
        // quitCommand.Shortcut <- Application.Instance.CommonModifier ||| Keys.Q
        // quitCommand.Executed.Add(fun e -> Application.Instance.Quit())

        // let aboutCommand = new Command(MenuText = "About...")
        // aboutCommand.Executed.Add(fun e ->
        //     let dlg = new AboutDialog()
        //     dlg.ShowDialog(this) |> ignore
        //     )
        
        // base.Menu <- new MenuBar()
        // let fileItem = new ButtonMenuItem(Text = "&File")
        // fileItem.Items.Add(clickMe) |> ignore
        // base.Menu.Items.Add(fileItem)

        (* add more menu items to the main menu...
        let editItem = new ButtonMenuItem(Text = "&Edit")
        base.Menu.Items.Add(editItem)
        let viewItem = new ButtonMenuItem(Text = "&View")
        base.Menu.Items.Add(viewItem)
        *)

        // base.Menu.ApplicationItems.Add(new ButtonMenuItem(Text = "&Preferences..."))
        // base.Menu.QuitItem <- quitCommand.CreateMenuItem()
        // base.Menu.AboutItem <- aboutCommand.CreateMenuItem()

        // base.ToolBar <- new ToolBar()
        // base.ToolBar.Items.Add(clickMe)


        
