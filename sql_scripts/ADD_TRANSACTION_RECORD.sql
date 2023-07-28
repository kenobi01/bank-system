USE [BankSystem]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ADD_TRANSACTION_RECORD]
(
	@Username varchar(20),
	@AccNumber varchar(12),
	@Amount float
)
AS
BEGIN
	-- Disable the row count for performance optimization
	SET NOCOUNT ON;

	DECLARE @id1 int	
	DECLARE @id2 int	

	-- Get the Account ID associated with the provided Account Number
	SELECT @id1 = Id FROM Account WHERE AccNumber = @AccNumber

	-- Insert a record for the credited transaction
	INSERT INTO Transactions (TranDate, Amount, TranType, AccId) VALUES (
		SYSDATETIME(),
		@Amount,
		'Credited',
		@id1
	)

	-- Get the Account ID associated with the provided Username
	SELECT @id2 = A.Id FROM Account A INNER JOIN Customer C ON A.CustId = C.Id WHERE C.Username = @Username

	-- Insert a record for the debited transaction
	INSERT INTO Transactions (TranDate, Amount, TranType, AccId) VALUES (
		SYSDATETIME(),
		@Amount,
		'Debited',
		@id2
	)
END
GO