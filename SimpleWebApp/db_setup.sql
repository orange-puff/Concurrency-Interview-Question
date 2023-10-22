-- Create Schema
DROP SCHEMA IF EXISTS MyAppDatabase;
CREATE SCHEMA IF NOT EXISTS MyAppDatabase CHARACTER SET utf8;

-- Create Table
DROP TABLE IF EXISTS MyAppDatabase.MyData;
CREATE TABLE MyAppDatabase.MyData (
                                      DataId int AUTO_INCREMENT,
                                      DataName varchar(255) NOT NULL,
                                      UNIQUE(DataName),
                                      PRIMARY KEY (DataId)
) AUTO_INCREMENT=1;

-- Create Procedure
DROP PROCEDURE IF EXISTS MyAppDatabase.GetOrCreateDataId;
DELIMITER //

CREATE PROCEDURE MyAppDatabase.GetOrCreateDataId(IN dataName VARCHAR(255), OUT resultId INT)
BEGIN
	DECLARE dataId INT;

	-- Get potentially existing DataId
    SELECT MyAppDatabase.MyData.DataId into dataId FROM MyAppDatabase.MyData WHERE MyAppDatabase.MyData.DataName = dataName;

    -- If data is found, return the DataId
    IF dataId IS NULL THEN
        -- If data is not found, insert a new entry and return the generated DataId
        INSERT INTO MyAppDatabase.MyData(DataName) VALUES (dataName);
            SET resultId = LAST_INSERT_ID();
        ELSE
            SET resultId = dataId;
    END IF;
END //
