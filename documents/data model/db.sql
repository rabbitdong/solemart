#CREATE DATABASE ilivemini;
#USE ilivemini;

CREATE TABLE Users(
	 UserID  	INTEGER PRIMARY KEY,
	 UserName 	VARCHAR(30),
	 Password 	CHAR(128),
	 Email 		VARCHAR(30),
	 Logtype    INT DEFAULT 0, #登录方式：0-本网站注册登录， 1-qq帐号登录
	 RegTime    TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	 Roles		VARCHAR(50),	#逗号分隔的RoleID列表
	 OpenID		VARCHAR(32) DEFAULT '' #外部网站验证时提供的ID信息
) CHARACTER SET utf8;

create index idx_email_users on [Users](Email);

--User Append Info
CREATE TABLE UserAppends(
	UserID 		INTEGER PRIMARY KEY REFERENCES Users(UserID),
	NickName	VARCHAR(30),
	Question  	VARCHAR(100),
	Answer    	VARCHAR(30),
	RealName	VARCHAR(30),
	Sex			INTEGER,			#0未知，1表示男， 2表示女
	HeadImage	VARCHAR(255),
	Birthday	DATETIME,
	SpaceURL	VARCHAR(300),
	Hobits		VARCHAR(160),
	Country		VARCHAR(30),
	Province	VARCHAR(20),
	City		VARCHAR(20),
	Phone		VARCHAR(12),
	Address		VARCHAR(120),
	ExtContent  VARCHAR(2000),    -- 扩展字段，json结构
) CHARACTER SET utf8;

#用户组表(用户创建的分组列表)
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

-- The product categories
CREATE TABLE Categories(
	CatID  		INTEGER AUTO_INCREMENT PRIMARY KEY, #类别ID
	CatName		VARCHAR(10) NOT NULL,
	Description	VARCHAR(100),			#类别的描述
	OwnedCatID	INTEGER NULL REFERENCES Categories(CatID),			#该类别的父类别ID
) CHARACTER SET utf8;

#Brand of production
CREATE TABLE Brands(
	BrandID		INTEGER(32) AUTO_INCREMENT PRIMARY KEY,
	BrandName	VARCHAR(20),
	EnName		VARCHAR(20),		#品牌的英文名称
	Description	VARCHAR(100),
	BrandUrl	VARCHAR(100),		#品牌的url
	Image		VARCHAR(20)		#商标图片名称
) CHARACTER SET utf8;

#The first record of brand is no brand.
INSERT INTO Brands(BrandName, EnName, Description) VALUES(N'杂牌', N'nobrand', N'无正规的知名品牌，或贴牌商品');

#The vendor infomation
CREATE TABLE Vendors(
	VendorID	INTEGER AUTO_INCREMENT PRIMARY KE,
	VendorName	NVARCHAR(50) NOT NULL,
	Address		NVARCHAR(150),
	Phone		VARCHAR(15),
	RecordTime	DATETIME DEFAULT CURRENT_TIMESTAMP,	
	Contact		NVARCHAR(10),		#联系人姓名
	VendorUrl	NVARCHAR(300),		#供应商的网站链接地址
	Evaluation	NVARCHAR(50),		#供应商评价
	ExtContent  NVARCHAR(2000),		#JSON结构的扩展字段
)CHARACTER SET utf8;

#The product infomation.
CREATE TABLE Products(
	ProductID		INTEGER AUTO_INCREMENT PRIMARY KEY,
	ProductName		NVARCHAR(25) NOT NULL,
	CateID			INTEGER REFERENCES Categories(CatID),
	Specification	NVARCHAR(30),
	BrandID			INTEGER REFERENCES Brands(BrandID),
	StockCount	 	INTEGER, 		#表示库存数量
	ReserveCount	INTEGER  DEFAULT 0,	#商品保留的数量(如果用户订购了该商品，订购数量就是保留的数量)
	Unit			NVARCHAR(5) DEFAULT N'件',	#单位，如斤、件、套等
	FirstInStock  	DATETIME,		#第一次进货的时间
	Description 	NVARCHAR(1000),
	VendorID		INTEGER REFERENCES Vendors(VendorID)
	Size			INTEGER(32),		#商品的尺寸
	Color			INTEGER default 0,	#商品的颜色
	Weight 			INTEGER,		#商品的重量，以kg为单位
	ExtContent		NVARCHAR(2000), #JSON结构的扩展字段
) CHARACTER SET utf8;

#销售商品表(在该表中的产品才进行销售)
CREATE TABLE SaleProducts(
	ProductID	INTEGER PRIMARY KEY REFERENCES Products(ProductID),
	Price		DECIMAL(10,4) DEFAULT 0.0,		#商品的销售价格
	Discount	INTEGER DEFAULT 100,						#销售的折扣
	SpecialFlag	INTEGER DEFAULT 0,				#Indicate the special price. 0 indicate not special price.
) CHARACTER SET utf8;

#商品的图像信息表
CREATE TABLE ProductImages(
	ImageID		INTEGER AUTO_INCREMENT PRIMARY KEY ,
	ProductID	INTEGER,
	MimeType	VARCHAR(20),
	ImageUrl	NVARCHAR(50),
	Description NVARCHAR(200)
) CHARACTER SET utf8;

#商品的评价表
CREATE TABLE ProductEvaluates(
	EvaluateID	INTEGER AUTO_INCREMENT PRIMARY KEY,
	ProductID	INTEGER REFERENCES Products(ProductID),
	UserID		INTEGER REFERENCES [Users](UserID),
	Grade		INTEGER NOT NULL DEFAULT 5,
	Content		VARCHAR(200),
	EvaluateTime	DATETIME DEFAULT CURRENT_TIMESTAMP 	#评价的时间，默认是记录写入的时间
)CHARACTER SET utf8;

CREATE TABLE ProductPriceHistories(
	PPHID		INTEGER AUTO_INCREMENT PRIMARY KEY,
	ProductID	INTEGER REFERENCES Products(ProductID),
	StartTime	DATETIME,
	EndTime		DATETIME
	YuanPrice	DECIMAL(10,4) NOT NULL DEFAULT 0.0,  --The price of yuan.,
	DollarPrice	DECIMAL(10,4)
)CHARACTER SET utf8;

#In stock infomations.
CREATE TABLE InStocks(
	InStockID	INTEGER AUTO_INCREMENT PRIMARY KEY,
	ProductID	INTEGER REFERENCES Products(ProductID),
	InStockTime	DATETIME DEFAULT CURRENT_TIMESTAMP,	#商品入库的时间
	Amount		INTEGER NOT NULL DEFAULT 0,			#商品的库存数量
	Price		DECIMAL(10,4) NOT NULL DEFAULT 0.0		#商品的入库价格
)CHARACTER SET utf8;

#Temporary order info. There's a Temporary order a user.(some user has no temporary order), in fact, it's the shop cart.
CREATE TABLE TempOrders(
	UserID		INTEGER REFERENCES [Users](UserID),
	ProductID	INTEGER REFERENCES Products(ProductID),
	Amount		INTEGER	NOT NULL DEFAULT 0,
	CONSTRAINT pk_TempOrders PRIMARY KEY(UserID, ProductID)
);

#订单信息表
CREATE TABLE Orders(
	OrderID		INTEGER AUTO_INCREMENT PRIMARY KEY,
	UserID		INTEGER,
	OrderTime	DATETIME DEFAULT CURRENT_TIMESTAMP,
	SendTime	DATETIME,
	ReceiveTime	DATETIME,
	Remark		NVARCHAR(300),	#订单说明，客户对产品的要求
	Status		INTEGER DEFAULT 0,		#订单状态: 0: 下订单  1: 订单取消  2:已发送  3: 已接收  4: 已退货 
	TotalPrice	DECIMAL(10,2),
	Address		NVARCHAR(100),
	Phone		VARCHAR(15),
	PostCode	VARCHAR(10),
	Receiver	NVARCHAR(20),	#收货人姓名
	Channel		INTEGER NOT NULL DEFAULT 1,		--送货渠道，1: 送货上门、2: 快递、3: 平邮 4: 特快
	PayType		INTEGER NOT NULL DEFAULT 1,		--支付方式  1: 货到付款、2: 网上支付、3: 银行转账
	HasPay		INTEGER NOT NULL DEFAULT 0,    --是否已经付款，1已付款，0未付款
	TradeNo		VARCHAR(64),
	Appraise	NVARCHAR(150)	--收货人评价
)CHARACTER SET utf8;

CREATE TABLE OrderDetails(
	OrderID		INTEGER REFERENCES Orders(OrderID),
	ProductID	INTEGER REFERENCES Products(ProductID),
	Amount		INTEGER NOT NULL,
	UnitPrice	DECIMAL(10,4) NOT NULL,		--商品的单价信息
	CONSTRAINT pk_OrderDetails PRIMARY KEY(OrderID, ProductID)
)CHARACTER SET utf8;

--User deliver goods info.
CREATE TABLE DeliveryInfos(
	UserID 		INTEGER REFERENCES [Users](UserID) PRIMARY KEY,
	Receiver	NVARCHAR(20) NOT NULL,		--The name of receiver.
	Address		NVARCHAR(100) NOT NULL,		--The address of delivery.
	Phone		VARCHAR(15) NOT NULL,
	PostCode	VARCHAR(10),
	Channel		INTEGER NOT NULL DEFAULT 0,		--The channel of delivery	0: Home delivery	1: Express delivery		2: Normal delivery
	PayType		INTEGER NOT NULL DEFAULT 0			--The pay type				0: cash on delivery	1: pay on line			2: pay on bank transfer.
);

#记录用户的诉求和建议
CREATE TABLE Advises(
	AdviseID	INTEGER AUTO_INCREMENT PRIMARY KEY,
	UserID		INTEGER REFERENCES [Users](UserID),
	Content		NVARCHAR(200) NOT NULL,
	AdviseTime	DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	Viewed		INTEGER NOT NULL DEFAULT 0		--该建议是否被考虑, 0 indicate the advise isn't be adopt.
) CHARACTER SET utf8;

#system billboards.
CREATE TABLE BillBoards(
   BillBoardID  INTEGER PRIMARY KEY AUTO_INCREMENT,
   Content      NVARCHAR(500),
   PublishTime	DATETIME DEFAULT CURRENT_TIMESTAMP,
   AbortTime    DATETIME
) CHARACTER SET utf8;

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


#User favorite list.
CREATE TABLE favoritetb(
	UserID			INTEGER REFERENCES [Users](UserID),
	ProductID		INTEGER REFERENCES Products(ProductID),
	FavoriteTime	DATETIME DEFAULT CURRENT_TIMESTAMP,
	Description		NVARCHAR(200),
	CONSTRAINT pk_Favorites PRIMARY KEY (UserID, ProductID)
) CHARACTER SET utf8;

#用户送货方式的信息(记录最后一次的送货信息，作为其送货信息)
CREATE TABLE usersendinfotb(
	user_id 	INTEGER(32),
	receiver	VARCHAR(20),		#收货人姓名
	address		VARCHAR(100),		#送货地址
	phone1		VARCHAR(15),
	phone2		VARCHAR(15),
	post_code	VARCHAR(10),
	channel		TINYINT,		#送货渠道，0: 送货上门、1: 快递、2: 平邮 3: 特快
	pay		TINYINT			#支付方式  0: 货到付款、1: 网上支付、2: 银行转账
) CHARACTER SET utf8;

