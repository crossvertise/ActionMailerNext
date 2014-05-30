@ECHO OFF
CLS

SET NewVersion=
SET /P NewVersion=What is the new version called? 
SET MvcDir=%CD%\src\ActionMailer.Net.Mvc\bin\Release
SET Mvc4Dir=%CD%\src\ActionMailer.Net.Mvc4\bin\Release
SET PostmarkDir=%CD%\src\ActionMailer.Net.Postmark\bin\Release
SET StandaloneDir=%CD%\src\ActionMailer.Net.Standalone\bin\Release
SET NgMvcDir=%CD%\nuget\base\ActionMailer
SET NgMvc4Dir=%CD%\nuget\base\ActionMailer.Mvc4
SET NgPostmarkDir=%CD%\nuget\base\ActionMailer.Postmark
SET NgStandaloneDir=%CD%\nuget\base\ActionMailer.Standalone

ECHO Performing clean release build...

CMD /c %WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %CD%\src\ActionMailer.Net.sln /t:Clean /p:Configuration=Release
CMD /c %WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %CD%\src\ActionMailer.Net.sln /t:Rebuild /p:Configuration=Release

ECHO Compressing ActionMailer.Net.Mvc to Zip...

CMD /c "%PROGRAMFILES%\7-Zip\7z.exe" a -tzip %MvcDir%\ActionMailer.Net.Mvc-v%NewVersion%.zip %MvcDir%\*.dll %MvcDir%\*.pdb %MvcDir%\*.txt %MvcDir%\*.xml
CMD /c "%PROGRAMFILES%\7-Zip\7z.exe" a -tzip %MvcDir%\ActionMailer.Net.Mvc4-v%NewVersion%.zip %Mvc4Dir%\*.dll %Mvc4Dir%\*.pdb %Mvc4Dir%\*.txt %Mvc4Dir%\*.xml
CMD /c "%PROGRAMFILES%\7-Zip\7z.exe" a -tzip %PostmarkDir%\ActionMailer.Net.Postmark-v%NewVersion%.zip %PostmarkDir%\*.dll %PostmarkDir%\*.pdb %PostmarkDir%\*.txt %PostmarkDir%\*.xml
CMD /c "%PROGRAMFILES%\7-Zip\7z.exe" a -tzip %StandaloneDir%\ActionMailer.Net.Standalone-v%NewVersion%.zip %StandaloneDir%\*.dll %StandaloneDir%\*.pdb %StandaloneDir%\*.txt %StandaloneDir%\*.xml

ECHO Creating NuGet Packages...

DEL /Q %NgMvcDir%\lib\Net40\*.*
DEL /Q %NgMvc4Dir%\lib\Net45\*.*
DEL /Q %NgPostmarkDir%\lib\Net40\*.*
DEL /Q %NgStandaloneDir%\lib\Net40\*.*

COPY %MvcDir%\ActionMailer*.dll %NgMvcDir%\lib\Net40\
COPY %MvcDir%\ActionMailer*.pdb %NgMvcDir%\lib\Net40\
COPY %MvcDir%\ActionMailer*.xml %NgMvcDir%\lib\Net40\
COPY %Mvc4Dir%\ActionMailer*.dll %NgMvc4Dir%\lib\Net45\
COPY %Mvc4Dir%\ActionMailer*.pdb %NgMvc4Dir%\lib\Net45\
COPY %Mvc4Dir%\ActionMailer*.xml %NgMvc4Dir%\lib\Net45\
COPY %PostmarkDir%\ActionMailer*.dll %NgPostmarkDir%\lib\Net40\
COPY %PostmarkDir%\ActionMailer*.pdb %NgPostmarkDir%\lib\Net40\
COPY %PostmarkDir%\ActionMailer*.xml %NgPostmarkDir%\lib\Net40\
COPY %StandaloneDir%\ActionMailer*.dll %NgStandaloneDir%\lib\Net40\
COPY %StandaloneDir%\ActionMailer*.xml %NgStandaloneDir%\lib\Net40\
COPY %StandaloneDir%\ActionMailer*.pdb %NgStandaloneDir%\lib\Net40\

CMD /c nuget pack %NgMvcDir%\ActionMailer.nuspec -Version %NewVersion% -BasePath %NgMvcDir% -OutputDirectory %CD%\nuget\output
CMD /c nuget pack %NgMvc4Dir%\ActionMailer.Mvc4.nuspec -Version %NewVersion% -BasePath %NgMvc4Dir% -OutputDirectory %CD%\nuget\output
CMD /c nuget pack %NgPostmarkDir%\ActionMailer.Postmark.nuspec -Version %NewVersion% -BasePath %NgPostmarkDir% -OutputDirectory %CD%\nuget\output
CMD /c nuget pack %NgStandaloneDir%\ActionMailer.Standalone.nuspec -Version %NewVersion% -BasePath %NgStandaloneDir% -OutputDirectory %CD%\nuget\output

ECHO Release v%NewVersion% complete!