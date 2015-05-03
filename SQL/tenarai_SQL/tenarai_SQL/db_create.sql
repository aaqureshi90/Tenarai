CREATE TYPE [dbo].[DT_CODE01] FROM [char](1) NULL
GO

CREATE TYPE [dbo].[DT_COUNT] FROM [tinyint] NOT NULL
GO

CREATE TYPE [dbo].[DT_DATETIME] FROM [datetime] NULL
GO

CREATE TYPE [dbo].[DT_DESCRIPTION] FROM [varchar](80) NULL
GO

CREATE TYPE [dbo].[DT_EMAIL] FROM [varchar](254) NULL
GO

CREATE TYPE [dbo].[DT_FREQUENCY] FROM [bigint] NULL
GO

CREATE TYPE [dbo].[DT_NAME] FROM [varchar](32) NULL
GO

CREATE TYPE [dbo].[DT_PASSWORD] FROM [varchar](256) NULL
GO

CREATE TYPE [dbo].[DT_PHONE] FROM [varchar](13) NULL
GO

CREATE TYPE [dbo].[DT_QUESTION] FROM [varchar](80) NULL
GO

CREATE TYPE [dbo].[DT_SYMBOL] FROM [nvarchar](1) NOT NULL
GO

CREATE TYPE [dbo].[DT_TIME] FROM [binary](8) NOT NULL
GO

CREATE TYPE [dbo].[DT_USERNAME] FROM [varchar](32) NULL
GO

CREATE TYPE [dbo].[DT_YESNO] FROM [char](1) NOT NULL
GO

CREATE TYPE [dbo].[DT_YESNOBLANK] FROM [char](1) NULL
GO


/*********************************************************
 *                Create all the tables                  *
 ********************************************************/
CREATE TABLE [dbo].[tblComponent](
	[cmp_COMPONENT] [dbo].[DT_SYMBOL] NOT NULL,
	[cmp_IS_RADICAL] [dbo].[DT_YESNO] NOT NULL,
	[cmp_STROKE_COUNT] [dbo].[DT_COUNT] NOT NULL
)
GO


CREATE TABLE [dbo].[tblComponentPermutation](
	[cpr_COMPONENT] [dbo].[DT_SYMBOL] NOT NULL,
	[cpr_PERMUTATION] [dbo].[DT_SYMBOL] NOT NULL
)
GO


CREATE TABLE [dbo].[tblEmailType](
	[emt_CODE] [dbo].[DT_CODE01] NOT NULL,
	[emt_DESCRIPTION] [dbo].[DT_DESCRIPTION] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[emt_CODE] ASC
	)
)
GO


CREATE TABLE [dbo].[tblKanji](
	[kan_KANJI] [dbo].[DT_SYMBOL] NOT NULL,
	[kan_STROKE_COUNT] [dbo].[DT_COUNT] NOT NULL,
	[kan_GRADE_LEARNED] [dbo].[DT_COUNT] NULL
)
GO


CREATE TABLE [dbo].[tblPhoneType](
	[pht_CODE] [dbo].[DT_CODE01] NOT NULL,
	[pht_DESCRIPTION] [dbo].[DT_DESCRIPTION] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[pht_CODE] ASC
	)
) 
GO


CREATE TABLE [dbo].[tblSecurityQuestion](
	[srq_QUESTION] [dbo].[DT_QUESTION] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[srq_QUESTION] ASC
	)
)
GO



CREATE TABLE [dbo].[tblTestKanji](
	[kan_Kanji] [dbo].[DT_SYMBOL] NOT NULL,
	[kan_Component] [dbo].[DT_SYMBOL] NOT NULL,
	[del_key] [tinyint] NULL
)
GO



CREATE TABLE [dbo].[tblUser](
	[usr_USERNAME] [dbo].[DT_USERNAME] NOT NULL,
	[usr_PASSWORD_ENCRYPTED] [dbo].[DT_PASSWORD] NULL,
	[usr_FIRST_NAME] [dbo].[DT_NAME] NULL,
	[usr_LAST_NAME] [dbo].[DT_NAME] NULL,
	[usr_CREATED_DATE] [dbo].[DT_DATETIME] NOT NULL,
	[usr_LAST_LOGIN_DATE] [dbo].[DT_DATETIME] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[usr_USERNAME] ASC
	)
)
GO


CREATE TABLE [dbo].[tblUserEmail](
	[usr_USERNAME] [dbo].[DT_USERNAME] NOT NULL,
	[emt_CODE] [dbo].[DT_CODE01] NOT NULL,
	[uem_EMAIL_ADDRESS] [dbo].[DT_EMAIL] NOT NULL,
	 CONSTRAINT [pk_tblUserEmail] PRIMARY KEY CLUSTERED 
	(
		[usr_USERNAME] ASC,
		[emt_CODE] ASC
	)
)
GO

ALTER TABLE [dbo].[tblUserEmail]  WITH CHECK ADD  CONSTRAINT [fk_tblEmailType_tblUserEmail] FOREIGN KEY([emt_CODE])
REFERENCES [dbo].[tblEmailType] ([emt_CODE])
GO

ALTER TABLE [dbo].[tblUserEmail] CHECK CONSTRAINT [fk_tblEmailType_tblUserEmail]
GO

ALTER TABLE [dbo].[tblUserEmail]  WITH CHECK ADD  CONSTRAINT [fk_tblUser_tblUserEmail] FOREIGN KEY([usr_USERNAME])
REFERENCES [dbo].[tblUser] ([usr_USERNAME])
GO

ALTER TABLE [dbo].[tblUserEmail] CHECK CONSTRAINT [fk_tblUser_tblUserEmail]
GO


CREATE TABLE [dbo].[tblUserPhone](
	[usr_USERNAME] [dbo].[DT_USERNAME] NOT NULL,
	[pht_CODE] [dbo].[DT_CODE01] NOT NULL,
	[uph_PHONE_NUMBER] [dbo].[DT_PHONE] NOT NULL,
	CONSTRAINT [pk_tblUserPhone] PRIMARY KEY CLUSTERED 
	(
		[usr_USERNAME] ASC,
		[pht_CODE] ASC
	)
) 
GO

ALTER TABLE [dbo].[tblUserPhone]  WITH CHECK ADD  CONSTRAINT [fk_tblPhoneType_tblUserPhone] FOREIGN KEY([pht_CODE])
REFERENCES [dbo].[tblPhoneType] ([pht_CODE])
GO

ALTER TABLE [dbo].[tblUserPhone] CHECK CONSTRAINT [fk_tblPhoneType_tblUserPhone]
GO

ALTER TABLE [dbo].[tblUserPhone]  WITH CHECK ADD  CONSTRAINT [fk_tblUser_tblUserPhone] FOREIGN KEY([usr_USERNAME])
REFERENCES [dbo].[tblUser] ([usr_USERNAME])
GO

ALTER TABLE [dbo].[tblUserPhone] CHECK CONSTRAINT [fk_tblUser_tblUserPhone]
GO



CREATE TABLE [dbo].[tblUserSecurityQuestion](
	[usr_USERNAME] [dbo].[DT_USERNAME] NOT NULL,
	[srq_QUESTION] [dbo].[DT_QUESTION] NOT NULL,
	[srq_ANSWER] [varchar](32) NOT NULL,
	[srq_SEQUENCE] [int] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[usr_USERNAME] ASC,
		[srq_QUESTION] ASC,
		[srq_SEQUENCE] ASC
	)
)
GO

ALTER TABLE [dbo].[tblUserSecurityQuestion]  WITH CHECK ADD  CONSTRAINT [fk_tblUser_tblUserSecurityQuestion] FOREIGN KEY([usr_USERNAME])
REFERENCES [dbo].[tblUser] ([usr_USERNAME])
GO

ALTER TABLE [dbo].[tblUserSecurityQuestion] CHECK CONSTRAINT [fk_tblUser_tblUserSecurityQuestion]
GO

