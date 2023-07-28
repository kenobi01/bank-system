CREATE TABLE Customer (
Id int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(100),
DOB DATE,
Phone VARCHAR(12),
Email VARCHAR(50),
Address VARCHAR(200),
Username VARCHAR(20),
Password VARCHAR(20),
Reg_Date DATETIME
)

CREATE TABLE Branch (
Id int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(20),
Description VARCHAR(100),
IBAN VARCHAR(10),
Phone VARCHAR(12)
)

CREATE TABLE Account (
Id int IDENTITY(1,1) PRIMARY KEY,
AccNumber VARCHAR(12),
AccType VARCHAR(10),
Reg_Date DATETIME,
Balance FLOAT(8),
CustId int FOREIGN KEY REFERENCES Customer(Id),
BranchId int FOREIGN KEY REFERENCES Branch(Id)
)

CREATE TABLE Transactions (
Id int IDENTITY(1,1) PRIMARY KEY,
TranDate DATETIME,
Amount FLOAT(8),
TranType VARCHAR(10),
AccId int FOREIGN KEY REFERENCES Account(Id)
)

INSERT INTO Branch VALUES ('İstanbul', 'Cumhuriyet Mahallesi, Vatan Caddesi No:20, Fatih, İstanbul', 'SBIN007000', '0212-1234567')
INSERT INTO Branch VALUES ('Ankara', 'Kızılay Mahallesi, Atatürk Bulvarı No:10, Çankaya, Ankara', 'SBIN000080', '0312-9876543')
INSERT INTO Branch VALUES ('İzmir', 'Konak Mahallesi, Cumhuriyet Meydanı No:5, Konak, İzmir', 'SBIN000069', '0232-5555555')
SELECT * FROM Branch