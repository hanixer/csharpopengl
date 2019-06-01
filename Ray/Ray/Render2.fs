module Render2
open System.Drawing
open Scene

let render (bitmap : Bitmap) (scene : Scene) =
    let w = bitmap.Width
    let h = bitmap.Height
    for r = 0 to h - 1 do
        for c = 0 to w - 1 do
            printfn ""
    1