DROP TABLE IF EXISTS ADVERTISEMENTS
DROP TABLE IF EXISTS USER_DETAILS
DROP TABLE IF EXISTS USERS
DROP TABLE IF EXISTS CITIES
DROP TABLE IF EXISTS REGIONS
DROP TABLE IF EXISTS CATEGORIES
DROP TABLE IF EXISTS COLORS
DROP TABLE IF EXISTS ENGINE_TYPES
DROP TABLE IF EXISTS BIKE_MODELS
DROP TABLE IF EXISTS BIKE_BRANDS 
GO

-- DBCC CHECKIDENT('your_table', RESEED, 0)


-- Добавяне на таблица марки
-- ////////////////////////////

CREATE TABLE BIKE_BRANDS
(
	ID		INT			NOT NULL IDENTITY(1,1),
	NAME	NVARCHAR(32) NOT NULL

	CONSTRAINT PK_BIKE_BRANDS_ID PRIMARY KEY (ID)
)
GO

-- Добавяне на таблица модели
-- ////////////////////////////

CREATE TABLE BIKE_MODELS
(
	ID			INT			NOT NULL IDENTITY(1,1),
	BRAND_ID	INT				NOT NULL,
	NAME		NVARCHAR(32)	NOT NULL

	CONSTRAINT PK_BIKE_MODELS_ID PRIMARY KEY (ID)
)
GO

ALTER TABLE BIKE_MODELS
ADD CONSTRAINT FK_BIKE_BRANDS_BRAND_ID FOREIGN KEY (BRAND_ID) REFERENCES BIKE_BRANDS(ID)
GO

-- Добавяне на таблица типове двигатели
-- ////////////////////////////

CREATE TABLE ENGINE_TYPES
(
	ID		INT			NOT NULL IDENTITY(1,1),
	NAME	NVARCHAR(32) NOT NULL

	CONSTRAINT PK_ENGINE_TYPES_ID PRIMARY KEY (ID)
)
GO

-- Добавяне на таблица цветове
-- ////////////////////////////

CREATE TABLE COLORS
(
	ID		INT			NOT NULL IDENTITY(1,1),
	NAME	NVARCHAR(32) NOT NULL

	CONSTRAINT PK_COLORS_ID PRIMARY KEY (ID)
)
GO

-- Добавяне на таблица категория мотоциклети
-- ////////////////////////////

CREATE TABLE CATEGORIES
(
	ID		INT			NOT NULL IDENTITY(1,1),
	NAME	NVARCHAR(32) NOT NULL

	CONSTRAINT PK_CATEGORIES_ID PRIMARY KEY (ID)
)
GO

-- Добавяне на таблица региони
-- ////////////////////////////

CREATE TABLE REGIONS
(
	ID		INT			NOT NULL IDENTITY(1,1),
	NAME	NVARCHAR(32) NOT NULL

	CONSTRAINT PK_REGIONS_ID PRIMARY KEY (ID)
)
GO

-- Добавяне на таблица градове
-- ////////////////////////////

CREATE TABLE CITIES
(
	ID			INT			NOT NULL IDENTITY(1,1),
	REGION_ID	INT			NOT NULL,
	NAME		NVARCHAR(32) NOT NULL

	CONSTRAINT PK_CITIES_ID PRIMARY KEY (ID)
)
GO

ALTER TABLE CITIES
ADD CONSTRAINT FK_CITIES_REGION_ID FOREIGN KEY (REGION_ID) REFERENCES REGIONS(ID)
GO

-- Добавяне на таблица потребители
-- ////////////////////////////

CREATE TABLE USERS
(
	ID			INT				NOT NULL IDENTITY(1,1),
	USERNAME	NVARCHAR(128)	NOT NULL,
	PASSWORD	CHAR(64)		NOT NULL,
	IS_ADMIN	BIT				NOT NULL

	CONSTRAINT PK_USERS_ID PRIMARY KEY (ID)
)
GO

-- Добавяне на таблица данни за потребител
-- ////////////////////////////

CREATE TABLE USER_DETAILS
(
	USER_ID		INT				NOT NULL,
	FIRST_NAME	NVARCHAR(128)	NOT NULL,
	LAST_NAME	NVARCHAR(128)	NOT NULL,
	PHONE		NVARCHAR(16)		NOT NULL

	CONSTRAINT PK_USER_DETAILS_ID PRIMARY KEY (USER_ID)
)
GO

ALTER TABLE USER_DETAILS 
ADD CONSTRAINT FK_USER_DETAILS_USER_ID FOREIGN KEY (USER_ID) REFERENCES USERS(ID)
GO

-- Добавяне на таблица обяви
-- ////////////////////////////

CREATE TABLE ADVERTISEMENTS
(
	ID				INT				NOT NULL IDENTITY(1,1),
	MODEL_ID		INT				NOT NULL,
	PHOTO_PATH		NVARCHAR(512)	NOT NULL,
	EXTRAS			VARBINARY(16)	NOT NULL,
	HORSEPOWER		INT				NOT NULL,
	ENGINE_SIZE		INT				NOT NULL,
	ENGINE_TYPE_ID	INT				NOT NULL,
	PRODUCTION_YEAR DATE			NOT NULL,
	MILEAGE			INT				NOT NULL,
	PRICE			FLOAT			NOT NULL,
	CITY_ID			INT				NOT	NULL,
	COLOR_ID		INT				NOT NULL,
	CATEGORY_ID		INT				NOT NULL,
	DESCRIPTION  	NVARCHAR(512) 	NOT NULL,
	USER_ID			INT				NOT NULL
	CONSTRAINT PK_ADVERTISEMENTS_ID PRIMARY KEY (ID)
)
GO

ALTER TABLE ADVERTISEMENTS
ADD CONSTRAINT FK_ADVERTISEMENTS_MODEL_ID FOREIGN KEY (MODEL_ID) REFERENCES BIKE_MODELS(ID),
CONSTRAINT FK_ADVERTISEMENTS_ENGINE_TYPE_ID FOREIGN KEY (ENGINE_TYPE_ID) REFERENCES ENGINE_TYPES(ID),
CONSTRAINT FK_ADVERTISEMENTS_CITY_ID FOREIGN KEY (CITY_ID) REFERENCES CITIES(ID),
CONSTRAINT FK_ADVERTISEMENTS_COLOR_ID FOREIGN KEY (COLOR_ID) REFERENCES COLORS(ID),
CONSTRAINT FK_ADVERTISEMENTS_CATEGORY_ID FOREIGN KEY (CATEGORY_ID) REFERENCES CATEGORIES(ID),
CONSTRAINT FK_ADVERTISEMENTS_USER_ID FOREIGN KEY (USER_ID) REFERENCES USERS(ID)
GO