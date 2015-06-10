using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Solemart.BusinessLib;
using Solemart.SystemUtil;
using Solemart.DataProvider.Entity;
using WinSolemart.Models;

namespace WinSolemart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            SplashScreen splash = new SplashScreen("SplashScreen.png");
            splash.Show(true);

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
    }
}
