using System;
using System.Windows.Forms;
using MBG.Extensions.Data.Sql;
using MBG.Extensions.Data.SqlClient;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using MBG.Extensions.Collections;

namespace SqlDemo
{
    public partial class Form1 : Form
    {
        private const string SQL_CONNECTION_STRING_FORMAT = "Data Source={0};Initial Catalog={1};User={2}Password={3}";
        private const string SQL_CONNECTION_STRING_FORMAT_WA = "Data Source={0};Initial Catalog={1};Integrated Security=true";

        private string ConnectionString
        {
            get
            {
                if (cbUseWindowsAuthentication.Checked)
                {
                    return string.Format(
                        SQL_CONNECTION_STRING_FORMAT_WA,
                        cmbServer.SelectedItem.ToString(),
                        cmbDatabase.SelectedIndex != -1 ? cmbDatabase.SelectedItem.ToString() : "master");
                }
                else
                {
                    return string.Format(
                        SQL_CONNECTION_STRING_FORMAT,
                        cmbServer.SelectedItem.ToString(),
                        cmbDatabase.SelectedIndex != -1 ? cmbDatabase.SelectedItem.ToString() : "master",
                        txtUserName.Text.Trim(),
                        txtPassword.Text);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbServer.Items.AddRange(SqlDataSourceEnumerator.Instance.GetAvailableSqlServers());
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                if (!connection.Validate())
                { return; }

                cmbDatabase.Items.AddRange(connection.GetDatabaseNames());
            }
        }

        private void cmbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDatabase.SelectedIndex != -1)
            {
                lbTables.Items.Clear();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    lbTables.Items.AddRange(connection.GetTableNames());
                }
            }
        }

        private void cbUseWindowsAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            txtUserName.Enabled = txtPassword.Enabled = !cbUseWindowsAuthentication.Checked;
        }

        private void lbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbTables.SelectedIndex != -1)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    dgvForeignKeyInfo.DataSource = connection.GetForeignKeyData(lbTables.SelectedItem.ToString());
                    dgvColumnInfo.DataSource = connection.GetColumnData(lbTables.SelectedItem.ToString());
                }
            }
        }
    }
}