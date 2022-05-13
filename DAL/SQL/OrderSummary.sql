CREATE OR ALTER PROCEDURE OrderSummary
@id int
AS
BEGIN
	SELECT o.Id, o.[DateTime], COUNT(op.ProductsId) AS Count
	FROM [Order] as o
	JOIN OrderProduct as op ON op.OrdersId = o.Id
	WHERE o.Id = @id
	GROUP BY o.Id, o.[DateTime]
END