#load "..\\references.fsx"

open FSharp.Charting
open System

Chart.Line [ for x in 1.0 .. 100.0 -> (x, x ** 2.0) ]