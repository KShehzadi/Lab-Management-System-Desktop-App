﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System;
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
    public class Connection
    {


        public static void LoadStudentForm(ref DataGridView dataGridView1,ref ComboBox cb_status)
        {
            SqlConnection conn = Connection.buildconnection();
            String cmd = "SELECT * FROM [ProjectB].[dbo].[Student]";
            SqlCommand command = new SqlCommand(cmd, conn);
            // Add the parameters if required
            command.Parameters.Add(new SqlParameter("0", 1));
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;

            string query = "select Lookupid, Name from Lookup Where Category = 'STUDENT_STATUS'";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "lookup");
            cb_status.DisplayMember = "Name";
            cb_status.ValueMember = "Lookupid";
            cb_status.DataSource = ds.Tables["lookup"];

            for(int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                dataGridView1[7, i].Value = Connection.getLookupNamebyId(Convert.ToInt32(dataGridView1[6, i].Value));
            }
        }

        public static void Delete_Student(int i)
        {
            SqlConnection conn = Connection.buildconnection();
            if(Connection.DeleteStudentAttendancebyStudentid(i))
            {
                MessageBox.Show("All the attendance of this student deleted successfully!");
            }
            if (Connection.DeleteStudentResultbyStudentId(i))
            {
                MessageBox.Show("All the Results of this student deleted successfully!");
            }
            String query = "DELETE FROM [ProjectB].[dbo].[Student] Where id=@id";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@id", i);



                int result = command.ExecuteNonQuery();

                // Check Error
                if (result < 0) Console.WriteLine("Error Deleting data From Database!");


            }
            MessageBox.Show("Deleted Successfully!");
        }
        public static void InsertAndUpdate_Student(ref TextBox tb_FirstName, ref TextBox tb_LastName, ref TextBox tb_Contact, ref TextBox tb_RegNo, ref TextBox tb_Email, ref ComboBox cb_status, ref Button btn_insert, ref int index)
        {
            
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;
            // First Name and Last Name can be same for two Students but all other fields are unique
            String cmdcheck = "SELECT * FROM [ProjectB].[dbo].[Student] Where Contact = @contact OR Email = @email OR RegistrationNumber = @regno  ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@firstname", tb_FirstName.Text);
                command.Parameters.AddWithValue("@lastname", tb_LastName.Text);
                command.Parameters.AddWithValue("@contact", tb_Contact.Text);
                command.Parameters.AddWithValue("@email", tb_Email.Text);
                command.Parameters.AddWithValue("@regno", tb_RegNo.Text);


                reader = command.ExecuteReader();
            }
            if (reader.HasRows && btn_insert.Text != "Update")
            {
                MessageBox.Show("Student with same Date already exist !");
                return;
            }


            if (btn_insert.Text == "Insert Student")
            {
                if (tb_FirstName.Text == "" || tb_LastName.Text == "" || tb_Email.Text == "" || tb_Contact.Text == "" || tb_RegNo.Text == "" || cb_status.Text == "")
                {
                    MessageBox.Show("You can't insert a record with empty fields");
                    return;
                }

                try
                {
                    String query = "INSERT INTO dbo.Student (FirstName,LastName,Contact,Email,RegistrationNumber,Status) VALUES (@firstname,@lastname,@contact, @email, @regno, @status)";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@firstname", tb_FirstName.Text);
                        command.Parameters.AddWithValue("@lastname", tb_LastName.Text);
                        command.Parameters.AddWithValue("@contact", tb_Contact.Text);
                        command.Parameters.AddWithValue("@email", tb_Email.Text);
                        command.Parameters.AddWithValue("@regno", tb_RegNo.Text);

                        DataRowView drv = cb_status.SelectedItem as DataRowView;
                        int i = Convert.ToInt32(drv.Row["Lookupid"]);
                        command.Parameters.AddWithValue("@status", i);

                        int result = command.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                            Console.WriteLine("Error inserting data into Database!");
                    }
                }
                catch
                {
                    MessageBox.Show("Trying to Insert Invalid data in database!");
                    return;
                }
            }
            else if (btn_insert.Text == "Update")
            {
                try
                {
                    String query = "UPDATE dbo.Student SET FirstName = @firstname,LastName = @lastname,Contact = @contact,Email = @email,RegistrationNumber = @regno,Status = @status Where id = @id";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@firstname", tb_FirstName.Text);
                        command.Parameters.AddWithValue("@lastname", tb_LastName.Text);
                        command.Parameters.AddWithValue("@contact", tb_Contact.Text);
                        command.Parameters.AddWithValue("@email", tb_Email.Text);
                        command.Parameters.AddWithValue("@regno", tb_RegNo.Text);

                        DataRowView drv = cb_status.SelectedItem as DataRowView;
                        int i = Convert.ToInt32(drv.Row["Lookupid"]);
                        command.Parameters.AddWithValue("@status", i);
                        command.Parameters.AddWithValue("@id", index);

                        int result = command.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                            Console.WriteLine("Error Updating data into Database!");
                    }
                }
                catch
                {
                    MessageBox.Show("This Record can not be updated due to invalid data or dependencies  in database!");
                    return;

                }
            }
            tb_FirstName.Clear();
            tb_LastName.Clear();
            tb_RegNo.Clear();
            tb_Email.Clear();
            tb_Contact.Clear();
            btn_insert.Text = "Insert Student";
        }


        public static void LoadCLOForm(ref DataGridView dataGridView1)
        {
            SqlConnection conn = Connection.buildconnection();
            String cmd = "SELECT * FROM [ProjectB].[dbo].[CLO]";
            SqlCommand command = new SqlCommand(cmd, conn);
            // Add the parameters if required
            command.Parameters.Add(new SqlParameter("0", 1));
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
        }
        public static void DeleteCLO(int i)
        {
            SqlConnection conn = Connection.buildconnection();
            String query = "Select Id FROM [ProjectB].[dbo].[Rubric] Where CloId=@id";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@id", i);

                SqlDataReader readerrubric = command.ExecuteReader();
                int recordcount = 0;
                while (readerrubric.Read())
                {
                    recordcount++;
                }
                int[] rubricarray = new int[recordcount];
                readerrubric.Close();
                readerrubric = command.ExecuteReader();
                int count = 0;

                while (readerrubric.Read())
                {
                    int rubricid1 = readerrubric.GetInt32(0);
                    rubricarray[count] = rubricid1;
                    count++;
                }
                readerrubric.Close();
                foreach (int rubricid in rubricarray)
                {


                    query = "Select Id FROM [ProjectB].[dbo].[AssessmentComponent] Where RubricId=@id";
                    using (SqlCommand commandr = new SqlCommand(query, conn))
                    {
                        commandr.Parameters.AddWithValue("@id", rubricid);


                        SqlDataReader readerassessmentcomponent = command.ExecuteReader();
                        count = 0;
                        recordcount = 0;
                        while (readerassessmentcomponent.Read())
                        {
                            recordcount++;
                        }
                        int[] assessmentcomponentarray = new int[recordcount];
                        readerassessmentcomponent.Close();
                        readerassessmentcomponent = command.ExecuteReader();
                        while (readerassessmentcomponent.Read())
                        {
                            int rubricid1 = readerassessmentcomponent.GetInt32(0);
                            assessmentcomponentarray[count] = rubricid1;
                            count++;
                        }
                        readerassessmentcomponent.Close();
                        foreach (int rubriclevelid in assessmentcomponentarray)
                        {

                            query = "Delete FROM [ProjectB].[dbo].[StudentResult] Where AssessmentComponentId=@id";
                            using (SqlCommand commandl = new SqlCommand(query, conn))
                            {
                                commandl.Parameters.AddWithValue("@id", rubriclevelid);
                                int result = commandl.ExecuteNonQuery();
                            }
                        }
                        foreach (int asrubricid in rubricarray)
                        {

                            query = "Delete FROM [ProjectB].[dbo].[AssessmentComponent] Where RubricId=@id";
                            using (SqlCommand commandl = new SqlCommand(query, conn))
                            {
                                commandl.Parameters.AddWithValue("@id", asrubricid);
                                int result = commandl.ExecuteNonQuery();
                            }
                        }
                    }
                }
                foreach (int rubricid in rubricarray)
                {

                    query = "Select Id FROM [ProjectB].[dbo].[RubricLevel] Where RubricId=@id";
                    using (SqlCommand commandr = new SqlCommand(query, conn))
                    {
                        commandr.Parameters.AddWithValue("@id", rubricid);


                        SqlDataReader readerrubriclevel = command.ExecuteReader();
                        count = 0;
                        recordcount = 0;
                        while (readerrubriclevel.Read())
                        {
                            recordcount++;
                        }
                        int[] rubriclevelarray = new int[recordcount];
                        readerrubriclevel.Close();
                        readerrubriclevel = command.ExecuteReader();
                        while (readerrubriclevel.Read())
                        {
                            int rubricid1 = readerrubriclevel.GetInt32(0);
                            rubriclevelarray[count] = rubricid1;
                            count++;
                        }
                        readerrubriclevel.Close();
                        foreach (int rubriclevelid in rubriclevelarray)
                        {

                            query = "Delete FROM [ProjectB].[dbo].[StudentResult] Where RubricMeasurementId=@id";
                            using (SqlCommand commandl = new SqlCommand(query, conn))
                            {
                                commandl.Parameters.AddWithValue("@id", rubriclevelid);
                                int result = commandl.ExecuteNonQuery();
                            }
                        }
                        foreach (int rubriclevelid in rubricarray)
                        {

                            query = "Delete FROM [ProjectB].[dbo].[RubricLevel] Where RubricId=@id";
                            using (SqlCommand commandl = new SqlCommand(query, conn))
                            {
                                commandl.Parameters.AddWithValue("@id", rubriclevelid);
                                int result = commandl.ExecuteNonQuery();
                            }
                        }

                    }
            }


            query = "Delete  FROM [ProjectB].[dbo].[Rubric] Where CloId=@id";
            using (SqlCommand commandl = new SqlCommand(query, conn))
            {
                commandl.Parameters.AddWithValue("@id", i);
                int result = commandl.ExecuteNonQuery();
            }

            query = "DELETE FROM [ProjectB].[dbo].[CLO] Where id=@id";
            using (SqlCommand command3 = new SqlCommand(query, conn))
            {
                command3.Parameters.AddWithValue("@id", i);

                int result = command3.ExecuteNonQuery();

                // Check Error
                if (result < 0) Console.WriteLine("Error Deleting data From Database!");

            }
        }

    }

    public static void InsertAndUpdateCLO(ref TextBox tb_cloname,ref int index, ref DataGridView dataGridView1,ref Button btn_clo)
        {

            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;
            // First Name and Last Name can be same for two Students but all other fields are unique
            String cmdcheck = "SELECT * FROM [ProjectB].[dbo].[Clo] Where Name = @name ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@name", tb_cloname.Text);
             

                reader = command.ExecuteReader();
            }
            if (reader.HasRows && btn_clo.Text != "Update")
            {
                MessageBox.Show("same data exist can't update!");
                return;
            }
            if(reader.HasRows)
            {
                MessageBox.Show("Warnning: same CLO already exist!");
                return;
            }
            if (btn_clo.Text == "Add CLO")
            {
                
                string check = "Select * from dbo.CLO Where Name = @name";
                using(SqlCommand checkcmd = new SqlCommand(check, conn))
                {
                    checkcmd.Parameters.AddWithValue("@name", tb_cloname.Text);
                    reader = checkcmd.ExecuteReader();
                    if(reader.HasRows)
                    {
                        MessageBox.Show("CLO with same name already exist");
                        return;
                    }
                }
                String query = "INSERT INTO dbo.CLO (Name,DateCreated, DateUpdated) VALUES (@name,@datecreated,@dateupdated)";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@name", tb_cloname.Text);
                    command.Parameters.AddWithValue("@datecreated", DateTime.Now);
                    command.Parameters.AddWithValue("@dateupdated", DateTime.Now);


                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0) Console.WriteLine("Error inserting data into Database!");
                    else
                    {
                        tb_cloname.Clear();
                        Connection.LoadCLOForm(ref dataGridView1);
                    }
                }
            }
            else if (btn_clo.Text == "Update")
            {
               
                String query = "UPDATE CLO SET Name =@name,DateCreated=@dateupdated WHERE id = @id";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@name", tb_cloname.Text);

                    command.Parameters.AddWithValue("@dateupdated", DateTime.Now);
                    command.Parameters.AddWithValue("@id", index);

                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0)
                    {
                        Console.WriteLine("Error Updating data into Database!");
                    }
                    else
                    {
                        tb_cloname.Clear();
                        Connection.LoadCLOForm(ref dataGridView1);
                        btn_clo.Text = "Add CLO";
                    }
                }

            }
        }
      
        public static void LoadRubricLevelForm(ref DataGridView dataGridView1,ref ComboBox cb_rubric)
        {
            SqlConnection conn = Connection.buildconnection();
            string query = "select id,Details from [ProjectB].[dbo].[Rubric]";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "rubric");
            cb_rubric.DisplayMember = "Details";
            cb_rubric.ValueMember = "id";
            cb_rubric.DataSource = ds.Tables["rubric"];

            String cmd = "SELECT * FROM [ProjectB].[dbo].[RubricLevel]";
            SqlCommand command = new SqlCommand(cmd, conn);
            // Add the parameters if required
            command.Parameters.Add(new SqlParameter("0", 1));
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                dataGridView1[4, i].Value = Connection.getRubricNamebyId(Convert.ToInt32(dataGridView1[1, i].Value));
            }

        }
        public static void insertorupdaterubriclevel(ref TextBox tb_measurement, ref TextBox tb_details,ref int index, ref DataGridView dataGridView1, ref ComboBox cb_rubric, ref Button btn_Add)
        {

            if (tb_measurement.Text == "" || tb_details.Text == "" || cb_rubric.Text == "")
            {
                MessageBox.Show("You can't insert a record with empty fields");
                return;
            }
            foreach (char c in tb_measurement.Text)
            {
                if (c < 48 || c > 57)
                {
                    MessageBox.Show("Measurement Level can only have Digits!");
                    return;
                }
            }
            if(tb_measurement.Text != "1" && tb_measurement.Text != "2" && tb_measurement.Text != "3" && tb_measurement.Text != "4")
            {
                MessageBox.Show("You can only assign 1-4 Measurement Level to a Rubric Level");
                return;
            }
            int ml = -1;
            ml = Convert.ToInt32(tb_measurement.Text);
            if (ml == -1)
            {
                MessageBox.Show("Incorrect Measurement Level!");
            }
            int rubric_id;
            DataRowView drv = cb_rubric.SelectedItem as DataRowView;
            rubric_id = Convert.ToInt32(drv.Row["id"]);


            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;


            String cmdcheck = "SELECT * FROM [ProjectB].[dbo].[RubricLevel] Where MeasurementLevel = @i AND RubricId = @rid";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@i", ml);
                command.Parameters.AddWithValue("@rid", rubric_id);


                reader = command.ExecuteReader();
            }
            if (reader.HasRows)
            {
                MessageBox.Show("This Rubric Already have this measurement level record in database!");
                return;
            }
            if (btn_Add.Text == "Add Level")
            {


                try
                {
                    String query = "INSERT INTO dbo.RubricLevel (RubricId, Details,MeasurementLevel) VALUES (@rid,@d,@ml)";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@rid", rubric_id);
                        command.Parameters.AddWithValue("@d", tb_details.Text);
                        command.Parameters.AddWithValue("@ml", ml);




                        int result = command.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                            Console.WriteLine("Error inserting data into Database!");
                    }
                }
                catch
                {
                    MessageBox.Show("Trying to Insert Invalid data in database!");
                    return;
                }
            }
            else if (btn_Add.Text == "Update")
            {
                try
                {
                    String query = "UPDATE dbo.RubricLevel SET RubricId = @rid,Details = @d,MeasurementLevel = @m Where id = @id";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@rid", rubric_id);
                        command.Parameters.AddWithValue("@d", tb_details.Text);
                        command.Parameters.AddWithValue("@m", ml);

                        command.Parameters.AddWithValue("@id", index);

                        int result = command.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                            Console.WriteLine("Error Updating data into Database!");
                    }
                }
                catch
                {
                    MessageBox.Show("This Record can not be updated due to invalid data or dependencies  in database!");
                    return;

                }
            }
            String cmd = "SELECT * FROM [ProjectB].[dbo].[RubricLevel]";
            SqlCommand commandf = new SqlCommand(cmd, conn);
            // Add the parameters if required
            commandf.Parameters.Add(new SqlParameter("0", 1));
            reader = commandf.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            tb_details.Clear();
            tb_measurement.Clear();

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                dataGridView1[4, i].Value = Connection.getRubricNamebyId(Convert.ToInt32(dataGridView1[1, i].Value));
            }
            btn_Add.Text = "Add Level";
        }
    public static SqlConnection buildconnection()
        {
            String conURL = "Data Source = DESKTOP-NGEMSRA; Initial Catalog = ProjectB; Integrated Security = True; MultipleActiveResultSets = True";
            SqlConnection conn = new SqlConnection(conURL);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();

            }
            return conn;

        }

        public static DateTime getAttendanceDatebyId(int id)
        {
            SqlConnection conn =  Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT AttendanceDate FROM [ProjectB].[dbo].[ClassAttendance] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }
            // add reader's datetime in date object
            DateTime date = DateTime.Now;
            if (!reader.HasRows)
            {
                MessageBox.Show("CLO with this id does not exist");
                return DateTime.Now;
            }

            
            while (reader.Read())
            {
                date = reader.GetDateTime(0);
            }

            return date;
        }

        public static int getMeasurementLevelbyRubricLevelId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT MeasurementLevel FROM [ProjectB].[dbo].[RubricLevel] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }
            // add reader's datetime in date object
            int ml = -1;
            if (!reader.HasRows)
            {
                MessageBox.Show("RubricLevel with this id does not exist");
                return ml;
            }


            while (reader.Read())
            {
                ml = reader.GetInt32(0);
            }

            return ml;
        }

        public static int getRubricIdbyAssessmentComponentId(int asid)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT RubricId FROM [ProjectB].[dbo].[AssessmentComponent] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", asid);


                reader = command.ExecuteReader();
            }
            // add reader's datetime in date object
            int ml = -1;
            if (!reader.HasRows)
            {
                MessageBox.Show("Rubric with this id does not exist");
                return ml;
            }


            while (reader.Read())
            {
                ml = reader.GetInt32(0);
            }

            return ml;
        }
        public static int getMaximumRubricLevelbyRubricId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT Max(MeasurementLevel) FROM [ProjectB].[dbo].[RubricLevel] Where RubricId = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }
            // add reader's datetime in date object
            int ml = -1;
            if (!reader.HasRows)
            {
                MessageBox.Show("Rubric with this id does not exist");
                return ml;
            }

            if(reader.HasRows)
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        ml = reader.GetInt32(0);
                    }
                   
                }
            }
            

            return ml;
        }

        public static bool UpdateRubricMeasurementLevelIdFromStudentResult(int Studentid, int assessmentcomponentid, int rubricmeasurementid)
        {
            try
            {
                SqlConnection conn = Connection.buildconnection();

                String query = "UPDATE dbo.StudentResult SET RubricMeasurementId = @rmid WHERE StudentId = @sid AND AssessmentComponentId = @asid";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@sid", Studentid);
                    command.Parameters.AddWithValue("@asid", assessmentcomponentid);
                    command.Parameters.AddWithValue("@rmid", rubricmeasurementid);



                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0) return false;

                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteFromStudentResult(int Studentid, int assessmentcomponentid, int rubricmeasurementid)
        {
            try
            {
                SqlConnection conn = Connection.buildconnection();

                String query = "DELETE FROM dbo.StudentResult WHERE RubricMeasurementId = @rmid AND StudentId = @sid AND AssessmentComponentId = @asid";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@sid", Studentid);
                    command.Parameters.AddWithValue("@asid", assessmentcomponentid);
                    command.Parameters.AddWithValue("@rmid", rubricmeasurementid);



                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0) return false;

                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        public static int getIdbyRubricIdAndMeasurementLevelFromRubricLevel(int rubricid, int measurementlevel)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT id FROM [ProjectB].[dbo].[RubricLevel] Where RubricId = @id AND MeasurementLevel = @ml ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", rubricid);
                command.Parameters.AddWithValue("@ml", measurementlevel);


                reader = command.ExecuteReader();
            }
            // add reader's datetime in date object
            int id = -1;
            if (!reader.HasRows)
            {
                MessageBox.Show("RubricLevel with this Rubric id and Measurement Level does not exist");
                return id;
            }


            while (reader.Read())
            {
                id = reader.GetInt32(0);
            }

            return id;
        }


        public static int getAttendanceidbydatefromclassAttendance(DateTime id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT Id FROM [ProjectB].[dbo].[ClassAttendance] Where AttendanceDate = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }
            // add reader's datetime in date object
            int aid = -1;
            if (!reader.HasRows)
            {
                MessageBox.Show("CLO with this id does not exist");
                return aid;
            }


            while (reader.Read())
            {
                aid = reader.GetInt32(0);
            }

            return aid;
        }

        public static bool CheckExistingClassesbyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT * FROM [ProjectB].[dbo].[StudentAttendance] Where AttendanceId = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }
            
            if (reader.HasRows)
            {
               
                return true;
            }


           
            return false;
        }

        public static bool CheckExistingResult(int studentid, int assessmentcomponentid)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT * FROM [ProjectB].[dbo].[StudentResult] Where StudentId = @sid AND AssessmentComponentId = @acid";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@sid", studentid);
                command.Parameters.AddWithValue("@acid", assessmentcomponentid);
                


                reader = command.ExecuteReader();
            }

            if (reader.HasRows)
            {

                return true;
            }



            return false;
        }

        public static string getCloNamebyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;
            String cmdcheck = "SELECT Name FROM [ProjectB].[dbo].[Clo] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }
            if(!reader.HasRows)
            {
                MessageBox.Show("CLO with this id does not exist");
                return "error";
            }

            string cloname = "cloname";
            while (reader.Read())
            {
                cloname = reader.GetString(0);
            }
           
            return cloname;
        }

        public static string getLookupNamebyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;
            String cmdcheck = "SELECT Name FROM [ProjectB].[dbo].[Lookup] Where LookupId = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }
            if (!reader.HasRows)
            {
                MessageBox.Show("LookUp with this id does not exist");
                return "error";
            }

            string name = "cloname";
            while (reader.Read())
            {
                name = reader.GetString(0);
            }

            return name;
        }
        public static string getRubricNamebyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;
            
            String cmdcheck = "SELECT Details FROM [ProjectB].[dbo].[Rubric] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }
           
            
            if (!reader.HasRows)
            {
                MessageBox.Show("Rubric with this id does not exist");
                return "error";
            }

            string rubricdetails = "rubricdetails";
            while (reader.Read())
            {
                rubricdetails = reader.GetString(0);
            }

            return rubricdetails;
        }
        public static string getStudentRegNobyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT RegistrationNumber FROM [ProjectB].[dbo].[Student] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Student with this id does not exist");
                return "error";
            }

            string RegNo = "RegNo";
            while (reader.Read())
            {
                RegNo = reader.GetString(0);
            }

            return RegNo;
        }

        public static string getStudentFirstNamebyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT FirstName FROM [ProjectB].[dbo].[Student] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Student with this id does not exist");
                return "error";
            }

            string Firstname = "first name";
            while (reader.Read())
            {
                Firstname = reader.GetString(0);
            }

            return Firstname;
        }
        public static string getStudentLastNamebyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT LastName FROM [ProjectB].[dbo].[Student] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Student with this id does not exist");
                return "error";
            }

            string Lastname = "last name";
            while (reader.Read())
            {
                Lastname = reader.GetString(0);
            }

            return Lastname;
        }
        public static string getStudentEmailbyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT Email FROM [ProjectB].[dbo].[Student] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Student with this id does not exist");
                return "error";
            }

            string email = "email";
            while (reader.Read())
            {
                email = reader.GetString(0);
            }

            return email;
        }
        public static string getRubricDetailsbyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT Details FROM [ProjectB].[dbo].[Rubric] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Rubric with this id does not exist");
                return "error";
            }

            string email = "Rubric";
            while (reader.Read())
            {
                email = reader.GetString(0);
            }

            return email;
        }
        public static string getStudentContactbyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT Contact FROM [ProjectB].[dbo].[Student] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Student Contact with this id does not exist");
                return "error";
            }

            string contact = "contact";
            while (reader.Read())
            {
                contact = reader.GetString(0);
            }

            return contact;
        }


        public static int getStudentIdbyRegistrationNumber(string reg)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT Id FROM [ProjectB].[dbo].[Student] Where RegistrationNumber = @reg ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@reg", reg);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Student with this Registeration Number does not exist");
                return -1;
            }

            int id = 0;
            while (reader.Read())
            {
                id = reader.GetInt32(0);
            }

            return id;
        }

        public static int getLookupIdbyName(string name)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT LookupId FROM [ProjectB].[dbo].[Lookup] Where Name = @name ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@name", name);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Lookup with this name does not exist");
                return -1;
            }

            int id = 0;
            while (reader.Read())
            {
                id = reader.GetInt32(0);
            }

            return id;
        }

        public static string getAssessmentComponentNamebyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT Name FROM [ProjectB].[dbo].[AssessmentComponent] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Assessment Component with this id does not exist");
                return "error";
            }

            string  name = "";
            while (reader.Read())
            {
                name = reader.GetString(0);
            }

            return name;
        }

        public static string getAssessmentNamebyId(int id)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT Title FROM [ProjectB].[dbo].[Assessment] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", id);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Assessment with this id does not exist");
                return "error";
            }

            string name = "";
            while (reader.Read())
            {
                name = reader.GetString(0);
            }

            return name;
        }

        public static int getAssessmentComponentTotalMarksbyid(int asid)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT TotalMarks FROM [ProjectB].[dbo].[AssessmentComponent] Where Id = @id ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", asid);


                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Assessment Component with this name does not exist");
                return -1;
            }

            int marks = 0;
            while (reader.Read())
            {
                marks = reader.GetInt32(0);
            }

            return marks;
        }

        public static int getRubricMeasurementIdbyAssessmentComponentIdAndStudentIdFromStudentResult(int Studentid,int asid)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT RubricMeasurementId FROM [ProjectB].[dbo].[StudentResult] Where AssessmentComponentId = @id AND StudentId = @sid ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {

                command.Parameters.AddWithValue("@id", asid);
                command.Parameters.AddWithValue("@sid", Studentid);
                reader = command.ExecuteReader();
            }


            if (!reader.HasRows)
            {
                MessageBox.Show("Student Result with these ids does not exist");
                return -1;
            }

            int id = 0;
            while (reader.Read())
            {
                id = reader.GetInt32(0);
            }

            return id;
        }



        public static SqlDataReader getStudentIdAndAttendanceStatusFromStudentAttendance(int attendanceid)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader reader;

            String cmdcheck = "SELECT StudentId, AttendanceStatus FROM [ProjectB].[dbo].[StudentAttendance] Where AttendanceId = @at ";
            using (SqlCommand command = new SqlCommand(cmdcheck, conn))
            {
                command.Parameters.AddWithValue("@at", attendanceid);
                reader = command.ExecuteReader();
            }


            if(!reader.HasRows)
            {
                return null;
            }
            

            return reader;
        }

        public static bool MarkStudentAttendance(int attendanceid,int studentid,int attendancestatus)
        {
            try
            {
                SqlConnection conn = Connection.buildconnection();
                String query = "INSERT INTO dbo.StudentAttendance (AttendanceId, StudentId,AttendanceStatus) VALUES (@aid,@sid,@ast)";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@aid", attendanceid);
                    command.Parameters.AddWithValue("@sid", studentid);
                    command.Parameters.AddWithValue("@ast", attendancestatus);




                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0)
                    {
                        Console.WriteLine("Error inserting data into Database!");
                        return false;

                    }


                }

            }
          
            catch
            {
                return false;
            }
            return true;
        }






        public static bool MarkStudentAssessment(int studentid, int assessmentcomponentid, int rubricmeasurementid, DateTime date)
        {
            try
            {
                SqlConnection conn = Connection.buildconnection();
                String query = "INSERT INTO dbo.StudentResult (StudentId, AssessmentComponentId,RubricMeasurementId, EvaluationDate) VALUES (@sid,@acid,@rmid,@date)";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@sid", studentid);
                    command.Parameters.AddWithValue("@acid", assessmentcomponentid);
                    command.Parameters.AddWithValue("@rmid", rubricmeasurementid);
                    command.Parameters.AddWithValue("@date", date);




                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0)
                    {
                        Console.WriteLine("Error inserting data into Database!");
                        return false;

                    }


                }

            }

            catch
            {
               return false;
            }
            return true;
        }
        public static bool UpdateStudentAttendance(ref int astatus, ref int indexclassid, ref int indexstudentid)
        {
            SqlConnection conn = Connection.buildconnection();

            String query = "UPDATE StudentAttendance SET AttendanceStatus =@ast WHERE AttendanceId = @aid AND StudentId = @sid";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@ast", astatus);

                command.Parameters.AddWithValue("@aid", indexclassid);
                command.Parameters.AddWithValue("@sid", indexstudentid);

                int result = command.ExecuteNonQuery();

                // Check Error
                if (result <= 0)
                {
                    Console.WriteLine("Error Updating data into Database!");
                    return false;
                }
               
            }
            return true;
        }
        public static bool DeleteStudentAttendance(ref int classid, ref int studentid)
        {
            SqlConnection conn = Connection.buildconnection();
            String query = "DELETE FROM [ProjectB].[dbo].[StudentAttendance] Where AttendanceId=@aid AND StudentId = @sid";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@aid", classid);
                command.Parameters.AddWithValue("@sid", studentid);




                int result = command.ExecuteNonQuery();

                // Check Error
                if (result <= 0)
                {
                    Console.WriteLine("Error Deleting data From Database!");
                    return false;
                }


            }
            return true;
        }

        public static bool DeleteStudentAttendancebyClassid(int id)
        {

            SqlConnection conn = Connection.buildconnection();
            String query = "DELETE FROM [ProjectB].[dbo].[StudentAttendance] Where AttendanceId=@aid";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@aid", id);
                




                int result = command.ExecuteNonQuery();

                // Check Error
                if (result <= 0)
                {
                    Console.WriteLine("Error Deleting data From Database!");
                    return false;
                }


            }

            return true;

        }
        public static bool DeleteStudentResultbyRubricMeasurementid(int id)
        {

            SqlConnection conn = Connection.buildconnection();
            String query = "DELETE FROM [ProjectB].[dbo].[StudentResult] Where RubricMeasurementId=@aid";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@aid", id);





                int result = command.ExecuteNonQuery();

                // Check Error
                if (result <= 0)
                {
                    Console.WriteLine("Error Deleting data From Database!");
                    return false;
                }


            }

            return true;

        }

        public static bool DeleteStudentResultbyAssessmentComponentid(int id)
        {

            SqlConnection conn = Connection.buildconnection();
            String query = "DELETE FROM [ProjectB].[dbo].[StudentResult] Where AssessmentComponentId=@aid";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@aid", id);





                int result = command.ExecuteNonQuery();

                // Check Error
                if (result <= 0)
                {
                    Console.WriteLine("Error Deleting data From Database!");
                    return false;
                }


            }

            return true;

        }
        public static bool DeleteStudentResultbyStudentId(int id)
        {

            SqlConnection conn = Connection.buildconnection();
            String query = "DELETE FROM [ProjectB].[dbo].[StudentResult] Where StudentId=@aid";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@aid", id);





                int result = command.ExecuteNonQuery();

                // Check Error
                if (result <= 0)
                {
                    Console.WriteLine("Error Deleting data From Database!");
                    return false;
                }


            }

            return true;

        }

        public static bool DeleteStudentAttendancebyStudentid(int id)
        {

            SqlConnection conn = Connection.buildconnection();
            String query = "DELETE FROM [ProjectB].[dbo].[StudentAttendance] Where StudentId=@aid";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@aid", id);





                int result = command.ExecuteNonQuery();

                // Check Error
                if (result <= 0)
                {
                    Console.WriteLine("Error Deleting data From Database!");
                    return false;
                }


            }

            return true;

        }
        public static bool Delete_Rubricbyid(int i)
        {
            SqlConnection conn = Connection.buildconnection();


            SqlDataReader AssessmentComponentreader;
            int assessmentid;
            String cmdcheck1 = "SELECT Id FROM [ProjectB].[dbo].[AssessmentComponent] Where RubricId = @at ";
            using (SqlCommand command = new SqlCommand(cmdcheck1, conn))
            {
                command.Parameters.AddWithValue("@at", i);
                AssessmentComponentreader = command.ExecuteReader();
            }
            if(AssessmentComponentreader.HasRows)
            {
                while(AssessmentComponentreader.Read())
                {
                    assessmentid = AssessmentComponentreader.GetInt32(0);
                    Connection.DeleteStudentResultbyAssessmentComponentid(assessmentid);
                }
            }
            SqlDataReader Rubriclevelreader;
            int rubriclevelid;
            String cmdcheck2 = "SELECT Id FROM [ProjectB].[dbo].[AssessmentComponent] Where RubricId = @at ";
            using (SqlCommand command = new SqlCommand(cmdcheck2, conn))
            {
                command.Parameters.AddWithValue("@at", i);
                Rubriclevelreader = command.ExecuteReader();
            }
            if (Rubriclevelreader.HasRows)
            {
                while (Rubriclevelreader.Read())
                {
                    rubriclevelid = Rubriclevelreader.GetInt32(0);
                    Connection.DeleteStudentResultbyAssessmentComponentid(rubriclevelid);
                }
            }


            String query = "Delete FROM [ProjectB].[dbo].[AssessmentComponent] Where RubricId=@id";
            using (SqlCommand commandl = new SqlCommand(query, conn))
            {
                commandl.Parameters.AddWithValue("@id", i);
                int result = commandl.ExecuteNonQuery();
            }
            query = "Delete FROM [ProjectB].[dbo].[RubricLevel] Where RubricId=@id";
            using (SqlCommand commandl = new SqlCommand(query, conn))
            {
                commandl.Parameters.AddWithValue("@id", i);
                int result = commandl.ExecuteNonQuery();
            }
            query = "DELETE FROM [ProjectB].[dbo].[Rubric] Where id=@id";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@id", i);



                int result = command.ExecuteNonQuery();

                // Check Error
                if (result < 0) return false;

            }
            return true;
        }
        public static bool DeleteAssessmentbyid(int i)
        {
            SqlConnection conn = Connection.buildconnection();
            SqlDataReader AssessmentComponentreader;
            int assessmentid;
            String cmdcheck1 = "SELECT Id FROM [ProjectB].[dbo].[AssessmentComponent] Where RubricId = @at ";
            using (SqlCommand command = new SqlCommand(cmdcheck1, conn))
            {
                command.Parameters.AddWithValue("@at", i);
                AssessmentComponentreader = command.ExecuteReader();
            }
            if (AssessmentComponentreader.HasRows)
            {
                while (AssessmentComponentreader.Read())
                {
                    assessmentid = AssessmentComponentreader.GetInt32(0);
                    Connection.DeleteStudentResultbyAssessmentComponentid(assessmentid);
                }
            }

            String query = "DELETE FROM [ProjectB].[dbo].[AssessmentComponent] Where AssessmentId=@id";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@id", i);



                int result = command.ExecuteNonQuery();

                // Check Error
                if (result < 0) return false;


            }
            query = "DELETE FROM [ProjectB].[dbo].[Assessment] Where id=@id";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@id", i);



                int result = command.ExecuteNonQuery();

                // Check Error
                if (result < 0) return false;


            }

            return true;
        }

        public static bool GenerateCloReportbyStudentId(int studentid)
        {
            try
            {
                SqlConnection conn = Connection.buildconnection();
                string studentregisterationnumber = Connection.getStudentRegNobyId(studentid);
                string studentEmail = Connection.getStudentEmailbyId(studentid);
                string studentContact = Connection.getStudentContactbyId(studentid);
                string queryclo = "SELECT Id, Name FROM CLO ";
                SqlCommand Commandclo = new SqlCommand(queryclo, conn);
                SqlDataReader readerClo = Commandclo.ExecuteReader();
                DataTable datatableclo = new DataTable();

                string studentname = Connection.getStudentFirstNamebyId(studentid) + " " + Connection.getStudentLastNamebyId(studentid);
                Document document = new Document();
                PdfWriter.GetInstance(document, new FileStream("E:/1A Semesters/6th semester/DBMS Labs/Reports/" + studentregisterationnumber + "CloReport.pdf", FileMode.Create));
                document.Open();



                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font contentfont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font headingfont = new iTextSharp.text.Font(bf, 20, iTextSharp.text.Font.BOLD);

                Paragraph p = new Paragraph(new Chunk("CLO Report \n" , headingfont));
                p.Alignment = Element.ALIGN_CENTER;
                
                Paragraph p1 = new Paragraph(new Chunk("Registeration Number: " + studentregisterationnumber, font));
                Paragraph p2 = new Paragraph(new Chunk("Name: " + studentname, font));
                Paragraph p3 = new Paragraph(new Chunk("Email: " + studentEmail, font));
                Paragraph p4 = new Paragraph(new Chunk("Contact: " + studentContact, font));
                Paragraph p5 = new Paragraph(new Chunk(" ", font));
                document.Add(p);
                document.Add(p1);
                document.Add(p2);
                document.Add(p3);
                document.Add(p4);
                document.Add(p5);
               

                datatableclo.Load(readerClo);
                int CLOid;
                int RubricId;
                int assessmentcomponentid;
                PdfPTable Report = new PdfPTable(5);
                Paragraph c1 = new Paragraph(new Chunk("CLO", font));
                Paragraph c2 = new Paragraph(new Chunk("Rubric", font));
                Paragraph c3 = new Paragraph(new Chunk("Assessment Component", font));
                Paragraph c4 = new Paragraph(new Chunk("Total Marks", font));
                Paragraph c5 = new Paragraph(new Chunk("Obtained Marks", font));
                Report.AddCell(c1);
                Report.AddCell(c2);
                Report.AddCell(c3);
                Report.AddCell(c4);
                Report.AddCell(c5);


                for (int i = 0; i < datatableclo.Rows.Count; i++)
                {
                    CLOid = Convert.ToInt32(datatableclo.Rows[i].ItemArray[0]);
                    conn = Connection.buildconnection();
                    string queryrubric = "SELECT Id, Details FROM Rubric WHERE CloId = @cloid";
                    SqlCommand Commandrubric = new SqlCommand(queryrubric, conn);
                    Commandrubric.Parameters.AddWithValue("@cloid", CLOid);
                    SqlDataReader readerRubric = Commandrubric.ExecuteReader();
                    DataTable datatablerubric = new DataTable();
                    datatablerubric.Load(readerRubric);
                    string cloname = Connection.getCloNamebyId(CLOid);
                    if (datatablerubric.Rows.Count == 0)
                    {
                        p = new Paragraph(new Chunk(cloname, contentfont));
                        Report.AddCell(p);
                        p = new Paragraph(new Chunk("Nil", contentfont));
                        Report.AddCell(p);
                        Report.AddCell(p);
                        Report.AddCell(p);
                        Report.AddCell(p);
                    }
                    for (int j = 0; j < datatablerubric.Rows.Count; j++)
                    {
                        RubricId = Convert.ToInt32(datatablerubric.Rows[j].ItemArray[0]);
                        conn = Connection.buildconnection();
                        string queryassessment = "SELECT Id, Name FROM AssessmentComponent WHERE RubricId = @rid";
                        SqlCommand Commandassessment = new SqlCommand(queryassessment, conn);
                        Commandassessment.Parameters.AddWithValue("@rid", RubricId);
                        SqlDataReader readerassessment = Commandassessment.ExecuteReader();
                        DataTable datatableassessment = new DataTable();
                        datatableassessment.Load(readerassessment);
                        string Rubricname = Connection.getRubricDetailsbyId(RubricId);
                        if (datatableassessment.Rows.Count == 0)
                        {
                            p = new Paragraph(new Chunk(cloname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(Rubricname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk("Nil", contentfont));
                            Report.AddCell(p);
                            Report.AddCell(p);
                            Report.AddCell(p);
                        }
                        for (int k = 0; k < datatableassessment.Rows.Count; k++)
                        {
                            assessmentcomponentid = Convert.ToInt32(datatableassessment.Rows[k].ItemArray[0]);
                            string assessmentname = Connection.getAssessmentComponentNamebyId(assessmentcomponentid);
                            int componentmarks = Connection.getAssessmentComponentTotalMarksbyid(assessmentcomponentid);
                            int studentrubriclevel = Connection.getMeasurementLevelbyRubricLevelId(Connection.getRubricMeasurementIdbyAssessmentComponentIdAndStudentIdFromStudentResult(studentid, assessmentcomponentid));
                            int maxrubriclevel = Connection.getMaximumRubricLevelbyRubricId(RubricId);
                            float obtainedmarks = -1;
                            if (studentrubriclevel == -1 || maxrubriclevel == -1)
                            {
                                
                            }
                            else
                            {
                                obtainedmarks = (float)((((float)studentrubriclevel) / ((float)maxrubriclevel)) * ((float)componentmarks));
                            }
                           
                            p = new Paragraph(new Chunk(cloname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(Rubricname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(assessmentname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(componentmarks.ToString(), contentfont));
                            Report.AddCell(p);
                            if(obtainedmarks == -1)
                            {
                                p = new Paragraph(new Chunk("Nil", contentfont));
                            }
                            else
                            {
                                p = new Paragraph(new Chunk(obtainedmarks.ToString(), contentfont));
                            }
                           
                            Report.AddCell(p);
                        }



                    }
                }

                Report.HorizontalAlignment = 12;
                document.Add(Report);
                document.Close();

            }
            catch
            {
                return false;
            }
            return true;
        }
        
        public static bool GenerateClassCloReport()
        {
            try
            {
                SqlConnection conn = Connection.buildconnection();
              
                string queryStudent = "SELECT Id, RegistrationNumber FROM Student ";
                SqlCommand CommandStudent = new SqlCommand(queryStudent, conn);
                SqlDataReader readerStudent = CommandStudent.ExecuteReader();
                DataTable datatableStudent = new DataTable();

                conn = Connection.buildconnection();
                datatableStudent.Load(readerStudent);
                string queryclo = "SELECT Id, Name FROM CLO ";
                SqlCommand Commandclo = new SqlCommand(queryclo, conn);
                SqlDataReader readerClo = Commandclo.ExecuteReader();
                DataTable datatableclo = new DataTable();

                
                Document document = new Document();
                PdfWriter.GetInstance(document, new FileStream("E:/1A Semesters/6th semester/DBMS Labs/Reports/ClassCloReport.pdf", FileMode.Create));
                document.Open();
                datatableclo.Load(readerClo);


                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font contentfont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font headingfont = new iTextSharp.text.Font(bf, 20, iTextSharp.text.Font.BOLD);

                Paragraph p = new Paragraph(new Chunk("Class CLO Report \n", headingfont));
                p.Alignment = Element.ALIGN_CENTER;

               
                Paragraph p5 = new Paragraph(new Chunk(" \n", font));
                document.Add(p);
                document.Add(p5);
                PdfPTable Report = new PdfPTable(datatableclo.Rows.Count+1);
                Paragraph c1 = new Paragraph(new Chunk("Student Registration Number", font));
                Report.AddCell(c1);
                for(int i = 0; i < datatableclo.Rows.Count; i++)
                {
                    string CLOName = Connection.getCloNamebyId(Convert.ToInt32(datatableclo.Rows[i].ItemArray[0]));
                    c1 = new Paragraph(new Chunk(CLOName, font));
                    Report.AddCell(c1);
                }
                

                for(int sid = 0; sid < datatableStudent.Rows.Count; sid++)
                {
                    int studentid = Convert.ToInt32(datatableStudent.Rows[sid].ItemArray[0]);

                    string studentregisterationnumber = Connection.getStudentRegNobyId(studentid);
                    string studentEmail = Connection.getStudentEmailbyId(studentid);
                    string studentContact = Connection.getStudentContactbyId(studentid);
                    string studentname = Connection.getStudentFirstNamebyId(studentid) + " " + Connection.getStudentLastNamebyId(studentid);
                    c1 = new Paragraph(new Chunk(studentregisterationnumber, font));
                    Report.AddCell(c1);

                    int CLOid;
                    int RubricId;
                    int assessmentcomponentid;



                    for (int i = 0; i < datatableclo.Rows.Count; i++)
                    {
                        CLOid = Convert.ToInt32(datatableclo.Rows[i].ItemArray[0]);
                        conn = Connection.buildconnection();
                        string queryrubric = "SELECT Id, Details FROM Rubric WHERE CloId = @cloid";
                        SqlCommand Commandrubric = new SqlCommand(queryrubric, conn);
                        Commandrubric.Parameters.AddWithValue("@cloid", CLOid);
                        SqlDataReader readerRubric = Commandrubric.ExecuteReader();
                        DataTable datatablerubric = new DataTable();
                        datatablerubric.Load(readerRubric);
                        string cloname = Connection.getCloNamebyId(CLOid);
                        if (datatablerubric.Rows.Count == 0)
                        {
                            
                            p = new Paragraph(new Chunk("Nil", font));
                            Report.AddCell(p);
                            
                        }
                        float totalcomponentmarks = 0;
                        float totalstudentmarks = 0;
                        for (int j = 0; j < datatablerubric.Rows.Count; j++)
                        {
                            RubricId = Convert.ToInt32(datatablerubric.Rows[j].ItemArray[0]);
                            conn = Connection.buildconnection();
                            string queryassessment = "SELECT Id, Name FROM AssessmentComponent WHERE RubricId = @rid";
                            SqlCommand Commandassessment = new SqlCommand(queryassessment, conn);
                            Commandassessment.Parameters.AddWithValue("@rid", RubricId);
                            SqlDataReader readerassessment = Commandassessment.ExecuteReader();
                            DataTable datatableassessment = new DataTable();
                            datatableassessment.Load(readerassessment);
                            string Rubricname = Connection.getRubricDetailsbyId(RubricId);
                            
                            for (int k = 0; k < datatableassessment.Rows.Count; k++)
                            {
                                assessmentcomponentid = Convert.ToInt32(datatableassessment.Rows[k].ItemArray[0]);
                                string assessmentname = Connection.getAssessmentComponentNamebyId(assessmentcomponentid);

                                int componentmarks = Connection.getAssessmentComponentTotalMarksbyid(assessmentcomponentid);
                                totalcomponentmarks = (float)totalcomponentmarks + (float)componentmarks;
                                int studentrubriclevel = Connection.getMeasurementLevelbyRubricLevelId(Connection.getRubricMeasurementIdbyAssessmentComponentIdAndStudentIdFromStudentResult(studentid, assessmentcomponentid));
                                int maxrubriclevel = Connection.getMaximumRubricLevelbyRubricId(RubricId);
                                float obtainedmarks = -1;
                                if (studentrubriclevel == -1 || maxrubriclevel == -1)
                                {
                                    totalstudentmarks = totalstudentmarks + 0;
                                }
                                else
                                {
                                    obtainedmarks = (float)((((float)studentrubriclevel) / ((float)maxrubriclevel)) * ((float)componentmarks));
                                    totalstudentmarks = totalstudentmarks + obtainedmarks;
                                }  
                            }
                        }
                        float percentage = (totalstudentmarks / totalcomponentmarks) * 100;
                        if (datatablerubric.Rows.Count > 0)
                        {
                            c1 = new Paragraph(new Chunk((percentage.ToString()) + "%", contentfont));
                            Report.AddCell(c1);
                        }
                    }


                }

                Report.HorizontalAlignment = 12;
                document.Add(Report);
                document.Close();

            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool GenerateAssessmentReportbyStudentId(int studentid)
        {
            try
            {
                SqlConnection conn = Connection.buildconnection();
                string studentregisterationnumber = Connection.getStudentRegNobyId(studentid);
                string studentEmail = Connection.getStudentEmailbyId(studentid);
                string studentContact = Connection.getStudentContactbyId(studentid);
                string queryAssessment = "SELECT Id, Title FROM Assessment ";
                SqlCommand CommandAssessment = new SqlCommand(queryAssessment, conn);
                SqlDataReader readerAssessment = CommandAssessment.ExecuteReader();
                DataTable datatableAssessment = new DataTable();

                string studentname = Connection.getStudentFirstNamebyId(studentid) + " " + Connection.getStudentLastNamebyId(studentid);
                Document document = new Document();
                PdfWriter.GetInstance(document, new FileStream("E:/1A Semesters/6th semester/DBMS Labs/Reports/" + studentregisterationnumber + "AssessmentReport.pdf", FileMode.Create));
                document.Open();



                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font contentfont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font headingfont = new iTextSharp.text.Font(bf, 20, iTextSharp.text.Font.BOLD);

                Paragraph p = new Paragraph(new Chunk("Assessment Report \n", headingfont));
                p.Alignment = Element.ALIGN_CENTER;

                Paragraph p1 = new Paragraph(new Chunk("Registeration Number: " + studentregisterationnumber, font));
                Paragraph p2 = new Paragraph(new Chunk("Name: " + studentname, font));
                Paragraph p3 = new Paragraph(new Chunk("Email: " + studentEmail, font));
                Paragraph p4 = new Paragraph(new Chunk("Contact: " + studentContact, font));
                Paragraph p5 = new Paragraph(new Chunk(" ", font));
                document.Add(p);
                document.Add(p1);
                document.Add(p2);
                document.Add(p3);
                document.Add(p4);
                document.Add(p5);


                datatableAssessment.Load(readerAssessment);
                int AssessmentId;
                int RubricId;
                int assessmentcomponentid;
                PdfPTable Report = new PdfPTable(6);
                Paragraph c1 = new Paragraph(new Chunk("Assessment", font));
                Paragraph c2 = new Paragraph(new Chunk("Assessment Component", font));
                Paragraph c3 = new Paragraph(new Chunk("Rubric", font));
                Paragraph c4 = new Paragraph(new Chunk("Total Marks", font));
                Paragraph c5 = new Paragraph(new Chunk("Student Rubric Level", font));
                Paragraph c6 = new Paragraph(new Chunk("Obtained Marks", font));
                Report.AddCell(c1);
                Report.AddCell(c2);
                Report.AddCell(c3);
                Report.AddCell(c4);
                Report.AddCell(c5);
                Report.AddCell(c6);


                for (int i = 0; i < datatableAssessment.Rows.Count; i++)
                {
                    AssessmentId = Convert.ToInt32(datatableAssessment.Rows[i].ItemArray[0]);
                    conn = Connection.buildconnection();
                    string queryAssessmentComponent = "SELECT Id, Name, RubricId FROM AssessmentComponent WHERE AssessmentId = @assessmentid";
                    SqlCommand CommandAssessmentComponent = new SqlCommand(queryAssessmentComponent, conn);
                    CommandAssessmentComponent.Parameters.AddWithValue("@assessmentid", AssessmentId);
                    SqlDataReader readerAssessmentComponent = CommandAssessmentComponent.ExecuteReader();
                    DataTable datatableAssessmentComponent = new DataTable();
                    datatableAssessmentComponent.Load(readerAssessmentComponent);
                    string Assessmentname = Connection.getAssessmentNamebyId(AssessmentId);
                    if (datatableAssessmentComponent.Rows.Count == 0)
                    {
                        p = new Paragraph(new Chunk(Assessmentname, contentfont));
                        Report.AddCell(p);
                        p = new Paragraph(new Chunk("Nil", contentfont));
                        Report.AddCell(p);
                        Report.AddCell(p);
                        Report.AddCell(p);
                        Report.AddCell(p);
                        Report.AddCell(p);
                    }
                    for (int j = 0; j < datatableAssessmentComponent.Rows.Count; j++)
                    {
                        assessmentcomponentid = Convert.ToInt32(datatableAssessmentComponent.Rows[j].ItemArray[0]);
                        string assessmentcomponentname = Connection.getAssessmentComponentNamebyId(assessmentcomponentid);
                        RubricId = Convert.ToInt32(datatableAssessmentComponent.Rows[j].ItemArray[2]);
                        conn = Connection.buildconnection();
                        string querystudentassessment = "SELECT RubricMeasurementId FROM StudentResult WHERE AssessmentComponentId = @rid AND StudentId = @sid";
                        SqlCommand Commandstudentassessment = new SqlCommand(querystudentassessment, conn);
                        Commandstudentassessment.Parameters.AddWithValue("@rid", assessmentcomponentid);
                        Commandstudentassessment.Parameters.AddWithValue("@sid", studentid);
                        SqlDataReader readerstudentassessment = Commandstudentassessment.ExecuteReader();
                        DataTable datatablestudentassessment = new DataTable();
                        datatablestudentassessment.Load(readerstudentassessment);
                        string Rubricname = Connection.getRubricDetailsbyId(RubricId);
                        if (datatablestudentassessment.Rows.Count == 0)
                        {
                            p = new Paragraph(new Chunk(Assessmentname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(assessmentcomponentname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(Rubricname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk("Nil", contentfont));
                            Report.AddCell(p);
                            Report.AddCell(p);
                            Report.AddCell(p);
                        }
                        for (int k = 0; k < datatablestudentassessment.Rows.Count; k++)
                        {
                            
                         
                            int componentmarks = Connection.getAssessmentComponentTotalMarksbyid(assessmentcomponentid);
                            int studentrubriclevel = Connection.getMeasurementLevelbyRubricLevelId(Connection.getRubricMeasurementIdbyAssessmentComponentIdAndStudentIdFromStudentResult(studentid, assessmentcomponentid));
                            int maxrubriclevel = Connection.getMaximumRubricLevelbyRubricId(RubricId);
                            float obtainedmarks = -1;
                            if (studentrubriclevel == -1 || maxrubriclevel == -1)
                            {

                            }
                            else
                            {
                                obtainedmarks = (float)((((float)studentrubriclevel) / ((float)maxrubriclevel)) * ((float)componentmarks));
                            }
                            p = new Paragraph(new Chunk(Assessmentname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(assessmentcomponentname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(Rubricname, contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(componentmarks.ToString(), contentfont));
                            Report.AddCell(p);
                            p = new Paragraph(new Chunk(studentrubriclevel.ToString(), contentfont));
                            Report.AddCell(p);
                            if (obtainedmarks == -1)
                            {
                                p = new Paragraph(new Chunk("Nil", contentfont));
                            }
                            else
                            {
                                p = new Paragraph(new Chunk(obtainedmarks.ToString(), contentfont));
                            }
                            Report.AddCell(p);
                        }



                    }
                }

                Report.HorizontalAlignment = 12;
                document.Add(Report);
                document.Close();

            }
            catch
            {
              return false;
            }
            return true;
        }
        public static bool GenerateClassAssessmentReport()
        {
            try
            {
                SqlConnection conn = Connection.buildconnection();
                string queryStudent = "SELECT Id, RegistrationNumber FROM Student ";
                SqlCommand CommandStudent = new SqlCommand(queryStudent, conn);
                SqlDataReader readerStudent = CommandStudent.ExecuteReader();
                DataTable datatableStudent = new DataTable();
                datatableStudent.Load(readerStudent);


                conn = Connection.buildconnection();
               
                string queryAssessment = "SELECT Id, Title FROM Assessment ";
                SqlCommand CommandAssessment = new SqlCommand(queryAssessment, conn);
                SqlDataReader readerAssessment = CommandAssessment.ExecuteReader();
                DataTable datatableAssessment = new DataTable();

                
                Document document = new Document();
                PdfWriter.GetInstance(document, new FileStream("E:/1A Semesters/6th semester/DBMS Labs/Reports/ClassAssessmentReport.pdf", FileMode.Create));
                document.Open();



                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font contentfont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font headingfont = new iTextSharp.text.Font(bf, 20, iTextSharp.text.Font.BOLD);

                Paragraph p = new Paragraph(new Chunk("Class Assessment Report \n", headingfont));
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph(new Chunk("\n", headingfont));
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                datatableAssessment.Load(readerAssessment);
                PdfPTable Report = new PdfPTable(datatableAssessment.Rows.Count+1);
                p = new Paragraph(new Chunk("Student Registration Number", font));
                Report.AddCell(p);

                Paragraph p1 = new Paragraph(new Chunk("", font));
                for (int asid = 0; asid < datatableAssessment.Rows.Count; asid++)
                {
                    string assessment = Connection.getAssessmentNamebyId(Convert.ToInt32(datatableAssessment.Rows[asid].ItemArray[0]));
                    p1 = new Paragraph(new Chunk(assessment, font));
                    Report.AddCell(p1);
                }
                for (int students = 0; students< datatableStudent.Rows.Count; students++ )
                {
                    int studentid = Convert.ToInt32(datatableStudent.Rows[students].ItemArray[0]);
                    string studentregisterationnumber = Connection.getStudentRegNobyId(studentid);
                    string studentEmail = Connection.getStudentEmailbyId(studentid);
                    string studentContact = Connection.getStudentContactbyId(studentid);
                    int AssessmentId;
                    int RubricId;
                    int assessmentcomponentid;

                    p1 = new Paragraph(new Chunk(studentregisterationnumber, font));
                    Report.AddCell(p1);

                    for (int i = 0; i < datatableAssessment.Rows.Count; i++)
                    {
                        AssessmentId = Convert.ToInt32(datatableAssessment.Rows[i].ItemArray[0]);
                        conn = Connection.buildconnection();
                        string queryAssessmentComponent = "SELECT Id, Name, RubricId FROM AssessmentComponent WHERE AssessmentId = @assessmentid";
                        SqlCommand CommandAssessmentComponent = new SqlCommand(queryAssessmentComponent, conn);
                        CommandAssessmentComponent.Parameters.AddWithValue("@assessmentid", AssessmentId);
                        SqlDataReader readerAssessmentComponent = CommandAssessmentComponent.ExecuteReader();
                        DataTable datatableAssessmentComponent = new DataTable();
                        datatableAssessmentComponent.Load(readerAssessmentComponent);
                        string Assessmentname = Connection.getAssessmentNamebyId(AssessmentId);
                        if (datatableAssessmentComponent.Rows.Count == 0)
                        {
                            p = new Paragraph(new Chunk("Nil", contentfont));
                            Report.AddCell(p);
                          
                        }
                        float totalcomponentmarks = 0;
                        float totalstudentmarks = 0;
                        for (int j = 0; j < datatableAssessmentComponent.Rows.Count; j++)
                        {
                            assessmentcomponentid = Convert.ToInt32(datatableAssessmentComponent.Rows[j].ItemArray[0]);
                            string assessmentcomponentname = Connection.getAssessmentComponentNamebyId(assessmentcomponentid);
                            RubricId = Convert.ToInt32(datatableAssessmentComponent.Rows[j].ItemArray[2]);
                            conn = Connection.buildconnection();
                            string querystudentassessment = "SELECT RubricMeasurementId FROM StudentResult WHERE AssessmentComponentId = @rid AND StudentId = @sid";
                            SqlCommand Commandstudentassessment = new SqlCommand(querystudentassessment, conn);
                            Commandstudentassessment.Parameters.AddWithValue("@rid", assessmentcomponentid);
                            Commandstudentassessment.Parameters.AddWithValue("@sid", studentid);
                            SqlDataReader readerstudentassessment = Commandstudentassessment.ExecuteReader();
                            DataTable datatablestudentassessment = new DataTable();
                            datatablestudentassessment.Load(readerstudentassessment);
                            string Rubricname = Connection.getRubricDetailsbyId(RubricId);
                           
                            
                            for (int k = 0; k < datatablestudentassessment.Rows.Count; k++)
                            {

                                int componentmarks = Connection.getAssessmentComponentTotalMarksbyid(assessmentcomponentid);
                                totalcomponentmarks = (float)totalcomponentmarks + (float)componentmarks;
                                int studentassignedrubriclevelid = Connection.getRubricMeasurementIdbyAssessmentComponentIdAndStudentIdFromStudentResult(studentid, assessmentcomponentid);
                                int studentrubriclevel = Connection.getMeasurementLevelbyRubricLevelId(studentassignedrubriclevelid);
                                int maxrubriclevel = Connection.getMaximumRubricLevelbyRubricId(RubricId);
                                float obtainedmarks = -1;
                                if (studentrubriclevel == -1 || maxrubriclevel == -1)
                                {
                                    totalstudentmarks = totalstudentmarks + 0;
                                }
                                else
                                {
                                    obtainedmarks = (float)((((float)studentrubriclevel) / ((float)maxrubriclevel)) * ((float)componentmarks));
                                    totalstudentmarks = totalstudentmarks + obtainedmarks;
                                }
                            }
                           
                        }
                        float percentage = (totalstudentmarks / totalcomponentmarks) * 100;
                        if (datatableAssessmentComponent.Rows.Count > 0)
                        {
                            p1 = new Paragraph(new Chunk((percentage.ToString()) +"%", contentfont));
                            Report.AddCell(p1);
                        }
                       
                    }
                }
                Report.HorizontalAlignment = 12;
                document.Add(Report);
                document.Close();

            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteRubricLevelById(int i)
        {
            SqlConnection conn = Connection.buildconnection();
            Connection.DeleteStudentResultbyRubricMeasurementid(i);

            String query = "DELETE FROM [ProjectB].[dbo].[RubricLevel] Where id=@id";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@id", i);



                int result = command.ExecuteNonQuery();

                // Check Error
                if (result < 0) return false;


            }
            return true;
        }
    }
}
