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

type Scene = {
    Camera : Camera
    Materials : Map<string, Material>
    Lights : Map<string, Light>
    LightsList : Light list
    Environment : Vector3d
    Samples : int
    // AreaLights : Node list
    Primitive : Primitive
}

let getMaterial scene name = Map.find name scene.Materials

let printIdent level =
    for i = 1 to level do
        printf " "

let readFloating (xml : XmlNode) (name : string) defaultVal =
    let xml = xml :?> XmlElement
    if not (isNull xml) then
        xml.Attributes
        |> Seq.cast<XmlAttribute>
        |> Seq.tryFind (fun attr -> attr.Name = name)
        |> Option.map (fun attr ->
            Double.Parse(attr.Value, CultureInfo.InvariantCulture)
            )
        |> Option.defaultValue defaultVal
    else
        defaultVal

let readVector (xml : XmlNode) (defaultVec : Vector3d) =
    if not (isNull xml) then
        let xml = xml :?> XmlElement
        let x = readFloating xml "x" defaultVec.X
        let y = readFloating xml "y" defaultVec.Y
        let z = readFloating xml "z" defaultVec.Z
        let value = readFloating xml "value" 1.0
        Vector3d(x, y, z) * value
    else
        defaultVec

let readColor (xml : XmlNode) (defaultVec : Vector3d) =
    if not (isNull xml) then
        let xml = xml :?> XmlElement
        let x = readFloating xml "r" defaultVec.X
        let y = readFloating xml "g" defaultVec.Y
        let z = readFloating xml "b" defaultVec.Z
        let value = readFloating xml "value" 1.0
        Vector3d(x, y, z) * value
    else    
        defaultVec

let select (xml : XmlElement) name selector defaultV =
    xml.SelectNodes name
    |> Seq.cast<XmlElement>
    |> Seq.tryHead
    |> Option.map selector
    |> Option.defaultValue defaultV

let getChildElements (xml : XmlElement) =
    xml.ChildNodes
    |> Seq.cast<XmlNode>
    |> Seq.filter (fun child -> child.NodeType = XmlNodeType.Element)
    |> Seq.cast<XmlElement>

let loadTransform (xml : XmlElement) level =
    let chooseTransform (xml : XmlElement) =
        match xml.Name with
        | "translate" ->
            printIdent level
            let v = readVector xml Vector3d.Zero
            let tm = translate v
            printfn "  translate %A" v
            Some tm
        | "scale" ->
            printIdent level
            let v = readVector xml Vector3d.One
            let tm = scale v
            printfn "  scale %A" v
            Some tm
        | "rotate" ->
            printIdent level
            let axis = readVector xml Vector3d.Zero
            let angle = readFloating xml "angle" 0.0
            let tm = rotate axis angle
            printfn "  rotate %A, %A" axis angle
            Some tm
        | _ ->
            None
    getChildElements xml
    |> Seq.choose chooseTransform
    |> Seq.fold (fun a b -> compose b a) identityTransform

let loadMaterial (xml : XmlNode) =
    let chooseMaterial blinn (xml : XmlElement) =
        match xml.Name with
        | "diffuse" ->
            let c = readColor xml Vector3d.One
            printfn "  diffuse %A" c
            {blinn with DiffuseColor = c}
        | "specular" ->
            let c = readColor xml Vector3d.One
            printfn "  specular %A" c
            {blinn with SpecularColor = c}
        | "glossiness" ->
            let glos = readFloating xml "value" 1.0
            printfn "  glossiness %A" glos
            {blinn with Glossiness = glos}
        | "reflection" ->
            let v = readColor xml Vector3d.One
            printfn "  reflection %A" v
            {blinn with Reflection = v}
        | "refraction" ->
            let v = readColor xml Vector3d.One
            let ior = readFloating xml "index" 1.0            
            printfn "  reflection %A, index %A" v ior
            {blinn with Refraction = v; Ior = ior}
        | _ ->
            blinn
    let xml = xml :?> XmlElement
    let name =
        let nameAttr = xml.Attributes.["name"]
        if not (isNull nameAttr) then
            nameAttr.InnerText
        else
            ""
    printfn "material [%s]" name
    let material = 
        match xml.Attributes.["type"].Value with
        | "blinn" ->
            printf " - Blinn"
            getChildElements xml
            |> Seq.fold chooseMaterial defaultBlinn
            |> Blinn
            |> Some
        | "emissive" ->
            printfn " - Emissive"
            let color = readColor (xml.SelectSingleNode "./color") Vector3d.One
            Some(Emissive(color))
        | _ -> None
            
    printfn ""
    material
    |> Option.map (fun material ->
        (name, material))

let loadLight (xml : XmlNode) =
    let xml = xml :?> XmlElement
    let name =
        let nameAttr = xml.Attributes.["name"]
        if not (isNull nameAttr) then
            nameAttr.InnerText
        else
            ""
    printf "light [%s]" name
    match xml.Attributes.["type"].Value with
    | "direct" ->
        printfn " - Direct"
        let intensity =
            readColor (xml.SelectSingleNode "./intensity") Vector3d.One
        printfn "  intensity %A" intensity
        let direction =
            readVector (xml.SelectSingleNode "./direction") Vector3d.One
        printfn "  direction %A" direction
        DirectLight(intensity, direction) |> Some
    | "point" ->
        printfn " - Point"
        let intensity =
            readColor (xml.SelectSingleNode "./intensity") Vector3d.One
        printfn "  intensity %A" intensity
        let position =
            readVector (xml.SelectSingleNode "./position") Vector3d.Zero
        printfn "  position %A" position
        PointLight(intensity, position) |> Some
    | "area" ->
        printfn " - Area"
        let o = xml.Attributes.["object"].Value
        printfn "  object %A" o
        AreaLight(o) |> Some
    | "ambient" ->
        printfn " - Ambient"
        let intensity =
            readColor (xml.SelectSingleNode "./intensity") Vector3d.One
        printfn "  intensity %A" intensity
        AmbientLight(intensity) |> Some
    | _ -> None
    |> Option.map (fun light -> (name, light))

let readStrAttribute (xml : XmlElement) (name : string) =
    let nameAttr = xml.Attributes.[name]
    if not (isNull nameAttr) then
        nameAttr.InnerText
    else
        ""

let getObjectFromType (xml : XmlElement) =
    let typeAttr = xml.Attributes.["type"]
    if not (isNull typeAttr) then
        match typeAttr.InnerText with
        | "sphere" ->
            printf " - Sphere"
            Some Object.Sphere
        | "plane" ->
            printf " - Plane"
            Some(Object.Plane)
        | "triangle" ->
            printf " - Triangle"
            let vz = Vector3d.Zero
            let p0 = select xml "./p0" (fun elem -> readVector elem vz) vz
            let p1 = select xml "./p1" (fun elem -> readVector elem vz) vz
            let p2 = select xml "./p2" (fun elem -> readVector elem vz) vz
            Some(Object.Triangle(p0, p1, p2))
        | "rectangle" ->
            printf " - Rectangle"
            let vz = Vector3d.Zero
            let p0 = select xml "./p0" (fun elem -> readVector elem vz) vz
            let p1 = select xml "./p1" (fun elem -> readVector elem vz) vz
            let p2 = select xml "./p2" (fun elem -> readVector elem vz) vz
            Some(Object.Rectangle(p0, p1, p2))
        | "box" ->
            printf " - Box"
            let vz = Vector3d.Zero
            let p0 = select xml "./p0" (fun elem -> readVector elem vz) vz
            let p1 = select xml "./p1" (fun elem -> readVector elem vz) vz
            Some(Object.makeBox p0 p1)
        | "cylinder" ->
            printf " - Cylinder"
            Some Object.Cylinder
        | "disk" ->
            printf " - Disk"
            Some Object.Disk
        | "rectWithHoles" ->
            printf " - rect with holes"
            let radius = readFloating xml "radius" 0.1
            Some(Object.RectXYWithHoles(1.0, radius))
        | "obj" ->
            printf " - triangle obj model"
            let filename = readStrAttribute xml "name"
            let data = TriangleMesh.loadFromFile filename
            Some(Object.TriangleObj(data))
        | _ ->
            printf " - UNKNOWN TYPE"
            None
    else None

let makeTrianglePrimitives objects material =
    Array.map (fun object -> GeometricPrimitive(object, material)) objects

let constructPrimitive objectOpt material transform children =
    let withChildren prim =
        if List.isEmpty children then
            prim
        else
            makeBVH (prim::children)

    let withTransform prim =
        if transform = Transform.identityTransform then
            withChildren prim
        else
            withChildren (makeTransformedPrimitive prim transform)

    match objectOpt with
    | Some(Object.TriangleObj(data)) ->
        let objects = Object.makeTriangleShapes data
        let primitives = makeTrianglePrimitives objects material
        makeBVH primitives
    | Some(object) ->
        withTransform (GeometricPrimitive(object, material))
    | _ ->
        withChildren (PrimitiveList[])

let rec loadObject (xml : XmlNode) level =
    let xml = xml :?> XmlElement
    let name = readStrAttribute xml "name"
    let material = readStrAttribute xml "material"
    printIdent level
    printf "object [%s]" name
    let object = getObjectFromType xml
    printfn ""
    let tm = loadTransform xml level
    let children = loadChildren xml level
    constructPrimitive object material tm children

and loadChildren (xml : XmlElement) level =
    xml.ChildNodes
    |> Seq.cast<XmlNode>
    |> Seq.filter (fun child -> child.Name = "object" && child.NodeType = XmlNodeType.Element)
    |> Seq.map (fun child -> loadObject child (level + 1))
    |> Seq.toList

let loadSceneObjects (xml : XmlElement) =
    loadChildren xml 0

let loadCamera (xml : XmlElement) =
    let pos = select xml "./position" (fun elem -> readVector elem defCameraPos) defCameraPos
    let target = select xml "./target" (fun elem -> readVector elem defCameraPos) defCameraPos
    let up = select xml "./up" (fun elem -> readVector elem defCameraPos) defCameraPos
    let fov =  select xml "./fov" (fun elem -> readFloating elem "value" defCameraFov) defCameraFov
    let width =  select xml "./width" (fun elem -> readFloating elem "value" defCameraWidth) defCameraWidth
    let height =  select xml "./height" (fun elem -> readFloating elem "value" defCameraHeight) defCameraHeight
    Camera(pos, target, up, fov, int width, int height)

let loadScene (xml : XmlDocument) =
    let xml = xml.Item "xml"
    if not (isNull xml) then
        let primitives =
            let sceneXml = xml.Item "scene"
            if not (isNull sceneXml) then
                loadSceneObjects sceneXml
            else
                failwith "scene tag not found"
        let camera =
            let cameraXml = xml.Item "camera"
            if not (isNull cameraXml) then
                loadCamera cameraXml
            else
                failwith "camera tag not found"
        let materials =
            xml.GetElementsByTagName "material"
            |> Seq.cast<XmlElement>
            |> Seq.map loadMaterial
            |> Seq.choose id
            |> Map.ofSeq
        let lights =
            xml.GetElementsByTagName "light"
            |> Seq.cast<XmlElement>
            |> Seq.map loadLight
            |> Seq.choose id
            |> Map.ofSeq
        let lightsList = lights |> Map.toList |> List.map snd
        let nodesMap = Map.empty //List.collect getNodePairs nodes |> Map.ofList
        let environment =
            readColor (xml.SelectSingleNode "scene/environment") Vector3d.Zero
        let samples =
            readFloating (xml.SelectSingleNode "scene/samples") "value" 1.0 |> int
        printfn "samples %A" samples
        {
          Camera = camera
          Materials = materials
          Lights = lights
          LightsList = lightsList
          Environment = environment
          Samples = samples
          Primitive = makeBVH primitives
         }
    else
        failwith "xml tag not found"

let loadSceneFromFile (filename : string) =
    let remember = Environment.CurrentDirectory
    let dir = Path.GetDirectoryName(filename)
    Environment.CurrentDirectory <- dir
    let xml = XmlDocument()
    xml.Load(Path.GetFileName(filename))
    let sc = loadScene xml
    Environment.CurrentDirectory <- remember
    sc