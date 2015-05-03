-- Drop stored procedure if it already exists
IF EXISTS (SELECT * 
			FROM INFORMATION_SCHEMA.ROUTINES 
			WHERE SPECIFIC_SCHEMA = 'dbo'
			AND SPECIFIC_NAME = 'sp_user_add')
	DROP PROCEDURE dbo.sp_user_add
GO

/*
 * Stored procedure used to create a new user
 */

CREATE PROCEDURE dbo.sp_user_add	@in_USERNAME			DT_USERNAME,
									@in_PASSWORD_ENCRYPTED	DT_PASSWORD,
									@in_FIRST_NAME			DT_NAME,
									@in_LAST_NAME			DT_NAME,
									@in_EMAIL_ADDRESS		DT_EMAIL
AS
BEGIN TRANSACTION

BEGIN TRY
	SELECT	@in_USERNAME = LTRIM(RTRIM(@in_USERNAME)),
			@in_FIRST_NAME = LTRIM(RTRIM(@in_FIRST_NAME)),
			@in_LAST_NAME = LTRIM(RTRIM(@in_LAST_NAME)),
			@in_EMAIL_ADDRESS = LTRIM(RTRIM(@in_EMAIL_ADDRESS))
	
	IF EXISTS (SELECT * 
				FROM tblUser
				WHERE usr_USERNAME = @in_USERNAME)
		RAISERROR('This username is already in use',1,1)
			
	INSERT tblUser(	usr_USERNAME,
					usr_PASSWORD_ENCRYPTED,
					usr_FIRST_NAME,
					usr_LAST_NAME)
			VALUES(	@in_USERNAME,
					@in_PASSWORD_ENCRYPTED,
					@in_FIRST_NAME,
					@in_LAST_NAME)
	
	INSERT tblUserEmail(usr_USERNAME,
						emt_CODE,
						uem_EMAIL_ADDRESS)
				VALUES(	@in_USERNAME,
						'P',
						@in_EMAIL_ADDRESS)
						
	COMMIT TRANSACTION
END TRY

BEGIN CATCH
	ROLLBACK TRANSACTION
	RETURN -999
END CATCH

RETURN 0
GO
