FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5000"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5001"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5002"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5003"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5004"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5005"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5006"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5007"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5008"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5009"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5010"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5011"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5012"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5013"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5014"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5015"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5016"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5017"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5018"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5019"`) DO taskkill /F /PID %%i


 @echo off
set "p=%1"
if "%p%"=="" set /p "p=Enter Value: " || set "p=Test"
echo %p%
 
echo "Starting"

cd Services/Gateway/
start /B dotnet run %p%

cd ../../Services/iTodo/
start /B dotnet run %p%

cd ../../Services/PersonalAssistant/
start /B dotnet run %p%

cd ../../Services/RepeatManager/
start /B dotnet run %p%


cd ../../Services/iMemory/
start /B dotnet run %p%

cd ../../Services/iGoal/
start /B dotnet run %p%

cd ../../Services/iGroup/
start /B dotnet run %p%

cd ../../Services/iAssistant/
start /B dotnet run %p%

cd ../../Services/iLocation/
start /B dotnet run %p%

call :wait

@REM cd ../../__Test__/Services/iTodoTest/
@REM start /B dotnet test --filter "WhenSelectGroupShouldGetBoardByMember2"

echo "Finishing"