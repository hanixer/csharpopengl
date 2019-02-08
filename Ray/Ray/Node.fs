module Node

open Object
open Transform

type Node = {
    Name : string
    Object : Object
    Children : Node list
    Transform : Transform
}