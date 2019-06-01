module TriangleMesh
open OpenTK
open System.IO
open ObjLoader.Loader.Loaders

type Face = int[]

type Mesh = {
    V : Vector3d[] // vertex list
    F : Face[] // faces
    VN : Vector3d[] // vertex normal
    FN : Face[] // normal faces
    VT : Vector3d[] // texture vertices
    FT : Face[] // texture faces
}

let private toVector3d (v : ObjLoader.Loader.Data.VertexData.Vertex) =
    Vector3d(float v.X, float v.Y, float v.Z)

let private normalToVector3d (v : ObjLoader.Loader.Data.VertexData.Normal) =
    Vector3d(float v.X, float v.Y, float v.Z)

let private vtToVector3d (v : ObjLoader.Loader.Data.VertexData.Texture) =
    Vector3d(float v.X, float v.Y, 0.0)

let private loadResultToMesh (lr : LoadResult) =
    let v = Seq.map toVector3d lr.Vertices |> Seq.toArray
    let vn = Seq.map normalToVector3d lr.Normals |> Seq.toArray
    let vt = Seq.map vtToVector3d lr.Textures |> Seq.toArray
    let f = System.Collections.Generic.List<Face>()
    let fn = System.Collections.Generic.List<Face>()
    let ft = System.Collections.Generic.List<Face>()
    if lr.Groups.Count <> 1 then
        failwith "expected 1 group"
    let g = lr.Groups.[0]
    for face in g.Faces do
        if face.Count < 3 then
            failwith "expected 3 and more face points"
        for i = 1 to face.Count - 2 do
            printfn "%A" i
            let vertexFace = [| face.[0].VertexIndex; face.[i].VertexIndex; face.[i+1].VertexIndex |]
            let normalFace = [| face.[0].NormalIndex; face.[i].NormalIndex; face.[i+1].NormalIndex |]
            let texFace = [| face.[0].TextureIndex; face.[i].TextureIndex; face.[i+1].TextureIndex |]
            f.Add(vertexFace)
            fn.Add(normalFace)
            ft.Add(texFace)
    { V = v; F = f.ToArray(); VN = vn; FN = fn.ToArray(); VT = vt; FT = ft.ToArray()}

let loadFromFile filename =
    let fs = new FileStream(filename, FileMode.Open)
    let loader = ObjLoaderFactory().Create()
    let loadResult = loader.Load(fs)
    loadResultToMesh loadResult
