using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace YelpApp_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            updateNonQuery();
            addStates();
            addColumnsToGrid();
        }

        private string buildConnectionString()
        {
            return "Host=localhost; Username = postgres; Database = YELP; password=tarlydyl";
        }

        private void updateNonQuery()
        {   
            // business table - numtips
            string sqlstr = "UPDATE Business " +
                "SET numTips = T.tipCount " +
                "FROM (SELECT Tip.business_id, COUNT(Tip.business_id) AS tipCount " +
                "FROM Tip " +
                "GROUP BY (Tip.business_id) " +
                ") AS T " +
                "WHERE Business.business_id = T.business_id;";
            executeNonQuery(sqlstr);

            // users table - tipCount
            sqlstr = "UPDATE Users " +
                "SET tipCount = T.tipCount " +
                "FROM (SELECT Tip.user_id, COUNT(Tip.user_id) AS tipCount " +
                "FROM Tip " +
                "GROUP BY (Tip.user_id) " +
                ") as T " +
                "WHERE Users.user_id = T.user_id;";
            executeNonQuery(sqlstr);

            // business table - numcheckins
            sqlstr = "UPDATE Business " +
                "SET numCheckins = C.checkinCount " +
                "FROM (SELECT Checkins.business_id, COUNT(Checkins.business_id) as checkinCount " +
                "FROM Checkins " +
                "GROUP BY (Checkins.business_id) " +
                ") AS C " +
                "WHERE Business.business_id = C.business_id;";
            executeNonQuery(sqlstr);

            // user table - totallikes
            sqlstr = "UPDATE Users " +
                "SET totalLikes = T.likes " +
                "FROM (SELECT Tip.user_id, SUM(Tip.likes) AS likes " +
                "FROM Tip " +
                "GROUP BY (Tip.user_id) " +
                ") AS T " +
                "WHERE Users.user_id = T.user_id;";
            executeNonQuery(sqlstr);
        }

        private void addStates()
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT distinct state FROM business ORDER BY state";
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            stateComboBox.Items.Add(reader.GetString(0));
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        System.Windows.MessageBox.Show("SQL Error - " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void addCity(NpgsqlDataReader R)
        {
            cityListBox.Items.Add(R.GetString(0));
        }

        private void stateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityListBox.Items.Clear();
            zipcodeListBox.Items.Clear();
            businessCategoryListBox.Items.Clear();

            if (stateComboBox.SelectedIndex > -1)
            {
                string sqlstr = "SELECT DISTINCT city FROM business WHERE state = '"
                                + stateComboBox.SelectedItem.ToString()
                                + "' ORDER BY city;";

                executeQuery(sqlstr, addCity);
            }
        }

        private void addZipcode(NpgsqlDataReader R)
        {
            zipcodeListBox.Items.Add(R.GetString(0));
        }

        private void cityListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            zipcodeListBox.Items.Clear();
            businessCategoryListBox.Items.Clear();

            if (cityListBox.SelectedIndex > -1)
            {
                string sqlstr = "SELECT DISTINCT zipcode FROM business WHERE state = '"
                                + stateComboBox.SelectedItem.ToString()
                                + "' AND city = '"
                                + cityListBox.SelectedItem.ToString()
                                + "' ORDER BY zipcode;";

                executeQuery(sqlstr, addZipcode);
            }
        }

        private void addBusinessCategory(NpgsqlDataReader R)
        {
            businessCategoryListBox.Items.Add(R.GetString(0));
        }

        private void zipcodeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessCategoryListBox.Items.Clear();

            if (zipcodeListBox.SelectedIndex > -1)
            {
                string sqlstr = "SELECT DISTINCT category_name FROM business NATURAL JOIN categories WHERE state = '"
                                + stateComboBox.SelectedItem.ToString()
                                + "' AND city = '"
                                + cityListBox.SelectedItem.ToString()
                                + "' AND zipcode = '"
                                + zipcodeListBox.SelectedItem.ToString()
                                + "' ORDER BY category_name;";

                executeQuery(sqlstr, addBusinessCategory);
            }
        }

        private void executeQuery(string sqlstr, Action<NpgsqlDataReader> myf)
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sqlstr;
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            myf(reader);
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        MessageBox.Show("SQL Error - " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void executeNonQuery(string sqlstr)
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sqlstr;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        MessageBox.Show("SQL Error - " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void addGridRow(NpgsqlDataReader R)
        {
            businessGrid.Items.Add(new Business()
            {   
                name = R.GetString(0),
                address = R.GetString(1),
                city = R.GetString(2),
                state = R.GetString(3),
                distance = R.GetDouble(4),
                stars = R.GetDouble(5),
                tip_count = R.GetInt32(6),
                checkin_count = R.GetInt32(7),
                bid = R.GetString(8)
            });
        }

        private void addColumnsToGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("name");
            col1.Header = "Business Name";
            col1.Width = 100;
            businessGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("address");
            col2.Header = "Address";
            col2.Width = 100;
            businessGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("city");
            col3.Header = "City";
            col3.Width = 100;
            businessGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("state");
            col4.Header = "State";
            col4.Width = 50;
            businessGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("distance");
            col5.Header = "Distance (miles)";
            col5.Width = 100;
            businessGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Binding = new Binding("stars");
            col6.Header = "Stars";
            col6.Width = 50;
            businessGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Binding = new Binding("tip_count");
            col7.Header = "# of Tips";
            col7.Width = 100;
            businessGrid.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Binding = new Binding("checkin_count");
            col8.Header = "Total Checkins";
            col8.Width = 100;
            businessGrid.Columns.Add(col8);

            DataGridTextColumn col9 = new DataGridTextColumn();
            col9.Binding = new Binding("bid");
            col9.Header = "";
            col9.Width = 0;
            businessGrid.Columns.Add(col9);
        }

        private void searchBusinessesButton_Click(object sender, RoutedEventArgs e)
        {
            businessGrid.Items.Clear();
            if (zipcodeListBox.SelectedIndex > -1)
            {
                string sqlstr = "SELECT DISTINCT name, address, city, state, latitude, stars, numtips, numcheckins, business_id FROM business "
                                + "WHERE state = '"
                                + stateComboBox.SelectedItem.ToString()
                                + "' AND city = '"
                                + cityListBox.SelectedItem.ToString()
                                + "' AND zipcode = '"
                                + zipcodeListBox.SelectedItem.ToString()
                                + "'";


                for (int i = 0; i < selectedBusinessCategoryListBox.Items.Count; i++)
                {
                    sqlstr += " AND business_id IN (SELECT business_id FROM categories "
                            + "WHERE category_name = '"
                            + selectedBusinessCategoryListBox.Items[i].ToString()
                            + "')";
                }

                sqlstr += " ORDER BY name;";

                executeQuery(sqlstr, addGridRow);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (businessCategoryListBox.SelectedIndex > -1
                && !selectedBusinessCategoryListBox.Items.Contains(businessCategoryListBox.SelectedItem))
            {
                selectedBusinessCategoryListBox.Items.Add(businessCategoryListBox.SelectedItem);
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBusinessCategoryListBox.SelectedIndex > -1)
            {
                selectedBusinessCategoryListBox.Items.Remove(selectedBusinessCategoryListBox.SelectedItem);
            }
        }

        private void showTipsButton_Click(object sender, RoutedEventArgs e)
        {
            if (businessGrid.SelectedIndex > -1)
            {
                Business B = businessGrid.Items[businessGrid.SelectedIndex] as Business;
                TipsWindow tipsWindow = new TipsWindow(B);
                tipsWindow.Show();
            }
        }
    }
}
