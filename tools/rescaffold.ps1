<#
.SYNOPSIS
  Re-runs the Database-First scaffold of the SchoolSaaS database and re-distributes the
  output into the Clean Architecture layout (entities -> Domain, DbContext -> Persistence).

.DESCRIPTION
  This project is Database-First: the schema lives in ..\DataBase_Script\*.sql and the
  physical SchoolSaaS database. EF Core entities and the model configuration are generated
  FROM that database, never migrated TO it.

  `dotnet ef dbcontext scaffold` cannot split entities and context across two projects, so
  this script scaffolds into src\SchoolPortal.Persistence\_scaffold\ and then:
    * moves the 20 entity POCOs to src\SchoolPortal.Domain\Entities\  (namespace rewrite)
    * moves SchoolPortalDbContext.cs to src\SchoolPortal.Persistence\ (namespace + using rewrite)
    * re-applies the School <-> AcademicYear 1:many correction (scaffolder mis-infers 1:1
      from the filtered unique index UX_ACADEMIC_YEARS_ONE_CURRENT_PER_SCHOOL)
    * deletes _scaffold\

  Hand-written files are NOT touched and must be reviewed after a schema change:
    * src\SchoolPortal.Domain\Entities\EntityContracts.cs      (marker interface attachment)
    * src\SchoolPortal.Domain\Entities\Behavior\*.cs           (domain factories/methods)
    * src\SchoolPortal.Persistence\SchoolPortalDbContext.Tenancy.cs  (filters + SaveChanges)

.EXAMPLE
  pwsh ./tools/rescaffold.ps1
#>
[CmdletBinding()]
param(
    [string]$ConnectionString = "Server=localhost;Database=SchoolSaaS;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$persistence = Join-Path $root "src/SchoolPortal.Persistence"
$domainEntities = Join-Path $root "src/SchoolPortal.Domain/Entities"
$scaffold = Join-Path $persistence "_scaffold"

Write-Host "==> Scaffolding from SchoolSaaS ..." -ForegroundColor Cyan
Push-Location $persistence
try {
    dotnet ef dbcontext scaffold $ConnectionString Microsoft.EntityFrameworkCore.SqlServer `
        --context SchoolPortalDbContext `
        --output-dir _scaffold/Entities `
        --context-dir _scaffold `
        --namespace SchoolPortal.Persistence._scaffold.Entities `
        --context-namespace SchoolPortal.Persistence._scaffold `
        --no-onconfiguring `
        --force
}
finally {
    Pop-Location
}

Write-Host "==> Distributing entities -> Domain ..." -ForegroundColor Cyan
Get-ChildItem (Join-Path $scaffold "Entities") -Filter *.cs | ForEach-Object {
    (Get-Content $_.FullName -Raw).Replace(
        "namespace SchoolPortal.Persistence._scaffold.Entities;",
        "namespace SchoolPortal.Domain.Entities;"
    ) | Set-Content (Join-Path $domainEntities $_.Name) -NoNewline
}

Write-Host "==> Distributing DbContext -> Persistence ..." -ForegroundColor Cyan
$ctx = (Get-Content (Join-Path $scaffold "SchoolPortalDbContext.cs") -Raw).
    Replace("namespace SchoolPortal.Persistence._scaffold;", "namespace SchoolPortal.Persistence;").
    Replace("using SchoolPortal.Persistence._scaffold.Entities;", "using SchoolPortal.Domain.Entities;")

# Re-apply the School <-> AcademicYear 1:many correction.
$ctx = $ctx.Replace(
    "entity.HasOne(d => d.School).WithOne(p => p.AcademicYear)`r`n                .HasForeignKey<AcademicYear>(d => d.SchoolId)",
    "entity.HasOne(d => d.School).WithMany(p => p.AcademicYears)`r`n                .HasForeignKey(d => d.SchoolId)")
if ($ctx -match "WithOne\(p => p\.AcademicYear\)") {
    Write-Warning "Could not auto-apply the School<->AcademicYear 1:many fix - patch SchoolPortalDbContext.cs and School.cs by hand."
}
$ctx | Set-Content (Join-Path $persistence "SchoolPortalDbContext.cs") -NoNewline

Remove-Item $scaffold -Recurse -Force

Write-Host ""
Write-Host "Done. Now review by hand for any schema changes:" -ForegroundColor Green
Write-Host "  * src/SchoolPortal.Domain/Entities/School.cs           (AcademicYears must be ICollection)"
Write-Host "  * src/SchoolPortal.Domain/Entities/EntityContracts.cs  (new tenant/audited entities?)"
Write-Host "  * src/SchoolPortal.Application/Common/Interfaces/IApplicationDbContext.cs (new DbSets?)"
Write-Host "  * src/SchoolPortal.Persistence/SchoolPortalDbContext.Tenancy.cs"
Write-Host ""
Write-Host "Then: dotnet build && dotnet test" -ForegroundColor Green
