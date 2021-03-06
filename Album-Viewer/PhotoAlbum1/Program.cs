﻿///test git cmd 'git push origin master'



using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace PhotoAlbumViewOfTheGods
{
    static class Program
    {
        
        
        // URL: http://www.ai.uga.edu/mc/SingleInstance.html
        /// <summary>
        /// Photo Album Application that allows the user to create albums, fill them with pictures,
        /// and modify their attributes such as: name and description.
        /// 
        /// Programmed by Cavan Crawford and Zach Gardner, Team 3
        /// Album Viewer: cycle 1
        /// First draft: September 21, 2011
        /// Last Modified: October 11, 2011
        /// No assistance from other students, only various websites for coding method references
        /// Permission to publish
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool anotherInstance;
            Mutex m = new Mutex(true, "PhotoAlbumViewerOfTheGods", out anotherInstance);

            if (!anotherInstance)
            {
                MessageBox.Show("Another instance is already running.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            if (Environment.Version.Major < 4)
            {
                MessageBox.Show(".NET version 4.0 or greater is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form_Main());
            GC.KeepAlive(m); // important!
        }
    }
}
