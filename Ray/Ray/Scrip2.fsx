#load "..\\references.fsx"

open Transform
open OpenTK
// float imageAspectRatio = imageWidth / (float)imageHeight; // assuming width > height
// float Px = (2 * ((x + 0.5) / imageWidth) - 1) * tan(fov / 2 * M_PI / 180) * imageAspectRatio;
// float Py = (1 - 2 * ((y + 0.5) / imageHeight) * tan(fov / 2 * M_PI / 180);

let near = 1e-2
let far = 1e4
let p = perspective 0.0 near far
let v = Vector3d(1., 2., -4e-2)
let v1 = transformPoint p v
let v2 = transformPointInv p v1
(far - near) * 0.5