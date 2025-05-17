# Changelog

## [Unreleased]
- Fixed naming conflict in `Chat.aspx` that prevented build by renaming the response `<pre>` element to `JsonResponse` and updating the code-behind.
- Attempted to build the project but `dotnet` and `msbuild` tools were unavailable in the environment.
