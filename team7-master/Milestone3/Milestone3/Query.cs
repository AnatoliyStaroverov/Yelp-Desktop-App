using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Team7GUI
{
    internal static class Query
    {
        private static string BuildConnectionString()
        {
            return "Host=localhost; Username=postgres; Password=team7; Database=milestone2db";
        }

        /// <summary>
        /// If all attributes for a 'select * from table_name' are wanted with no 'where' clauses
        /// </summary>
        /// <param name="type">string of what class</param>
        /// <returns>List of classtype</returns>
        public static List<Information> BaseQuery(string table)
        {
            return BaseQuery(table, new Dictionary<string, string>());
        }

        /// <summary>
        /// Generic SQL statement builder, takes in what table for a 'select * from table_name' and stores all the results into a List of that class type
        /// </summary>
        /// <param name="table">String of what table</param>
        /// <param name="filters">Dictionary for how to filter results, such as item1=businessid, item2='id_string'</param>
        /// <returns>List of class instances</returns>
        public static List<Information> BaseQuery(string table, Dictionary<string, string> filters)
        {
            if (ClassTypeExist(table) != null)
            {
                table = table.ToLower();

                string query = "select * from " + table;
                if (filters.Count > 0)
                {
                    query += " where ";
                    foreach (string key in filters.Keys)
                    {
                        query += key + " = '" + filters[key] + "'";
                        if (!filters[key].Equals(filters.Last().Value))
                        {
                            query += " and ";
                        }
                    }
                }

                List<object[]> queryResults = RunQuery(query);
                List<Information> queryList = new List<Information>();
                foreach (object[] element in queryResults)
                {
                    if (table.Equals("users"))
                    {
                        queryList.Add(new Users()
                        {
                            Userid = element[0].ToString(),
                            Name = element[1].ToString(),
                            Yelpingsince = element[2].ToString(),
                            Reviewcount = (int)element[3],
                            Fans = (int)element[4],
                            Averagestars = (double)element[5],
                            Funny = (int)element[6],
                            Useful = (int)element[7],
                            Cool = (int)element[8],
                            Latitude = (double)element[9],
                            Longitude = (double)element[10]
                        });
                    }
                    else if (table.Equals("business"))
                    {
                        queryList.Add(new Business()
                        {
                            Businessid = element[0].ToString(),
                            Name = element[1].ToString(),
                            Address = element[2].ToString(),
                            City = element[3].ToString(),
                            State = element[4].ToString(),
                            Zipcode = (int)element[5],
                            Latitude = (double)element[6],
                            Longitude = (double)element[7],
                            Stars = (double)element[8],
                            Reviewcount = (int)element[9],
                            Isopen = (bool)element[10],
                            Numcheckins = (int)element[11],
                            Reviewrating = Math.Round((double)element[12], 2)
                        });
                    }
                    else if (table.Equals("review"))
                    {
                        queryList.Add(new Review()
                        {
                            Reviewid = element[0].ToString(),
                            Userid = element[1].ToString(),
                            Businessid = element[2].ToString(),
                            Reviewtext = element[3].ToString(),
                            Stars = (double)element[4],
                            Reviewdate = ((DateTime)element[5]).ToString("yyyy-MM-dd"),
                            Funny = (int)element[6],
                            Useful = (int)element[7],
                            Cool = (int)element[8]
                        });
                    }
                }
                return queryList;
            }
            else
            {
                throw new TypeLoadException("This class is not yet implemented");
            }
        }

        /// <summary>
        /// Gets a list of categories that do not exist anymore from a list of filtered current businesses.
        /// </summary>
        /// <param name="state">Selected state</param>
        /// <param name="city">Selected city</param>
        /// <param name="zipcode">Selected zipcode</param>
        /// <param name="filterList">List of categories selected</param>
        /// <param name="e">If a category filter was added or removed</param>
        /// <returns></returns>
        public static List<string> UpdateCategoriesList(string state, string city, string zipcode, List<string> filterList, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Append filters to query
            string filters = string.Empty;
            foreach (string filter in filterList)
            {
                filters += "and exists (select * from businesscategories where b1.businessid = businesscategories.businessid and categoryname = '" + filter.Replace("'", "''") + "')";
            }

            string query = string.Empty;

            if (e.AddedItems.Count > 0)
            {
                query = "select temp2.categoryname from " +
                "((SELECT categoryname FROM businesscategories inner join business on businesscategories.businessid = business.businessid " +
                "WHERE state = '" + state + "' AND city = '" + city + "' " +
                "AND zipcode = '" + zipcode + "') except " +
                "(SELECT categoryname FROM businesscategories as b1 inner join business on b1.businessid = business.businessid " +
                "WHERE state = '" + state + "' AND city = '" + city + "' " +
                "AND zipcode = '" + zipcode + "' " + filters + ")) as temp2";
            }
            else if (e.RemovedItems.Count > 0)
            {
                query = "SELECT categoryname, Count(categoryname) as total FROM businesscategories as b1 inner join business on b1.businessid = business.businessid " +
                "WHERE state = '" + state + "' AND city = '" + city + "' " +
                "AND zipcode = '" + zipcode + "' " + filters + " group by categoryname order by total";
            }

            List<object[]> queryResults = RunQuery(query);
            List<string> queryList = new List<string>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(element[0].ToString());
            }

            return queryList;
        }

        /// <summary>
        /// List of businesses that have the selected categories
        /// </summary>
        /// <param name="state">Selected State</param>
        /// <param name="city">Selected City</param>
        /// <param name="zipcode">Selected Zipcode</param>
        /// <param name="filterList">List of categories selected</param>
        /// <returns></returns>
        public static List<Information> UpdateBusinessGrid(string state, string city, string zipcode, List<string> filterList)
        {
            // Append filters to query
            string filters = string.Empty;
            foreach (string filter in filterList)
            {
                // Replace is there to create an escape for SQL query for apostrophe
                filters += "and exists (select * from businesscategories where b1.businessid = businesscategories.businessid and categoryname = '" + filter.Replace("'", "''") + "')";
            }

            string query = "select distinct name,address,city,state,zipcode,reviewcount,reviewrating,numcheckins,stars,businessid from " +
                            "(SELECT name,address,city,state,zipcode,reviewcount,reviewrating,numcheckins,stars,business.businessid,categoryname " +
                            "FROM businesscategories as b1 inner join business on b1.businessid = business.businessid " +
                            "WHERE state = '" + state + "' " +
                            "AND city = '" + city + "' " +
                            "AND zipcode = '" + zipcode + "' " +
                            filters + ") as temp" +
                            " ORDER BY name";

            List<object[]> queryResults = RunQuery(query);
            List<Information> queryList = new List<Information>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(new Business()
                {
                    Name = element[0].ToString(),
                    Address = element[1].ToString(),
                    City = element[2].ToString(),
                    State = element[3].ToString(),
                    Zipcode = (int)element[4],
                    Reviewcount = (int)element[5],
                    Reviewrating = Math.Round((double)element[6], 2),
                    Numcheckins = (int)element[7],
                    Stars = Math.Round((double)element[8], 1),
                    Businessid = element[9].ToString()
                });
            }

            return queryList;
        }

        /// <summary>
        /// Gets the reviews for a selected business that were left by friends of the user
        /// </summary>
        /// <param name="business">Selected business</param>
        /// <param name="users">Current user</param>
        /// <returns></returns>
        public static List<Information> BusinessFriendsReviews(Business business, Users users)
        {
            string query = "select reviewid, review.userid, businessid, reviewtext, stars, reviewdate, review.funny, review.useful, review.cool, name from review inner join users on review.userid = users.userid, " +
                "(select isfriend from friends where (friends.userid = '" + users.Userid + "')) as f1 " +
                "where review.businessid = '" + business.Businessid + "' and review.userid = f1.isfriend " +
                "order by reviewdate";

            List<object[]> queryResults = RunQuery(query);
            List<Information> queryList = new List<Information>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(new Review()
                {
                    Reviewid = element[0].ToString(),
                    Userid = element[1].ToString(),
                    Businessid = element[2].ToString(),
                    Reviewtext = element[3].ToString(),
                    Stars = (double)element[4],
                    Reviewdate = ((DateTime)element[5]).ToString("yyyy-MM-dd"),
                    Funny = (int)element[6],
                    Useful = (int)element[7],
                    Cool = (int)element[8],
                    Username = element[9].ToString()
                });
            }

            return queryList;
        }

        /// <summary>
        /// Gets the Attributes, Categories, and Hours for the selected business
        /// </summary>
        /// <param name="business">Selected business</param>
        /// <returns></returns>
        public static Dictionary<string, string> BusinessInformation(Business business)
        {
            Dictionary<string, string> queryDictionary = new Dictionary<string, string>();
            StringBuilder sb = new StringBuilder();

            string query = "select attributename, value from businessattributes where businessid = '" + business.Businessid + "' and value <> 'False' order by attributename";
            List<object[]> queryResults = RunQuery(query);

            // Attributes
            foreach (object[] element in queryResults)
            {
                if (element[1].ToString() == "True")
                {
                    sb.AppendLine(element[0].ToString() + ", ");
                }
                else
                {
                    sb.AppendLine(element[0].ToString() + "(" + element[1].ToString() + "), ");
                }
            }

            // Remove the final appendline and comma
            if (sb.Length > 4)
            {
                sb.Remove(sb.Length - 4, 4);
            }

            queryDictionary.Add("attributes", sb.ToString());
            query = "select categoryname from businesscategories where businessid = '" + business.Businessid + "' order by categoryname";
            queryResults.Clear();
            sb.Clear();
            queryResults = RunQuery(query);

            // Categories
            foreach (object[] element in queryResults)
            {
                sb.AppendLine(element[0].ToString());
            }
            queryDictionary.Add("categories", sb.ToString());
            query = "select today, opentime, closetime from (select to_char(current_timestamp, 'FMDay'::text) as today, business.businessid from business) as tempquery left outer join businesshours on tempquery.businessid = businesshours.businessid and tempquery.today = businesshours.day where tempquery.businessid = '" + business.Businessid + "'";
            queryResults.Clear();
            sb.Clear();
            queryResults = RunQuery(query);

            // Hours
            foreach (object[] element in queryResults)
            {
                sb.AppendLine("Today (" + element[0].ToString() + ")");
                if (string.IsNullOrEmpty(element[1].ToString()))
                {
                    sb.AppendLine("  Opens: N/A");
                }
                else
                {
                    sb.AppendLine("  Opens: " + new DateTime().Add((TimeSpan)element[1]).ToString("hh:mm tt"));
                }
                if (string.IsNullOrEmpty(element[2].ToString()))
                {
                    sb.AppendLine("  Closes: N/A");
                }
                else
                {
                    sb.AppendLine("  Closes: " + new DateTime().Add((TimeSpan)element[2]).ToString("hh:mm tt"));
                }
            }
            queryDictionary.Add("hours", sb.ToString());

            return queryDictionary;
        }

        /// <summary>
        /// Gets the business categories that exist within the businesses at the said state, city, and zipcode
        /// </summary>
        /// <param name="state">Selected State</param>
        /// <param name="city">Selected City</param>
        /// <param name="zipcode">Selected Zipcode</param>
        /// <returns></returns>
        public static List<string> BusinessCategories(string state, string city, string zipcode)
        {
            string query = "SELECT categoryname, Count(categoryname) as total FROM businesscategories inner join business on businesscategories.businessid = business.businessid WHERE state = '" + state + "' AND city = '" + city + "' AND zipcode = '" + zipcode + "' group by categoryname order by total desc;";
            List<object[]> queryResults = RunQuery(query);
            List<string> queryList = new List<string>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(element[0].ToString());
            }

            return queryList;
        }

        /// <summary>
        /// Gets businesses with the city, state, and zipcode filter
        /// </summary>
        /// <param name="state">Selected state</param>
        /// <param name="city">Selectd City</param>
        /// <param name="zipcode">Selected Zipcode</param>
        /// <returns>List of business objects</returns>
        public static List<Information> BusinessList(string state, string city, string zipcode)
        {
            string query = "SELECT * FROM business WHERE state = '" + state + "' AND city = '" + city + "' AND zipcode = '" + zipcode + "' ORDER BY name;";
            List<object[]> queryResults = RunQuery(query);
            List<Information> queryList = new List<Information>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(new Business()
                {
                    Businessid = element[0].ToString(),
                    Name = element[1].ToString(),
                    Address = element[2].ToString(),
                    City = element[3].ToString(),
                    State = element[4].ToString(),
                    Zipcode = (int)element[5],
                    Latitude = (double)element[6],
                    Longitude = (double)element[7],
                    Stars = (double)element[8],
                    Reviewcount = (int)element[9],
                    Isopen = (bool)element[10],
                    Numcheckins = (int)element[11],
                    Reviewrating = Math.Round((double)element[12], 2)
                });
            }

            return queryList;
        }

        /// <summary>
        /// Fill zipcode ListBox
        /// </summary>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <returns>List of string of zipcodes</returns>
        public static List<string> BusinessZipCodes(string state, string city)
        {
            string query = "SELECT DISTINCT zipcode FROM business WHERE state = '" + state + "' AND city = '" + city + "' ORDER BY zipcode;";
            List<object[]> queryResults = RunQuery(query);
            List<string> queryList = new List<string>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(element[0].ToString());
            }

            return queryList;
        }

        /// <summary>
        /// Fill cities ListBox
        /// </summary>
        /// <param name="state">State to retrieve cities from</param>
        /// <returns>List of strings of cities</returns>
        public static List<string> BusinessCities(string state)
        {
            string query = "SELECT DISTINCT city FROM business WHERE state = '" + state + "' ORDER BY city;";
            List<object[]> queryResults = RunQuery(query);
            List<string> queryList = new List<string>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(element[0].ToString());
            }

            return queryList;
        }

        /// <summary>
        /// Fills the State dropdown box
        /// </summary>
        /// <returns>List of strings to bind data</returns>
        public static List<string> BusinessStates()
        {
            string query = "SELECT DISTINCT state FROM business ORDER BY state";
            List<object[]> queryResults = RunQuery(query);
            List<string> queryList = new List<string>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(element[0].ToString());
            }

            return queryList;
        }

        /// <summary>
        /// Parses the array of query results and stores them into a list of Users instances
        /// </summary>
        /// <param name="searchName"></param>
        /// <returns>List of Users</returns>
        public static List<Information> SearchUsers(string searchName)
        {
            // Repair input to remove all whitespaces and make it all lowercase for searching purposes
            searchName = searchName.Replace(" ", string.Empty).ToLower();

            string query = "select * from users where lower(name) like '%" + searchName + "%'";

            List<object[]> queryResults = RunQuery(query);
            List<Information> queryList = new List<Information>();

            Information myObject = (Information)Activator.CreateInstance(ClassTypeExist("users"));

            foreach (object[] element in queryResults)
            {
                queryList.Add(new Users()
                {
                    Userid = element[0].ToString(),
                    Name = element[1].ToString(),
                    Yelpingsince = ((DateTime)element[2]).ToString("yyyy-MM-dd"),
                    Reviewcount = (int)element[3],
                    Fans = (int)element[4],
                    Averagestars = (double)element[5],
                    Funny = (int)element[6],
                    Useful = (int)element[7],
                    Cool = (int)element[8],
                    Latitude = (double)element[9],
                    Longitude = (double)element[10]
                });
            }

            return queryList;
        }

        /// <summary>
        /// Parses the array of query results and stores them into a list of Business instances
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns>List of Businesses</returns>
        public static List<Information> FavoriteBusiness(Users currentUser)
        {
            string query = "select name, address, city, state, zipcode, reviewrating, reviewcount, numcheckins, latitude, longitude from favorite inner join business on favorite.businessid = business.businessid where userid = '" + currentUser.Userid + "'";

            List<object[]> queryResults = RunQuery(query);
            List<Information> queryList = new List<Information>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(new Business()
                {
                    Name = element[0].ToString(),
                    Address = element[1].ToString() + "\n" + element[2].ToString() + ", " + element[3].ToString() + " " + (int)element[4],
                    Reviewrating = (double)element[5],
                    Reviewcount = (int)element[6],
                    Numcheckins = (int)element[7],
                    Latitude = (double)element[8],
                    Longitude = (double)element[9]
                });
            }

            return queryList;
        }

        /// <summary>
        /// Parses the array of query results and stores them into a list of Review instances
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns>List of Reviews</returns>
        public static List<Information> RecentReviews(Users currentUser)
        {
            string query = "select reviewdate, business.name, review.stars, review.reviewtext, review.funny, review.useful, review.cool from review inner join business on review.businessid = business.businessid where userid = '" + currentUser.Userid + "' order by reviewdate desc";

            List<object[]> queryResults = RunQuery(query);
            List<Information> queryList = new List<Information>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(new Review()
                {
                    Reviewdate = ((DateTime)element[0]).ToString("yyyy-MM-dd"),
                    Businessname = element[1].ToString(),
                    Stars = (double)element[2],
                    Reviewtext = element[3].ToString(),
                    Funny = (int)element[4],
                    Useful = (int)element[5],
                    Cool = (int)element[6]
                });
            }

            return queryList;
        }

        /// <summary>
        /// Parses the array of query results and stores them into a list of Users instances
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns>List of Users</returns>
        public static List<Information> FriendsInformation(Users currentUser)
        {
            string query = "select users.userid, name, yelpingsince, reviewcount, fans, averagestars, funny, useful, cool, latitude, longitude " +
                "from friends inner join users on friends.isfriend = users.userid where friends.userid = '" + currentUser.Userid + "' order by Name";

            List<object[]> queryResults = RunQuery(query);
            List<Information> queryList = new List<Information>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(new Users()
                {
                    Userid = element[0].ToString(),
                    Name = element[1].ToString(),
                    Yelpingsince = element[2].ToString(),
                    Reviewcount = (int)element[3],
                    Fans = (int)element[4],
                    Averagestars = (double)element[5],
                    Funny = (int)element[6],
                    Useful = (int)element[7],
                    Cool = (int)element[8],
                    Latitude = (double)element[9],
                    Longitude = (double)element[10]
                });
            }

            return queryList;
        }

        /// <summary>
        /// Parses the array of query results and stores them into a list of Review instances
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns>List of Reviews</returns>
        public static List<Information> FriendsReviews(Users currentUser)
        {
            // Friend Reviews
            string query = "select p3.uname, p3.bname, p3.stars, p3.reviewtext, p3.reviewdate " +
                "from (select review.userid, users.name as uname, business.name as bname, review.stars, review.reviewdate, review.reviewtext, " +
                "(row_number() over(partition by review.userid, users.name order by reviewdate desc)) as rn " +
                "from review inner join friends on review.userid = friends.isfriend " +
                "inner join business on business.businessid = review.businessid " +
                "inner join users on users.userid = friends.isfriend where friends.userid = '" + currentUser.Userid + "') as p3 " +
                "where rn=1 order by p3.reviewdate desc";

            List<object[]> queryResults = RunQuery(query);
            List<Information> queryList = new List<Information>();

            foreach (object[] element in queryResults)
            {
                queryList.Add(new Review()
                {
                    Username = element[0].ToString(),
                    Businessname = element[1].ToString(),
                    Stars = (double)element[2],
                    Reviewtext = element[3].ToString(),
                    Reviewdate = ((DateTime)element[4]).ToString("yyyy-MM-dd")
                });
            }

            return queryList;
        }

        /// <summary>
        /// Inserts a checkin for a business
        /// </summary>
        /// <param name="businessid">Business id to check into</param>
        public static void CheckIn(string businessid)
        {
            string query = "insert into businesscheckins (day, checkintime, businessid, count) " +
                "values (default, default, @businessid, default)" +
                "on conflict (day, checkintime, businessid) do update set count = businesscheckins.count + 1";
            using (var conn = new NpgsqlConnection(BuildConnectionString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("businessid", businessid);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Inserts a written review into the database
        /// </summary>
        /// <param name="review">Review written</param>
        public static void SubmitReview(Review review)
        {
            using (var conn = new NpgsqlConnection(BuildConnectionString()))
            {
                conn.Open();
                string reviewid = GenerateId();
                bool duplicate = true;

                // Check to see (however unlikely) if our reviewid generated already exists
                while (duplicate)
                {
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select reviewid from review where reviewid = '" + reviewid + "'";
                        using (var reader = cmd.ExecuteReader())
                        {
                            // If can read, then a tuple found, means reviewid already exists somehow
                            if (reader.Read())
                            {
                                reviewid = GenerateId();
                            }
                            else
                            {
                                duplicate = false;
                            }
                        }
                    }
                }

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO review (reviewid, userid, businessid, stars, reviewdate, reviewtext) " +
                    "VALUES (@reviewid, @userid, @businessid, @stars, @reviewdate, @reviewtext)"
                    + "ON CONFLICT (reviewid) DO NOTHING;";
                    cmd.Parameters.AddWithValue("reviewid", reviewid);
                    cmd.Parameters.AddWithValue("userid", review.Userid);
                    cmd.Parameters.AddWithValue("businessid", review.Businessid);
                    cmd.Parameters.AddWithValue("stars", review.Stars);
                    cmd.Parameters.AddWithValue("reviewdate", DateTime.Today);
                    cmd.Parameters.AddWithValue("reviewtext", review.Reviewtext);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Generate a random 22-digit ID that userid, reviewid, and businessid all seem to use
        /// </summary>
        /// <returns></returns>
        private static string GenerateId()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
            return new string(Enumerable.Repeat(chars, 22)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Runs a SQL query with the given string
        /// </summary>
        /// <param name="query"></param>
        /// <returns>List of arrays which are the rows from the query</returns>
        private static List<object[]> RunQuery(string query)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            List<object[]> objectList = new List<object[]>();

            using (var conn = new NpgsqlConnection(BuildConnectionString()))
            {
                conn.Open();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                ds.Reset();

                da.Fill(ds);
                dt = ds.Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    objectList.Add(row.ItemArray);
                }
                conn.Close();
            }
            return objectList;
        }

        /// <summary>
        /// Determines if the class exists
        /// </summary>
        /// <param name="thisType">class type</param>
        /// <returns>Type that exists</returns>
        private static Type ClassTypeExist(string thisType) => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(t => t.Name.Equals(thisType, StringComparison.OrdinalIgnoreCase));
    }
}