#load "references.fsx"
#load "Render.fs"

open FsCheck
open Render

let revRevIsOrig (xs:list<int>) = List.rev(List.rev xs) = xs



Check.Quick revRevIsOrig