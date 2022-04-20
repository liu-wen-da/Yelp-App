#CptS 451 - Spring 2022
# https://www.psycopg.org/docs/usage.html#query-parameters

#  if psycopg2 is not installed, install it using pip installer :  pip install psycopg2  (or pip3 install psycopg2)
import json
from unicodedata import category
import psycopg2

def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")

def int2BoolStr (value):
    if value == 0:
        return 'False'
    else:
        return 'True'

def insert2BusinessTable():
    #reading the JSON file
    with open('.//yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='YELP' user='postgres' host='localhost' password='tarlydyl'")

        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            # Generate the INSERT statement for the current business
            try:
                cur.execute("INSERT INTO Business (business_id, name, address, state, city, zipcode, latitude, longitude, stars, numCheckins, numTips, is_open)"
                       + " VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)",
                         (data['business_id'],cleanStr4SQL(data["name"]), cleanStr4SQL(data["address"]), data["state"], data["city"], data["postal_code"], data["latitude"], data["longitude"], data["stars"], 0 , 0 , [False,True][data["is_open"]] ) )
            except Exception as e:
                print("Insert to Business failed!",e)
            conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2Categories():
    #reading the JSON file
    with open('.//yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='YELP' user='postgres' host='localhost' password='tarlydyl'")

        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)

            categories = data["categories"].split(', ')

            for i in categories:
                cat = "INSERT INTO Categories (business_id, category_name) " + "VALUES ('"  + data['business_id'] + "', '" + cleanStr4SQL(i) + "');"
                # Generate the INSERT statement for the current categories
                try:
                    cur.execute(cat)
                except Exception as e:
                    print("Insert to Categories failed!",e)
                conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def flaten(d):
    l = []
    for i in d.items():
        # check if there's an inner dict
        if type(i[1]) is dict:
            l += flaten(i[1])
        else:
            l.append((i[0], i[1]))
    return l

def insert2Attributes():
    #reading the JSON file
    with open('.//yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='YELP' user='postgres' host='localhost' password='tarlydyl'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()


        while line:
            data = json.loads(line)
            attributes = flaten(data["attributes"])
            #print(attributes)
            for i in attributes:
                try:
                        cur.execute("INSERT INTO Attributes (business_id, attr_name, value) "
                            + "VALUES (%s,%s,%s)",
                            (data["business_id"],i[0], i[1]))

                except Exception as e:
                    print("Insert to Attributes failed!",e)
                conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2UserTable():
    #reading the JSON file
    with open('.//yelp_user.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='YELP' user='postgres' host='localhost' password='tarlydyl'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            # Generate the INSERT statement for the current User
            try:
                cur.execute("INSERT INTO Users (user_id, name, yelping_since, tipCount, totalLikes, cool, funny, useful, fans, average_stars)"
                       + " VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s)",
                         (data['user_id'],data["name"], data["yelping_since"], 0, 0, data["cool"], data["funny"], data["useful"], data["fans"], data["average_stars"]) )
            except Exception as e:
                print("Insert to Users failed!",e)
            conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2TipTable():
    #reading the JSON file
    with open('.//yelp_tip.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='YELP' user='postgres' host='localhost' password='tarlydyl'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            # Generate the INSERT statement for the current business
            try:
                cur.execute("INSERT INTO Tip (user_id, business_id, tipDate, tipText, likes)"
                       + " VALUES (%s, %s, %s, %s, %s)",
                         (data['user_id'],data['business_id'], cleanStr4SQL(data["date"]), cleanStr4SQL(data["text"]), data["likes"]) )
            except Exception as e:
                print("Insert to Tip failed!",e)
            conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2CheckinTable():
      #reading the JSON file
    with open('.//yelp_checkin.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='YELP' user='postgres' host='localhost' password='tarlydyl'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()



        while line:
            data = json.loads(line)

            date = data["date"].split(',')

            for i in date:
                # Generate the INSERT statement for the current business
                try:
                    cur.execute("INSERT INTO Checkins (business_id, checkinDate)"
                        + "VALUES (%s,%s)",
                        (data['business_id'],i) )
                except Exception as e:
                    print("Insert to checkins failed!",e)
                conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2FriendTable():
      #reading the JSON file
    with open('.//yelp_user.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='YELP' user='postgres' host='localhost' password='tarlydyl'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()


        while line:

            data = json.loads(line)
            # Generate the INSERT statement for the current Friend
            for i in data["friends"]:
                try:
                    cur.execute("INSERT INTO Friend (user_id, friend_id)"
                        + "VALUES (%s,%s)",
                        (data['user_id'],i) )
                except Exception as e:
                    print("Insert to Friend failed!",e)
                    print (i)
                conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

def insert2HoursTable():
      #reading the JSON file
    with open('.//yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        try:
            conn = psycopg2.connect("dbname='YELP' user='postgres' host='localhost' password='tarlydyl'")
        except:
            print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            hours = data["hours"]
            for i in hours:
                #insert values into a string instead of in th eexecute function
                hours_sql_str = "INSERT INTO Hours(business_id, dayofweek, open, close) VALUES ('" + cleanStr4SQL(data['business_id']) + "','" + i + "','" + \
                hours[i].split("-")[0] + "','" + hours[i].split("-")[1] + "');" #Split the day from the open and close times

                try:
                    cur.execute(hours_sql_str)
                except:
                    print("Insert to hours failed!")
                    print(hours_sql_str)
                conn.commit()
            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(count_line)
    f.close()

#insert2BusinessTable() #-- DONE
#insert2Categories() #-- DONE
#insert2Attributes() #-- DONE
#insert2UserTable() #-- DONE
#insert2TipTable() #--DONE
#insert2CheckinTable() #- -DONE DO NOT UNCOMMENT UNLESS YOU WANT TO START A FIRE
#insert2FriendTable() #-- DONE
#insert2HoursTable() #-- DONE
