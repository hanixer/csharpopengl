module Node

open Object
open Transform

type Node = {
    Object : Object
    Children : Node list
    Transform : Transform
}