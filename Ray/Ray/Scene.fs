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

type Scene = {
    Camera : Camera 
    Nodes : Node list
    Materials : Map<string, Material>
    Lights : Map<string, Light>
}

let getMaterial scene name = Map.find name scene.Materials

let printIdent level =
    for i = 1 to level do 
        printf " "

let readFloating (xml : XmlElement) (name : string) defaultVal =
    xml.Attributes
    |> Seq.cast<XmlAttribute>
    |> Seq.tryFind (fun attr -> attr.Name = name)
    |> Option.map (fun attr ->
        Double.Parse(attr.Value, CultureInfo.InvariantCulture)
        )
    |> Option.defaultValue defaultVal

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
    let chooseMaterial (diffuse, specular, glossy as acc) (xml : XmlElement) =
        match xml.Name with
        | "diffuse" ->
            let c = readColor xml Vector3d.One
            printfn "  diffuse %A" c
            (c, specular, glossy)
        | "specular" ->
            let c = readColor xml Vector3d.One
            printfn "  specular %A" c
            (diffuse, c, glossy)
        | "glossiness" ->
            let glos = readFloating xml "value" 1.0
            printfn "  glossiness %A" glos
            (diffuse, specular, glos)
        | _ ->
            acc
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
            |> Seq.fold chooseMaterial (defBlinnDiffuse, defBlinnSpecular, defBlinnGlossiness)
            |> Blinn
            |> Some
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
    | "ambient" ->
        printfn " - Ambient"
        let intensity =
            readColor (xml.SelectSingleNode "./intensity") Vector3d.One
        printfn "  intensity %A" intensity
        AmbientLight(intensity) |> Some
    | _ -> None
    |> Option.map (fun light -> (name, light))

let getObjectFromType (typeAttr : XmlAttribute) = 
    if not (isNull typeAttr) then
        match typeAttr.InnerText with
        | "sphere" -> 
            printf " - Sphere"
            Some Object.Sphere
        | _ -> 
            printf " - UNKNOWN TYPE"
            None
    else None

let readStrAttribute (xml : XmlElement) (name : string) =
    let nameAttr = xml.Attributes.[name]
    if not (isNull nameAttr) then
        nameAttr.InnerText
    else
        ""

let rec loadObject (xml : XmlNode) level =
    let xml = xml :?> XmlElement
    let name = readStrAttribute xml "name"
    let material = readStrAttribute xml "material"
    printIdent level
    printf "object [%s]" name
    let object = getObjectFromType xml.Attributes.["type"]
    printfn ""
    let tm = loadTransform xml level
    let children = loadChildren xml level
    {Name = name; Object = object; Children = children; Transform = tm; Material = material}

and loadChildren (xml : XmlElement) level =
    xml.ChildNodes    
    |> Seq.cast<XmlNode>
    |> Seq.filter (fun child -> child.Name = "object" && child.NodeType = XmlNodeType.Element)
    |> Seq.map (fun child -> loadObject child (level + 1))
    |> Seq.toList

let loadSceneObjects (xml : XmlElement) =   
    loadChildren xml 0     

let loadCamera (xml : XmlElement) =
    let select name selector defaultV =
        xml.SelectNodes name
        |> Seq.cast<XmlElement>
        |> Seq.tryHead
        |> Option.map selector
        |> Option.defaultValue defaultV
    let pos = select "./position" (fun elem -> readVector elem defCameraPos) defCameraPos
    let target = select "./target" (fun elem -> readVector elem defCameraPos) defCameraPos
    let up = select "./up" (fun elem -> readVector elem defCameraPos) defCameraPos
    let fov =  select "./fov" (fun elem -> readFloating elem "value" defCameraFov) defCameraFov
    let width =  select "./width" (fun elem -> readFloating elem "value" defCameraWidth) defCameraWidth
    let height =  select "./height" (fun elem -> readFloating elem "value" defCameraHeight) defCameraHeight
    Camera(pos, target, up, fov, int width, int height)

let loadScene (xml : XmlDocument) =
    let xml = xml.Item "xml"    
    if not (isNull xml) then
        let nodes = 
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
        {Nodes = nodes; Camera = camera; Materials = materials; Lights = lights}
    else
        failwith "xml tag not found"

let loadSceneFromFile (filename : string) =
    let xml = XmlDocument()
    xml.Load(filename)    
    loadScene xml