module Types

open OpenTK

type Ray =
    { Origin : Vector3d
      Direction : Vector3d
      TMin : float
      TMax : float }

type Bounds =
    { PMin : Vector3d
      PMax : Vector3d }

type Object =
    | Sphere
    | Cylinder
    | RectXYWithHoles of float * float // width, radius
    | Triangle of Vector3d * Vector3d * Vector3d
    | Disk
    | Rectangle of Vector3d * Vector3d * Vector3d
    | ObjectList of Object list
    | Plane
    | TriangleObj of TriangleMesh.Data
    | TriangleObjPart of int * int * int * TriangleMesh.Data

type AreaLight2 =
    { Object : Object
      Radiance : Vector3d }

type Transform =
    { M : Matrix4d
      Inv : Matrix4d }

type BlinnData = {
    DiffuseColor : Vector3d
    SpecularColor : Vector3d
    Glossiness : float
    Reflection : Vector3d
    Refraction : Vector3d
    Ior : float
    Absorption : Vector3d
}

type Material =
    | Blinn of BlinnData
    | ReflectMaterial of Vector3d
    | Emissive of Vector3d
    
type Primitive =
    | GeometricPrimitive of Object * material : string * areaLight : AreaLight2 option
    | TransformedPrimitive of prim : Primitive * primToWorld : Transform * worldToPrim : Transform
    | PrimitiveList of Primitive list
    | OctreeAgregate of OctreeNode
    | BVHAccelerator of BVHNode

and BVHNode =
    | BVHLeaf of prim : Primitive * bounds : Bounds
    | BVHInterior of left : BVHNode * right : BVHNode * bounds : Bounds

and OctreeNode =
    { Children : OctreeNode []
      Primitives : Primitive list
      Bounds : Bounds }

type HitInfo =
    { T : float
      Point : Vector3d
      Normal : Vector3d
      Material : string
      Prim : Primitive option }

type Bsdf =
    | Diffuse of color : Vector3d