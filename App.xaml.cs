using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GameLibrary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "SteamMadeInChina";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Only one instance of the application can be running at one given time! Closing this instance...");
                Application.Current.Shutdown();
            }

            base.OnStartup(e);
        }
    }
}
