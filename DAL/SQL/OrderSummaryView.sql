CREATE VIEW View_OrderSummary AS
	SELECT o.Id, o.[DateTime], COUNT(op.ProductsId) AS Count
	FROM [Order] as o
	JOIN OrderProduct as op ON op.OrdersId = o.Id
	GROUP BY o.Id, o.[DateTime]
