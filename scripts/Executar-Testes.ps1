[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path -Parent $PSScriptRoot
$taskVswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $taskVswhere)) {
    throw 'Instale o Visual Studio com MSBuild e suporte a .NET Framework 4.8.'
}
$taskVsPath = & $taskVswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
if (-not $taskVsPath) { throw 'MSBuild não encontrado pelo vswhere.' }
$taskMsbuild = Join-Path $taskVsPath 'MSBuild\Current\Bin\amd64\MSBuild.exe'
$taskVstest = Join-Path $taskVsPath 'Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe'
if (-not (Test-Path -LiteralPath $taskMsbuild)) { throw "MSBuild ausente: $taskMsbuild" }
if (-not (Test-Path -LiteralPath $taskVstest)) { throw "VSTest ausente: $taskVstest" }

# Evita variáveis Path/PATH duplicadas em terminais hospedados.
$taskPathValue = $env:Path
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:Path = $taskPathValue

Push-Location $taskRoot
try {
    & dotnet test 'MVC_InventoryMasters.Tests\MVC_InventoryMasters.Tests.csproj' --logger 'trx;LogFileName=mvc-integracao.trx' --results-directory TestResults
    if ($LASTEXITCODE -ne 0) { throw 'Falha na suíte MVC.' }

    & $taskMsbuild 'InventoryMastersTests\TCC_Inventory_Masters_Kinect.Tests.csproj' /restore /t:Build /p:Configuration=Debug /p:Platform=x64 /p:UseSharedCompilation=false /m:1 /v:minimal
    if ($LASTEXITCODE -ne 0) { throw 'Falha na compilação Kinect.' }

    & $taskVstest 'InventoryMastersTests\bin\x64\Debug\net48\TCC_Inventory_Masters_Kinect.Tests.dll' /Platform:x64 '/Logger:trx;LogFileName=kinect-integracao.trx' /ResultsDirectory:TestResults
    if ($LASTEXITCODE -ne 0) { throw 'Falha na suíte Kinect.' }
}
finally {
    Pop-Location
}

