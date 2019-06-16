module Scene

open System
open System.Globalization
open Camera
open Node
open System.Xml
open Transform
open OpenTK
open Material
open Light
open System.IO
open Sampling

type Scene = {
    Camera : Camera
    Materials : Map<string, Material>
    Lights : Map<string, Light>
    LightsList : Light list
    Environment : Vector3d
    Sampler : Sampler
    // AreaLights : Node list
    Primitive : Primitive
}

let getMaterial scene name = Map.find name scene.Materials

