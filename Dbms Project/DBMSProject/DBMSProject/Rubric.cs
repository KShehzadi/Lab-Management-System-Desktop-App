﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBMSProject
{
    public partial class Rubric : Form
    {
        public int index;
        
        public Rubric()
        {
            InitializeComponent();
        }

        private void Rubric_Load(object sender, EventArgs e)
        {

            SqlConnection conn = Connection.buildconnection();
            String cmd = "SELECT * FROM [ProjectB].[dbo].[Rubric]";
            SqlCommand command = new SqlCommand(cmd, conn);
            // Add the parameters if required
            command.Parameters.Add(new SqlParameter("0", 1));
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
           
                string query = "select id, name from CLO";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "Clo");
                cb_clo.DisplayMember = "name";
                cb_clo.ValueMember = "id";
                cb_clo.DataSource = ds.Tables["Clo"];
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                dataGridView1[3, i].Value = Connection.getCloNamebyId(Convert.ToInt32(dataGridView1[2, i].Value));
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if(textBox1.Text == "")
            {
                panel2.Controls.Clear();
                panel1.Controls.Clear();
            }
            try
            {
                int txtno = int.Parse(textBox1.Text);
                int pointX = 30;
                int pointY = 40;
                panel2.Controls.Clear();
                panel1.Controls.Clear();
                for (int i = 0; i < txtno; i++)
                {
                    TextBox a = new TextBox();
                    a.Name = "Rubric"+ (i + 1).ToString();
                    a.Location = new Point(pointX, pointY);
                    
                    panel1.Controls.Add(a);
                    panel1.Show();
                   
                    pointY += 25;
                }
                pointX = 30;
                pointY = 40;
                for (int i = 0; i < txtno; i++)
                {
                    
                    TextBox b = new TextBox();
                    b.Name = "Id" + (i + 1).ToString();
                    b.Location = new Point(pointX, pointY);
                    panel2.Controls.Add(b);
                    panel2.Show();
                    pointY += 25;
                }
            }
            catch (Exception)
            {
                if(textBox1.Text!="")MessageBox.Show("Invalid Entry in Rubric Count");
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
       
        private void btn_rubric_Click(object sender, EventArgs e)
        {

            SqlConnection conn = Connection.buildconnection();
            try
            {
                int number;
                int j = 0;
                int[] idarray = new int[50];
                if (btn_rubric.Text == "Add Rubric")
                {
                    foreach (Control ctr in panel2.Controls)
                    {
                        
                        bool result = Int32.TryParse(((TextBox)ctr).Text, out number);
                        if (!result)
                        {
                            MessageBox.Show("Warnning: "+ ((TextBox)ctr).Text + " Rubric id not allowed!");
                            return;
                        }
                        if (((TextBox)ctr).Text=="")
                        {
                            return;

                        }
                        idarray[j] = Convert.ToInt32(((TextBox)ctr).Text);
                        j++;
                    }
                    j = 0;
                    foreach (Control ctr in panel1.Controls)
                    {
                        if (ctr is TextBox)
                        {
                            string value = ((TextBox)ctr).Text;
                            if(value=="")
                            {
                                MessageBox.Show("Warning: Empty fields in form!");
                                return;
                            }
                            String query = "INSERT INTO dbo.Rubric (id,Details,CloId) VALUES (@i,@name,@id)";
                            DataRowView drv = cb_clo.SelectedItem as DataRowView;
                            int i = Convert.ToInt32(drv.Row["id"]);
                            using (SqlCommand command = new SqlCommand(query, conn))
                            {

                                command.Parameters.AddWithValue("@name", value);
                                command.Parameters.AddWithValue("@id", i);
                                command.Parameters.AddWithValue("@i", idarray[j]);

                                int result = command.ExecuteNonQuery();

                                // Check Error
                                if (result < 0) Console.WriteLine("Error inserting data into Database!");
                                else j++;

                            }

                        }
                    }

                }
                else if (btn_rubric.Text == "Update")
                {
                    int p = 0;
                    foreach (Control ctr in panel2.Controls)
                    {
                        if (ctr is TextBox)
                        {
                            foreach (char c in ((TextBox)ctr).Text)
                            {
                                if (c < 48 || c > 57)
                                {
                                    MessageBox.Show("Warning: RubricId only have Digits!");
                                    return;
                                }
                            }
                            p = Convert.ToInt32(((TextBox)ctr).Text);
                        }
                    }
                    foreach (Control ctr in panel1.Controls)
                    {
                        if (ctr is TextBox)
                        {
                            string query;
                            string value = ((TextBox)ctr).Text;
                            try
                            {
                                query = "INSERT INTO dbo.Rubric (id,Details,CloId) VALUES (@i,@name,@id)";
                                DataRowView drv = cb_clo.SelectedItem as DataRowView;
                                int newcloid = Convert.ToInt32(drv.Row["id"]);
                                using (SqlCommand command = new SqlCommand(query, conn))
                                {

                                    command.Parameters.AddWithValue("@name", value);
                                    command.Parameters.AddWithValue("@id", newcloid);
                                    command.Parameters.AddWithValue("@i", p);

                                    int result = command.ExecuteNonQuery();

                                    // Check Error
                                    if (result < 0) Console.WriteLine("Error inserting data into Database!");


                                }
                                query = "Update dbo.AssessmentComponent SET RubricId=@newid where RubricId = @index";

                                using (SqlCommand command = new SqlCommand(query, conn))
                                {
                                    command.Parameters.AddWithValue("@newid", p);
                                    command.Parameters.AddWithValue("@index", index);

                                    int result = command.ExecuteNonQuery();

                                    // Check Error
                                    if (result < 0) Console.WriteLine("Error Updating data into Database!");

                                }
                                query = "Update dbo.RubricLevel SET RubricId=@newid where RubricId = @index";

                                using (SqlCommand command = new SqlCommand(query, conn))
                                {
                                    command.Parameters.AddWithValue("@newid", p);
                                    command.Parameters.AddWithValue("@index", index);

                                    int result = command.ExecuteNonQuery();

                                    // Check Error
                                    if (result < 0) Console.WriteLine("Error Updating data into Database!");

                                }

                                query = "Delete from dbo.Rubric where id = @index";
                                using (SqlCommand command = new SqlCommand(query, conn))
                                {

                                    command.Parameters.AddWithValue("@index", index);

                                    int result = command.ExecuteNonQuery();

                                    // Check Error
                                    if (result < 0) Console.WriteLine("Error Updating data into Database!");

                                }
                            }
                            catch
                            {
                                query = "Update dbo.Rubric Set Details = @name,CloId = @id Where id = @index";
                                DataRowView drv = cb_clo.SelectedItem as DataRowView;
                                int newcloid = Convert.ToInt32(drv.Row["id"]);
                                using (SqlCommand command = new SqlCommand(query, conn))
                                {

                                    command.Parameters.AddWithValue("@name", value);
                                    command.Parameters.AddWithValue("@id", newcloid);
                                    command.Parameters.AddWithValue("@index", index);


                                    int result = command.ExecuteNonQuery();

                                    // Check Error
                                    if (result < 0) Console.WriteLine("Error inserting data into Database!");
                                }

                            }
                           

                           

                        }
                    }
                    
                }
            }
            catch
            {
                MessageBox.Show("Same Rubric Key Insertion Not Allowed!");
               
            }
            textBox1.Show();
            textBox1.Clear();
            
            String cmd = "SELECT * FROM [ProjectB].[dbo].[Rubric]";
            SqlCommand commandf = new SqlCommand(cmd, conn);
            // Add the parameters if required
            commandf.Parameters.Add(new SqlParameter("0", 1));
            SqlDataReader reader = commandf.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            btn_rubric.Text = "Add Rubric";
            panel2.Controls.Clear();
            panel1.Controls.Clear();

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                dataGridView1[3, i].Value = Connection.getCloNamebyId(Convert.ToInt32(dataGridView1[2, i].Value));
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            SqlConnection conn = Connection.buildconnection();
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0 && senderGrid.Columns[e.ColumnIndex].HeaderText == "Update")
            {
                textBox1.Clear();
                textBox1.Hide();
                index = Convert.ToInt32(dataGridView1[0, e.RowIndex].Value);


                int pointX = 30;
                int pointY = 40;


                TextBox a = new TextBox();
                a.Name = "Rubric" + index.ToString();
                a.Location = new Point(pointX, pointY);
               
                a.Text = Convert.ToString(dataGridView1[1, e.RowIndex].Value);
                panel1.Controls.Add(a);
                panel1.Show();
                pointY += 25;


                pointX = 30;
                pointY = 40;
                TextBox b = new TextBox();
                b.Name = "Id" + index.ToString();
                b.Location = new Point(pointX, pointY);
                
                b.Text = Convert.ToString(dataGridView1[0, e.RowIndex].Value);
                panel2.Controls.Add(b);
                panel2.Show();
                pointY += 25;

                cb_clo.SelectedValue = Convert.ToInt32(dataGridView1[2, e.RowIndex].Value);
                btn_rubric.Text = "Update";
            }
            else if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
               e.RowIndex >= 0 && senderGrid.Columns[e.ColumnIndex].HeaderText == "Delete")
            {

                int i = Convert.ToInt32(dataGridView1[0, e.RowIndex].Value);
                if(Connection.Delete_Rubricbyid(i))
                {
                    MessageBox.Show("Deleted Successfully!");
                }
                else
                {
                    MessageBox.Show("Error Occured!");
                }
                
            }

            String cmd = "SELECT * FROM [ProjectB].[dbo].[Rubric]";
            SqlCommand commandf = new SqlCommand(cmd, conn);
            // Add the parameters if required
            commandf.Parameters.Add(new SqlParameter("0", 1));
            SqlDataReader reader = commandf.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                dataGridView1[3, i].Value = Connection.getCloNamebyId(Convert.ToInt32(dataGridView1[2, i].Value));
            }

        }
    }
}
