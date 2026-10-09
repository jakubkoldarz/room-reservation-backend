$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

Remove-Item -Recurse -Force TestResults, CoverageReport -ErrorAction SilentlyContinue

dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings --results-directory TestResults

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html
Start-Process "CoverageReport/index.html"
