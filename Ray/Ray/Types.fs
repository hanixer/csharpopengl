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

type HitInfo =
    { T : float
      Point : Vector3d
      Normal : Vector3d
      Material : string }

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

type OctreeNode =
    { Children : OctreeNode []
      Primitives : Primitive list
      Bounds : Bounds }

and BVHNode =
    | BVHLeaf of prim : Primitive * bounds : Bounds
    | BVHInterior of left : BVHNode * right : BVHNode * bounds : Bounds

and Primitive =
    | GeometricPrimitive of Object * material : string * areaLight : AreaLight2 option
    | TransformedPrimitive of prim : Primitive * primToWorld : Transform * worldToPrim : Transform
    | PrimitiveList of Primitive list
    | OctreeAgregate of OctreeNode
    | BVHAccelerator of BVHNode
