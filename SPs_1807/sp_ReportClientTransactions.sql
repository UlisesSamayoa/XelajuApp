SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_ReportClientTransactions]
(
    @StartDate DATE,
    @EndDate DATE,
    @ClientID INT
)
AS
BEGIN
SET NOCOUNT ON;
SELECT
    T.ReferenceNumber,
    T.DateC,
    C.FirstName + ' ' + C.LastName AS ClientName,
    CO.Name AS CompanyName,
    T.Amount,
    T.Comission,
    T.FixedCommission,
    ISNULL(T.Comission,0)+ISNULL(T.FixedCommission,0) AS TotalCommission,
    T.TotalAmount,
    T.CalculationMode,
    T.TransactionType
FROM Transactions T
INNER JOIN Clients C ON C.IdClient=T.IdClient_fk
LEFT JOIN Companies CO ON CO.IdCompany=T.SenderCompany
WHERE T.Status=1 AND T.IdClient_fk=@ClientID AND CAST(T.DateC AS DATE) BETWEEN @StartDate AND @EndDate
ORDER BY T.DateC, T.ReferenceNumber;
END
GO
