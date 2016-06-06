-------MoyeBuyComLog
USE [Expose178Com];
IF Exists(select 1 from sys.all_objects where name='tbl_Article')
BEGIN
DROP TABLE tbl_Article;
END
Create Table tbl_Article
(
	ArticleID nvarchar(30) primary key not null,
	ArticleTile nvarchar(300) null,
	ArticleBody nvarchar(max) null,
	ArticleDate datetime DEFAULT(GETDATE()),
	--ReadNum int null,
	BackgroundImgUrl nvarchar(300) null,
	IsValidated bit default(1),
	IsDraft bit default(0),
	AritcleTypeCode nvarchar(30) null,
	ReadRoleTypeCode nvarchar(30) null,
	UpdatedByUserID nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_Article_UpdatedByUserID ON tbl_Article(UpdatedByUserID)



---------------tbl_ReplyToArticle
USE [Expose178Com];
IF Exists(select 1 from sys.all_objects where name='tbl_ReplyToArticle')
BEGIN
DROP TABLE tbl_ReplyToArticle;
END
CREATE TABLE tbl_ReplyToArticle
(
	ReplyID nvarchar(30) primary key not null,
	ArticleID nvarchar(30) not null,
	ReplyBody nvarchar(1000) null,
	IsValidated bit default(1),
	UpdatedByUserID nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_ReplyToArticle_UpdatedByUserID ON tbl_ReplyToArticle(UpdatedByUserID)
CREATE INDEX index_tbl_ReplyToArticle_ArticleID ON tbl_ReplyToArticle(ArticleID)

------------------tbl_ArticleAdditional
USE [Expose178Com];
IF Exists(select 1 from sys.all_objects where name='tbl_ArticleAdditional')
BEGIN
DROP TABLE tbl_ArticleAdditional;
END
CREATE TABLE tbl_ArticleAdditional
(
	AdditionalID nvarchar(30) primary key not null,
	ArticleID nvarchar(30) not null,
	ReadNum int null,
	ReplyNum int null,
	UpdatedByUserID nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_ArticleAdditional_ArticleID ON tbl_ArticleAdditional(ArticleID)

--------------------tbl_AritcleType
USE [Expose178Com];
IF Exists(select 1 from sys.all_objects where name='tbl_AritcleType')
BEGIN
DROP TABLE tbl_AritcleType;
END
Create Table tbl_AritcleType
(
	AritcleTypeCode nvarchar(30) primary key not null,
	AritcleTypeDesc nvarchar(100) null,
	UpdatedByUserID nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)

--------------------tbl_ReadRoleType
USE [Expose178Com];
IF Exists(select 1 from sys.all_objects where name='tbl_ReadRoleType')
BEGIN
DROP TABLE tbl_ReadRoleType;
END
Create Table tbl_ReadRoleType
(
	ReadRoleTypeCode nvarchar(30) primary key not null,
	ReadRoleTypeDesc nvarchar(100) null,
	UpdatedByUserID nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)

-------------tbl_User
USE [Expose178Com];
IF Exists(select 1 from sys.all_objects where name='tbl_User')
BEGIN
DROP TABLE tbl_User;
END
Create Table tbl_User
(
	UserID nvarchar(30) primary key not null,
	UserEmail nvarchar(50) unique not null,
	UserName nvarchar(100) null,
	PwdSalt nvarchar(30) not null,
	PwdHash nvarchar(100) not null,
	IsEffective bit not null,
	IsNeedChangePwd bit null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
-------------tbl_TempUser
USE [Expose178Com];
IF Exists(select 1 from sys.all_objects where name='tbl_TempUser')
BEGIN
DROP TABLE tbl_TempUser;
END
Create Table tbl_TempUser
(
	UserID nvarchar(30) primary key not null,
	IP nvarchar(50) null,
	Browser nvarchar(100) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)


--------------Expose178ComLog
USE [Expose178ComLog];
IF Exists(select 1 from sys.all_objects where name='tbl_SystemLog')
BEGIN
DROP TABLE tbl_SystemLog;
END
Create Table tbl_SystemLog
(
	SystemLogID nvarchar(30) primary key not null,
	SystemLogMsg nvarchar(max) null,
	SystemLogPosition nvarchar(500) null,
	UpdatedByUserID nvarchar(30) not null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_SystemLog_UpdatedByUserID ON tbl_SystemLog(UpdatedByUserID)

---------tbl_UserOperatorTrack
IF Exists(select 1 from sys.all_objects where name='tbl_UserOperatorTrack')
BEGIN
DROP TABLE tbl_UserOperatorTrack;
END
CREATE TABLE tbl_UserOperatorTrack
(
	UserOperatorTrackID nvarchar(30) primary key ,
	ParentUserOperatorTrackID nvarchar(30) null,
	UserID nvarchar(30) not null,
	FromURL nvarchar(500) null,
	PageName nvarchar(30) null,
	LastUpdatedDate datetime DEFAULT(GETDATE())
)
CREATE INDEX index_tbl_UserOperatorTrack_UpdatedByUserID ON tbl_UserOperatorTrack(ParentUserOperatorTrackID)
