using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neo4jClient;
using Neo4jClient.Cypher;


namespace AirPortNeo4j
{
    public partial class Form1 : Form
    {
        bool flightSearch = false, PassSearch = false;
        GraphClient graphClient = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "12345");
        public Form1()
        {
            InitializeComponent();
        }
        public string GetDestination()
        {
            return textBox3.Text;
        }

        public string GetFlightNum()
        {
            return textBox1.Text;
        }

        public string GetAirType()
        {
            return textBox2.Text;
        }

        public string GetFLightType()
        {
            return comboBox1.Text;
        }
        public string GetFlightStatus()
        {
            return comboBox2.Text;
        }
        public string GetFLightTerminal()
        {
            return comboBox3.Text;
        }
        private void button3_Click(object sender, EventArgs e)
        {           
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // check connection
            try
            {
                graphClient.Connect();
            }
            catch
            {
                MessageBox.Show("Connection error!", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }

            // check correctness of data
            try
            { 
                        Flight someFlight = new Flight
                        {
                            destination = textBox3.Text,
                            flight_num = Convert.ToInt32(textBox1.Text),
                            aircraft_type = textBox2.Text,
                            flight_type = comboBox1.Text
                        };

              string willWhat = "", wSrt = String.Concat("(t.name='", comboBox3.Text, "')");
              if (comboBox2.Text == "Аrrival") willWhat = "Will_take";
              if (comboBox2.Text == "Dispatch") willWhat = "Will_send";
              string cStr = String.Concat("(t)-[:", willWhat, "]->(f)");

               graphClient.Cypher
                    .Match("(t:Terminal)")
                    .Where(wSrt)
                    .Create("(f:Flight{a})")
                    .WithParam("a", someFlight)
                    .Create(cStr)
                    .ExecuteWithoutResults();
                MessageBox.Show("The new flight has been successfully added!", "DataBase", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Please enter the correct data!", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Flight_list_update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                graphClient.Connect();
            }
            catch
            {
                MessageBox.Show("Connection error!", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                Passenger somePassenger = new Passenger
                {
                    name = textBox5.Text,
                    seat_num = Convert.ToInt32(textBox4.Text),
                    class_type = comboBox5.Text
                };

                string wSrt = String.Concat("(f.flight_num=", comboBox4.Text, ")");

                graphClient.Cypher
                     .Match("(f:Flight)")
                     .Where(wSrt)
                     .Create("(p:Passenger{a})")
                     .WithParam("a", somePassenger)
                     .Create("(p)-[:Fly_on]->(f)")
                     .ExecuteWithoutResults();
                MessageBox.Show("The new passenger has been successfully added!", "DataBase", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Please enter the correct data!", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Flight_list_update();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //comboBox4.Text = comboBox5.Text = textBox4.Text = textBox5.Text = "";   
            flightSearch = true;
            Results a = new Results(flightSearch, PassSearch);
            a.Owner = this;
            this.Hide();
            a.ShowDialog();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            flightSearch = PassSearch = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text = textBox3.Text   = textBox4.Text = textBox5.Text = "";
            comboBox1.SelectedItem = null; comboBox2.SelectedItem = null; comboBox3.SelectedItem = null;
            comboBox4.SelectedItem = null; comboBox5.SelectedItem = null;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //textBox1.Text = comboBox1.Text = comboBox2.Text = textBox2.Text = textBox3.Text = comboBox3.Text ="";
            PassSearch = true;
            Results a = new Results(flightSearch, PassSearch);
            a.Owner = this;
            this.Hide();
            a.ShowDialog();
        }

        private void Flight_list_update()
        {
            try
            {
                graphClient.Connect();
            }
            catch
            {
                MessageBox.Show("Connection error!", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var flights = graphClient.Cypher
                .Match("(n:Flight)")
                .Return(n => n.As<Flight>())
                .Results;
            foreach (Flight a in flights)
            {
                comboBox4.Items.Add(a.flight_num);
            }
        }
    }
}
