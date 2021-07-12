FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5000"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5001"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5002"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5003"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5004"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5005"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5006"`) DO taskkill /F /PID %%i
FOR /F "usebackq tokens=5" %%i IN (`netstat -aon ^| find "5007"`) DO taskkill /F /PID %%i


 
 
echo "Starting"

cd Services/Gateway/
start /B dotnet run

cd ../../Services/iTodo/  
start /B dotnet run

cd ../../Services/PersonalAssistant/  
start /B dotnet run

call :wait

@REM cd ../../__Test__/Services/iTodoTest/
@REM start /B dotnet test --filter "WhenSelectGroupShouldGetBoardByMember2"

echo "Finishing"