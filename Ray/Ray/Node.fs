module Node

open Object
open Transform

type Node = {
    Name : string
    Object : Object option
    Children : Node list
    Transform : Transform
    Material : string
}