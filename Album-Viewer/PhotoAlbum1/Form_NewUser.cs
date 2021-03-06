﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace PhotoAlbumViewOfTheGods
{
    /// <summary>
    /// Form to create a new user profile
    /// </summary>
    public partial class Form_NewUser : Form
    {
        private bool _firstRun;
        private string _usersDirectory;
        private List<string> _users;

        /// <summary>
        /// Initialize form
        /// </summary>
        /// <param name="users">List uf current users</param>
        /// <param name="usersDirectory">Directory containing user folders</param>
        /// <param name="isFirstRun">Checks if the user folders has been initialized</param>
        public Form_NewUser(ref List<string> users, string usersDirectory, bool isFirstRun)
        {
            InitializeComponent();
            _users = users;
            _usersDirectory = usersDirectory;
            _firstRun = isFirstRun;
            if (_firstRun)
            {
                this.ControlBox = false;
            }
        }

        /// <summary>
        /// Adds the new profile, checking to see that the name is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_adduser_Click(object sender, EventArgs e)
        {
            string userName = text_username.Text.Trim();
            if (userName == "")
            {
                MessageBox.Show("Please enter a user name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }else if (!Utilities.isValidString(userName))
            {
                MessageBox.Show("User name may only contain spaces, underscores, hyphens,  alphanumeric characters ,and Chinese character.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (_users.Contains(userName, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("User already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (Utilities.checkStringLength(userName, 100))
            {
                MessageBox.Show("User name must be 100 characters or less.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }else{
                Directory.CreateDirectory(_usersDirectory + "\\" + userName);
                _users.Add(userName);
                MessageBox.Show("User '" + userName + "' has been created.", "User Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
        
        /// <summary>
        /// Cancels the creation of a new profile
        /// If it's the first run, inform the user that a profile is required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cancel_Click(object sender, EventArgs e)
        {
            if (_firstRun && MessageBox.Show("You must create a user before you can use this program. Canceling will exit the program. Are you sure you want to quit?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                this.Close();
            }
            else if (text_username.Text.Trim() != "")
            {
                if (MessageBox.Show("Are you sure you want to discard changes?", "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    text_username.Text = "";
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }
    }
}
