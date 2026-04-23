# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$slnPath = Join-Path $packFolder "../"
$srcPath = Join-Path $slnPath "src"
$dstPackFolder = Join-Path $packFolder "../../aspnet-core/nupkg"



# List of projects
$projects = (
    "ADTOSharp",
    "ADTOSharp.AspNetCore",
    "ADTOSharp.AspNetCore.SignalR",
    "ADTOSharp.AspNetCore.PerRequestRedisCache",
    "ADTOSharp.AutoMapper",
    "ADTOSharp.Castle.Log4Net",
    "ADTOSharp.EntityFramework.Common",
    "ADTOSharp.EntityFrameworkCore",
    "ADTOSharp.RedisCache",
    "ADTOSharp.RedisCache.ProtoBuf",
    "ADTOSharp.Quartz",
    "ADTOSharp.Web.Common",
    "ADTOSharp.Zero.Common",
    "ADTOSharp.ZeroCore",
    "ADTOSharp.ZeroCore.EntityFrameworkCore",
	"ADTOSharp.MailKit",
	"ADTOSharp.Castle.Serilog",
	"ADTOSharp.Snowflakes",
	"ADTOSharp.HangFire",
	"ADTOSharp.HangFire.AspNetCore",
	"ADTOSharp.Dapper",
	"ADTO.OpenIddict",
	"ADTO.OpenIddict.EntityFrameworkCore",
	"ADTO.AspNetCore.OpenIddict",
	"ADTO.Swashbuckle",
	"ADTO.DistributedLocking.Abstractions",
	"ADTO.DistributedLocking"
)

# Rebuild solution
Set-Location $slnPath
& dotnet restore

# Copy all nuget packages to the pack folder
foreach($project in $projects) {
    
    $projectFolder = Join-Path $srcPath $project

    # Create nuget pack
    Set-Location $projectFolder
    Get-ChildItem (Join-Path $projectFolder "bin/Release") -ErrorAction SilentlyContinue | Remove-Item -Recurse
    & dotnet msbuild /p:Configuration=Release
    & dotnet msbuild /p:Configuration=Release /t:pack /p:IncludeSymbols=true /p:SymbolPackageFormat=snupkg

    # Copy nuget package
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $project + ".*.nupkg")
    Move-Item $projectPackPath $dstPackFolder

	# Copy symbol package
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $project + ".*.snupkg")
    Move-Item $projectPackPath $dstPackFolder
	
	
	
}

# Go back to the pack folder
Set-Location $packFolder