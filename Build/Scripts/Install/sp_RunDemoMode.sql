IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[sp_RunDemoMode]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[sp_RunDemoMode]
END

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[sp_RunDemoMode]

as


DECLARE @riderID int
DECLARE @driverID int
DECLARE @companyID int
DECLARE @vehicleID int
DECLARE @tripID int
DECLARE @lowBid float

--###### Bid on most recent ride (random driver) ######
-- Get random driver/company/dispatcher
SELECT TOP 1 @driverID = UserID, @companyID = CompanyID 
	FROM Users 
	WHERE Role = 2 OR Role = 3 OR Role = 4 
	ORDER BY NEWID()

-- Get a vehicle
SELECT TOP 1 @vehicleID = VehicleID 
	FROM Vehicles 
	WHERE CompanyID=@companyID

-- Get most recent ride ID
SELECT TOP 1 @tripID = TripID 
	FROM Trip 
	WHERE TripStatus = 2 
	ORDER BY TripID DESC

-- Get lowest bid
SELECT TOP 1 @lowBid = BidAmount 
	FROM Bids 
	WHERE TripID=@tripID

IF @lowBid > 5
BEGIN

	-- Add bid
	SET @lowBid = @lowBid - 5
	INSERT INTO Bids (BidAmount, Notes, DatePlaced, ValidTill, BidStatus, CancelationDeadline, UserID, TripID, VehicleID)
		VALUES (@lowBid, NULL, SYSUTCDATETIME(), DATEADD(Hour, 1, SYSUTCDATETIME()), 1, DATEADD(Hour, 1, SYSUTCDATETIME()), @driverID, @tripID, @vehicleID)

END

--###### Bid on a random ride (random driver) ######
-- Get random driver/company/dispatcher
SELECT TOP 1 @driverID = UserID, @companyID = CompanyID 
	FROM Users 
	WHERE Role = 2 OR Role = 3 OR Role = 4 
	ORDER BY NEWID()

-- Get a vehicle
SELECT TOP 1 @vehicleID = VehicleID 
	FROM Vehicles 
	WHERE CompanyID=@companyID

-- Get most recent ride ID
SELECT TOP 1 @tripID = TripID 
	FROM Trip 
	WHERE TripStatus = 2 
	ORDER BY NEWID()

-- Get lowest bid
SELECT TOP 1 @lowBid = BidAmount 
	FROM Bids 
	WHERE TripID=@tripID

IF @lowBid > 5
BEGIN

	-- Add bid
	SET @lowBid = @lowBid - 5
	INSERT INTO Bids (BidAmount, Notes, DatePlaced, ValidTill, BidStatus, CancelationDeadline, UserID, TripID, VehicleID)
		VALUES (@lowBid, NULL, SYSUTCDATETIME(), DATEADD(Hour, 1, SYSUTCDATETIME()), 1, DATEADD(Hour, 1, SYSUTCDATETIME()), @driverID, @tripID, @vehicleID)

END

--###### Create new trip ######
-- Get random rider
SELECT TOP 1 @riderID = UserID 
	FROM Users 
	WHERE Role = 1 
	ORDER BY NEWID()
	
-- Create trip
DECLARE @randomNum int
SELECT @randomNum = CAST(RAND() * 2 AS INT)
DECLARE @isImmediate bit
IF @randomNum = 1
	SET @isImmediate = 1
ELSE
	SET @isImmediate = 0
	
DECLARE @totalMileage int
SET @totalMileage = CAST(RAND() * 200 AS INT)

DECLARE @estimatedTotalTripTime bigint
SET @estimatedTotalTripTime = CAST(RAND() * 60000000000 AS BIGINT) + 14000000000

DECLARE @numPassengers int
SET @numPassengers = CAST(RAND() * 4 AS INT)

INSERT INTO Trip (LastUpdated, DateCreated, IsImmediate, Notes, 
					PickupTime, ValidTill, 
					TripType, TripStatus, EstimatedTotalMileage, EstimatedTotalTripTime, 
					RiderTripConfirmation, DriverTripConfirmation, CompanyTripConfirmation, 
					PreferredVehicleCategory, DistanceFromUser, NumPassengers, 
					RecordLocator, RiderID, CompanyID, DriverID)
	VALUES(SYSUTCDATETIME(), SYSUTCDATETIME(), @isImmediate, NULL, 
	DATEADD(Hour, 1, SYSUTCDATETIME()), DATEADD(Hour, 1, SYSUTCDATETIME()),
	1, 2, @totalMileage, @estimatedTotalTripTime, 
	NULL, NULL, NULL,
	NULL, 5020, @numPassengers, 'XYZ123',
	@riderID, NULL, NULL
)

declare @lat as float
declare @long as float
declare @currLocation geography

SET @lat = CAST(RAND() AS float)  - 76
SET @long = CAST(RAND() AS float) + 39
set @currLocation = geography::STGeomFromText('Point(' + Cast(@lat as varchar(1000)) + ' ' + Cast(@long as varchar(1000)) + ')',4326);

SELECT @tripID = SCOPE_IDENTITY()
	
INSERT INTO TripAddresses (Line1, Line2, City, State, Zip, Sequence, WaitDuration, 
							Notes, Location, AddressType, EstimatedMileage, 
							EstimatedTripTime, TripID)
	VALUES ('Demo Address', NULL, 'Baltimore', 'MD', '21124', 0, 0, NULL, 
		@currLocation, 1, 0, 0, @tripID)
			
			
SET @lat = CAST(RAND() AS float)  - 76
SET @long = CAST(RAND() AS float) + 39
set @currLocation = geography::STGeomFromText('Point(' + Cast(@lat as varchar(1000)) + ' ' + Cast(@long as varchar(1000)) + ')',4326);

INSERT INTO TripAddresses (Line1, Line2, City, State, Zip, Sequence, WaitDuration, 
							Notes, Location, AddressType, EstimatedMileage, 
							EstimatedTripTime, TripID)
	VALUES ('Demo Address', NULL, 'Baltimore', 'MD', '21124', 0, 0, NULL, 
		@currLocation, 2, @totalMileage, @estimatedTotalTripTime, @tripID)