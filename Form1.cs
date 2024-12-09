using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ClientAppointment
{
    public partial class Form1 : Form
    {
        // Connection string to the database
        private const string ConnectionString = "Data Source=LAPTOP-R6NA64TT\\SQLEXPRESS;Initial Catalog=\"Appointment Scheduling\";Integrated Security=True;Encrypt=False";

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private void submitBtn_Click(object sender, EventArgs e)
        {
            string query = @"INSERT INTO Appointments (Name, [Contact Number], Date, Time, [Appointment Details], Reschedule) 
                             VALUES (@Name, @ContactNumber, @Date, @Time, @AppointmentDetails, @Reschedule)";

            // Open connection and execute the SQL query
            ExecuteQuery(query, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@Name", TxtBxName.Text.Trim());
                cmd.Parameters.AddWithValue("@ContactNumber", TxtBxContactNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@Date", dateTimePicker.Value);
                cmd.Parameters.AddWithValue("@Time", GetValidTime());
                cmd.Parameters.AddWithValue("@AppointmentDetails", string.IsNullOrEmpty(RTBNotes.Text) ? DBNull.Value : (object)RTBNotes.Text.Trim());
                cmd.Parameters.AddWithValue("@Reschedule", RBYes.Checked ? 1 : 0);
            });
            // Ask the user if they want a PDF copy
            var result = MessageBox.Show("Do you want to generate a PDF of the appointment?",
                                         "Generate PDF",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            // If the user clicks 'Yes', generate the PDF
            if (result == DialogResult.Yes)
            {
                GeneratePdf();
                MessageBox.Show("Thank you!");
            }
            else
            {
                MessageBox.Show("Thank you!");
            }
        }
        // This function executes a given query with the provided parameters
        private void ExecuteQuery(string query, Action<SqlCommand> addParameters)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        addParameters(cmd);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        MessageBox.Show(rowsAffected > 0 ? "Operation successful." : "Operation failed.");
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"SQL Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        // Helper function to ensure the time format is valid
        private string GetValidTime()
        {
            if (DateTime.TryParse(CBTime.Text.Trim(), out DateTime parsedTime))
                return parsedTime.ToString("HH:mm:ss");
            MessageBox.Show("Invalid time format. Please use HH:mm:ss.");
            return string.Empty;
        }
        // Function to generate a PDF with appointment details
        private void GeneratePdf()
        {
            string filePath = "C:\\Users\\Meri\\Downloads\\appointment_details.pdf";
            using (Document doc = new Document())
            {
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                doc.Open();
                doc.Add(new Paragraph("Appointment Details"));
                doc.Add(new Paragraph($"Name: {TxtBxName.Text.Trim()}"));
                doc.Add(new Paragraph($"Contact: {TxtBxContactNumber.Text.Trim()}"));
                doc.Add(new Paragraph($"Date: {dateTimePicker.Value.ToShortDateString()}"));
                doc.Add(new Paragraph($"Time: {CBTime.Text.Trim()}"));
                doc.Add(new Paragraph($"Details: {RTBNotes.Text.Trim()}"));
                doc.Add(new Paragraph($"Reschedule: {(RBYes.Checked ? "Yes" : "No")}"));
            }
            MessageBox.Show("PDF generated successfully!");
        }
        // Function to load all appointments from the database
        private void LoadAppointments()
        {
            string query = "SELECT * FROM Appointments";
            ExecuteQuery(query, cmd => { });
        }
        /* Function to update an appointment in the database
        private void updateBtn_Click(object sender, EventArgs e)
        {
            string query = @"UPDATE Appointments 
                             SET [Appointment Details] = @NewDetails, Reschedule = @Reschedule
                             WHERE Name = @Name AND [Contact Number] = @ContactNumber";
            ExecuteQuery(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@Name", TxtBxName.Text.Trim());
                cmd.Parameters.AddWithValue("@ContactNumber", TxtBxContactNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@NewDetails", RTBNotes.Text.Trim());
                cmd.Parameters.AddWithValue("@Reschedule", RBYes.Checked ? 1 : 0);
            });
        }

        // Function to delete an appointment from the database
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            string query = @"DELETE FROM Appointments WHERE Name = @Name AND [Contact Number] = @ContactNumber";
            ExecuteQuery(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@Name", TxtBxName.Text.Trim());
                cmd.Parameters.AddWithValue("@ContactNumber", TxtBxContactNumber.Text.Trim());
            });
        }*/

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }
    }
}
