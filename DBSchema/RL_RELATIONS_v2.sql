CREATE TABLE Business
(business_id    CHAR(22),
 name	VARCHAR(100) NOT NULL,
 city   VARCHAR(100) NOT NULL,
 state  CHAR(2) NOT NULL,
 zipcode    CHAR(5) NOT NULL,
 latitude   DECIMAL(13,6) NOT NULL,
 longitude  DECIMAL (13,6) NOT NULL,
 address VARCHAR(100) NOT NULL,
 is_open    BOOLEAN DEFAULT FALSE NOT NULL,
 numTips    INTEGER DEFAULT 0 NOT NULL CHECK (numTips>=0),
 numCheckins    INTEGER DEFAULT 0 NOT NULL CHECK (numCheckins>=0),
 stars  FLOAT CHECK (stars>=0.0 AND stars<=5.0) NOT NULL,
 PRIMARY KEY (business_id)
);

CREATE TABLE Categories
(
business_id CHAR(22),    
category_name   VARCHAR(50),
PRIMARY KEY(business_id, category_name),
FOREIGN KEY (business_id) REFERENCES Business(business_id)
);

CREATE TABLE Attributes
(
business_id CHAR(22),
attr_name   VARCHAR(50),
value   VARCHAR(50) NOT NULL,
PRIMARY KEY (business_id, attr_name),
FOREIGN KEY (business_id) REFERENCES Business(business_id) ON DELETE CASCADE
);

CREATE TABLE Checkins
(
business_id CHAR(22),
checkinDate TIMESTAMP NOT NULL,
PRIMARY KEY(business_id, checkinDate),
FOREIGN KEY (business_id) REFERENCES Business(business_id) ON DELETE CASCADE
);

CREATE TABLE Hours
(business_id    CHAR(22),
dayofweek   VARCHAR(10),
open    TIME(0),
close   TIME(0),
PRIMARY KEY (dayofweek, business_id),
FOREIGN KEY (business_id) REFERENCES Business(business_id) ON DELETE CASCADE
);

CREATE TABLE Users
(user_id    CHAR(22),
 name	VARCHAR(35) NOT NULL,
 yelping_since	DATE NOT NULL,
 tipCount   INTEGER DEFAULT 0 NOT NULL CHECK (tipCount>=0),
 totalLikes INTEGER DEFAULT 0 NOT NULL CHECK (totalLikes>=0),
 cool   INTEGER DEFAULT 0 CHECK (cool>=0),
 funny  INTEGER DEFAULT 0 CHECK (funny>=0),
 useful INTEGER DEFAULT 0 CHECK (useful>=0),
 fans   INTEGER DEFAULT 0 CHECK (fans>=0),
 average_stars  FLOAT CHECK (average_stars>=1.0 AND average_stars <=5.0),
 PRIMARY KEY (user_id)
);

CREATE TABLE Friend
(
user_id CHAR(22),
friend_id CHAR(22),
PRIMARY KEY(user_id, friend_id),
FOREIGN KEY (friend_id) REFERENCES Users(user_id) ON DELETE CASCADE
);

CREATE TABLE Tip
(
user_id CHAR(22),
business_id CHAR(22),
tipDate timestamp,
tipText VARCHAR(750),
likes   INTEGER DEFAULT 0 NOT NULL,
PRIMARY KEY (user_id, business_id, tipDate),
FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE,
FOREIGN KEY (business_id) REFERENCES Business(business_id) ON DELETE CASCADE
);

