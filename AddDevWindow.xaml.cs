using GameLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameLibrary
{
    /// <summary>
    /// Interaction logic for AddDevWindow.xaml
    /// </summary>
    public partial class AddDevWindow : Window
    {
        public AddDevWindow()
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Reset();
        }

        public void Reset()
        {
            textbox_Name.BorderBrush = Brushes.GhostWhite;
            textbox_Name.Text = "";
        }

        public bool ValidationCheck()
        {
            textbox_Name.BorderBrush = Brushes.White;
            if (textbox_Name.Text == "") 
            {
                MessageBox.Show("Name field left empty!");
                textbox_Name.BorderBrush = Brushes.Red;
                return false;
            }

            foreach (Developer dev in MainWindow.Context.Developers)
            { 
                if (dev.Name == textbox_Name.Text)
                {
                    MessageBox.Show("Developer name already taken!");
                    textbox_Name.BorderBrush = Brushes.Red;
                    return false;
                }
            }
            return true;
        }

        private void Onbutton_AddDeveloperClicked(object sender, RoutedEventArgs e)
        {
            if (ValidationCheck())
            {
                MainWindow.Context.Developers.Add(new Developer(textbox_Name.Text));
                MainWindow.Context.SaveChanges();
                this.Close();
            }
        }

        private void Onbutton_CloseClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.DragMove();
        }
    }
}
