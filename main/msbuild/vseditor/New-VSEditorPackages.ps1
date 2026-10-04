<#
.SYNOPSIS
  Builds the local NuGet feed of VS editor packages at $(NuGetVersionVSEditor) (main/Directory.Build.props).

.DESCRIPTION
  That version was only published on a retired internal feed. This script recreates it:
  - public packages: repacks the nuget.org 16.1.89 builds under $(NuGetVersionVSEditor);
  - closed-source Platform.VSEditor / Text.Internal: packs the DLLs of a local Visual Studio 2019 install.
  Dependencies between the packages are rewritten to the same version so restore sees one consistent set.
  Output: main/build/vseditor-feed (gitignored; referenced by NuGet.config). Requires VS 2019 (any 16.x, core editor).
#>
param(
	[string] $VsInstallPath,
	[string] $PublicVersion = '16.1.89',
	[string] $OutputPath = (Join-Path $PSScriptRoot '..\..\build\vseditor-feed')
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem
$mainDir = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$nugetExe = Join-Path $mainDir 'external\nuget-binary\NuGet.exe'

$props = [xml](Get-Content (Join-Path $mainDir 'Directory.Build.props'))
$version = ($props.Project.PropertyGroup | ForEach-Object { $_.NuGetVersionVSEditor } | Where-Object { $_ } | Select-Object -First 1).Trim()
if (-not $version) { throw 'NuGetVersionVSEditor not found in Directory.Build.props' }

$publicIds = @(
	'Microsoft.VisualStudio.CoreUtility'
	'Microsoft.VisualStudio.Text.Data'
	'Microsoft.VisualStudio.Text.Logic'
	'Microsoft.VisualStudio.Text.UI'
	'Microsoft.VisualStudio.Text.UI.Wpf'
	'Microsoft.VisualStudio.Language'
	'Microsoft.VisualStudio.Language.Intellisense'
	'Microsoft.VisualStudio.Language.NavigateTo.Interfaces'
)
$closedIds = @('Microsoft.VisualStudio.Platform.VSEditor', 'Microsoft.VisualStudio.Text.Internal')
$allIds = $publicIds + $closedIds

if (-not $VsInstallPath) {
	$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
	$VsInstallPath = & $vswhere -version '[16.0,17.0)' -products * -latest -property installationPath
	if (-not $VsInstallPath) { throw 'Visual Studio 2019 not found (pass -VsInstallPath).' }
}

$work = Join-Path ([IO.Path]::GetTempPath()) "vseditor-pack-$PID"
New-Item -ItemType Directory -Force $work, $OutputPath | Out-Null
$OutputPath = Resolve-Path $OutputPath

function Set-Dependencies([xml] $nuspec) {
	# point dependencies on packages of this set to $version, so restore doesn't mix versions
	$ns = New-Object Xml.XmlNamespaceManager $nuspec.NameTable
	$ns.AddNamespace('n', $nuspec.DocumentElement.NamespaceURI)
	foreach ($dep in $nuspec.SelectNodes('//n:dependency', $ns)) {
		if ($allIds -contains $dep.id) { $dep.SetAttribute('version', $version) }
		else { Resolve-PublishedMinimum $dep }
	}
}

$script:packageBaseAddresses = $null
function Get-PublishedVersions([string] $id) {
	# versions of $id across the v3 feeds of the repository NuGet.config
	if ($null -eq $script:packageBaseAddresses) {
		$config = [xml](Get-Content (Join-Path $mainDir '..\NuGet.config'))
		$script:packageBaseAddresses = @($config.configuration.packageSources.add | Where-Object { $_.value -match '^https?://.*index\.json$' } | ForEach-Object {
			try { ((Invoke-RestMethod $_.value).resources | Where-Object { $_.'@type' -eq 'PackageBaseAddress/3.0.0' } | Select-Object -First 1).'@id' } catch { }
		} | Where-Object { $_ })
	}
	$lower = $id.ToLowerInvariant()
	$script:packageBaseAddresses | ForEach-Object {
		try { (Invoke-RestMethod "$($_.TrimEnd('/'))/$lower/index.json").versions } catch { }
	} | Sort-Object -Unique
}

function Resolve-PublishedMinimum($dep) {
	# some 16.1.89 dependencies name a minimum that was never published (NU1603, an error with
	# TreatWarningsAsErrors): raise it to the lowest published stable version satisfying the range
	if ($dep.version -notmatch '^\[?([0-9][^,\]\)]*)$') { return }
	$min = [version](($Matches[1] -split '-')[0])
	$versions = Get-PublishedVersions $dep.id
	if (-not $versions -or $versions -contains $Matches[1]) { return }
	$best = $versions | Where-Object { $_ -notmatch '-' -and [version]$_ -ge $min } | Sort-Object { [version]$_ } | Select-Object -First 1
	if ($best) { $dep.SetAttribute('version', $best) }
}

function Invoke-Pack([string] $dir, [string] $nuspecPath) {
	& $nugetExe pack $nuspecPath -BasePath $dir -OutputDirectory $OutputPath -NoDefaultExcludes -NoPackageAnalysis -NonInteractive | Out-Null
	if ($LASTEXITCODE) { throw "nuget pack failed: $nuspecPath" }
}

try {
	foreach ($id in $publicIds) {
		$lower = $id.ToLowerInvariant()
		$nupkg = Join-Path $work "$lower.nupkg"
		Invoke-WebRequest "https://api.nuget.org/v3-flatcontainer/$lower/$PublicVersion/$lower.$PublicVersion.nupkg" -OutFile $nupkg -UseBasicParsing
		$dir = Join-Path $work $lower
		[IO.Compression.ZipFile]::ExtractToDirectory($nupkg, $dir)
		# drop the package signature and OPC metadata: the content changes, nuget pack regenerates the rest
		Remove-Item -Recurse -Force -ErrorAction SilentlyContinue -LiteralPath (Join-Path $dir '.signature.p7s'), (Join-Path $dir '_rels'), (Join-Path $dir 'package'), (Join-Path $dir '[Content_Types].xml')
		$nuspecPath = Get-ChildItem $dir -Filter *.nuspec | Select-Object -First 1 -ExpandProperty FullName
		$nuspec = [xml](Get-Content $nuspecPath)
		$nuspec.package.metadata.version = $version
		Set-Dependencies $nuspec
		$nuspec.Save($nuspecPath)
		Invoke-Pack $dir $nuspecPath
		Write-Host "packed $id $version (from $PublicVersion)"
	}

	$ide = Join-Path $VsInstallPath 'Common7\IDE'
	foreach ($id in $closedIds) {
		$dll = Get-ChildItem $ide -Recurse -Filter "$id.dll" -ErrorAction SilentlyContinue | Select-Object -First 1
		if (-not $dll) { throw "$id.dll not found under $ide" }
		$dir = Join-Path $work $id
		$lib = Join-Path $dir 'lib\net472'
		New-Item -ItemType Directory -Force $lib | Out-Null
		Copy-Item $dll.FullName $lib
		Get-ChildItem $dll.DirectoryName -Directory | ForEach-Object {
			$res = Join-Path $_.FullName "$id.resources.dll"
			if (Test-Path $res) { New-Item -ItemType Directory -Force (Join-Path $lib $_.Name) | Out-Null; Copy-Item $res (Join-Path $lib $_.Name) }
		}
		if ($id -eq 'Microsoft.VisualStudio.Platform.VSEditor') {
			# like the official package: no compile reference (ReferencesVSEditor.Windows.props references the lib DLL explicitly)
			New-Item -ItemType Directory -Force (Join-Path $dir 'ref\net472') | Out-Null
			Set-Content (Join-Path $dir 'ref\net472\_._') ''
		}
		$deps = ($allIds | Where-Object { $_ -ne $id -and $_ -ne 'Microsoft.VisualStudio.Language.NavigateTo.Interfaces' -and $_ -ne 'Microsoft.VisualStudio.Platform.VSEditor' } |
			ForEach-Object { "        <dependency id=`"$_`" version=`"$version`" />" }) -join "`n"
		$fileVersion = $dll.VersionInfo.FileVersion
		$nuspecPath = Join-Path $dir "$id.nuspec"
		Set-Content $nuspecPath @"
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd">
  <metadata>
    <id>$id</id>
    <version>$version</version>
    <authors>Microsoft</authors>
    <description>$id repackaged from a local Visual Studio 2019 install (file version $fileVersion).</description>
    <dependencies>
      <group targetFramework=".NETFramework4.7.2">
$deps
      </group>
    </dependencies>
  </metadata>
</package>
"@
		Invoke-Pack $dir $nuspecPath
		Write-Host "packed $id $version (from $($dll.FullName), $fileVersion)"
	}

	# stale copies in the global packages folder would shadow the regenerated packages
	$globalPackages = if ($env:NUGET_PACKAGES) { $env:NUGET_PACKAGES } else { Join-Path $env:USERPROFILE '.nuget\packages' }
	foreach ($id in $allIds) {
		Remove-Item -Recurse -Force -ErrorAction SilentlyContinue (Join-Path $globalPackages "$($id.ToLowerInvariant())\$version")
	}
} finally {
	Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $work
}

Write-Host "feed: $OutputPath"
