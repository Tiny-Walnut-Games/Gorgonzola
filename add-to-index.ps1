# Auto-stage all changes to git index
# Usage: .\add-to-index.ps1 [all|tracked|untracked]

param(
    [string]$Mode = "all"
)

$repo = "E:\Tiny_Walnut_Games\Gorgonzola"
Set-Location $repo

Write-Host "📦 Staging files to git index..." -ForegroundColor Cyan

switch ($Mode.ToLower()) {
    "all" {
        Write-Host "  → Staging ALL changes (modified, new, deleted)"
        git add -A
    }
    "tracked" {
        Write-Host "  → Staging only tracked file changes"
        git add -u
    }
    "untracked" {
        Write-Host "  → Staging only new untracked files"
        git add .
    }
    default {
        Write-Host "  ❌ Invalid mode. Use: all|tracked|untracked" -ForegroundColor Red
        exit 1
    }
}

Write-Host "✅ Done!" -ForegroundColor Green
git status --short