@ECHO OFF
CLS

SET NewVersion=
SET /P NewVersion=What is the new version called? 
SET MvcDir=%CD%\src\ActionMailer.Net.Mvc\bin\Release
SET PostmarkDir=%CD%\src\ActionMailer.Net.Postmark\bin\Release
SET StandaloneDir=%CD%\src\ActionMailer.Net.Standalone\bin\Release
SET NgMvcDir=%CD%\nuget\base\ActionMailer
SET NgPostmarkDir=%CD%\nuget\base\ActionMailer.Postmark
SET NgStandaloneDir=%CD%\nuget\base\ActionMailer.Standalone

ECHO Performing clean release build...

CMD /c C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %CD%\src\ActionMailer.Net.sln /t:Clean /p:Configuration=Release
CMD /c C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %CD%\src\ActionMailer.Net.sln /t:Rebuild /p:Configuration=Release

ECHO Compressing ActionMailer.Net.Mvc to Zip...

CMD /c "%PROGRAMFILES%\7-Zip\7z.exe" a -tzip %MvcDir%\ActionMailer.Net.Mvc-v%NewVersion%.zip %MvcDir%\*.dll %MvcDir%\*.pdb %MvcDir%\*.txt %MvcDir%\*.xml
CMD /c "%PROGRAMFILES%\7-Zip\7z.exe" a -tzip %PostmarkDir%\ActionMailer.Net.Mvc-v%NewVersion%.zip %PostmarkDir%\*.dll %PostmarkDir%\*.pdb %PostmarkDir%\*.txt %PostmarkDir%\*.xml
CMD /c "%PROGRAMFILES%\7-Zip\7z.exe" a -tzip %StandaloneDir%\ActionMailer.Net.Mvc-v%NewVersion%.zip %StandaloneDir%\*.dll %StandaloneDir%\*.pdb %StandaloneDir%\*.txt %StandaloneDir%\*.xml

ECHO Creating NuGet Packages...

CMD /c nuget pack %NgMvcDir%\ActionMailer.nuspec -Version %NewVersion% -BasePath %NgMvcDir% -OutputDirectory %CD%\nuget\output
CMD /c nuget pack %NgPostmarkDir%\ActionMailer.Postmark.nuspec -Version %NewVersion% -BasePath %NgPostmarkDir% -OutputDirectory %CD%\nuget\output
CMD /c nuget pack %NgStandaloneDir%\ActionMailer.Standalone.nuspec -Version %NewVersion% -BasePath %NgStandaloneDir% -OutputDirectory %CD%\nuget\output

ECHO Release v%NewVersion% complete!