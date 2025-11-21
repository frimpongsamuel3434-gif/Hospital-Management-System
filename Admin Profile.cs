using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HealthCare_Plus
{
    public partial class Admin_Profile : Form
    {
        public Admin_Profile()
        {
            InitializeComponent();

            // Assuming you've obtained the admin's ID from the login process
            int loggedInAdminID = GetLoggedInAdminID();

            // Set the logged-in admin's ID
            LoggedInAdmin loggedInAdmin = LoggedInAdmin.Instance;
            loggedInAdmin.AdminID = 1;

            LoadAdminDetails();
            LoadAdminImage();
        }

        private void BackToDashboardButton_Click(object sender, EventArgs e)
        {
            // Create an instance of Admin_Dashboard form
            Admin_Dashboard adminDashboard = new Admin_Dashboard();

            // Set the window state of Admin_Dashboard to Maximized
            adminDashboard.WindowState = FormWindowState.Maximized;

            // Show the Admin_Dashboard form
            adminDashboard.Show();

            // Hide the current Admin_Profile form
            this.Hide();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg; *.jpeg; *.png; *.bmp|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Display the selected image in the pictureBox1 control
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                    // Update the image in the database for the logged-in admin
                    string connectionString = "Server=localhost;Database=healthcare_plus;User Id=root;Password=Thamindu4420#;";
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "UPDATE admin_registration SET AdminProfilePhoto = @Image WHERE AdminID = @AdminID";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            // Assuming you have a way to track the logged-in admin's ID
                            int loggedInAdminID = GetLoggedInAdminID();

                            // Convert the selected image to bytes
                            byte[] imageBytes = ImageToByteArray(pictureBox1.Image);

                            command.Parameters.AddWithValue("@Image", imageBytes);
                            command.Parameters.AddWithValue("@AdminID", loggedInAdminID);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Image changed successfully.");
                            }
                            else
                            {
                                MessageBox.Show("Failed to update image.");
                            }
                            LoadAdminDetails();
                        }
                    }
                }
            }
        }

        private int GetLoggedInAdminID()
        {
            // Assuming you have a class to store the logged-in admin details
            LoggedInAdmin loggedInAdmin = LoggedInAdmin.Instance; 

            if (loggedInAdmin != null)
            {
                return loggedInAdmin.AdminID;
            }
            else
            {
                return -1; 
            }
        }

        private void LoadAdminDetails()
        {
            try
            {
                int loggedInAdminID = GetLoggedInAdminID();

                string connectionString = "Server=localhost;Database=healthcare_plus;User Id=root;Password=Sammy3434;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Username, Email, Password FROM admin_registration WHERE AdminID = @AdminID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AdminID", loggedInAdminID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                RetreiveUsername.Text = reader.GetString("Username");
                                RetreiveEmail.Text = reader.GetString("Email");
                                RetreivePassword.Text = reader.GetString("Password");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                int loggedInAdminID = GetLoggedInAdminID();

                string connectionString = "Server=localhost;Database=healthcare_plus;User Id=root;Password=Sammy3434;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE admin_registration SET AdminProfilePhoto = @Image WHERE AdminID = @AdminID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Convert the selected image to bytes
                        byte[] imageBytes = ImageToByteArray(pictureBox1.Image);

                        command.Parameters.AddWithValue("@Image", imageBytes);
                        command.Parameters.AddWithValue("@AdminID", loggedInAdminID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Image saved successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Failed to save image.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            LoadAdminDetails();
        }

        private void LoadAdminImage()
        {
            try
            {
                int loggedInAdminID = GetLoggedInAdminID();

                string connectionString = "Server=localhost;Database=healthcare_plus;User Id=root;Password=Thamindu4420#;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT AdminProfilePhoto FROM admin_registration WHERE AdminID = @AdminID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AdminID", loggedInAdminID);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            byte[] imageBytes = (byte[])result;
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                pictureBox1.Image = Image.FromStream(ms);
                            }
                            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

            byte[] ImageToByteArray(Image image)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, ImageFormat.Jpeg);
                    return memoryStream.ToArray();
                }
            }
        public class LoggedInAdmin
        {
            private static LoggedInAdmin? instance;

            public int AdminID { get; set; }

            private LoggedInAdmin() { }

            public static LoggedInAdmin Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new LoggedInAdmin();
                    }
                    return instance;
                }
            }
        }

    }
}

    





