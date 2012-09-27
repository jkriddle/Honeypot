IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[sp_GetTripsInRange]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[sp_GetTripsInRange]
END

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[sp_GetTripsInRange]

@stringlo varchar(1000),		-- Retrieve trips in relation to this GPS point (see @range)
@range int,						-- Retrieve trips in a radius of this amount from the GPS location
@lastUpdated datetime,			-- Retrieve trips last updated after this time
@tripStatus int,				-- Retrive trips with specific status
@bidThreshold decimal(19,5),	-- Only retrieve trips with lowest bid above this amount
@currentCompanyId int,			-- Only retrieve trips awarded to this company
@isImmediate bit,				-- Only retrieve immediate trips
@biddingCompanyId int,			-- Only retrieve trips this company is bidding on
@onlyValidTrips bit,				-- If true, only retrieve trips that are not past the ValidTill date/time
@riderId int

as
declare @currLocation geography
set @currLocation = geography::STGeomFromText('Point('+Cast(@stringlo as varchar(1000))+')',4326);

SELECT distinct 
	   t.TripID
      ,t.[IsImmediate]
      ,t.[DateCreated]
      ,t.[Notes]
      ,t.[PickupTime]
      ,t.[ValidTill]
      ,t.[TripType]
      ,t.[TripStatus]
      ,t.[EstimatedTotalMileage]
      ,t.[EstimatedTotalTripTime]
      ,t.[RiderTripConfirmation]
      ,t.[DriverTripConfirmation]
      ,t.[CompanyTripConfirmation]
      ,t.[LastUpdated]
      ,t.[RecordLocator]
      ,t.[RiderID]
      ,t.[CompanyID]
      ,t.[DriverID]
      ,t.[PreferredVehicleCategory]
      ,t.[NumPassengers]
	  ,ad.location.STDistance(@currLocation) As DistanceFromUser
FROM Trip t 
	INNER JOIN TripAddresses ad ON t.TripID = ad.TripID
	INNER JOIN Users u ON t.RiderID = u.UserID
WHERE ad.Sequence = 0
	AND (@currLocation is null or ad.location.STDistance(@currLocation) < @range)
	AND (@lastUpdated is null or t.LastUpdated >= @lastUpdated)
	AND (@tripStatus is null or t.TripStatus = @tripStatus) 
	AND (@isImmediate is null or t.IsImmediate = @isImmediate) 
	AND (@bidThreshold is null OR (SELECT Top 1 BidAmount 
			FROM Bids b 
			WHERE b.TripId = t.TripId 
			ORDER BY BidAmount ASC)  >= @bidThreshold
		)
	AND (@currentCompanyId IS NULL 
		OR t.PreferredVehicleCategory IS NULL 
		OR (EXISTS(SELECT * FROM Vehicles v 
					WHERE CompanyId=@currentCompanyId
					AND VehicleCategory=t.PreferredVehicleCategory))
		)
	AND (@biddingCompanyId IS NULL 
		OR EXISTS(
			SELECT * FROM Bids b
				INNER JOIN Users u ON b.UserID = u.UserID
				WHERE u.CompanyID = @biddingCompanyId
				AND b.TripId = t.TripId
		)
	)
	AND (@onlyValidTrips IS NULL
		OR (t.ValidTill >= getutcdate()))
	AND (@riderId IS NULL
		OR (u.UserID = @riderId))
GO