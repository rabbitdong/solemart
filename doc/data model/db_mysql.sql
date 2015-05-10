CREATE DATABASE solemart;
USE solemart;

CREATE TABLE UserItems(
	 UserID  	INTEGER AUTO_INCREMENT PRIMARY KEY,
	 UserName 	VARCHAR(30),
	 PASSWORD 	CHAR(128),
	 Email 		VARCHAR(30),
	 LoginType    	INT DEFAULT 0, #登录方式：0-本网站注册登录， 1-qq帐号登录
	 RegTime    DATETIME,
	 Roles		VARCHAR(50),	#逗号分隔的RoleID列表
	 OpenID		VARCHAR(32) DEFAULT '' #外部网站验证时提供的ID信息
) CHARACTER SET utf8;

CREATE INDEX idx_email_users ON UserItems(Email);
ALTER TABLE UserItems ADD CONSTRAINT uni_username UNIQUE (UserName);

#匿名账户的用户ID(UserID=1固定为匿名用户)
INSERT INTO UserItems VALUES(1, 'Anonymous', '', '', 0, '0001-01-01 00:00:00', 1, '');

#User Append Info
CREATE TABLE UserAppendInfoItems(
	UserID 		INTEGER PRIMARY KEY REFERENCES Users(UserID),
	NickName	VARCHAR(30),
	Question  	VARCHAR(100),
	Answer    	VARCHAR(30),
	RealName	VARCHAR(30),
	Sex			INTEGER,			#0未知，1表示男， 2表示女
	HeadImageUrl	VARCHAR(255),
	BirthDay	DATETIME,
	SpaceURL	VARCHAR(300),
	Hobits		VARCHAR(160),
	Country		VARCHAR(30),
	Province	VARCHAR(20),
	City		VARCHAR(20),
	Phone		VARCHAR(12),
	Address		VARCHAR(120),
	ExtContent  VARCHAR(2000)    -- 扩展字段，json结构
) CHARACTER SET utf8;

#The product categories
CREATE TABLE CategoryItems(
	CategoryID			INTEGER AUTO_INCREMENT PRIMARY KEY, #类别ID
	CategoryName		NVARCHAR(10) NOT NULL,
	Description			NVARCHAR(100),			#类别的描述
	ParentCategoryID	INTEGER NULL REFERENCES CategoryItems(CategoryID)			#该类别的父类别ID
) CHARACTER SET utf8;

#Brand of production
CREATE TABLE BrandItems(
	BrandID		INTEGER AUTO_INCREMENT PRIMARY KEY,
	ZhName		NVARCHAR(20),
	EnName		VARCHAR(40),		#品牌的英文名称
	BrandLogo	VARCHAR(255),		#品牌的LOGO 文件地址
	Description	VARCHAR(100),
	BrandUrl	VARCHAR(100),		#品牌的url
	Popularity	INTEGER				#品牌的受欢迎程度
) CHARACTER SET utf8;

#The first record of brand is no brand.
INSERT INTO BrandItems(ZhName, EnName, Description) VALUES(N'杂牌', N'nobrand', N'无正规的知名品牌，或贴牌商品');

#The vendor infomation
CREATE TABLE VendorItems(
	VendorID	INTEGER AUTO_INCREMENT PRIMARY KEY,
	VendorName	NVARCHAR(50) NOT NULL,
	Address		NVARCHAR(150),
	ContactName	NVARCHAR(10),		#联系人姓名
	VendorUrl	NVARCHAR(300),		#供应商的网站链接地址
	VendorEmail VARCHAR(50),
	VendorPhone	VARCHAR(15),
	RecordTime	DATETIME,	
	Evaluation	NVARCHAR(50),		#供应商评价
	ExtContent  NVARCHAR(2000)		#JSON结构的扩展字段
)CHARACTER SET utf8;

#The product infomation.
CREATE TABLE ProductItems(
	ProductID		INTEGER AUTO_INCREMENT PRIMARY KEY,
	CategoryID		INTEGER REFERENCES CategoryItems(CategoryID),
	ProductName		NVARCHAR(25) NOT NULL,
	Description 	NVARCHAR(1000),
	ProducingArea 	NVARCHAR(30),		#产地
	Specification	NVARCHAR(30),
	Unit			NVARCHAR(5) DEFAULT N'件',	#单位，如斤、件、套等
	StockCount	 	DECIMAL(10,2) DEFAULT 0.0, 		#表示库存数量
	ReserveCount	DECIMAL(10,2)  DEFAULT 0.0,	#商品保留的数量(如果用户订购了该商品，订购数量就是保留的数量)
	BrandID			INTEGER REFERENCES BrandItems(BrandID),
	FirstInStockTime  	DATETIME,		#第一次进货的时间
	VendorID		INTEGER REFERENCES VendorItems(VendorID),
	Size			INTEGER,		#商品的尺寸
	Color			NVARCHAR(10) DEFAULT '',	#商品的颜色
	Weight 			INTEGER,		#商品的重量，以kg为单位
	ExtContent		NVARCHAR(2000) 		#JSON结构的扩展字段
) CHARACTER SET utf8;

#套餐的信息表
CREATE TABLE PackageItems(
	PackageID 		INTEGER AUTO_INCREMENT PRIMARY KEY,
	Price 			DECIMAL(10,2) DEFAULT 0.0,		#套餐的销售价格
	StartTime		DATETIME,				#开始销售日期
	Remark 			NVARCHAR(2000),				#套餐说明
	EndTime			DATETIME,				#销售终止日期
	Amount 			DECIMAL(10,2) DEFAULT 0			#套餐的数量	
)

#套餐详细信息表
CREATE TABLE PackageDetailItems(
	PackageID 		INTEGER REFERENCES PackageItems(PackageID),
	ProductID		INTEGER REFERENCES ProductItems(ProductID),
	Amount 			DECIMAL(10,2) DEFAULT 0,
	CONSTRAINT PK_PackageDetailItems PRIMARY KEY (PackageID, ProductID)	
)

#销售商品表(在该表中的产品才进行销售)
CREATE TABLE SaledProductItems(
	ProductID	INTEGER PRIMARY KEY REFERENCES ProductItems(ProductID),
	Price		DECIMAL(10,2) DEFAULT 0.0,		#商品的销售价格
	Discount	INTEGER DEFAULT 100,						#销售的折扣
	SpecialFlag	INTEGER DEFAULT 0				#Indicate the special price. 0 indicate not special price.
) CHARACTER SET utf8;

#商品的图像信息表
CREATE TABLE ProductImageItems(
	ImageID		INTEGER AUTO_INCREMENT PRIMARY KEY ,
	ProductID	INTEGER REFERENCES ProductItems(ProductID) ON DELETE CASCADE,
	MimeType	VARCHAR(20),
	ImageUrl	NVARCHAR(50),
	Description NVARCHAR(200),
	ForLogo     INTEGER DEFAULT 1,  #是否作为LOGO图片显示（首页的图片显示）
	ADDTIME     DATETIME
) CHARACTER SET utf8;

#商品的评价表
CREATE TABLE ProductCommentItems(
	CommentID	INTEGER AUTO_INCREMENT PRIMARY KEY,
	ProductID	INTEGER REFERENCES ProductItems(ProductID),
	UserID		INTEGER REFERENCES UserItems(UserID),
	Grade		INTEGER NOT NULL DEFAULT 5,
	Content		NVARCHAR(500),
	CommentTime	DATETIME 	#评价的时间，默认是记录写入的时间
)CHARACTER SET utf8;

CREATE TABLE PriceHistoryItems(
	HistoryID	INTEGER AUTO_INCREMENT PRIMARY KEY,
	ProductID	INTEGER REFERENCES ProductItems(ProductID),
	StartTime	DATETIME,
	EndTime		DATETIME,
	Price		DECIMAL(10,2) NOT NULL DEFAULT 0.0  #The price of yuan.,
)CHARACTER SET utf8;

#In stock infomations.
CREATE TABLE InStockItems(
	InStockID	INTEGER AUTO_INCREMENT PRIMARY KEY,
	ProductID	INTEGER REFERENCES ProductItems(ProductID),
	InStockTime	DATETIME,	#商品入库的时间
	Amount		DECIMAL NOT NULL DEFAULT 0,			#商品的库存数量
	Price		DECIMAL(10,2) NOT NULL DEFAULT 0.0,		#商品的入库价格
	Remark		NVARCHAR(200)
)CHARACTER SET utf8;

#Temporary order info. There's a Temporary order a user.(some user has no temporary order), in fact, it's the shop cart.
CREATE TABLE CartItems(
	UserID		INTEGER REFERENCES UserItems(UserID),
	ProductID	INTEGER REFERENCES ProductItems(ProductID),
	Amount		DECIMAL(10,2) NOT NULL DEFAULT 0,
	UnitPrice	DECIMAL(10,2) DEFAULT 0.0,
	Description NVARCHAR(100) DEFAULT '',
	CONSTRAINT pk_TempOrders PRIMARY KEY(UserID, ProductID)
);

#订单信息表
CREATE TABLE OrderItems(
	OrderID		INTEGER AUTO_INCREMENT PRIMARY KEY,
	UserID		INTEGER,
	OrderTime	DATETIME,
	SendTime	DATETIME,
	ReceiveTime	DATETIME,
	CancelTime  DATETIME,
	RejectTime	DATETIME,
	STATUS		INTEGER DEFAULT 0,		#订单状态: 0: 下订单  1: 订单取消  2:已发送  3: 已接收  4: 已退货 
	TotalPrice	DECIMAL(10,2),
	RealPrice   DECIMAL(10,2),
	DeliverPrice DECIMAL(10,2),
	Address		NVARCHAR(100),
	Phone		VARCHAR(15),
	PostCode	VARCHAR(10),
	Receiver	NVARCHAR(20),	#收货人姓名
	DeliverWay	INTEGER NOT NULL DEFAULT 1,		#送货渠道，1: 送货上门、2: 快递、3: 平邮 4: 特快
	PaymentType	INTEGER NOT NULL DEFAULT 1,		#支付方式  1: 货到付款、2: 网上支付、3: 银行转账
	HasPay		INTEGER NOT NULL DEFAULT 0,    #是否已经付款，1已付款，0未付款
	TradeNo		VARCHAR(64),
	Appraise	NVARCHAR(150),	#收货人评价
	Remark		NVARCHAR(300)	#订单说明，客户对产品的要求
)CHARACTER SET utf8;

CREATE TABLE OrderDetailItems(
	OrderID		INTEGER REFERENCES OrderItems(OrderID),
	ProductID	INTEGER REFERENCES ProductItems(ProductID),
	Amount		DECIMAL(10,2) NOT NULL,
	UnitPrice	DECIMAL(10,2) NOT NULL,		#商品的单价信息
	Remark		NVARCHAR(100) DEFAULT '',  # Remark of the product item.
	CONSTRAINT pk_OrderDetails PRIMARY KEY(OrderID, ProductID)
)CHARACTER SET utf8;

#User deliver goods info.
CREATE TABLE SendAddressItems(
	UserID 		INTEGER PRIMARY KEY REFERENCES UserItems(UserID),
	Receiver	NVARCHAR(20) NOT NULL,		#The name of receiver.
	Address		NVARCHAR(100) NOT NULL,		#The address of delivery.
	Phone		VARCHAR(15) NOT NULL,
	PostCode	VARCHAR(10),
	DeliverWay	INTEGER NOT NULL DEFAULT 1,		#送货渠道，1: 送货上门、2: 快递、3: 平邮 4: 特快
	Channel		INTEGER NOT NULL DEFAULT 0,	#The channel of delivery	0: Home delivery	1: Express delivery		2: Normal delivery
	PaymentType	INTEGER NOT NULL DEFAULT 0	#The pay type				0: cash on delivery	1: pay on line			2: pay on bank transfer.
) CHARACTER SET utf8;

#记录用户的诉求和建议
CREATE TABLE AdviserItems(
	AdviseID	INTEGER AUTO_INCREMENT PRIMARY KEY,
	UserID		INTEGER REFERENCES UserItems(UserID),
	Content		NVARCHAR(200) NOT NULL,
	AdviseTime	DATETIME,
	IsViewed	INTEGER NOT NULL DEFAULT 0		#该建议是否被考虑, 0 indicate the advise isn't be adopt.
) CHARACTER SET utf8;

#system billboards.
CREATE TABLE BulletinItems(
   BulletinID   INTEGER PRIMARY KEY AUTO_INCREMENT,
   Content      NVARCHAR(500),
   PublishTime	DATETIME,
   AbortTime    DATETIME
) CHARACTER SET utf8;


#User favorite list.
CREATE TABLE FavoriteItems(
	UserID			INTEGER REFERENCES UserItems(UserID),
	ProductID		INTEGER REFERENCES Products(ProductID),
	FavoriteTime		DATETIME,
	Description		NVARCHAR(200),
	CONSTRAINT pk_Favorites PRIMARY KEY (UserID, ProductID)
) CHARACTER SET utf8;

#下面是非系统关键表
#用户组表(用户创建的分组列表)
/*
CREATE TABLE Groups(
   GroupID      INTEGER PRIMARY KEY AUTO_INCREMENT,
   GroupName    VARCHAR(30) NOT NULL,
   OwnedUser    INTEGER REFERENCES Users(UserID),    #创建该组的用户
   Remark		VARCHAR(200),
) CHARACTER SET utf8;

create index idx_usergrouptb_group on Groups(OwnedUser);

#用户的朋友列表
CREATE TABLE UserFriends(
   UserID       INTEGER REFERENCES Users(UserID),
   FriendID     INTEGER REFERENCES Users(UserID),
   GroupID      INTEGER REFERENCES Groups(GroupID),
   Remark		VARCHAR(50),
   CONSTRAINT PK_UserFriends PRIMARY KEY (UserID, FriendID, GroupID)
) CHARACTER SET utf8;

--User Role
CREATE TABLE Roles(
	RoleID			INTEGER PRIMARY KEY,
	RoleName		VARCHAR(10) NOT NULL,
	Description 	VARCHAR(50)
) CHARACTER SET utf8;

INSERT INTO [Roles](RoleID, RoleName) VALUES(-1, 'anonymous');
INSERT INTO [Roles](RoleID, RoleName) VALUES(1, 'user');
INSERT INTO [Roles](RoleID, RoleName) VALUES(2, 'operator');
INSERT INTO [Roles](RoleID, RoleName) VALUES(3, 'su');

#产品搜索表
CREATE TABLE Searchs(
   Keyword      NVARCHAR(10) PRIMARY KEY,   --关键字
   ProductIDS	VARCHAR(500) NOT NULL               --产品的ID列表
) CHARACTER SET utf8;

#关键字表
CREATE TABLE Keyworks(
   PreKeyword	NVARCHAR(6) PRIMARY KEY,
   Keyword		NVARCHAR(20),
   Frequency	INTEGER             --该关键字的频率
) CHARACTER SET utf8;
*/
