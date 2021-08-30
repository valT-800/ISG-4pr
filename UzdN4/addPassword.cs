using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace UzdN4
{
    public partial class addPassword : Form
    {
        Main Form;

        private string cKey = "SUPERRANDOMKEY";

        private string CSV_PATH = "Passwords.csv";

        public addPassword(Main Main)
        {
            Form = Main;

            InitializeComponent();
        }

        List<Data> readToList()
        {
            using (var reader = new StreamReader(CSV_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = new List<Data>();
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var record = new Data
                    {
                        Title = csv.GetField("Title"),
                        Password = csv.GetField("Password"),
                        URL = csv.GetField("URL"),
                        Comment = csv.GetField("Comment")
                    };

                    records.Add(record);
                }
                return records;
            }
        }

        bool containsRecordByTitle(string title)
        {
            bool contains = false;

            using (var reader = new StreamReader(CSV_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = new List<Data>();
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var record = new Data { Title = csv.GetField("Title") };

                    if (record.Title == title)
                    {
                        contains = true;
                        break;
                    }
                }
            }
            return contains;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (containsRecordByTitle(titleTextBox.Text))
                {
                    MessageBox.Show("An entry with this password title already exists");
                    return;
                }

                if (string.IsNullOrEmpty(titleTextBox.Text))
                {
                    MessageBox.Show("Fill in the password title");
                    return;
                }
                if (string.IsNullOrEmpty(passwTextBox.Text))
                {
                    MessageBox.Show("Fill in the password");
                    return;
                }
                if (string.IsNullOrEmpty(urlTextBox.Text))
                {
                    MessageBox.Show("Fill in the Apllication title or URL");
                    return;
                }

                string encryptedPw = SimpleAES.AES256.Encrypt(titleTextBox.Text, cKey);

                List<Data> Loaded = readToList();
                Loaded.Add(new Data { Title = titleTextBox.Text, Password = encryptedPw, URL = urlTextBox.Text, Comment = commentTextBox.Text });


                using (var writer = new StreamWriter(CSV_PATH))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    csv.WriteRecords(Loaded);

                Form.loadData();

                Form.Show();

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add password, please try again!\nError: " + ex.Message);
            }
        }

        private void addPassword_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[16];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[random.Next(chars.Length)];

            var finalString = new String(stringChars);

            passwTextBox.Text = finalString;
        }

        private void addPassword_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form.Show();
        }
    }
}