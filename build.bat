SET MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
%MSBUILD% .\WootzJs.Compiler\WootzJs.Compiler.csproj
%MSBUILD% .\WootzJs.Runtime\WootzJs.Runtime.csproj
%MSBUILD% .\WootzJs.System\WootzJs.System.csproj
%MSBUILD% .\WootzJs.ComponentModel.DataAnnotations\WootzJs.ComponentModel.DataAnnotations.csproj
%MSBUILD% .\WootzJs.Web\WootzJs.Web.csproj
%MSBUILD% .\WootzJs.Web\WootzJs.Testing.csproj
%MSBUILD% .\WootzJs.Web\WootzJs.Injection.csproj
%MSBUILD% .\WootzJs.Web\WootzJs.JQuery.csproj
%MSBUILD% .\WootzJs.Rx\WootzJs.Rx.csproj
%MSBUILD% .\WootzJs.Rx.Tests\WootzJs.Rx.Tests.csproj
%MSBUILD% .\WootzJs.Models\WootzJs.Models.csproj
%MSBUILD% .\WootzJs.Mvc\WootzJs.Mvc.csproj
%MSBUILD% .\WootzJs.Compiler.Tests\WootzJs.Compiler.Tests.csproj
rem %MSBUILD% .\WootzJs.sln
