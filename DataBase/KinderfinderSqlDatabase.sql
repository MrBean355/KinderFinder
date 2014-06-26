/*
SELECT * FROM [Restaurant]
SELECT * FROM [ApplicationUser]
SELECT * FROM [Tag]
*/
DROP Database KinderFinder
GO

CREATE Database KinderFinder
GO 

USE KinderFinder
GO

CREATE TABLE [Restaurant] (
	[ID] int IDENTITY(1, 1),
	[Name] varchar(50),

	PRIMARY KEY([ID])
) 

CREATE TABLE [ApplicationUser] (
	[ID] int IDENTITY(1, 1),
	[Name] varchar(50),
	[Surname] varchar(50),
	[Password] varchar(50),
	[PhoneModel] varchar(50),
	[PhoneNumber] int,

	PRIMARY KEY([ID])
) 

CREATE TABLE [Tag] (
	[ID] int IDENTITY(1, 1),
	[User] int,

	PRIMARY KEY([ID]),
	FOREIGN KEY([User]) REFERENCES [ApplicationUser]([ID])
)

INSERT into [Restaurant] VALUES ('Spur')
INSERT into [Restaurant] VALUES ('Steers')
INSERT into [Restaurant] VALUES ('Nandos')

INSERT into [ApplicationUser] VALUES ('Justin','Timberlake', 'password', 'phone1', 1234567890)
INSERT into [ApplicationUser] VALUES ('Mariah', 'Carrey', 'password', 'phone1', 1234567890)
INSERT into [ApplicationUser] VALUES ('2', 'Pac', 'password', 'phone1', 1234567890)
INSERT into [ApplicationUser] VALUES ('Britany', 'Spears', 'password', 'phone1', 1234567890)
INSERT into [ApplicationUser] VALUES ('Bob', 'Marley', 'password', 'phone1', 1234567890)
INSERT into [ApplicationUser] VALUES ('Andrea', 'Bocelli', 'password', 'phone1', 1234567890)
INSERT into [ApplicationUser] VALUES ('Skwatta', 'Camp', 'password', 'phone1', 1234567890)
INSERT into [ApplicationUser] VALUES ('BB','King', 'password', 'phone1', 1234567890)
INSERT into [ApplicationUser] VALUES ('Dolly' ,'Parton', 'password', 'phone1', 1234567890)

INSERT into [Tag] VALUES (1)
INSERT into [Tag] VALUES (2)
INSERT into [Tag] VALUES (3)
INSERT into [Tag] VALUES (5)
INSERT into [Tag] VALUES (6)
INSERT into [Tag] VALUES (7)
INSERT into [Tag] VALUES (8)
INSERT into [Tag] VALUES (9)