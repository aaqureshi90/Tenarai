USE [DB_67163_kanji]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_ideograph_information]    Script Date: 2015/10/18 9:46:07 ******/
DROP PROCEDURE [dbo].[sp_get_ideograph_information]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_ideograph_information]    Script Date: 2015/10/18 9:46:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		MTL
-- Create date: 2015.03.22
-- Description:	Retrieve standard ideograph information

-- @in_ideograph				Input ideograph/symbol we wish to retrieve
--								basic information for
-- @in_ideograph_source			One-character source code corresponding to
--								the language of the ideograph
--								C: Chinese
--								J: Japanese
--								K: Korean

-- @return						Transaction failure: -999
--								Success: 0
--								Invalid character: 1
--								Unsupported language: 2
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_ideograph_information]
	@in_ideograph				DT_SYMBOL = NULL,
	@in_ideograph_source		VARCHAR(1) = NULL
AS
BEGIN

	SET NOCOUNT ON;

	--** Declarations
	DECLARE @retval AS INT

	--** Initializations
	SELECT @retval = -999

	--** Program flow
	IF (UPPER(@in_ideograph_source) = 'C')
		SELECT @retval = 2
	ELSE
		BEGIN
			IF (UPPER(@in_ideograph_source) = 'J')
			BEGIN
				IF ISNULL(@in_ideograph,' ') = ' '
					BEGIN
						SELECT * FROM tblKanji
						SELECT @retval = 0
					END
				ELSE
					BEGIN
						IF EXISTS(SELECT * FROM tblKanji
									WHERE kan_KANJI = @in_ideograph)
						BEGIN
							SELECT * FROM tblKanji
								WHERE kan_Kanji = @in_ideograph

							SELECT @retval = 0
						END
						ELSE
							SELECT @retval = 1
					END
			END
			ELSE
				BEGIN
					IF (UPPER(@in_ideograph_source) = 'K')
						SELECT @retval = 2
					ELSE
						-- Default fall-through for no language
						SELECT @retval = 2
				END
		END

	RETURN @retval
END 



GO
