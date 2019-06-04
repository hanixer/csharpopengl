#load @"..\references.fsx"

open Transform
open OpenTK
open Bounds

// (0.00, 0.00, 2.00) (0.03, -0.20, -0.98) (-1.00, -1.00, -1.00) (1.00, 1.00, 1.00)

let b1 = makeBounds (Vector3d(-1., -1., -1.)) (Vector3d(1., 1.,1.))
let r = Common.makeRay (Vector3d(0.,0.,2.)) (Vector3d(0.03,-0.2,-0.98).Normalized())
let res2 = hitBoundingBox r b1

// Generate a list of 100 integers
let fullList = [ 1 .. 100 ]

// Create a slice from indices 1-5 (inclusive)
let smallSlice = fullList.[1..5]
printfn "Small slice: %A" smallSlice

// Create a slice from the beginning to index 5 (inclusive)
let unboundedBeginning = fullList.[..5]
printfn "Unbounded beginning slice: %A" unboundedBeginning

// Create a slice from an index to the end of the list
let unboundedEnd = fullList.[94..]
printfn "Unbounded end slice: %A" unboundedEnd

let bigList = [ 1 .. 10000000 ]
let bigList2 = [ 1 .. 10000000 ]
let slice = bigList2.[1..200]