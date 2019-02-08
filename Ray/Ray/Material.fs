module Material
open OpenTK

type Material =
    | Blinn of Vector3d * Vector3d * float // diffuse, specular, glossy
    
let shade ray material hitInfo lights =
    failwith ""

let defBlinnDiffuse = Vector3d(0.5)
let defBlinnSpecular = Vector3d(0.7)
let defBlinnGlossiness = 20.0