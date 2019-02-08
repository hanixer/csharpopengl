module Camera

open OpenTK

type Camera(lookFrom : Vector3d, lookAt : Vector3d, up : Vector3d, fov : float, width, height) =

    member this.Nothing x = 0