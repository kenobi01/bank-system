USE [BankSystem]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[INSERT_INTO_BANK_TABLES]
(
	@name	 varchar(100),
	@dob	 date,
	@phone	 varchar(12),
	@email	 varchar(50),
	@address varchar(200),
	@username varchar(20),
	@password varchar(20),
	--@accNumber varchar,
	@accType varchar(10),
	@branchId int
)
AS
BEGIN

	SET NOCOUNT ON;

	-- Insert customer information into the Customer table
	INSERT INTO Customer (Name, DOB, Phone, Email, Address, Username, Password, Reg_Date) VALUES (
		@name,
		@dob,
		@phone,
		@email,
		@address,
		@username,
		@password,
		SYSDATETIME()
	)

	Declare @id int
	Declare @accNumber varchar(12)
	SET @accNumber = null

	-- Retrieve the Customer ID based on the username
	SELECT @id = Id FROM Customer WHERE Username = @username

	-- Retrieve the latest account number from the Account table
	SELECT Top 1 @accNumber = AccNumber FROM Account ORDER BY Id DESC
	
	-- If no account number is found, start with '100000000000'
	IF (@accNumber is null)
		SET @accNumber = '100000000000'
	ELSE
		-- Increment the account number by 1
		SELECT @accNumber = CAST((CAST(@accNumber AS bigint) + 1) AS varchar)

	-- Insert account information into the Account table
	INSERT INTO Account (AccNumber, AccType, Reg_Date, Balance, CustId, BranchId) VALUES (
		@accNumber,
		@accType,
		SYSDATETIME(),
		3000.00,
		@id,
		@branchId
	)

END
GO
