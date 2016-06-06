@echo off
call GetAllFileName.bat

del ListAllSql.txt
for /F "tokens=1-1 delims=" %%i in ('type ListAll.txt ^| find /i ".sql" ') do (
  echo %%i >>ListAllSql.txt
)

del InstuallSql.txt
for /F "tokens=1-1 delims=" %%i in ('type ListAllSql.txt ^| find /i /V "drop" ') do (
  echo %%i >>InstuallSql.txt
)


@Echo Off
cls

REM ***************************************************************************
REM **
REM **        Name: InstallDatabases
REM **        Desc: SQL Server Database Setup for MoyeBuy.Com Application.
REM **
REM **        Date: 07/22/2012
REM **
REM **************************************************************************/

@Echo.
@Echo *******************************************************************************
@Echo *                                                                             *
@Echo * MoyeBuy.Com Database Setup Script                                           *
@Echo *                                                                             *
@Echo *                                                                             *
@Echo * This script will create MoyeBuy.Com databases and register them             *
@Echo * for Sql Cache Dependency. You should have SQL Server 2005 installed         *
@Echo * on you local machine as default instance.                                   *
@Echo *                                                                             *
@Echo * Modify this script to reflect the way you connect to the SQL Server         *
@Echo * as well as if location of the .NET 2.0 runtime on your computer             *
@Echo * is different from C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727.            *
@Echo *                                                                             *
@Echo * If you wish to cancel, press [CTRL]-C to terminate the batch job.           *
@Echo *                                                                             *
@Echo *******************************************************************************
@Echo. 
pause

for /F "tokens=1-1 delims=" %%i in ('type InstuallSql.txt ^| find /i ".sql"') do (
	echo %%i
	osql -E -i %%i
)

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo *                                                                             *
@Echo * Database Setup Complete                                                     *
@Echo *                                                                             *
@Echo *******************************************************************************
@Echo.
pause