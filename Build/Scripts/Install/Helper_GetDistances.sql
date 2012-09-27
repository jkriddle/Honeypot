declare @stringlo varchar(45)
--set @stringlo = '-76.567033700000025 39.2774113' -- 3500 boston
--set @stringlo = '-76.319752999999992 39.477659' -- branchwood ct
--set @stringlo = '-76.60841700000003 39.286931' -- 500 e. pratt
 set @stringlo = '-76.7857 39.477659'  -- company 1 coordinates?

declare @rangeInMiles int
set @rangeInMiles = 5


-- No need to edit past here
declare @currLocation geography
set @currLocation = geography::STGeomFromText('Point('+Cast(@stringlo as varchar(1000))+')',4326);

declare @rangeInMeters int
set @rangeInMeters = @rangeInMiles * 1609.34

print 'Mile Range : ' + convert(varchar(45), @rangeInMiles)
print 'Meter Range : ' + convert(varchar(45), @rangeInMeters)


--exec sp_GetTripsInRange @stringlo, @rangeInMeters, null, null
-- DOESNT WORK BECAUSE ad.SEQUENCE = 1
SELECT distinct * from Trip where tripid in(SELECT distinct ad.[TripID]
FROM  TripAddresses ad
where ad.location.STDistance(@currLocation) < @rangeInMeters and ad.Sequence = 0)


SELECT distinct ad.[TripID] 
, ad.location.Lat as Lat
, ad.location.Long as Long
, ad.location.STDistance(@currLocation) * 0.000621371 as Miles
, ad.sequence
, ad.addresstype
, t.*
FROM  TripAddresses ad
inner join trip t on ad.tripid=t.tripid
--where ad.location.STDistance(@currLocation) < @rangeInMeters  
 --and ad.Sequence = 1
 order by ad.tripid desc
 
 