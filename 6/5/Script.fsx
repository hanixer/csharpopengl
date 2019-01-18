#load "../references.fsx"
// #load "Render.fs"
#load "Camera.fs"

// open FsCheck
// open Render

// let revRevIsOrig (xs:list<int>) = List.rev(List.rev xs) = xs



// Check.Quick revRevIsOrig

open OpenTK
open System


let sleepWorkflow  = async{
    printfn "Starting sleep workflow at %O" DateTime.Now.TimeOfDay
    do! Async.Sleep 2000
    printfn "Finished sleep workflow at %O" DateTime.Now.TimeOfDay
    }

let nestedWorkflow  = async{

    printfn "Starting parent"
    let! childWorkflow = Async.StartChild sleepWorkflow

    // give the child a chance and then keep working
    do! Async.Sleep 100
    printfn "Doing something useful while waiting "

    // block on the child
    let! result = childWorkflow

    // done
    printfn "Finished parent" 
    }

let testLoop = async {
    for i in [1..100] do
        // do something
        printf "%i before.." i
        
        // sleep a bit
        do! Async.Sleep 10  
        printfn "..after"
    }
Async.RunSynchronously testLoop