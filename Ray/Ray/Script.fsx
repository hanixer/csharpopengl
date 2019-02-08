#load "references.fsx"
#load "Transform.fs"
#load "Camera.fs"
#load "Object.fs"
#load "Node.fs"
#load "Scene.fs"
open Transform
open OpenTK
open System.Xml
open Scene

let xml = XmlDocument()
xml.LoadXml("""
<nothing a="3.0" x="1.0" value="3.0" z="3"/>
""")
let el = xml.FirstChild :?> XmlElement
readFloating el "aa" 1.0
readVector el Vector3d.Zero