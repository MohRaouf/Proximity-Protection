using Proximity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;

namespace PlayerUI
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void startServer()
        {
            listener = new TcpServer(1234);
        }
        private static TcpServer listener;
        List<string> serverData = new List<string>();
        double txPower = -65;

        public static List<string[]> clientsLines = new List<string[]>();
        public static List<string> unknownBeacons = new List<string>();

        private async void button4_ClickAsync(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button4.Text = "Busy Listening";
            label4.Text = "Listenning";
            label4.ForeColor = Color.Green;
            Thread t = new Thread(new ThreadStart(startServer));
            t.IsBackground = true;
            t.Start();

            string connetionString;
            SqlConnection cnn;
            connetionString = @"data source=192.168.0.152;initial catalog=TGS;persist security info=True;user id=publish;password=AppPub123;MultipleActiveResultSets=True;App=EntityFramework";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            MessageBox.Show("Connection Open  !");
            SqlCommand command;
            SqlDataAdapter adapter = new SqlDataAdapter();
            String sql = "";
            sql = "value: 4c:11:ae:70:f5:8e,-87,-88,-88,-88,-87,-87,-88,-87,-87,-87,-89,-86,-88,-87,-88,R2";
            command = new SqlCommand(sql, cnn);
            adapter.InsertCommand = new SqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();

            for (int i = 0; i < 50; i++) adapter.InsertCommand.ExecuteNonQuery();
            //cnn.Close();

            await Task.Run(() =>
            {
                while (true)
                {
                    foreach (string clientLine in TcpServer.recievedData)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            textBox2.AppendText(clientLine);
                            textBox2.AppendText(Environment.NewLine);
                        });
                    }

                    TcpServer.recievedData.Clear();
                    Thread.Sleep(3000);
                    
                }
            });
        }

        public void FormatData(string packet, DatabaseDataSet db)
        {
            string[] newContent = packet.Split(',');
            string reader_mac;
            string readings;
            if (newContent.LastOrDefault().Length > 2) // to check that sensors return
            {
                reader_mac = newContent[newContent.Length - 2];// to get the reader mac (R1)
                readings = string.Join(",", newContent, 1, newContent.Length - 3);
                string sensors = newContent[newContent.Length - 1];
                readings += "," + sensors;
            }
            else if (newContent.LastOrDefault().Length < 2)
            {
                reader_mac = newContent[newContent.Length - 2];
                readings = string.Join(",", newContent, 1, newContent.Length - 3);
            }
            else
            {
                reader_mac = newContent[newContent.Length - 1];
                readings = string.Join(",", newContent, 1, newContent.Length - 2);
            }
            string readingbefore = readings;
            List<string> RSSI_string = readingbefore.Split(',').ToList();
            //  Sensors go here.....
            //  SensorsFromBeacon = RSSI_string[RSSI_string.Count-1];
            //  RSSI_string.RemoveAt(RSSI_string.Count - 1);
            EntityCommand db = new EntityCommand();
            if (RSSI_string.Count() != 0)
            {
                string Beacon_mac = newContent[0];
                var beacon_found = db.pcptgs006.FirstOrDefault(b => b.pcptgs006002 == Beacon_mac);

                if (beacon_found != null)
                {
                    using (pcptgs014 pcptgs014 = new pcptgs014())
                    {
                        TimeSpan TodayTime = DateTime.Now.TimeOfDay;
                        DateTime today = DateTime.Now;
                        pcptgs014.pcptgs014001 = today;
                        pcptgs014.pcptgs014002 = TodayTime;
                        pcptgs014.pcptgs014003 = reader_mac;
                        pcptgs014.pcptgs014004 = Beacon_mac;
                        pcptgs014.pcptgs014005 = readings;
                        db.pcptgs014.Add(pcptgs014);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'databaseDataSet.Table' table. You can move, or remove it, as needed.
            this.tableTableAdapter.Fill(this.databaseDataSet.Table);
            // TODO: This line of code loads data into the 'databaseDataSet.Table' table. You can move, or remove it, as needed.

        }

        private void tableBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.tableBindingSource1.EndEdit();

        }

        private void tableBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
