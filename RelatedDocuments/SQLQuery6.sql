/* create table Enterprises(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[Name] nvarchar(255) NOT NULL,
	[TaxCode] varchar(255) UNIQUE NOT NULL,
	[Address] nvarchar(500) NULL,
	[PhoneNumber] varchar(255) NULL,
	[Email] varchar(255) NULL,
	[Type] nvarchar(255) NULL,
	[CreatedAt] datetime2 NOT NULL,
	[UserId] nvarchar(450),
	constraint Enterprise_User foreign key (UserId) references AspNetUsers(Id) on delete cascade
);

create table Categories (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[Name] nvarchar(500) NOT NULL,
	[Description] text NULL,
	[IsDefault] bit NOT NULL,
	[UserId] nvarchar(450) NULL,
	[EnterpriseId] UNIQUEIDENTIFIER NULL,
	constraint Category_User foreign key (UserId) references AspNetUsers(Id) on delete no action,
	constraint Category_Enterprise foreign key (EnterpriseId) references Enterprises(Id) on delete set null
);

create table Products(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[Code] varchar(500) UNIQUE NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Unit] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[Quantity] [int] NULL,
	[PriceImport] [decimal](18, 2) NULL,
	[PriceWholesale] [decimal](18, 2) NULL,
	[PriceRetail] [decimal](18, 2) NULL,
	[InventoryStandard] [int] NULL,
	[Discount] [decimal](18, 2) NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[CategoryId] [UNIQUEIDENTIFIER] NULL,
	[EnterpriseId] [UNIQUEIDENTIFIER] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[Status] [nvarchar](50) NULL,
	[Website] [nvarchar](255) NULL,
	constraint Product_Category foreign key (CategoryId) references Categories(Id) on delete set null,
	constraint Product_Enterprise foreign key (EnterpriseId) references Enterprises(Id) on delete set null,
	constraint Product_User foreign key (UserId) references AspNetUsers(Id) on delete no action,
);

create table Factories(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[Name] nvarchar(500) NOT NULL,
	[Address] nvarchar(500) NULL,
	[ContactInfo] nvarchar(500) NULL,
	[CreatedAt] datetime2 NOT NULL,
	[UserId] nvarchar(450) NOT NULL,
	[EnterpriseId] UNIQUEIDENTIFIER NULL,
	constraint Factory_User foreign key(UserId) references AspNetUsers(Id) on delete no action,
	constraint Factory_Enterprise foreign key(EnterpriseId) references Enterprises(Id) on delete set null
);

create table [Batches] (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[ProductId] UNIQUEIDENTIFIER NOT NULL,
	[Code] varchar(500) NOT NULL,
	[ManufactureDate] datetime NULL,
	[ExpireDate] datetime NULL,
	[Quantity] int NOT NULL,
	[Status] nvarchar(255) NULL,
	[FactoryId] UNIQUEIDENTIFIER NULL,
	[EnterpriseId] UNIQUEIDENTIFIER NULL,
	[UserId] nvarchar(450) NOT NULL,
	[CreatedAt] datetime2 NOT NULL,
	UNIQUE (ProductId, Code),
	constraint Batch_Product foreign key (ProductId) references Products(Id) on delete cascade,
	constraint Batch_Factory foreign key (FactoryId) references Factories(Id) on delete set null,
	constraint Batch_Enterprise foreign key (EnterpriseId) references Enterprises(Id) on delete set null,
	constraint Batch_User foreign key (UserId) references AspNetUsers(Id) on delete no action
);

create table [TraceEvents](
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[BatchId] UNIQUEIDENTIFIER NOT NULL,
	[EventType] nvarchar(500) NOT NULL,
	[Description] text NULL,
	[Location] nvarchar(500) NULL,
	[TimeStamp] datetime2 NOT NULL,
	[UserId] nvarchar(450) NOT NULL,
	[EnterpriseId] UNIQUEIDENTIFIER NULL,
	[AttachmentUrl] varchar(500) NULL,
	constraint Trace_Batch foreign key (BatchId) references [Batches](Id) on delete cascade,
	constraint Trace_Enterprise foreign key (EnterpriseId) references Enterprises(Id) on delete set null,
	constraint Trace_User foreign key (UserId) references AspNetUsers(Id) on delete no action
);

create table [Files] (	
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[FileName] nvarchar(255) NULL,
	[FileType] nvarchar(50) NULL,
	[FileExtension] nvarchar(20) NULL,
	[Size] BIGINT NULL,
	[EntityType] nvarchar(50) NULL,
	[EntityId] UNIQUEIDENTIFIER NULL,
	[UserId] nvarchar(450) NOT NULL,
	[CreatedAt] datetime2 NOT NULL,
	constraint File_User foreign key (UserId) references [AspNetUsers](Id) on delete no action
); 

create table [Comments] (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[ProductId] UNIQUEIDENTIFIER NOT NULL,
	[Content] nvarchar(4000) NOT NULL,
	[UserId] nvarchar(450) NOT NULL,
	[CreatedAt] datetime2 NOT NULL,
	constraint Comment_User foreign key (UserId) references [AspNetUsers](Id) on delete no action,
	constraint Comment_Product foreign key (ProductId) references [Products](Id) on delete cascade
);


alter table [Batches]
	drop constraint Batch_Enterprise;

alter table [Batches] 
	drop column EnterpriseId;

alter table [TraceEvents]
	drop constraint Trace_Enterprise;

alter table [TraceEvents]
	drop column EnterpriseId;

alter table [TraceEvents]
	add CreatedAt datetime2;

alter table [Categories]
	add CreatedAt datetime2;


alter table [Products] 
	alter column [Description] nvarchar(Max) NULL;

alter table [Categories]
	alter column [Description] nvarchar(Max) NULL;

alter table [TraceEvents]
	alter column [Description] nvarchar(Max) NULL;



alter table [Enterprises]
	drop constraint Enterprise_User;

alter table [Enterprises]
	drop column UserId;



create table [EnterpriseUser](
	[Id] UNIQUEIDENTIFIER primary key,
	[EnterpriseId] UNIQUEIDENTIFIER NOT NULL,
	[UserId] nvarchar(450) NOT NULL,
	[UserId] bit NULL,
	constraint User_Enterprise foreign key (EnterpriseId) references [Enterprises](Id) on delete cascade,
	constraint Enterprise_User foreign key (UserId) references [AspNetUsers](Id) on delete no action
);



alter table [UserEnterprise]
	add unique(EnterpriseId, UserId);

alter table [Products]
	drop column PriceImport;

alter table [Products]
	drop column PriceWholesale;

alter table [Products]
	drop column PriceRetail;

alter table [Products]
	add Price [decimal](18, 2) NULL;



alter table [Products]
	drop column InventoryStandard;

CREATE TABLE [dbo].[RefreshTokens](
	[Id] [uniqueidentifier] primary key,
	[Token] [nvarchar](500) NOT NULL UNIQUE,
	[UserId] [nvarchar](450) NOT NULL,
	[ExpireTime] [datetime2](7) NOT NULL,
	constraint [RefreshToken_User] foreign key (UserId) references [AspNetUsers](Id) on delete cascade
);


alter table [AspNetUsers]
	alter column [PhoneNumber] nvarchar(500) NOT NULL;

alter table [AspNetUsers]
	add unique (PhoneNumber);



alter table [Enterprises]
	add GLNCode varchar(13) unique NULL;



alter table [EnterpriseUser]
	add unique (UserId, EnterpriseId);



alter table [AspNetUsers]
	add [Name] nvarchar(500) NOT NULL;



alter table [Enterprises]
	add [UpdatedAt] datetime2 NULL;

alter table [Enterprises]
	add [UpdatedBy] nvarchar(450) NULL;

alter table [Enterprises]
	add constraint EnterpriseUser_Update foreign key (UpdatedBy) references [AspNetUsers](Id) on delete set null; 



alter table [Categories]
	drop constraint Category_Enterprise;

alter table [Categories]
	drop column EnterpriseId;



alter table [AspNetUsers]
	add [CreatedAt] datetime2 null;



alter table [Products]
	drop column [Unit];


alter table [Batches]
	drop constraint Batch_Factory;

alter table [Batches]
	drop constraint Batch_Product;

alter table [Batches]
	drop constraint Batch_User;

drop table [Batches];

drop table [TraceEvents];

alter table [Products]
	add [TraceCode] varchar(500) NOT NULL;

EXEC sp_rename 'Products.Code',  'ProductCode', 'COLUMN';



alter table [Products]
	add [UserResponseId] nvarchar(450) NULL;

alter table [Products]
	add constraint Product_UserResponse foreign key (UserResponseId) references  [AspNetUsers] (Id) on delete set null;

alter table [Products]
	add [FactoryId] UNIQUEIDENTIFIER NULL;

alter table [Products]
	add constraint Product_Factory foreign key (FactoryId) references [Factories](Id) on delete set null;

alter table [Products]
	add [ProducerEnterpriseId] UNIQUEIDENTIFIER NULL;


EXEC sp_rename 'Products.EnterpriseId',  'OwnerEnterpriseId', 'COLUMN';

alter table [Products]
	drop constraint Product_Enterprise;

alter table [Products]
	add constraint Product_OwnerEnterprise foreign key (OwnerEnterpriseId) references [Enterprises](id) on delete no action;

alter table [Products]
	add constraint Product_ProducerEnterprise foreign key (ProducerEnterpriseId) references [Enterprises](Id) on delete set null;

alter table [Products]
	add [CarrierEnterpriseId] UNIQUEIDENTIFIER NULL;



alter table [Products]
	add constraint Product_CarrierEnterprise foreign key (ProducerEnterpriseId) references [Enterprises](Id) on delete no action;

alter table [Products]
	alter column [Website] nvarchar(1000) NULL;



EXEC sp_rename 'Products.UserId',  'CreatedUserId', 'COLUMN';

EXEC sp_rename 'Products.UserResponseId',  'ResponsibleUserId', 'COLUMN';

alter table [Products]
	drop constraint Product_User;

alter table [Products]	
	drop constraint Product_UserResponse;

alter table [Products]
	add constraint Product_UserCreated foreign key (CreatedUserId) references  [AspNetUsers] (Id) on delete no action;

alter table [Products]
	add constraint Product_ResponsibleUser foreign key (ResponsibleUserId) references  [AspNetUsers] (Id) on delete set null;

alter table [Factories]
	add [OwnerUserId] nvarchar(450) NULL;

alter table [Factories]
	add constraint Factory_OwnerUserId foreign key(OwnerUserId) references [AspNetUsers](Id) on delete no action;

EXEC sp_rename 'Factories.UserId',  'CreatedUserId', 'COLUMN';

alter table [Factories]	
	drop constraint Factory_User;

alter table [Factories]
	alter column [CreatedUserId] nvarchar(450) NULL;

alter table [Factories]
	add constraint Factory_CreatedUser foreign key(CreatedUserId) references [AspNetUsers](Id) on delete set null;



EXEC sp_rename 'Factories.EnterpriseId',  'OwnerEnterpriseId', 'COLUMN';



EXEC sp_rename 'Factories.OwnerEnterpriseId',  'EnterpriseId', 'COLUMN';



alter table [Categories]
	drop column IsDefault;



alter table [Products]
	add [OwnerUserId] nvarchar(450) NULL;


alter table [Products]
	add constraint Product_OwnerUser foreign key ([OwnerUserId]) references [AspNetUsers]([Id]) on delete no action;



alter table [Products]
	drop column Quantity;

alter table [Products]
	drop column Discount;



alter table [Products]
	drop column Status;



create table [Batches] (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[ProductId] UNIQUEIDENTIFIER NOT NULL,
	[Code] varchar(500) NOT NULL,
	[ManufactureDate] datetime NULL,
	[ExpireDate] datetime NULL,
	[Quantity] int NOT NULL,
	[Status] nvarchar(255) NULL,
	[FactoryId] UNIQUEIDENTIFIER NULL,
	[EnterpriseId] UNIQUEIDENTIFIER NULL,
	[CreatedUserId] nvarchar(450) NOT NULL,
	[CreatedAt] datetime2 NOT NULL,
	UNIQUE (ProductId, Code),
	constraint Batch_Product foreign key (ProductId) references Products(Id) on delete cascade,
	constraint Batch_Factory foreign key (FactoryId) references Factories(Id) on delete set null,
	constraint Batch_Enterprise foreign key (EnterpriseId) references Enterprises(Id) on delete set null,
	constraint Batch_User foreign key (CreatedUserId) references AspNetUsers(Id) on delete no action
);

create table [TraceEvents](
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[BatchId] UNIQUEIDENTIFIER NOT NULL,
	[EventType] nvarchar(500) NOT NULL,
	[Description] nvarchar(4000) NULL,
	[Location] nvarchar(500) NULL,
	[TimeStamp] datetime2 NOT NULL,
	[UserId] nvarchar(450) NOT NULL,
	[EnterpriseId] UNIQUEIDENTIFIER NULL,
	constraint Trace_Batch foreign key (BatchId) references [Batches](Id) on delete cascade,
	constraint Trace_Enterprise foreign key (EnterpriseId) references Enterprises(Id) on delete set null,
	constraint Trace_User foreign key (UserId) references AspNetUsers(Id) on delete no action
);



alter table [Products]
	drop column ProductCode;




alter table [Batches]
	drop constraint Batch_Enterprise;

alter table [Batches]
	drop column EnterpriseId;

alter table [TraceEvents]
	drop constraint Trace_Enterprise;

alter table [TraceEvents]
	drop column EnterpriseId;

EXEC sp_rename 'TraceEvents.UserId',  'CreatedUserId', 'COLUMN';


create table IndividualEnterprises(
	[OwnerUserId] nvarchar(450) PRIMARY KEY,
	[Name] nvarchar(255) NOT NULL,
	[TaxCode] varchar(255) NULL,
	[Address] nvarchar(500) NULL,
	[PhoneNumber] varchar(255) NULL,
	[Email] varchar(255) NULL,
	[Type] nvarchar(255) NULL,
	[CreatedAt] datetime2 NOT NULL,
	constraint IndividualEnterprise_User foreign key (OwnerUserId) references AspNetUsers(Id) on delete cascade
);


alter table IndividualEnterprises
	add GLNCode varchar(13) NULL unique;


alter table Factories
	drop constraint Factory_OwnerUserId;

alter table Factories
	drop column OwnerUserId;

alter table Factories
	add IndividualEnterpriseId nvarchar (450) NULL;

alter table Factories
	add constraint Factory_IndividualEnterprise foreign key (IndividualEnterpriseId) references [IndividualEnterprises](OwnerUserId) on delete no action;

	EXEC sp_rename 'Factories.OwnerIndividualEnterpriseId',  'IndividualEnterpriseId', 'COLUMN';


EXEC sp_rename 'Categories.UserId',  'CreatedUserId', 'COLUMN';



alter table [Products]
	drop constraint Product_OwnerUser;

alter table [Products]
	drop column OwnerUserId;

alter table [Products]
	add OwnerIndividualEnterpriseId nvarchar(450) NULL;



alter table [Products]
	add constraint Product_IndividualEnterprise foreign key ([OwnerIndividualEnterpriseId]) references [IndividualEnterprises](OwnerUserId) on delete no action;


alter table [Products]
	add unique (TraceCode);



alter table [Enterprises]	
	add unique (TaxCode);


alter table [Factories]
	add FactoryCode varchar(255) NOT NULL UNIQUE;

*/
alter table [IndividualEnterprises]
	add IndividualEnterpriseCode varchar(255) NOT NULL UNIQUE;

alter table [Enterprises]
	add EnterpriseCode varchar(255) NOT NULL UNIQUE;

	
	
-- Nhiều khóa ngoại trong 1 bảng thì bắt buộc có 1 khóa ngoại phải là Ondelete NoAction. Để Ondelete NoAction chỉ ở khóa ngoại liên kết với bảng User vì tất cả các bảng đều liên kết với bảng User nên chi xóa bản ghi của bảng User mới phải xóa thủ công bảng nhiều 