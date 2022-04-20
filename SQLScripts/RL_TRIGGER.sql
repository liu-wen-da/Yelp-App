-- Whenever a user provides a tip for a business, the “numTips” value for that business and the 
-- “tipCount” value for the user should be updated. 

-- numTips tipCount

CREATE OR REPLACE FUNCTION updateNumTips()
 RETURNS TRIGGER AS
' 
BEGIN 
    UPDATE Business
    SET numTips = numTips +1
    WHERE Business.business_id = NEW.business_id;
    RETURN NEW;
END
' LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION updateTipCount()
RETURNS TRIGGER AS ' 
BEGIN 
    UPDATE Users
    SET tipCount = tipCount +1
    WHERE Users.user_id = NEW.user_id;
    RETURN NEW;
END
' LANGUAGE plpgsql;

CREATE TRIGGER updateNumTips
AFTER INSERT
ON Tip
FOR EACH ROW
EXECUTE PROCEDURE updateNumTips();

CREATE TRIGGER updateTipCount
AFTER INSERT
ON Tip
FOR EACH ROW
EXECUTE PROCEDURE updateTipCount();

-- Similarly, when a customer checks-in a business, the “numCheckins” attribute value for that 
-- business should be updated.  

-- numCheckins

CREATE OR REPLACE FUNCTION updateCheckins()
RETURNS TRIGGER AS ' 
BEGIN 
    UPDATE Business
    SET numCheckins = numCheckins +1
    WHERE Business.business_id = NEW.business_id;
    RETURN NEW;
END
' LANGUAGE plpgsql;

CREATE TRIGGER updateCheckins
AFTER INSERT
ON Checkins
FOR EACH ROW
EXECUTE PROCEDURE updateCheckins();


-- When a user likes a tip, the “totalLikes” attribute value for the user who wrote that tip should be 
-- updated.  

--totalLikes

CREATE OR REPLACE FUNCTION updateTotalLikes()
RETURNS TRIGGER AS ' 
BEGIN
    UPDATE Users
    SET totalLikes = totalLikes +1
    WHERE Users.user_id = NEW.user_id;
    RETURN NEW;
END
' LANGUAGE plpgsql;

CREATE TRIGGER updateTotalLikes
AFTER UPDATE
OF likes
ON Tip
FOR EACH ROW
WHEN (OLD.likes < NEW.likes) -- only update if the new value is greater
EXECUTE PROCEDURE updateTotalLikes();

-- TESTS

-- Add a tip for a business (insert to tips table) and make sure that the tip, business,  and user 
-- tables are updated correctly. 

-- PASSED
INSERT INTO Tip
VALUES ('4XChL029mKr5hydo79Ljxg','gnKjwL_1w79qoiV3IC_xQQ', '2010-03-04 15:41:16', 'Insert test','0');

-- Check-in to a business (insert to checkins table)  and make sure that the checkin and 
-- business tables are updated correctly.  

-- PASSED
INSERT INTO Checkins
VALUES ('gnKjwL_1w79qoiV3IC_xQQ', '2009-11-13 01:00:36');


-- Like a tip (update the tips table) and make sure that the tips and user tables are updated 
-- correctly. 

-- PASSED
UPDATE Tip
SET likes = likes +1
WHERE business_id = 'gnKjwL_1w79qoiV3IC_xQQ'
AND tipText = 'Insert test';