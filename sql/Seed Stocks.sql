DECLARE @ProductIds TABLE (ProductID INT)

INSERT INTO @ProductIds (ProductID)
SELECT TOP 90 ProductID
FROM Products
ORDER BY ProductID DESC

DECLARE @ProductID INT
DECLARE @RandomQuantity INT
DECLARE @RandomCount INT

DECLARE product_cursor CURSOR FOR
SELECT ProductID FROM @ProductIds

OPEN product_cursor

FETCH NEXT FROM product_cursor INTO @ProductID

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @RandomCount = FLOOR(RAND() * 6)

    DECLARE @i INT = 0

    WHILE @i < @RandomCount
    BEGIN
        SET @RandomQuantity = FLOOR(RAND() * 101)

        INSERT INTO Stocks (ProductCode, StockCode, Quantity, CreatedAt, UpdatedAt)
        VALUES (
            (SELECT ProductCode FROM Products WHERE ProductID = @ProductID),
            NEWID(),
            @RandomQuantity,
            GETDATE(),
            GETDATE()
        )

        SET @i = @i + 1
    END

    FETCH NEXT FROM product_cursor INTO @ProductID
END

CLOSE product_cursor
DEALLOCATE product_cursor
