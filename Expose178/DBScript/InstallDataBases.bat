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

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating Databases...                                                       *
@Echo *******************************************************************************
@Echo. 

osql -E -i COMON\CreateDataBase\CreateDataBase.sql
pause

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Configuring Logins...                                                       *
@Echo *******************************************************************************
@Echo. 

osql -E -i COMON\LoginUser\LoginUser.sql
pause

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating Tables...                                                          *
@Echo *******************************************************************************
@Echo. 



osql -E -i COMON\CreateTable\CreateTable.sql
pause

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating COMON RandNum Procedures...                                         *
@Echo *******************************************************************************
@Echo. 
@Echo.

osql -E -i COMON\usp_GetRandNum.sql
pause

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating Expose178Com Procedures...                                          *
@Echo *******************************************************************************
@Echo. 
@Echo.
osql -E -i Expose178Com\usp_AddUpdateUserInfoByUserID.sql
osql -E -i Expose178Com\usp_GetUserByUserIDEmail.sql
osql -E -i Expose178Com\usp_AddUpdateAritcleType.sql
osql -E -i Expose178Com\usp_AddUpdateArticleAdditionalByAdditionalID.sql
osql -E -i Expose178Com\usp_AddUpdateArticleByArticleID.sql
osql -E -i Expose178Com\usp_AddUpdateReadRoleType.sql
osql -E -i Expose178Com\usp_AddUpdateReplyToArticleByReplyID.sql
osql -E -i Expose178Com\usp_GetAritcleType.sql
osql -E -i Expose178Com\usp_GetArticleByArticleID.sql
osql -E -i Expose178Com\usp_GetArticleTitle.sql
osql -E -i Expose178Com\usp_GetReadRoleType.sql
osql -E -i Expose178Com\usp_GetReplyToArticle.sql
pause

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating Expose178ComLog Procedures...                                       *
@Echo *******************************************************************************
@Echo. 
@Echo.

osql -E -i Expose178ComLog\usp_AddSystemLog.sql
osql -E -i Expose178ComLog\usp_AddUserOperatorTrack.sql
osql -E -i Expose178ComLog\usp_GetUserLogByUID.sql
osql -E -i Expose178ComLog\usp_GetUserTrackByUID.sql

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating Expose178Com LookupData...                                        *
@Echo *******************************************************************************
@Echo. 
@Echo.

osql -E -i LookupData\AdminUser.sql
osql -E -i LookupData\AritcleType.sql
osql -E -i LookupData\ReadRoleType.sql

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo *                                                                             *
@Echo * Database Setup Complete                                                     *
@Echo *                                                                             *
@Echo *******************************************************************************
@Echo.
pause
