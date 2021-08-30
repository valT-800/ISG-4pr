using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace UzdN4
{
    public partial class Main : Form
    {
        private static string CSV_PATH = "Passwords.csv";

        private string cKey = "SUPERRANDOMKEY";

        public Main()
        {
            InitializeComponent();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            string pwFile = File.ReadAllText("Passwords.csv");
            string pwFileCrypted = SimpleAES.AES256.Encrypt(pwFile, cKey);
            File.WriteAllText("Passwords.csv", pwFileCrypted);

            Environment.Exit(0);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            addPassword AP = new addPassword(this);
            AP.Show();
            Hide();
        }

        static void writeCSV(List<Data> list)
        {
            using (var writer = new StreamWriter(CSV_PATH))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                csv.WriteRecords(list);
        }

        public void loadData()
        {
            listView1.Items.Clear();

            int id = 1;

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
                        Password = SimpleAES.AES256.Decrypt(csv.GetField("Password"), cKey),
                        URL = csv.GetField("URL"),
                        Comment = csv.GetField("Comment")
                    };

                    listView1.Items.Add(new ListViewItem(new string[] { id.ToString(), record.Title, record.Password, record.URL, record.Comment }));

                    id++;
                }
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            string pwFile = File.ReadAllText("Passwords.csv");
            string pwFileCrypted = SimpleAES.AES256.Encrypt(pwFile, cKey);
            string pwFileDeCrypted = SimpleAES.AES256.Decrypt(pwFileCrypted, cKey);
            File.WriteAllText("Passwords.csv", pwFileDeCrypted);

            loadData();
        }

        private void passwTitleTextBox_TextChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            int id = 1;

            var records = new List<Data>();

            using (var reader = new StreamReader(CSV_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
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

                    if (record.Title.Contains(passwTitleTextBox.Text))
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { id.ToString(), record.Title, SimpleAES.AES256.Decrypt(record.Password, cKey), record.URL, record.Comment }));
                        records.Add(record);

                        newPasswTextBox.Enabled = true;
                        toolStripButton2.Enabled = true;
                        toolStripButton3.Enabled = true;

                    }

                    id++;
                }
            }
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            int id = 1;

            listView1.Items.Clear();

            var records = new List<Data>();

            using (var reader = new StreamReader(CSV_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
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

                    if (record.Title == passwTitleTextBox.Text) record.Password = newPasswTextBox.Text;

                    listView1.Items.Add(new ListViewItem(new string[] { id.ToString(), record.Title, SimpleAES.AES256.Decrypt(record.Password, cKey), record.URL, record.Comment }));

                    records.Add(record);
                    id++;
                }
            }
            writeCSV(records);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            int id = 1;

            listView1.Items.Clear();

            var records = new List<Data>();

            using (var reader = new StreamReader(CSV_PATH))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
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

                    if (record.Title != passwTitleTextBox.Text)
                    {
                        records.Add(record);

                        string decryptedPw = SimpleAES.AES256.Decrypt(record.Password, cKey);

                        listView1.Items.Add(new ListViewItem(new string[] { id.ToString(), record.Title, decryptedPw, record.URL, record.Comment }));

                        id++;
                    }
                }
            }
            writeCSV(records);
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Clipboard.SetText(listView1.SelectedItems[0].SubItems[1].Text);
        }
    }
}
