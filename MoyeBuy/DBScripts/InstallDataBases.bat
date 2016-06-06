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


@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Configuring Logins...                                                       *
@Echo *******************************************************************************
@Echo. 

osql -E -i COMON\LoginUser\LoginUser.sql



@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating Tables...                                                          *
@Echo *******************************************************************************
@Echo. 



osql -E -i COMON\CreateTable\CreateTable.sql


@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Loading Data...                                                             *
@Echo *******************************************************************************
@Echo. 

osql -E -i LookupData\Province.sql
osql -E -i LookupData\OrderLookup.sql
osql -E -i LookupData\MenuLayoutLookup.sql


@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating Procedures...                                                      *
@Echo *******************************************************************************
@Echo. 
@Echo.

osql -E -i COMON\usp_GetRandNum.sql

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating MoyeBuyCom Procedures...                                           *
@Echo *******************************************************************************
@Echo. 
@Echo.
osql -E -i MoyeBuyCom\usp_AddUpdateProductByProductID.sql
osql -E -i MoyeBuyCom\usp_AddUpdateProductCategory.sql
osql -E -i MoyeBuyCom\usp_AddUpdateProductComment.sql
osql -E -i MoyeBuyCom\usp_AddUpdateSupplier.sql
osql -E -i MoyeBuyCom\usp_GetProductByProductIDKeyWords.sql
osql -E -i MoyeBuyCom\usp_GetProductCategory.sql
osql -E -i MoyeBuyCom\usp_GetProductCommentByProductID.sql
osql -E -i MoyeBuyCom\usp_GetProductCommentCatgory.sql
osql -E -i MoyeBuyCom\usp_GetProductStore.sql
osql -E -i MoyeBuyCom\usp_GetProductStoreByProductIDSupplierID.sql
osql -E -i MoyeBuyCom\usp_GetSupplierBySupplierIDName.sql
osql -E -i MoyeBuyCom\usp_UpdateProductStore.sql
osql -E -i MoyeBuyCom\usp_GetMoyeBuyLayoutMenu.sql
osql -E -i MoyeBuyCom\usp_GetMoyeBuyAds.sql
osql -E -i MoyeBuyCom\usp_AddUpdateMoyeBuyMenu.sql
osql -E -i MoyeBuyCom\usp_AddUpdateSubMenuMapping.sql
osql -E -i MoyeBuyCom\usp_DelProductCategoryByID.sql
osql -E -i MoyeBuyCom\usp_DelProductSupplier.sql
osql -E -i MoyeBuyCom\usp_DelProductByID.sql

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating MoyeBuyComLog Procedures...                                        *
@Echo *******************************************************************************
@Echo. 
@Echo.
osql -E -i MoyeBuyComLog\usp_AddSystemLog.sql
osql -E -i MoyeBuyComLog\usp_AddUserOperatorTrack.sql
osql -E -i MoyeBuyComLog\usp_GetUserLogByUID.sql
osql -E -i MoyeBuyComLog\usp_GetUserTrackByUID.sql

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating MoyeBuyComOrder Procedures...                                      *
@Echo *******************************************************************************
@Echo. 
@Echo.
osql -E -i MoyeBuyComOrder\usp_AddOrderDetail.sql
osql -E -i MoyeBuyComOrder\usp_AddUpdateAddressByAddressID.sql
osql -E -i MoyeBuyComOrder\usp_AddUpdateOrderByOrderID.sql
osql -E -i MoyeBuyComOrder\usp_GetOrderDetailInfoByOrderID.sql
osql -E -i MoyeBuyComOrder\usp_GetOrderInfoByUID.sql

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating MoyeBuyComServices Procedures...                                   *
@Echo *******************************************************************************
@Echo. 
@Echo.
osql -E -i MoyeBuyComServices\usp_AddUpdateUserInfoByUserID.sql
osql -E -i MoyeBuyComServices\usp_GetUserByUserIDEmail.sql

@Echo.
@Echo.
@Echo *******************************************************************************
@Echo * Creating GiftCardService Procedures...                                      *
@Echo *******************************************************************************
@Echo. 
@Echo.
osql -E -i GiftCardService\usp_AddUsedCardNo.sql
osql -E -i GiftCardService\usp_GenerateGiftCardNo.sql
osql -E -i GiftCardService\usp_GetGiftCardByNo.sql


@Echo.
@Echo.
@Echo *******************************************************************************
@Echo *                                                                             *
@Echo * Database Setup Complete                                                     *
@Echo *                                                                             *
@Echo *******************************************************************************
@Echo.
pause
