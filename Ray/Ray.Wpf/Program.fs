namespace Ray.Wpf
module Program =

    open System
    open Ray

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platforms.Wpf)
        app.Run(new MainForm())
        0