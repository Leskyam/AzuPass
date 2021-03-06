/****** Object:  Database AzuPass    Script Date: 20/11/2007 05:10:16 a.m. ******/
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'AzuPass')
	DROP DATABASE [AzuPass]
GO

CREATE DATABASE [AzuPass]  ON (NAME = N'AzuPass.mdf', FILENAME = N'D:\Archivos de programa\Microsoft SQL Server\MSSQL\Data\AzuPass.mdf' , SIZE = 5, FILEGROWTH = 10%) LOG ON (NAME = N'AzuPass.ldf', FILENAME = N'D:\Archivos de programa\Microsoft SQL Server\MSSQL\Data\AzuPass.ldf' , SIZE = 3, FILEGROWTH = 10%)
 COLLATE Traditional_Spanish_CI_AS
GO

exec sp_dboption N'AzuPass', N'autoclose', N'false'
GO

exec sp_dboption N'AzuPass', N'bulkcopy', N'false'
GO

exec sp_dboption N'AzuPass', N'trunc. log', N'true'
GO

exec sp_dboption N'AzuPass', N'torn page detection', N'true'
GO

exec sp_dboption N'AzuPass', N'read only', N'false'
GO

exec sp_dboption N'AzuPass', N'dbo use', N'false'
GO

exec sp_dboption N'AzuPass', N'single', N'false'
GO

exec sp_dboption N'AzuPass', N'autoshrink', N'true'
GO

exec sp_dboption N'AzuPass', N'ANSI null default', N'false'
GO

exec sp_dboption N'AzuPass', N'recursive triggers', N'false'
GO

exec sp_dboption N'AzuPass', N'ANSI nulls', N'false'
GO

exec sp_dboption N'AzuPass', N'concat null yields null', N'false'
GO

exec sp_dboption N'AzuPass', N'cursor close on commit', N'false'
GO

exec sp_dboption N'AzuPass', N'default to local cursor', N'false'
GO

exec sp_dboption N'AzuPass', N'quoted identifier', N'false'
GO

exec sp_dboption N'AzuPass', N'ANSI warnings', N'false'
GO

exec sp_dboption N'AzuPass', N'auto create statistics', N'true'
GO

exec sp_dboption N'AzuPass', N'auto update statistics', N'true'
GO

if( (@@microsoftversion / power(2, 24) = 8) and (@@microsoftversion & 0xffff >= 724) )
	exec sp_dboption N'AzuPass', N'db chaining', N'false'
GO

use [AzuPass]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Perfil_Tiene_Una_CatOcupacional]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[tbl_Perfil] DROP CONSTRAINT FK_Perfil_Tiene_Una_CatOcupacional
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_tbl_Intereses_tbl_Perfil]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[tbl_Intereses] DROP CONSTRAINT FK_tbl_Intereses_tbl_Perfil
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Perfil_Tiene_Varios_RegistroUso]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[tbl_RegistroUso] DROP CONSTRAINT Perfil_Tiene_Varios_RegistroUso
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Intereses_I    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pa_tbl_Intereses_I]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pa_tbl_Intereses_I]
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_GE    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pa_tbl_Perfil_GE]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pa_tbl_Perfil_GE]
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_GExEmail    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pa_tbl_Perfil_GExEmail]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pa_tbl_Perfil_GExEmail]
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_GxEmailPasswd_Plus    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pa_tbl_Perfil_GxEmailPasswd_Plus]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pa_tbl_Perfil_GxEmailPasswd_Plus]
GO

/****** Object:  Stored Procedure dbo.pa_tbl_RegistroUso_I    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pa_tbl_RegistroUso_I]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pa_tbl_RegistroUso_I]
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_GxEmailPasswd    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pa_tbl_Perfil_GxEmailPasswd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pa_tbl_Perfil_GxEmailPasswd]
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_I    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pa_tbl_Perfil_I]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pa_tbl_Perfil_I]
GO

/****** Object:  Stored Procedure dbo.pa_CatOcupacional_GL    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[pa_CatOcupacional_GL]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[pa_CatOcupacional_GL]
GO

/****** Object:  Table [dbo].[tbl_Intereses]    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tbl_Intereses]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[tbl_Intereses]
GO

/****** Object:  Table [dbo].[tbl_RegistroUso]    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tbl_RegistroUso]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[tbl_RegistroUso]
GO

/****** Object:  Table [dbo].[tbl_Perfil]    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tbl_Perfil]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[tbl_Perfil]
GO

/****** Object:  Table [dbo].[tbl_CatOcupacional]    Script Date: 20/11/2007 05:10:22 a.m. ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tbl_CatOcupacional]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[tbl_CatOcupacional]
GO

/****** Object:  Login usr_wsAzuPass    Script Date: 20/11/2007 05:10:17 a.m. ******/
if not exists (select * from master.dbo.syslogins where loginname = N'usr_wsAzuPass')
BEGIN
	declare @logindb nvarchar(132), @loginlang nvarchar(132) select @logindb = N'AzuPass', @loginlang = N'Español'
	if @logindb is null or not exists (select * from master.dbo.sysdatabases where name = @logindb)
		select @logindb = N'master'
	if @loginlang is null or (not exists (select * from master.dbo.syslanguages where name = @loginlang) and @loginlang <> N'us_english')
		select @loginlang = @@language
	exec sp_addlogin N'usr_wsAzuPass', null, @logindb, @loginlang
END
GO

/****** Object:  Login BUILTIN\Administradores    Script Date: 20/11/2007 05:10:17 a.m. ******/
exec sp_addsrvrolemember N'BUILTIN\Administradores', sysadmin
GO

/****** Object:  User dbo    Script Date: 20/11/2007 05:10:17 a.m. ******/
/****** Object:  User usr_wsAzuPass    Script Date: 20/11/2007 05:10:17 a.m. ******/
if not exists (select * from dbo.sysusers where name = N'usr_wsAzuPass' and uid < 16382)
	EXEC sp_grantdbaccess N'usr_wsAzuPass', N'usr_wsAzuPass'
GO

/****** Object:  Table [dbo].[tbl_CatOcupacional]    Script Date: 20/11/2007 05:10:24 a.m. ******/
CREATE TABLE [dbo].[tbl_CatOcupacional] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Nombre] [varchar] (50) COLLATE Traditional_Spanish_CI_AS NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_Perfil]    Script Date: 20/11/2007 05:10:25 a.m. ******/
CREATE TABLE [dbo].[tbl_Perfil] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Nombre] [varchar] (25) COLLATE Traditional_Spanish_CI_AS NOT NULL ,
	[Apellidos] [varchar] (75) COLLATE Traditional_Spanish_CI_AS NOT NULL ,
	[e_mail] [varchar] (75) COLLATE Traditional_Spanish_CI_AS NOT NULL ,
	[Passwd] [nvarchar] (32) COLLATE Traditional_Spanish_CI_AS NOT NULL ,
	[FechaNac] [smalldatetime] NOT NULL ,
	[Sexo] [char] (1) COLLATE Traditional_Spanish_CI_AS NOT NULL ,
	[IdCatOcupacional] [int] NOT NULL ,
	[FechaHoraRegistro] [smalldatetime] NOT NULL ,
	[RegistradoDesde] [nvarchar] (150) COLLATE Traditional_Spanish_CI_AS NOT NULL ,
	[Habilitado] [bit] NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_Intereses]    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE TABLE [dbo].[tbl_Intereses] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[IdPerfil] [int] NOT NULL ,
	[Intereses] [nvarchar] (500) COLLATE Traditional_Spanish_CI_AS NOT NULL 
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_RegistroUso]    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE TABLE [dbo].[tbl_RegistroUso] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[IdPerfil] [int] NOT NULL ,
	[AppUtilizada] [nvarchar] (150) COLLATE Traditional_Spanish_CI_AS NOT NULL ,
	[FechaHora] [smalldatetime] NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_CatOcupacional] WITH NOCHECK ADD 
	CONSTRAINT [PK_tbl_CatOcupacional] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[tbl_Perfil] WITH NOCHECK ADD 
	CONSTRAINT [PK_tbl_Perfil] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[tbl_Intereses] WITH NOCHECK ADD 
	CONSTRAINT [PK_tbl_Intereses] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[tbl_RegistroUso] WITH NOCHECK ADD 
	CONSTRAINT [PK_tbl_RegistroUso] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[tbl_Perfil] ADD 
	CONSTRAINT [IX_email_tbl_Perfil] UNIQUE  NONCLUSTERED 
	(
		[e_mail]
	)  ON [PRIMARY] 
GO

 CREATE  INDEX [IX_IdCatOcupacional_tbl_Perfil] ON [dbo].[tbl_Perfil]([IdCatOcupacional]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_Intereses] ADD 
	CONSTRAINT [IX_IdPerfil_tbl_Intereses] UNIQUE  NONCLUSTERED 
	(
		[IdPerfil]
	)  ON [PRIMARY] 
GO

 CREATE  INDEX [IX_IdPerfil_tbl_RegistroUso] ON [dbo].[tbl_RegistroUso]([IdPerfil]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_Perfil] ADD 
	CONSTRAINT [FK_Perfil_Tiene_Una_CatOcupacional] FOREIGN KEY 
	(
		[IdCatOcupacional]
	) REFERENCES [dbo].[tbl_CatOcupacional] (
		[ID]
	) ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[tbl_Intereses] ADD 
	CONSTRAINT [FK_tbl_Intereses_tbl_Perfil] FOREIGN KEY 
	(
		[IdPerfil]
	) REFERENCES [dbo].[tbl_Perfil] (
		[ID]
	) ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[tbl_RegistroUso] ADD 
	CONSTRAINT [Perfil_Tiene_Varios_RegistroUso] FOREIGN KEY 
	(
		[IdPerfil]
	) REFERENCES [dbo].[tbl_Perfil] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.pa_CatOcupacional_GL    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE PROCEDURE [dbo].[pa_CatOcupacional_GL] 

AS

SELECT ID AS Id, Nombre AS Descripcion FROM tbl_CatOcupacional
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  EXECUTE  ON [dbo].[pa_CatOcupacional_GL]  TO [usr_wsAzuPass]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_GxEmailPasswd    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE PROCEDURE [dbo].[pa_tbl_Perfil_GxEmailPasswd] 
@Email varchar(75),
@Passwd varchar(32),
@Result int=0 output
AS

SET @Result = (SELECT ID FROM tbl_Perfil WHERE e_mail=@Email AND Passwd=@Passwd)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  EXECUTE  ON [dbo].[pa_tbl_Perfil_GxEmailPasswd]  TO [usr_wsAzuPass]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_I    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE PROCEDURE [pa_tbl_Perfil_I]
	(@ID 			[int] output,
	 @Nombre 		[varchar](25),
	 @Apellidos 		[varchar](75),
	 @e_mail 		[varchar](75),
	 @Passwd 		[nvarchar](32),
	 @FechaNac 		[smalldatetime],
	 @Sexo 		[char](1),
	 @IdCatOcupacional 	[int],
	 @FechaHoraRegistro 	[smalldatetime],
	 @RegistradoDesde 	[nvarchar](150),
	 @Habilitado 		[bit])

AS INSERT INTO [dbo].[tbl_Perfil] 
	 ([Nombre],
	 [Apellidos],
	 [e_mail],
	 [Passwd],
	 [FechaNac],
	 [Sexo],
	 [IdCatOcupacional],
	 [FechaHoraRegistro],
	 [RegistradoDesde],
	 [Habilitado]) 
 
VALUES 
	( @Nombre,
	 @Apellidos,
	 @e_mail,
	 @Passwd,
	 @FechaNac,
	 @Sexo,
	 @IdCatOcupacional,
	 @FechaHoraRegistro,
	 @RegistradoDesde,
	 @Habilitado)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  EXECUTE  ON [dbo].[pa_tbl_Perfil_I]  TO [usr_wsAzuPass]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Intereses_I    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE PROCEDURE [pa_tbl_Intereses_I]
	(@ID 	[int] output,
	 @IdPerfil 	[int],
	 @Intereses 	[nvarchar](500))

AS INSERT INTO [dbo].[tbl_Intereses] 
	 ([IdPerfil],
	 [Intereses]) 
 
VALUES 
	(@IdPerfil,
	 @Intereses)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  EXECUTE  ON [dbo].[pa_tbl_Intereses_I]  TO [usr_wsAzuPass]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_GE    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE PROCEDURE [dbo].[pa_tbl_Perfil_GE] 
@Id int output

AS

SELECT * FROM [tbl_Perfil]
WHERE (ID=@Id)
exec pa_CatOcupacional_GL
SELECT * FROM tbl_Intereses WHERE IdPerfil=@Id
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  EXECUTE  ON [dbo].[pa_tbl_Perfil_GE]  TO [usr_wsAzuPass]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_GExEmail    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE PROCEDURE [dbo].[pa_tbl_Perfil_GExEmail] 
@Email varchar(75) output

AS

SELECT * FROM [tbl_Perfil]
WHERE (e_mail=@Email)
exec pa_CatOcupacional_GL
SELECT * FROM tbl_Intereses WHERE IdPerfil=(SELECT ID FROM [tbl_Perfil] WHERE e_mail=@Email)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  EXECUTE  ON [dbo].[pa_tbl_Perfil_GExEmail]  TO [usr_wsAzuPass]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.pa_tbl_Perfil_GxEmailPasswd_Plus    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE PROCEDURE [dbo].[pa_tbl_Perfil_GxEmailPasswd_Plus] 
@Email varchar(75),
@Passwd varchar(32),
@AppUsed varchar(150),
@Result int=0 output
AS

SET @Result = (SELECT ID FROM tbl_Perfil WHERE e_mail=@Email AND Passwd=@Passwd)
if(@Result<>null)
INSERT INTO tbl_RegistroUso VALUES(@Result, @AppUsed, CONVERT(smalldatetime, GETDATE(), 101))
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  EXECUTE  ON [dbo].[pa_tbl_Perfil_GxEmailPasswd_Plus]  TO [usr_wsAzuPass]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

/****** Object:  Stored Procedure dbo.pa_tbl_RegistroUso_I    Script Date: 20/11/2007 05:10:26 a.m. ******/
CREATE PROCEDURE [pa_tbl_RegistroUso_I]
	(@IdPerfil_1 	[int],
	 @AppUtilizada_2 	[nvarchar](150),
	 @FechaHora_3 	[smalldatetime])

AS INSERT INTO [AzuPass].[dbo].[tbl_RegistroUso] 
	 ( [IdPerfil],
	 [AppUtilizada],
	 [FechaHora]) 
 
VALUES 
	( @IdPerfil_1,
	 @AppUtilizada_2,
	 @FechaHora_3)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  EXECUTE  ON [dbo].[pa_tbl_RegistroUso_I]  TO [usr_wsAzuPass]
GO

