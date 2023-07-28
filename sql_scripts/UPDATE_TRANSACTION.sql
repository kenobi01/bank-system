USE [BankSystem]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UPDATE_TRANSACTION]
(
	@Username varchar(20),
	@AccNumber varchar(12),
	@IBAN varchar(10),
	@AccHolder varchar(100),
	@Amount float,
	@text nvarchar(500) OUTPUT 
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Check the validity of account details
	SELECT COUNT(*) FROM Customer C INNER JOIN Account A ON C.Id = A.CustId INNER JOIN Branch B ON A.BranchId = B.Id 
	WHERE A.AccNumber = @AccNumber AND B.IBAN = @IBAN AND C.Name = @AccHolder

	-- If account details are valid, update the account balance
	IF (COUNT(*) > 0)
	BEGIN
		UPDATE Account SET Balance = Balance + @Amount WHERE AccNumber = @AccNumber
	END
	ELSE
	BEGIN
		SET @text = 'Transaction Failed.' -- Account details are invalid, set the output parameter
	END

	-- Deduct the amount from the user's account balance
	IF (COUNT(*) > 0)
	BEGIN
		UPDATE Account SET Balance = Balance - @Amount FROM Account A INNER JOIN Customer C ON A.CustId = C.Id WHERE C.Username = @Username
	END
	 
END
GO
