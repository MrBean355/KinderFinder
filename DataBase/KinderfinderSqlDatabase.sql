DROP Database KinderFinder
GO

CREATE Database KinderFinder
GO

CREATE TABLE ApplicationUsers
(
	UserID int primary key,
	User_Name varchar(50),
	User_Surname varchar(50),
	User_PhoneModel varchar(50),
	User_Password varchar(50),
	User_PhoneNumber int,
	User_Restaurant_ID int references Restaurant(RestaurantID)
) 

CREATE TABLE RF_ID_Tags
(
	RF_ID_Number int primary key,
	RF_ID_User_ID int references ApplicationUsers(UserID)
) 

CREATE TABLE Restaurant
(
	Restaurant_ID int primary key,
	Restaurant_Name varchar(50)
) 

INSERT into Restaurant VALUES (00001, 'Spur')
INSERT into Restaurant VALUES (00002, 'Steers')
INSERT into Restaurant VALUES (00003, 'Nandos')



INSERT into ApplicationUsers VALUES ('0705617', 'Justin','Timberlake', 'phone1', 'password', 1234567890, 00001)
INSERT into ApplicationUsers VALUES ('0705631', 'Mariah', 'Carrey', 'phone1', 'password', 1234567890, 00002)
INSERT into ApplicationUsers VALUES ('0705655', '2','Pac', 'phone1', 'password', 1234567890, 00001)
INSERT into ApplicationUsers VALUES ('0705693', 'Britany','Spears', 'phone1', 'password', 1234567890, 00002)
INSERT into ApplicationUsers VALUES ('0715605', 'Bob', 'Marley', 'phone1', 'password', 1234567890, 00003)
INSERT into ApplicationUsers VALUES ('0725609', 'Andrea', 'Bocelli', 'phone1', 'password', 1234567890, 00001)
INSERT into ApplicationUsers VALUES ('0745603', 'Skwatta', 'Camp', 'phone1', 'password', 1234567890, 00003)
INSERT into ApplicationUsers VALUES ('0765607', 'BB','King', 'phone1', 'password', 1234567890, 00002)
INSERT into ApplicationUsers VALUES ('0785601', 'Dolly' ,'Parton', 'phone1', 'password', 1234567890, 00003)


INSERT into RF_ID_Tags VALUES (0000112, 0705617)
INSERT into RF_ID_Tags VALUES (0000113, 0705631)
INSERT into RF_ID_Tags VALUES (0000114, 0705655)
INSERT into RF_ID_Tags VALUES (0000115, 0705693)
INSERT into RF_ID_Tags VALUES (0000116, 0715605)
INSERT into RF_ID_Tags VALUES (0000117, 0745603)
INSERT into RF_ID_Tags VALUES (0000118, 0765607)
INSERT into RF_ID_Tags VALUES (0000119, 0785601)