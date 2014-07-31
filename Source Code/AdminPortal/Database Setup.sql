CREATE DATABASE [KinderFinder]
GO

USE [KinderFinder]
GO

CREATE TABLE [Patron] (
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[Surname] [varchar](50) NULL,
	[PasswordHash] [varchar](50) NULL,
	[EmailAddress] [varchar](50) NULL,

	PRIMARY KEY([ID])
)
GO

CREATE TABLE [Tag] (
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CurrentPatron] [int] NULL,
	[Label] [varchar](50) NULL,

	PRIMARY KEY ([ID]),
	FOREIGN KEY ([CurrentPatron]) REFERENCES [Patron]([ID])
)
GO

CREATE TABLE [Map] (
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Data] [image] NULL,
	[Active] [bit] NOT NULL,

	PRIMARY KEY([ID])
)
GO
