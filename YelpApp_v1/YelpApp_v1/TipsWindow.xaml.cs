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
using System.Windows.Shapes;
using Npgsql;

namespace YelpApp_v1
{
    /// <summary>
    /// Interaction logic for TipsWindow.xaml
    /// </summary>
    public partial class TipsWindow : Window
    {
        private Business _business;

        public TipsWindow(Business business)
        {
            InitializeComponent();
            _business = business;
            addColumnsToGrid();
            reloadTips();
        }

        private void reloadTips()
        {
            tipsGrid.Items.Clear();
            string sqlstr = "SELECT tipdate, name, likes, tiptext FROM tip NATURAL JOIN users "
                                + "WHERE business_id = '"
                                + _business.bid.ToString()
                                + "' ORDER BY tipdate DESC;";

            executeQuery(sqlstr, addGridRow);
        }

        private string buildConnectionString()
        {
            return "Host=localhost; Username = postgres; Database = YELP; password=tarlydyl";
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
            tipsGrid.Items.Add(new Tip()
            {
                tipdate = R.GetDateTime(0).ToString(),
                name = R.GetString(1),
                likes = R.GetInt32(2),
                tiptext = R.GetString(3)
            });
        }

        private void addColumnsToGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("tipdate");
            col1.Header = "Date";
            col1.Width = 200;
            tipsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("name");
            col2.Header = "User Name";
            col2.Width = 150;
            tipsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("likes");
            col3.Header = "Likes";
            col3.Width = 50;
            tipsGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("tiptext");
            col4.Header = "Text";
            col4.Width = 600;
            tipsGrid.Columns.Add(col4);
        }

        private void addTipButton_Click(object sender, RoutedEventArgs e)
        {
            if (addTipTextBox.Text != "")
            {
                string sqlstr = "INSERT INTO Tip " +
                    "VALUES('jRyO2V1pA4CdVVqCIOPc1Q', '" + // this is temporary, every tip is added by this user for milestone2
                    _business.bid +
                    "', '" +
                    DateTime.Now.ToString("u") +
                    "', '" +
                    addTipTextBox.Text +
                    "', '0');";

                executeNonQuery(sqlstr);
                reloadTips();
            }
        }
    }
}
