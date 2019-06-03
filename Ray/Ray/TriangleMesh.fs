module TriangleMesh
open OpenTK
open System.IO
open ObjLoader.Loader.Loaders

type Face = int[]

type Data(v : Vector3d[], f : Face[], VN : Vector3d[],FN : Face[],VT : Vector3d[],FT : Face[]) =
    member __.FacesCount = f.Length
    member __.Vertex(i : int) = v.[i-1]
    member __.Faces = f
    override __.ToString() = "nothing"

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
            let vertexFace = [| face.[0].VertexIndex; face.[i].VertexIndex; face.[i+1].VertexIndex |]
            let normalFace = [| face.[0].NormalIndex; face.[i].NormalIndex; face.[i+1].NormalIndex |]
            let texFace = [| face.[0].TextureIndex; face.[i].TextureIndex; face.[i+1].TextureIndex |]
            f.Add(vertexFace)
            fn.Add(normalFace)
            ft.Add(texFace)
    Data(v, f.ToArray(), vn, fn.ToArray(), vt, ft.ToArray())

let loadFromFile filename =
    use fs = new FileStream(filename, FileMode.Open)
    let loader = ObjLoaderFactory().Create()
    let loadResult = loader.Load(fs)
    loadResultToMesh loadResult

let meshToList (data : Data) =
    Seq.map (fun (face : Face) -> data.Vertex(face.[0]), data.Vertex(face.[1]), data.Vertex(face.[2])) data.Faces