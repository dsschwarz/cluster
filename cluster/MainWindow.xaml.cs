using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace cluster
{
    /// <summary>
    /// Load a web page, parse for links, visit all linked web pages, parse fo links
    /// Create a cluster graph of all the nodes
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClusterGraphService mainService;

        public MainWindow()
        {
            InitializeComponent();
            mainService = new ClusterGraphService();
        }

        private void LoadWebPage(object sender, RoutedEventArgs e)
        {
            var url = Uri.IsWellFormedUriString(urlInput.Text, UriKind.Absolute) ? urlInput.Text : "http://" + urlInput.Text;
            mainService.UserLoadWebPage(url);
//            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
//            {
//                Console.WriteLine("Invalid URI");
//                return;
//            }
//            else
//            {
//                mainService.UserLoadWebPage(url);
//            }
        }
    }
}
