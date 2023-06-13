-- =============================================
-- Author:		Masoud shafaghi
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetMountlyTaskReport @ProjectId UNIQUEIDENTIFIER
    AS
BEGIN
	SET
NOCOUNT ON;
	DECLARE
@t TABLE
	(
	   Id UNIQUEIDENTIFIER,
	   [Date] DATE,
	   [Type] INT
	)

	DECLARE
@Date DATE = CONVERT(DATE, DATEADD(DAY, -90, GETDATE()))

    -- Insert statements for procedure here
	INSERT INTO @t
SELECT ID, CONVERT(DATE, a.CreatedAt), [Type]
FROM activities a
WHERE EXISTS (SELECT 1 FROM workpackagetasks w WHERE w.ProjectId = @ProjectId)


--INSERT INTO @History
--SELECT CONVERT(DATE, CreatedAt) [Date], COUNT(1) [Count] FROM @t w
--WHERE w.[State] = 4 OR w.[State] = 5 OR w.[State] = 9 AND CreatedAt > @Date

--SELECT CONVERT(DATE, CreatedAt) [Date], COUNT(1) [Count] FROM @t w
--WHERE w.[State] = 4 OR w.[State] = 5 OR w.[State] = 9 AND CreatedAt > CONVERT(DATE, DATEADD(DAY, -90, GETDATE())) OR EXISTS(SELECT 1 FROM activities a WHERE a.RecordId = w.Id AND a.[Type] = 623 AND a.CreatedAt)
--GROUP BY CONVERT(DATE, CreatedAt)

--SELECT CONVERT(DATE, CreatedAt) [Date], COUNT(1) [Count] FROM @t w
--WHERE (w.[State] = 4 OR w.[State] = 5 OR w.[State] = 9) AND CreatedAt > CONVERT(DATE, DATEADD(DAY, -90, GETDATE()))
--GROUP BY CONVERT(DATE, CreatedAt)

--SELECT CONVERT(DATE, CreatedAt) [Date], COUNT(1) [Count] FROM @t w
--WHERE CreatedAt > CONVERT(DATE, DATEADD(DAY, -90, GETDATE()))
--GROUP BY CONVERT(DATE, CreatedAt)

SELECT a.[Date] [Date], COUNT (1) [Count]
FROM @t a
WHERE a.[Date] > @Date AND (a.[Type] = 623 OR a.[Type] = 626 OR a.[Type] = 627)
GROUP BY CONVERT (DATE, a.[Date])

SELECT a.[Date] [Date], COUNT (1) [Count]
FROM @t a
WHERE a.[Date] > @Date AND (a.[Type] = 602 OR a.[Type] = 628 OR a.[Type] = 629)
GROUP BY CONVERT (DATE, a.[Date])

SELECT a.[Date] [Date], COUNT (1) [Count]
FROM @t a
WHERE a.[Date] > @Date AND (a.[Type] = 600)
GROUP BY CONVERT (DATE, a.[Date])
END