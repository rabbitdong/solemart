-- 增加订单的实收金额
ALTER TABLE OrderItems ADD COLUMN RealPrice DECIMAL(10,2) DEFAULT 0.0;
ALTER TABLE OrderItems ADD COLUMN DeliverPrice DECIMAL(10,2) DEFAULT 0.0;

ALTER TABLE TestOrderItems ADD COLUMN RealPrice DECIMAL(10,2) DEFAULT 0.0;
ALTER TABLE TestOrderItems ADD COLUMN DeliverPrice DECIMAL(10,2) DEFAULT 0.0;

ALTER TABLE SaledProductItems ADD COLUMN SaledPrice DECIMAL(10,2) DEFAULT 0 ;
ALTER TABLE SaledProductItems ADD COLUMN SetTop INTEGER DEFAULT 0;
UPDATE SaledProductItems SET SaledPrice = Price;

ALTER TABLE TestSaledProductItems ADD COLUMN SaledPrice DECIMAL(10,2) DEFAULT 0 ;
ALTER TABLE TestSaledProductItems ADD COLUMN SetTop INTEGER DEFAULT 0;
UPDATE TestSaledProductItems SET SaledPrice = Price;

ALTER TABLE productimageitems MODIFY COLUMN ImageUrl VARCHAR(500);
