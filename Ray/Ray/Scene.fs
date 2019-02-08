module Scene

open System
open System.Globalization
open Camera
open Node
open System.Xml
open Transform
open OpenTK

type Scene = {
    Camera : Camera 
    Nodes : Node list
}

let printIdent level =
    for i = 1 to level do 
        printf " "

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

let readFloating (xml : XmlElement) (name : string) defaultVal =
    xml.Attributes
    |> Seq.cast<XmlAttribute>
    |> Seq.tryFind (fun attr -> attr.Name = name)
    |> Option.map (fun attr ->
        Double.Parse(attr.Value, CultureInfo.InvariantCulture)
        )
    |> Option.defaultValue defaultVal

let readVector (xml : XmlElement) (defaultVec : Vector3d) =
    let x = readFloating xml "x" defaultVec.X
    let y = readFloating xml "y" defaultVec.Y
    let z = readFloating xml "z" defaultVec.Z
    let value = readFloating xml "value" 1.0
    Vector3d(x, y, z) * value

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
        | _ ->
            None
    xml.ChildNodes
    |> Seq.cast<XmlNode>
    |> Seq.filter (fun child -> child.NodeType = XmlNodeType.Element)
    |> Seq.cast<XmlElement>
    |> Seq.choose chooseTransform
    |> Seq.fold (fun a b -> compose b a) identityTransform

let rec loadObject (xml : XmlNode) level =
    let xml = xml :?> XmlElement
    let name =
        let nameAttr = xml.Attributes.["name"]
        if not (isNull nameAttr) then
            nameAttr.InnerText
        else
            ""
    printIdent level
    printf "object [%s]" name
    let object = getObjectFromType xml.Attributes.["type"]
    printfn ""
    object
    |> Option.map (fun object ->
        let tm = loadTransform xml level
        let children = loadChildren xml level
        {Name = name; Object = object; Children = children; Transform = tm}
        )

and loadChildren (xml : XmlElement) level =
    xml.ChildNodes    
    |> Seq.cast<XmlNode>
    |> Seq.filter (fun child -> child.Name = "object" && child.NodeType = XmlNodeType.Element)
    |> Seq.map (fun child -> loadObject child (level + 1))
    |> Seq.filter Option.isSome
    |> Seq.map Option.get
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
        {Nodes = nodes; Camera = camera}
    else
        failwith "xml tag not found"

let loadSceneFromFile (filename : string) =
    let xml = XmlDocument()
    xml.Load(filename)    
    loadScene xml