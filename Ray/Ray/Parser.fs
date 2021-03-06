module Parser

open Camera
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
open Types

let printIdent level =
    for i = 1 to level do
        printf " "

let readFloating (xml : XmlNode) (name : string) defaultVal =
    let xml = xml :?> XmlElement
    if not (isNull xml) then
        xml.Attributes
        |> Seq.cast<XmlAttribute>
        |> Seq.tryFind (fun attr -> attr.Name = name)
        |> Option.map (fun attr -> Double.Parse(attr.Value, CultureInfo.InvariantCulture))
        |> Option.defaultValue defaultVal
    else defaultVal

let readVector (xml : XmlNode) (defaultVec : Vector3d) =
    if not (isNull xml) then
        let xml = xml :?> XmlElement
        let x = readFloating xml "x" defaultVec.X
        let y = readFloating xml "y" defaultVec.Y
        let z = readFloating xml "z" defaultVec.Z
        let value = readFloating xml "value" 1.0
        Vector3d(x, y, z) * value
    else defaultVec

let readColor (xml : XmlNode) (defaultVec : Vector3d) =
    if not (isNull xml) then
        let xml = xml :?> XmlElement
        let x = readFloating xml "r" defaultVec.X
        let y = readFloating xml "g" defaultVec.Y
        let z = readFloating xml "b" defaultVec.Z
        let value = readFloating xml "value" 1.0
        Vector3d(x, y, z) * value
    else defaultVec

let trySelect (xml : XmlElement) name selector =
    xml.SelectNodes name
    |> Seq.cast<XmlElement>
    |> Seq.tryHead
    |> Option.map selector

let select (xml : XmlElement) name selector defaultV =
    trySelect xml name selector
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
        | _ -> None
    getChildElements xml
    |> Seq.choose chooseTransform
    |> Seq.fold (fun a b -> compose b a) identityTransform

let loadMaterial (xml : XmlNode) =
    let chooseMaterial blinn (xml : XmlElement) =
        match xml.Name with
        | "diffuse" ->
            let c = readColor xml Vector3d.One
            printfn "  diffuse %A" c
            { blinn with DiffuseColor = c }
        | "specular" ->
            let c = readColor xml Vector3d.One
            printfn "  specular %A" c
            { blinn with SpecularColor = c }
        | "glossiness" ->
            let glos = readFloating xml "value" 1.0
            printfn "  glossiness %A" glos
            { blinn with Glossiness = glos }
        | "reflection" ->
            let v = readColor xml Vector3d.One
            printfn "  reflection %A" v
            { blinn with Reflection = v }
        | "refraction" ->
            let v = readColor xml Vector3d.One
            let ior = readFloating xml "index" 1.0
            printfn "  reflection %A, index %A" v ior
            { blinn with Refraction = v
                         Ior = ior }
        | _ -> blinn

    let xml = xml :?> XmlElement

    let name =
        let nameAttr = xml.Attributes.["name"]
        if not (isNull nameAttr) then nameAttr.InnerText
        else ""
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
    material |> Option.map (fun material -> (name, material))

let loadLight (xml : XmlNode) =
    let xml = xml :?> XmlElement

    let name =
        let nameAttr = xml.Attributes.["name"]
        if not (isNull nameAttr) then nameAttr.InnerText
        else ""
    printf "light [%s]" name
    match xml.Attributes.["type"].Value with
    | "direct" ->
        printfn " - Direct"
        let intensity = readColor (xml.SelectSingleNode "./intensity") Vector3d.One
        printfn "  intensity %A" intensity
        let direction = readVector (xml.SelectSingleNode "./direction") Vector3d.One
        printfn "  direction %A" direction
        DirectLight(intensity, direction) |> Some
    | "point" ->
        printfn " - Point"
        let intensity = readColor (xml.SelectSingleNode "./intensity") Vector3d.One
        printfn "  intensity %A" intensity
        let position = readVector (xml.SelectSingleNode "./position") Vector3d.Zero
        printfn "  position %A" position
        PointLight(intensity, position) |> Some
    | "area" ->
        printfn " - Area"
        let o = xml.Attributes.["object"].Value
        printfn "  object %A" o
        AreaLight(o) |> Some
    | "ambient" ->
        printfn " - Ambient"
        let intensity = readColor (xml.SelectSingleNode "./intensity") Vector3d.One
        printfn "  intensity %A" intensity
        AmbientLight(intensity) |> Some
    | _ -> None
    |> Option.map (fun light -> (name, light))

let readStrAttribute (xml : XmlElement) (name : string) =
    let nameAttr = xml.Attributes.[name]
    if not (isNull nameAttr) then nameAttr.InnerText
    else ""

let getObjectFromType (xml : XmlElement) =
    let typeAttr = xml.Attributes.["type"]
    if not (isNull typeAttr) then
        match typeAttr.InnerText with
        | "sphere" ->
            printf " - Sphere"
            let radius = select xml "./radius" (fun elem -> readFloating elem "value" 1.) 1.
            Some(Object.Sphere(radius))
        | "plane" ->
            printf " - Plane"
            let side = select xml "./side" (fun elem -> readFloating elem "value" 1.) 1.
            Some(Object.makePlane side)
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
            let radius = select xml "./radius" (fun elem -> readFloating elem "value" 1.) 1.
            Some(Object.Disk(radius))
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
    Array.map (fun object -> makeGeometricPrimitive object material) objects

let loadLight2 primitives (xml : XmlNode) =
    let xml = xml :?> XmlElement
    let name =
        let nameAttr = xml.Attributes.["name"]
        if not (isNull nameAttr) then nameAttr.InnerText
        else ""
    printf "light [%s]" name
    match xml.Attributes.["type"].Value with
    | "area" ->
        printfn " - Area"
        let object =
            match trySelect xml "./object" getObjectFromType with
            | Some(Some(o)) -> o
            | _ -> Sphere(1.)
        let transform = loadTransform xml 0
        let radiance = readColor (xml.SelectSingleNode "./radiance") Vector3d.One
        let light =
            { Object = object
              Radiance = radiance
              ObjToWorld = transform }
        let prim = GeometricPrimitive(object, (Blinn{Material.defaultBlinn with DiffuseColor = Vector3d.Zero}), Some(light))
        let prim = makeTransformedPrimitive prim transform
        light, prim
    | _ ->
        failwith "wrong light type"

let constructPrimitive objectOpt material transform children =
    let geomPrim =
        match objectOpt with
        | Some(Object.TriangleObj(data)) ->
            let objects = Object.makeTriangleShapes data
            let primitives = makeTrianglePrimitives objects material
            makeBVH primitives
        | Some(object) -> makeGeometricPrimitive object material
        | _ -> PrimitiveList []

    let primAndChildren =
        if geomPrim <> PrimitiveList [] then makeBVH (geomPrim :: children)
        else makeBVH children

    if transform = Transform.identityTransform then primAndChildren
    else makeTransformedPrimitive primAndChildren transform

let rec loadObject (xml : XmlNode) materials level =
    let xml = xml :?> XmlElement
    let name = readStrAttribute xml "name"
    let materialName = readStrAttribute xml "material"
    printIdent level
    printf "object [%s]" name
    let object = getObjectFromType xml
    printfn ""
    let tm = loadTransform xml level
    let children = loadChildren xml materials level

    let material =
        match Map.tryFind materialName materials with
        | Some(m) -> m
        | _ -> failwithf "material '%s' not found" materialName
    constructPrimitive object material tm children

and loadChildren (xml : XmlElement) materials level =
    xml.ChildNodes
    |> Seq.cast<XmlNode>
    |> Seq.filter (fun child -> child.Name = "object" && child.NodeType = XmlNodeType.Element)
    |> Seq.map (fun child -> loadObject child materials (level + 1))
    |> Seq.toList

let loadSceneObjects (xml : XmlElement) materials = loadChildren xml materials 0

let loadCamera (xml : XmlElement) =
    let pos = select xml "./position" (fun elem -> readVector elem defCameraPos) defCameraPos
    let target = select xml "./target" (fun elem -> readVector elem defCameraPos) defCameraPos
    let up = select xml "./up" (fun elem -> readVector elem defCameraPos) defCameraPos
    let fov = select xml "./fov" (fun elem -> readFloating elem "value" defCameraFov) defCameraFov
    let width = select xml "./width" (fun elem -> readFloating elem "value" defCameraWidth) defCameraWidth
    let height = select xml "./height" (fun elem -> readFloating elem "value" defCameraHeight) defCameraHeight
    Camera(pos, target, up, fov, int width, int height)

let loadScene (xml : XmlDocument) =
    let xml = xml.Item "xml"
    if not (isNull xml) then
        let materials =
            xml.GetElementsByTagName "material"
            |> Seq.cast<XmlElement>
            |> Seq.map loadMaterial
            |> Seq.choose id
            |> Map.ofSeq

        let primitives =
            let sceneXml = xml.Item "scene"
            if not (isNull sceneXml) then loadSceneObjects sceneXml materials
            else failwith "scene tag not found"

        let camera =
            let cameraXml = xml.Item "camera"
            if not (isNull cameraXml) then loadCamera cameraXml
            else failwith "camera tag not found"

        let lights =
            xml.GetElementsByTagName "light"
            |> Seq.cast<XmlElement>
            |> Seq.map loadLight
            |> Seq.choose id
            |> Map.ofSeq

        let lightsList =
            lights
            |> Map.toList
            |> List.map snd

        let lights2 =
            xml.GetElementsByTagName "light2"
            |> Seq.cast<XmlElement>
            |> Seq.map (loadLight2 primitives)

        let primitives =
            Seq.map snd lights2
            |> Seq.append primitives

        let environment = readColor (xml.SelectSingleNode "scene/environment") Vector3d.Zero
        let samples = readFloating (xml.SelectSingleNode "scene/samples") "value" 1.0 |> int
        printfn "samples %A" samples
        { Scene.Camera = camera
          Scene.Materials = materials
          Scene.Lights = lights
          Scene.LightsList = lightsList
          Scene.Environment = environment
          Scene.Sampler = Sampling.makeSampler samples
          Scene.Primitive = makeBVH primitives
          Scene.AreaLights = Seq.toArray (Seq.map fst lights2) }
    else failwith "xml tag not found"

let loadIntegrator (xml : XmlDocument) =
    let el = xml.SelectSingleNode "xml/integrator"
    let def = Integrator.Simple
    if isNull el then def
    else
        match el.Attributes.["type"].Value with
        | "ao" -> Integrator.AmbientOcclusion
        | "simple" -> def
        | "whitted" -> Integrator.Whitted
        | _ -> def

let loadSceneAndIntegratorFromFile (filename : string) =
    let remember = Environment.CurrentDirectory
    let dir = Path.GetDirectoryName(filename)
    Environment.CurrentDirectory <- dir
    let xml = XmlDocument()
    xml.Load(Path.GetFileName(filename))
    let sc = loadScene xml
    let intgrtr = loadIntegrator xml
    Environment.CurrentDirectory <- remember
    sc, intgrtr
