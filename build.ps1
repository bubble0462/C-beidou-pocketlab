# 编译脚本 - 基于北斗时空数据的深远海养殖辅助分析终端软件
# 使用 Windows 自带 .NET Framework 4.x 编译器

$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if (-not (Test-Path $csc)) {
    $csc = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
}

if (-not (Test-Path $csc)) {
    Write-Host "错误: 未找到 csc.exe 编译器，请确认已安装 .NET Framework 4.x" -ForegroundColor Red
    exit 1
}

Write-Host "使用编译器: $csc" -ForegroundColor Cyan

# 创建输出目录
if (-not (Test-Path "bin")) {
    New-Item -ItemType Directory -Path "bin" | Out-Null
}

# 编译
$sourceFiles = @(
    "Program.cs",
    "MainForm.cs",
    "AppTheme.cs",
    "DemoData.cs"
)

$references = @(
    "/r:System.dll",
    "/r:System.Data.dll",
    "/r:System.Drawing.dll",
    "/r:System.Windows.Forms.dll"
)

$output = "bin\DeepSeaAquacultureTerminal.exe"

Write-Host "正在编译..." -ForegroundColor Yellow

& $csc /nologo /target:winexe /out:$output $references /platform:x64 $sourceFiles

if ($LASTEXITCODE -eq 0) {
    Write-Host "编译成功!" -ForegroundColor Green
    Write-Host "输出文件: $output" -ForegroundColor Cyan
    Write-Host "文件大小: $((Get-Item $output).Length / 1KB) KB" -ForegroundColor Cyan
} else {
    Write-Host "编译失败，请检查错误信息。" -ForegroundColor Red
    exit 1
}
