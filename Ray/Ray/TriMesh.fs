module TriMesh

open System.IO
open System.Collections.Generic
open OpenTK

let readVertex (line : string) =
    match line.Split([|' '|]) with
    | [|_; a; b; c|] ->
        let a = double a
        let b = double b
        let c = double c
        Vector3d(a, b, c)
    | _ ->
        failwith "wrong vertex format"

let r = readVertex "v -1 2.0 3.0"
type TriMesh(filename : string) =
    let vertices = List<Vector3d>()
    let faces = List<int * int * int>()
    let vertexNormal = List<Vector3d>()
    let normalFaces = List<int * int * int>()
    let textureVertices = List<Vector3d>()
    let textureFaces = List<int * int * int>()

    do
        for line in File.ReadAllLines(filename) do
            match line.Chars 0 with
            | 'v' ->
                match line.Chars 1 with
                | ' ' ->
                    let v = readVertex line
                    vertices.Add v
                | 't' ->
                    let v = readVertex line
                    textureVertices.Add v
                | 'n' ->
                    let v = readVertex line
                    vertexNormal.Add v                
                failwith ""