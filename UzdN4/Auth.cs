using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace UzdN4
{
    public partial class Auth : Form
    {
        private string cKey = "SUPERRANDOMKEY";

        private static string CSV_PATH = "Auth.csv";

        public Auth()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Password text box is empty");
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Username text box is empty");
                return;
            }

            if (!File.Exists(CSV_PATH))
            {
                List<User> uData = new List<User>();
                uData.Add(new User() { Username = textBox2.Text, Password = textBox1.Text });

                using (var writer = new StreamWriter(CSV_PATH))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    csv.WriteRecords(uData);

                string f = File.ReadAllText(CSV_PATH);
                File.WriteAllText(CSV_PATH, SimpleAES.AES256.Encrypt(f, cKey));
            }
            else
            {
                try
                {
                    string fa = File.ReadAllText(CSV_PATH);
                    File.WriteAllText(CSV_PATH, SimpleAES.AES256.Decrypt(fa, cKey));
                }
                catch
                {

                }

                string cTitle = "", cPass = "";

                using (var reader = new StreamReader(CSV_PATH))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        var record = new User
                        {
                            Username = csv.GetField("Username"),
                            Password = csv.GetField("Password"),
                        };

                        cTitle = record.Username;
                        cPass = record.Password;
                    }
                }

                string f = File.ReadAllText(CSV_PATH);
                File.WriteAllText(CSV_PATH, SimpleAES.AES256.Encrypt(f, cKey));

                if (textBox1.Text != cPass ||  textBox2.Text != cTitle)
                {
                    MessageBox.Show("Password is incorrect!");
                    return;
                }
            }

            new Main().Show();

            Hide();
        }

        private void Auth_Load(object sender, EventArgs e)
        {
            if (File.Exists("Auth.csv"))
            {
                label1.Text = "Login";
                button1.Text = "Login";
            }
        }

        private void Auth_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}