SELECT * FROM ProductItems;
(SELECT ProductID, SUM(Amount) FROM OrderDetailItems
   WHERE OrderID IN (SELECT OrderID FROM OrderItems WHERE STATUS=3)
   GROUP BY ProductID) ProductSaledTB

SELECT * FROM OrderItems;

UPDATE ProductItems SET ReserveCount=10 WHERE ProductID=1;
UPDATE ProductItems SET ReserveCount=12 WHERE ProductID=2;
UPDATE ProductItems SET ReserveCount=2 WHERE ProductID=3;
UPDATE ProductItems SET ReserveCount=5.2 WHERE ProductID=4;
UPDATE ProductItems SET ReserveCount=2 WHERE ProductID=6;
UPDATE ProductItems SET ReserveCount=2 WHERE ProductID=7;