IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ExpireTripsAndBids]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ExpireTripsAndBids]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[sp_ExpireTripsAndBids] as
update bids set bidstatus = 0  where tripid in( select tripid from trip where validTill < GetDate() and tripstatus = 2) and bidstatus = 1
update trip set tripstatus = 0 where ValidTill < GetDate() and tripstatus = 2
GO


/****** Object:  Job [ExpireTripsAndBids]    Script Date: 08/09/2012 10:39:11 ******/
declare @jobid as uniqueidentifier
SELECT @jobId = job_id FROM msdb.dbo.sysjobs WHERE (name = N'ExpireTripsAndBids')
IF (@jobId IS NOT NULL)
BEGIN
    EXEC msdb.dbo.sp_delete_job @jobId,@delete_unused_schedule=1
END 
GO

USE [msdb]
GO

/****** Object:  Job [ExpireTripsAndBids]    Script Date: 08/09/2012 10:39:11 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [Database Maintenance]    Script Date: 08/09/2012 10:39:11 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'Database Maintenance' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'Database Maintenance'
--IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END
DECLARE @owner varchar(255)
SELECT  @owner=sid from sys.server_principals p WHERE name='sa'
select suser_sname(owner_sid) from sys.databases where name = 'Northwind'

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'ExpireTripsAndBids', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'Checks the trips table for any trips that ''ValidTill'' has passed and has not accepted a bid.  Collects bids for each expired trip and expires the bid.  Finally expires the trip', 
		@category_name=N'Database Maintenance', 
		@owner_login_name=N'sa', 
		@job_id = @jobId OUTPUT
--IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Run Procedure]    Script Date: 08/09/2012 10:39:11 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Run Procedure', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'sp_ExpireTripsAndBids', 
		@database_name=N'CarFareCompare', 
		@flags=0
--IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
--IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'ExpireTripsAndBids', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=8, 
		@freq_subday_interval=1, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20120809, 
		@active_end_date=99991231, 
		@active_start_time=0, 
		@active_end_time=235959, 
		@schedule_uid=N'28b6fd81-e321-407d-90b3-0086cd36392c'
--IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
--IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
--GOTO EndSave
--QuitWithRollback:
--    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
--EndSave:

GO


