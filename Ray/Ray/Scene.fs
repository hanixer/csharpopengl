module Scene

open Camera
open Node

type Scene = {
    Camera : Camera 
    Root : Node
}

let loadScene xml =
    1