-- Whenever a user provides a tip for a business, the “numTips” value for that business and the 
-- “tipCount” value for the user should be updated. 

-- numtips

UPDATE Business
SET numTips = T.tipCount
FROM (SELECT Tip.business_id, COUNT(Tip.business_id) AS tipCount
        FROM Tip
        GROUP BY (Tip.business_id)
    ) AS T
WHERE Business.business_id = T.business_id;

-- tipcount

UPDATE Users
SET tipCount = T.tipCount
FROM (SELECT Tip.user_id, COUNT(Tip.user_id) AS tipCount
        FROM Tip
        GROUP BY (Tip.user_id)
    ) as T
WHERE User.user_id = T.user_id

-- Similarly, when a customer checks-in a business, the “numCheckins” attribute value for that 
-- business should be updated.  

--checkin

UPDATE Business
SET numCheckins = C.checkinCount
FROM (SELECT Checkins.business_id, COUNT(Checkins.business) as checkinCount
        FROM Checkins
        GROUP BY (Checkins.business_id)
    ) AS C
WHERE Business.business_id = C.business_id

-- When a user likes a tip, the “totalLikes” attribute value for the user who wrote that tip should be 
-- updated.  

--total likes

UPDATE Users
SET totalLikes = T.likes
FROM (SELECT Tip.user_id, SUM(Tip.likes) AS likes
        FROM Tip
        GROUP BY (Tip.user_id)
    ) AS T
WHERE Users.user_id = T.user_id
