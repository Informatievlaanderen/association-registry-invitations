#r "paket:
version 7.0.2
framework: net6.0
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 6.0.3 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.FileSystemOperators
open Fake.DotNet
open ``Build-generic``

let product = "Basisregisters Vlaanderen"
let copyright = "Copyright (c) Vlaamse overheid"
let company = "Vlaamse overheid"

let dockerRepository = "association-registry"
let assemblyVersionNumber = (sprintf "%s.0")
let nugetVersionNumber = (sprintf "%s")

let buildSolution = buildSolution assemblyVersionNumber
let buildSource = build assemblyVersionNumber
let buildTest = buildTest assemblyVersionNumber
let setVersions = (setSolutionVersions assemblyVersionNumber product copyright company)
let test = testSolution
let publishSource = publish assemblyVersionNumber
let pack = pack nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

supportedRuntimeIdentifiers <- [ "msil"; "linux-x64" ]

let testWithDotNet path =
  let fxVersion = getDotNetClrVersionFromGlobalJson()

  let cmd = sprintf "test --no-build --no-restore --logger trx --configuration Release --no-build --no-restore /p:RuntimeFrameworkVersion=%s --dcReportType=HTML" fxVersion

  let result = DotNet.exec (id) "dotcover" cmd
  if result.ExitCode <> 0 then failwith "Test Failure"

let testSolution sln =
  testWithDotNet (sprintf "%s.sln" sln)

// Solution -----------------------------------------------------------------------

Target.create "Restore_Solution" (fun _ -> restore "AssociationRegistry")

Target.create "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  buildSolution "AssociationRegistry.Invitations")

Target.create "SetSolutionInfo" (fun _ ->
  setVersions "SolutionInfo.cs"
 )


Target.create "Test_Solution" (fun _ -> testSolution "AssociationRegistry")

Target.create "Publish_Solution" (fun _ ->
  [
    "AssociationRegistry.Invitations"
  ] |> List.iter publishSource)

Target.create "Containerize_InvitationsApi" (fun _ -> containerize "AssociationRegistry.Invitations" "invitations-api")
Target.create "PushContainer_InvitationsApi" (fun _ -> push "invitations-api")


// --------------------------------------------------------------------------------

Target.create "Build" ignore
Target.create "Test" ignore
Target.create "Publish" ignore
Target.create "Pack" ignore
Target.create "Containerize" ignore
Target.create "Push" ignore

"NpmInstall"
  ==> "DotNetCli"
  ==> "Clean"
  ==> "Restore_Solution"
  ==> "Build_Solution"
  ==> "Build"

"Build"
  ==> "Test_Solution"
  ==> "Test"

"Test"
  ==> "Publish_Solution"
  ==> "Publish"

"Publish"
  ==> "Pack"

"Containerize"
  ==> "DockerLogin"
  ==> "PushContainer_InvitationsApi"
  ==> "Push"
// Possibly add more projects to push here

// By default we build & test
Target.runOrDefault "Test"
