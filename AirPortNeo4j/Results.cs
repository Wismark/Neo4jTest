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
    public partial class Results : Form
    {
        bool flightSearch = false, PassSearch = false; bool close = true;
        GraphClient graphClient = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "12345");
        Form1 main;
        private BindingSource bindingSource1 = new BindingSource();
        public Results(bool a, bool b)
        {
            main = this.Owner as Form1;
            flightSearch = a;
            PassSearch = b;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            close = false;
            this.Close();
            main = this.Owner as Form1;
            main.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
            string wstr = "", sStr="";
            try
            {
                if (flightSearch)
                {
                    foreach (Flight a in bindingSource1)
                    {
                        wstr = "(f.flight_num=" + Convert.ToString(a.flight_num) + ")";
                        //  sStr = "f.flight_type={" + a.flight_type + "}, " + "f.aircraft_type={" + a.aircraft_type + "}, " + "f.destination={" + a.destination + "}";
                        graphClient.Cypher
                            .Match("(f:Flight)")
                            .Where(wstr)
                            .Set("f.flight_type= {flight_type}, f.aircraft_type= {aircraft_type}, f.destination={destination}")
                            .WithParams(a)
                            .ExecuteWithoutResults();
                    }
                } else
                {
                    foreach (Passenger a in bindingSource1)
                    {
                        wstr = "(p.name='" + a.name + "')";
                        //  sStr = "f.flight_type={" + a.flight_type + "}, " + "f.aircraft_type={" + a.aircraft_type + "}, " + "f.destination={" + a.destination + "}";
                        graphClient.Cypher
                            .Match("(p:Passenger)")
                            .Where(wstr)
                            .Set("p.seat_num= {seat_num}, p.class_type= {class_type}")
                            .WithParams(a)
                            .ExecuteWithoutResults();
                    }
                }
                MessageBox.Show("Data was successfully updated!", "DataBase", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Error when updating data! Check the correctness of the data!", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Results_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (close) Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (flightSearch)
                {
                    string wStr = "(f.flight_num=" + dataGridView1.SelectedRows[0].Cells[0].Value.ToString() + ")";

                    if (this.dataGridView1.SelectedRows.Count > 0)
                    {
                        graphClient.Connect();
                        graphClient.Cypher
                            .Match("(f:Flight)")
                            .Where(wStr)
                            .DetachDelete("(f)")
                            .ExecuteWithoutResults();
                        bindingSource1.RemoveAt(this.dataGridView1.SelectedRows[0].Index);
                        MessageBox.Show("The row was successfully removed!", "DataBase", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Please select one row");
                    }
                }
                else
                {
                    string wStr = "(f.name='" + dataGridView1.SelectedRows[0].Cells[0].Value.ToString() + "')";

                    if (this.dataGridView1.SelectedRows.Count > 0)
                    {
                        graphClient.Connect();
                        graphClient.Cypher
                            .Match("(f:Passenger)")
                            .Where(wStr)
                            .DetachDelete("(f)")
                            .ExecuteWithoutResults();
                        bindingSource1.RemoveAt(this.dataGridView1.SelectedRows[0].Index);
                        MessageBox.Show("The row was successfully removed!", "DataBase", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Please select one row!", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch { MessageBox.Show("Please select one row!", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Results_Load(object sender, EventArgs e)
        { 
            try
            {              
                graphClient.Connect();
            }
            catch
            {
                MessageBox.Show("Connection error!", "Error..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
            
            if (flightSearch == true)
            {
                string wStr = "("; bool something = false;
                main = this.Owner as Form1;
                if (main.GetDestination() != "")
                {
                    wStr += "n.destination='" + main.GetDestination() + "')";
                    something = true;
                }

                if (main.GetFlightNum().ToString() != "")
                {
                    if (something)
                        wStr += "AND (n.flight_num=" + main.GetFlightNum().ToString() + ")";
                    else
                    {
                        wStr += "n.flight_num=" + main.GetFlightNum().ToString() + ")";
                        something = true;
                    }
                }

                if (main.GetAirType() != "")
                {
                    if (something)
                        wStr += "AND (n.aircraft_type='" + main.GetAirType() + "')";
                    else
                    {
                        wStr += "n.aircraft_type='" + main.GetAirType() + "')";
                        something = true;
                    }
                }

                if (main.GetFLightType() != "")
                {
                    if (something)
                        wStr += "AND (n.flight_type='" + main.GetFLightType() + "')";
                    else
                    {
                        wStr += "n.flight_type='" + main.GetFLightType() + "')";
                        something = true;
                    }
                }

                if (main.GetFlightStatus() != "")
                {
                    string q = "";
                    if (main.GetFlightStatus() == "Аrrival") q = "Will_take"; else q = "Will_send";
                    if (something)
                        wStr += "AND ((t)-[:" + q + "]->(n))";
                    else
                    {
                        wStr += "(t)-[:" + q + "]->(n))";
                        something = true;
                    }
                }
                if (!something) wStr = "(True)";
                label15.Text = "Flight's list";
                var flights = graphClient.Cypher
                    .Match("(t:Terminal)")
                    .Match("(n:Flight)")
                    .Where(wStr)
                    .Return(n => n.As<Flight>())
                    .Results;
                
                foreach (Flight a in flights)
                {
                    bindingSource1.Add(a);
                }
                
                dataGridView1.AutoSize = true;
                dataGridView1.DataSource = bindingSource1;
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
            }
            else
            if( PassSearch == true)
            {
                string wStr = "(True)";
                var passengers = graphClient.Cypher
                    .Match("(n:Passenger)")
                    .Where(wStr)
                    .Return(n => n.As<Passenger>())
                    .Results;

                foreach (Passenger a in passengers)
                {
                    bindingSource1.Add(a);
                }

                dataGridView1.AutoSize = true;
                dataGridView1.DataSource = bindingSource1;
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
            }
        }
    }
}
