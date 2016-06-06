@echo off
call GetAllFileName.bat

del ListAllSql.txt
for /F "tokens=1-1 delims=" %%i in ('type ListAll.txt ^| find /i ".sql" ') do (
  echo %%i >>ListAllSql.txt
)

del UninstuallSql.txt
for /F "tokens=1-1 delims=" %%i in ('type ListAllSql.txt ^| find /i "drop" ') do (
  echo %%i >>UninstuallSql.txt
)


@Echo Off
cls

REM ***************************************************************************
REM **
REM **        Name: UninstallDatabases
REM **        Desc: SQL Server Database Uninstaller for MoyeBuy.Com Application.
REM **
REM **        Date: 07/22/2012
REM **
REM **************************************************************************/

@Echo.
@Echo *******************************************************************************
@Echo *                                                                             *
@Echo * MoyeBuy.Com Database Uninstall Script                                       *
@Echo *                                                                             *
@Echo *                                                                             *
@Echo * This script will delete MoyeBuy.Com databases                               *
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

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Uninstalling Databases...                                                   *
@Echo *******************************************************************************
@Echo. 

::osql -E -i 1COMON\DropDataBase\MoyeBuyCom.sql
::osql -E -i 1COMON\DropDataBase\MoyeBuyComLog.sql
::osql -E -i 1COMON\DropDataBase\MoyeBuyComOrder.sql
::osql -E -i 1COMON\DropDataBase\MoyeBuyComServices.sql
::osql -E -i 1COMON\DropDataBase\GiftCardService.sql

for /F "tokens=1-1 delims=" %%i in ('type UninstuallSql.txt ^| find /i ".sql"') do (
	echo %%i
	osql -E -i %%i
	
)

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo *                                                                             *
@Echo * Databases Uninstalled                                                       *
@Echo *                                                                             *
@Echo *******************************************************************************
@Echo.
pause