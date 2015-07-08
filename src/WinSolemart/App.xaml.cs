using Solemart.SystemUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Threading;

namespace WinSolemart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private DispatcherTimer timer = new DispatcherTimer();
        private WebClient client = new WebClient();
        public App()
        {
            ConfigSettings.LoadAppConfig();

            this.DispatcherUnhandledException += MyAppExceptionHandler;

            //timer.Interval = new TimeSpan(0, 1, 0);
            //timer.Tick += GetBdgWeb;
            //timer.Start();
        }

        /// <summary>
        /// Get the web recure.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetBdgWeb(object sender, EventArgs e)
        {
            client.DownloadDataAsync(new Uri("http://www.51bdg.com"));
        }

        private void MyAppExceptionHandler(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
