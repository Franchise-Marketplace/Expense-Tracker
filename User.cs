﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Expense_Tracker
{
    internal class User
    {
        private string email;
        private string password;
        private string name;
        private Guid id;

        public User(string name, string email, string password, Guid id)
        {
            this.email = email;
            this.password = password;
            this.name = name;
            this.id = id;

        }
        public User(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
        public (bool, string, string, string, string) Login()
        {
            SqlConnection DB = new DBConnection().openConnection();
            SqlDataReader reader = null;

            try
            {
                string query = "SELECT * FROM [User] WHERE email = @Email AND password = @Password";
                SqlCommand cmd = new SqlCommand(query, DB);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                reader = cmd.ExecuteReader();

                if (reader != null && reader.Read())
                {
                    string userEmail = reader["email"].ToString();
                    string userName = reader["name"].ToString();
                    string userPassword = reader["password"].ToString();
                    string userId = reader["userId"].ToString();
                    return (true, userEmail, userName, userPassword, userId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (DB.State == ConnectionState.Open)
                    DB.Close();
            }

            return (false, null, null, null, null);
        }

        public bool register()
        {

            DBConnection DB = new DBConnection();
            SqlConnection conn = DB.openConnection();

            try
            {
                string query = "INSERT INTO [User] (userId,name,email,password) VALUES (@userId,@name,@email,@password)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);
                int row = cmd.ExecuteNonQuery();

                if (row > 0)
                {
                    return true;

                }

            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message, "Database connection errro", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK) { Environment.Exit(0); }
            }
            return false;

        }
        public bool Unregister()
        {
            string userId = getUserId();

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("User ID is empty. Cannot unregister.");
                return false;
            }

            SqlConnection DB = new DBConnection().openConnection();

            try
            {

                string deleteQuery = "DELETE FROM [User] WHERE userId = @userId";
                SqlCommand cmd = new SqlCommand(deleteQuery, DB);
                cmd.Parameters.AddWithValue("@userId", userId);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    MessageBox.Show("User unregistered successfully.");
                    return true;
                }
                else
                {
                    MessageBox.Show("No user found with the given ID.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (DB.State == ConnectionState.Open)
                    DB.Close();
            }
        }


        public string getUserId()
        {
            if (id == Guid.Empty)
            {
                return id.ToString();
            }
            else
            {
                SqlConnection DB = new DBConnection().openConnection();
                SqlDataReader reader = null;

                try
                {
                    string query = "select userId from [User] where email = @email";
                    SqlCommand cmd = new SqlCommand(query, DB);
                    cmd.Parameters.AddWithValue("@email", email);
                    reader = cmd.ExecuteReader();

                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            return reader["userId"].ToString();

                        }
                    }
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(ex.Message, "Database connection errro", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK) { Environment.Exit(0); }
                }

                return "";

            }
            
        }
    }
}
