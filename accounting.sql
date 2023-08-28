
---CREATE TABLES chart of accounts and journal ---
IF Object_id('Journals') is not null
      DROP TABLE [dbo].Journals
GO    
IF Object_id('Chart_of_Accounts') is not null
      DROP TABLE [dbo].Chart_of_Accounts
GO

CREATE TABLE [dbo].Chart_of_Accounts
(
      ID          INT IDENTITY(1,1),
      AccountNum  VARCHAR(12) UNIQUE NOT NULL,
      Descrip     VARCHAR(48),
      AcctType    CHAR(1)     CHECK (AcctType in ('A','L','O','R','E')),
      Balance     MONEY,
      CONSTRAINT PK_Chart_of_Accounts PRIMARY KEY (ID)
)
CREATE TABLE [dbo].Journals
(
      ID          INT IDENTITY(1,1),      -- Unique key per line item
      AccountID   INT,  
      JrnlType    CHAR(2),		          -- GJ, AR, AP, SJ, PJ, etc
      TransNum    INT,		              -- Key to group entries together.  
      DC          CHAR(1)     CHECK (DC in ('D','C')),
	  Posted	  CHAR(1)	  DEFAULT 'N',
	  TransDate	  DATETIME	  DEFAULT GetDate(),
	  PostDate	  DATETIME,
      Amount      MONEY NOT NULL,
      CONSTRAINT PK_Journals PRIMARY KEY (ID),
      CONSTRAINT FK_Chart FOREIGN KEY (AccountID) REFERENCES Chart_of_Accounts(ID)
)
GO

---CREATE SP To add and posts transactions to the journals and chart of accounts--

 IF Object_id('TransToTable') is not null
      DROP FUNCTION [dbo].TransToTable
GO

 CREATE Function [dbo].TransToTable 
 (@AcctList VARCHAR(1000) )
  RETURNS 
	@RowTable TABLE
		(	AcctNumber VARCHAR(12),
			Jrnl_Account_ID INT,
            DebitCredit CHAR(1),
		    Amt MONEY
    )
AS
BEGIN

	DECLARE @X INT
	DECLARE @Y INT
	DECLARE @OneLine VARCHAR(30)
	DECLARE @acctNUM VARCHAR(12)
	DECLARE @DebCred CHAR(1)
	DECLARE @TransAmt MONEY
	SET @AcctList=@AcctList+','

	SET @x = CHARINDEX(',',@AcctList)
	WHILE @x >0
	BEGIN
		SET @OneLine =  LEFT(@AcctList,@x-1)
		SET @AcctList = RTRIM(SUBSTRING(@AcctList,@x+1,9999))
		if LEN(@OneLine) > 0
		begin
			SET @Y = CHARINDEX('|',@OneLine)
			SET @AcctNum = LEFT(@OneLine,@y-1)
			SET @DebCred = SUBSTRING(@OneLine,@y+1,1)
			SET @OneLine = RTRIM(SUBSTRING(@OneLine,@y+3,9999))
			SET @TransAmt = CAST(@OneLine AS MONEY)
			INSERT INTO @RowTable VALUES (@AcctNum,-1,@DebCred,@TransAmt)
		end
		UPDATE @rowTable SET Jrnl_Account_ID = xx.id
		FROM (select id,accountNum FROM [dbo].chart_of_accounts) xx
		WHERE xx.accountNum=AcctNumber
		SET @x = CHARINDEX(',',@AcctList)
	END
	RETURN 
END
GO

IF Object_id('AddTransaction') is not null
      DROP PROCEDURE [dbo].AddTransaction
GO
CREATE PROCEDURE  [dbo].AddTransaction
( 
	@AcctList VARCHAR(1000),	-- Comma Separated: Format is AcctNum|D or C|Amount,
	@JrnlType CHAR(2) ='GJ'
 )
 AS
 BEGIN
	SET NOCOUNT ON
	-- Split the parameter into a table
	DECLARE @TransTable TABLE (AccoutNum VARCHAR(12),ID INT,DC CHAR(1),amt MONEY)
	INSERT INTO @TransTable
		SELECT * FROM [dbo].TransToTable(@AcctList)
	-- Validate all accounts, return -1 if any invalid accounts
	DECLARE @nCtr INT

	SELECT @nCtr = COUNT(*) FROM @TransTable WHERE ID <0
	IF (@nCtr >0 )
	BEGIN
		-- Optionally, could raise an error
		PRINT 'Missing account numbers'
		RETURN -1
	END

	-- Validate Debits = Credits, return -2 if not
	DECLARE @DebitTot MONEY
	DECLARE @CreditTot MONEY

	SELECT @DebitTot = SUM(amt) FROM @TransTable WHERE DC='D'
	SELECT @CreditTot = SUM(amt) FROM @TransTable WHERE DC='C'
	IF (@DebitTot <> @CreditTot )
	BEGIN
		-- Optionally, could raise an error
		PRINT 'Debits <> Credits'
		RETURN -2
	END
	-- Post the transaction into journals
	BEGIN TRANSACTION
		DECLARE @nNext INT
		SELECT @nNext = IsNull(max(transNum)+1,1) FROM  [dbo].Journals WHERE jrnlType=@JrnlType

		INSERT INTO [dbo].Journals (AccountID,JrnlType,TransNum,DC,Amount)
		SELECT ID,@JrnlType,@nNext,DC,amt
		FROM @TransTable
	COMMIT
	RETURN 0

 END
 GO

IF Object_id('PostTransaction') is not null
      DROP PROCEDURE [dbo].PostTransaction
GO
CREATE PROCEDURE  [dbo].PostTransaction( @TransNumb INT = 0 )
 AS
 BEGIN
	SET NOCOUNT ON
	UPDATE [dbo].Chart_of_Accounts SET Balance = Balance +xx.PostAmt
	FROM
	(
		SELECT AccountID,
		 Sum(
		 CASE WHEN jl.dc='D' THEN amount ELSE -1*amount END
		 ) as PostAmt
		FROM [dbo].Journals jl
		JOIN [dbo].Chart_of_Accounts ca on jl.AccountID=ca.id
		WHERE jl.posted='N' AND (Transnum = @TransNumb or @TransNumb=0) AND ca.AcctType in ('A','E')
		GROUP BY AccountID
	) xx
	WHERE xx.accountID=ID

	UPDATE [dbo].Chart_of_Accounts SET Balance = Balance +xx.PostAmt
	FROM
	(
		SELECT AccountID,
		 Sum(
		 CASE WHEN jl.dc='C' THEN amount ELSE -1*amount END
		 ) as PostAmt
		FROM [dbo].Journals jl
		JOIN [dbo].Chart_of_Accounts ca on jl.AccountID=ca.id
		WHERE jl.posted='N' AND (Transnum = @TransNumb or @TransNumb=0) AND ca.AcctType in ('L','O','R')
		GROUP BY AccountID
	) xx
	WHERE xx.accountID=ID
	UPDATE [dbo].Journals SET posted='Y',PostDate=getDate() WHERE posted='N' AND (Transnum = @TransNumb or @TransNumb=0) 
 END
 GO
 
 IF Object_id('ClosingEntry') is not null
      DROP PROCEDURE [dbo].ClosingEntry
GO

---CREATE REPORTVIEW--
IF Object_id('BalanceSheet') is not null
      DROP VIEW [dbo].BalanceSheet
GO

-- Balance sheet
CREATE VIEW [dbo].BalanceSheet 
AS
	SELECT AccountNum,Descrip,Balance FROM [dbo].Chart_of_Accounts WHERE AcctType='A'
	UNION 
	SELECT '1900','TOTAL ASSETS',Sum(Balance) FROM [dbo].Chart_of_Accounts WHERE AcctType='A'
	UNION
	SELECT AccountNum,Descrip,Balance FROM [dbo].Chart_of_Accounts WHERE AcctType='L'
	UNION 
	SELECT '2900','TOTAL LIABILITIES',Sum(Balance) FROM [dbo].Chart_of_Accounts WHERE AcctType='L'
	UNION
	SELECT AccountNum,Descrip,Balance FROM [dbo].Chart_of_Accounts WHERE AcctType='O'
	UNION 
	SELECT '3900','TOTAL EQUITY',Sum(Balance) FROM [dbo].Chart_of_Accounts WHERE AcctType='O'
	UNION 
	SELECT '3999','TOTAL LIABILITIES and EQUITY',Sum(Balance) FROM [dbo].Chart_of_Accounts WHERE AcctType IN ('L','O')

GO
IF Object_id('IncomeStatement') is not null
      DROP VIEW [dbo].IncomeStatement
GO
CREATE VIEW [dbo].IncomeStatement 
AS
	SELECT 4000 as Seq,'REVENUE' as 'Account Name',IsNull(Sum(jl.Amount),0) as Balance
	FROM [dbo].Journals jl
	JOIN [dbo].Chart_of_Accounts ca on ca.id=jl.AccountId
	WHERE jl.posted='N' and ca.AcctType='R'
	UNION
	SELECT ca.AccountNum,descrip,IsNull(Sum(jl.Amount),0) as Balance
	FROM [dbo].Journals jl
	JOIN [dbo].Chart_of_Accounts ca on ca.id=jl.AccountId
	WHERE jl.posted='N' and ca.AcctType='E'
	GROUP BY ca.descrip,ca.AccountNum
	UNION
	SELECT '9999','NET INCOME(loss)',xx.Balance
	FROM  (
			SELECT IsNull(
				Sum(CASE when jl.dc='D' then -1*jl.amount else jl.amount end),0 ) as Balance
			FROM [dbo].Journals jl
			JOIN [dbo].Chart_of_Accounts ca on ca.id=jl.AccountId AND jl.posted='N' and (ca.AcctType IN ('R','E'))
		) xx

GO
SELECT * FROM BalanceSheet
select * from IncomeStatement ORDER BY Seq


--LOADING SOME DATA--
SET NOCOUNT ON
TRUNCATE TABLE [dbo].Journals
DELETE FROM Chart_of_Accounts
DBCC CHECKIDENT ('Chart_of_Accounts', RESEED, 0)

-- Add Balance sheet accounts (Chapter one)
INSERT INTO [dbo].Chart_of_Accounts (AccountNum,Descrip,AcctType,Balance)
VALUES
('1000','Cash-Checking Account','A',0),
('1100','Software','A',0),
('1200','Subscriptions','A',0),
('1600','Computer System','A',0),
('2000','Loan For Computer','L',0),
('3000','Owner Equity','O',0),
('3100','Retained Earnings','O',0)
-- Add Income statement accounts (Chapter two)
INSERT INTO [dbo].Chart_of_Accounts (AccountNum,Descrip,AcctType,Balance)
VALUES
('4000','Sales Revenue','R',0),
('5000','Rent Expense','E',0),
('5100','Postage Expense','E',0),
('5200','Shipping Supplies Expense','E',0),
('5300','Office Supplies Expense','E',0)
GO
--Journal Entries
-- Chapter one

EXEC [dbo].AddTransaction '1000|D|10000,3000|C|10000','GJ'		    
EXEC [dbo].AddTransaction '1600|D|6000,1000|C|1000,2000|C|5000','GJ'
EXEC [dbo].AddTransaction '1100|D|794,1200|D|99,1000|C|893','GJ'	
EXEC [dbo].AddTransaction '1000|C|1000,2000|D|1000','GJ'			
GO
EXEC [dbo].PostTransaction

-- Chapter two
EXEC [dbo].AddTransaction '1000|D|250,4000|C|250','GJ'		    
EXEC [dbo].AddTransaction '1000|D|595,4000|C|595','GJ'		    
EXEC [dbo].AddTransaction '5300|D|75,1000|C|75','GJ'		    
EXEC [dbo].AddTransaction '4000|C|400,1000|D|360,5100|D|12,5200|D|28','GJ'		    
EXEC [dbo].AddTransaction '5000|D|600,1000|C|600','GJ'		    

GO
