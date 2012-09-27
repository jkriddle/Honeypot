
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBAE7ECF6C5EB984C]') AND parent_object_id = OBJECT_ID('Address'))
alter table Address  drop constraint FKBAE7ECF6C5EB984C


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKD3B18B7EB679DEA2]') AND parent_object_id = OBJECT_ID('Bids'))
alter table Bids  drop constraint FKD3B18B7EB679DEA2


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKD3B18B7ED68DC41C]') AND parent_object_id = OBJECT_ID('Bids'))
alter table Bids  drop constraint FKD3B18B7ED68DC41C


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKCFC3F9A3B679DEA2]') AND parent_object_id = OBJECT_ID('Devices'))
alter table Devices  drop constraint FKCFC3F9A3B679DEA2


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC346FFFBB679DEA2]') AND parent_object_id = OBJECT_ID('Trip'))
alter table Trip  drop constraint FKC346FFFBB679DEA2


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC346FFFBC5EB984C]') AND parent_object_id = OBJECT_ID('Trip'))
alter table Trip  drop constraint FKC346FFFBC5EB984C


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKABBB7ABBD68DC41C]') AND parent_object_id = OBJECT_ID('TripAddresses'))
alter table TripAddresses  drop constraint FKABBB7ABBD68DC41C


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK617D3C3AC5EB984C]') AND parent_object_id = OBJECT_ID('Users'))
alter table Users  drop constraint FK617D3C3AC5EB984C


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK544BB17CC5EB984C]') AND parent_object_id = OBJECT_ID('Zone'))
alter table Zone  drop constraint FK544BB17CC5EB984C


    if exists (select * from dbo.sysobjects where id = object_id(N'Address') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Address

    if exists (select * from dbo.sysobjects where id = object_id(N'Bids') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Bids

    if exists (select * from dbo.sysobjects where id = object_id(N'Company') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Company

    if exists (select * from dbo.sysobjects where id = object_id(N'Devices') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Devices

    if exists (select * from dbo.sysobjects where id = object_id(N'Trip') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Trip

    if exists (select * from dbo.sysobjects where id = object_id(N'TripAddresses') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table TripAddresses

    if exists (select * from dbo.sysobjects where id = object_id(N'Users') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Users

    if exists (select * from dbo.sysobjects where id = object_id(N'Zone') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Zone

    create table Address (
        AddressID INT IDENTITY NOT NULL,
       Line1 NVARCHAR(255) null,
       Line2 NVARCHAR(255) null,
       City NVARCHAR(255) null,
       State NVARCHAR(10) null,
       Zip NVARCHAR(25) null,
       Location GEOGRAPHY null,
       CompanyID INT not null,
       primary key (AddressID)
    )

    create table Bids (
        BidID INT IDENTITY NOT NULL,
       BidAmount DOUBLE PRECISION not null,
       Notes NVARCHAR(255) null,
       DatePlaced DATETIME not null,
       ValidTill DATETIME null,
       BidStatus INT not null,
       UserID INT not null,
       TripID INT not null,
       primary key (BidID)
    )

    create table Company (
        CompanyID INT IDENTITY NOT NULL,
       CompanyLegalName NVARCHAR(255) not null,
       CompanyDba NVARCHAR(255) not null,
       Live24X7Support BIT null,
       ReservationPhoneNumber NVARCHAR(15) not null,
       CompanyWebSite NVARCHAR(255) not null,
       FirstName NVARCHAR(100) not null,
       LastName NVARCHAR(100) not null,
       PhoneNumber NVARCHAR(15) not null,
       EmailAddress NVARCHAR(255) not null,
       BusinessStart INT null,
       RegisterDate DATETIME null,
       ApprovalDate DATETIME null,
       AcceptanceDate DATETIME null,
       RejectedDate DATETIME null,
       CompanyStatus INT null,
       FeeType NVARCHAR(255) null,
       Fee DECIMAL(19,5) null,
       NumberOfVehicles INT null,
       PermitNumber NVARCHAR(2500) null,
       PermitIssuer NVARCHAR(255) null,
       ApprovalToken NVARCHAR(255) null,
       primary key (CompanyID)
    )

    create table Devices (
        DeviceId INT IDENTITY NOT NULL,
       HardwareId NVARCHAR(255) not null,
       AuthToken NVARCHAR(255) null,
       UserId INT not null,
       primary key (DeviceId)
    )

    create table Trip (
        TripID INT IDENTITY NOT NULL,
       IsImmediate INT null,
       Notes NVARCHAR(255) null,
       PickupTime DATETIME null,
       TripType INT not null,
       TripStatus INT not null,
       UserID INT null,
       CompanyID INT null,
       primary key (TripID)
    )

    create table TripAddresses (
        TripAddressesID INT IDENTITY NOT NULL,
       Line1 NVARCHAR(255) null,
       Line2 NVARCHAR(255) null,
       City NVARCHAR(255) null,
       State NVARCHAR(10) null,
       Zip NVARCHAR(25) null,
       Sequence INT null,
       WaitDuration INT null,
       Notes NVARCHAR(255) null,
       Location GEOGRAPHY null,
       AddressType INT null,
       TripID INT not null,
       primary key (TripAddressesID)
    )

    create table Users (
        UserID INT IDENTITY NOT NULL,
       FirstName NVARCHAR(50) not null,
       LastName NVARCHAR(50) not null,
       Email NVARCHAR(150) not null unique,
       CellPhone NVARCHAR(10) null,
       HashedPassword VARBINARY(MAX) not null,
       Salt VARBINARY(MAX) not null,
       Role INT not null,
       ResetPasswordToken NVARCHAR(16) null,
       FacebookId BIGINT null,
       AccessToken NVARCHAR(255) null,
       CompanyID INT null,
       primary key (UserID)
    )

    create table Zone (
        ZoneID INT IDENTITY NOT NULL,
       ZipCode NVARCHAR(255) null,
       CompanyId INT null,
       primary key (ZoneID)
    )

    alter table Address 
        add constraint FKBAE7ECF6C5EB984C 
        foreign key (CompanyID) 
        references Company

    alter table Bids 
        add constraint FKD3B18B7EB679DEA2 
        foreign key (UserID) 
        references Users

    alter table Bids 
        add constraint FKD3B18B7ED68DC41C 
        foreign key (TripID) 
        references Trip

    alter table Devices 
        add constraint FKCFC3F9A3B679DEA2 
        foreign key (UserId) 
        references Users

    alter table Trip 
        add constraint FKC346FFFBB679DEA2 
        foreign key (UserID) 
        references Users

    alter table Trip 
        add constraint FKC346FFFBC5EB984C 
        foreign key (CompanyID) 
        references Company

    alter table TripAddresses 
        add constraint FKABBB7ABBD68DC41C 
        foreign key (TripID) 
        references Trip

    alter table Users 
        add constraint FK617D3C3AC5EB984C 
        foreign key (CompanyID) 
        references Company

    alter table Zone 
        add constraint FK544BB17CC5EB984C 
        foreign key (CompanyId) 
        references Company
