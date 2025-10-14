$publishDir = "$PSScriptRoot/publish"

if (Test-Path $publishDir -PathType Container) {
	Remove-Item $publishDir -Force -Recurse
}

New-Item $publishDir -ItemType Directory
New-Item "$publishDir/AudioOracleCompanion" -ItemType Directory

dotnet publish "$PSScriptRoot/Tools/AudioOracleCompanion/AudioOracleCompanion.csproj" `
	--configuration Release `
	--framework 'net9.0-windows' `
	--runtime 'win-x64' `
	--self-contained true `
	--output "$publishDir/AudioOracleCompanion" `
	/p:PublishSingleFile=false ` # Can't get assembly location (for config file path) when single file I guess...
	/p:PublishTrimmed=false # WPF doesn't support trimming, naturally...

docker compose -f "$PSScriptRoot/docker-compose.yml" build

docker save -o "$publishDir/audio-oracle.tar" 'audio-oracle:latest'
