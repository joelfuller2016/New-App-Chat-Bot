# Changelog

## [Unreleased]
- Fixed naming conflict in `Chat.aspx` that prevented build by renaming the response `<pre>` element to `JsonResponse` and updating the code-behind.
- Attempted to build the project but `dotnet` and `msbuild` tools were unavailable in the environment.
- Added explicit `System.Web` reference so the WebForms project compiles with reference assemblies.
- Updated documentation with revised project plan and deep dive analysis.
- Enabled C# 8 syntax by specifying `LangVersion` in project files.
