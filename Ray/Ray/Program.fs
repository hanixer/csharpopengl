// Learn more about F# at http://fsharp.org

open System

open System.Xml
open Scene

[<EntryPoint>]
let main argv =
    loadSceneFromFile "test2.xml"    
    0 // return an integer exit code
