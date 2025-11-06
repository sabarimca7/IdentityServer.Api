@echo.
@echo Setting current directory
CD /D %~dp0
dotnet ef dbcontext scaffold "Server=DESKTOP-M0PFDIJ;Database=AuthDb;User Id=sa;Password=Test@2025;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o "Persistence\Models" -t "OAuth.AppSetting" -t "OAuth.Client" -t "OAuth.ExceptionLog" -t "OAuth.GrantType" -t "OAuth.RefreshToken" -t "OAuth.RequestResponseLog" -t "OAuth.Scope" -t "OAuth.User" -t "OAuth.UserClientScope" -f --context-dir "Persistence\Context" -c AuthDbContext

@echo off
echo.
@echo Model updated.
echo.

pause
