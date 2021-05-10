using GameLibrary.Databases;
using GameLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace GameLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static GameListContext Context { get; set; }
        public GameUnit SelectedGame { get; set; }
        public Developer SelectedDeveloper { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Reset();
            
            if (Context == null)
            {
                Context = new GameListContext();
            }

            Context.Games.Load();
            gameList.ItemsSource = Context.Games.Local;
            
            Context.Developers.Load();
            developersList.ItemsSource = Context.Developers.Local;

            this.DataContext = this;

            if (Context.Games.Count() == 0) //for testing purposes
            {
                Context.Developers.Add(new Developer("1C Studios"));
                Context.Developers.Add(new Developer("Bohemia Interactive"));
                Context.Developers.Add(new Developer("Paradox Interactive"));
                Context.Developers.Add(new Developer("Anomaly Developers"));
                Context.Games.Add(new GameUnit("IL-2 Sturmovik: 1946", "Old, but gold! An epic WW2 combat flight-simulator. Even has the Yugoslav Rogozarski IK-3, how cool is that?!", "", "", new List<Genre> { Genre.Action, Genre.Simulation }, PlayMode.Both, 64, 4.7, DateTime.Parse("08/12/2006"), 1));
                Context.Games.Add(new GameUnit("IL-2 Sturmovik: Battle of Stalingrad", "Third generation entry in the IL-2 Sturmovik franchise, bringing a new engine with impressive details and unrivaled simulation models!", @"https://www.youtube.com/watch?v=9EttVIq9Pig", "", new List<Genre> { Genre.Action, Genre.Simulation }, PlayMode.Both, 128, 4.9, DateTime.Parse("19.11.2013"), 1));
                Context.Games.Add(new GameUnit("Arma: Cold War Assault", "Originally released as \"Operation Flashpoint: Cold War Crisis\" before Codemasters and Bohemia Interactive split, this is a hardcore military simulator with thrilling action across vast battlefields.", "", "", new List<Genre> { Genre.Action, Genre.Simulation }, PlayMode.Both, 64, 5, DateTime.Parse("22.06.2001"), 2));
                Context.Games.Add(new GameUnit("Hearts of Iron 4", "Glavni razlog zasto ovu aplikaciju nisam zavrsio prosle sedmice", "", "", new List<Genre> { Genre.Strategy }, PlayMode.Both, 32, 5, DateTime.Parse("06.06.2016"), 3));
                Context.Games.Add(new GameUnit("S.T.A.L.K.E.R: Anomaly", "The best STALKER mod from the community, for the community - combines the maps from all three games into a single map allowing you to lose yourself to the charms of the Chernobyl Exclusion Zone", "", "", new List<Genre> { Genre.Action, Genre.Adventure }, PlayMode.Singleplayer, 1, 5, DateTime.Now, 4));
                Context.SaveChanges();
            }
        }

        private void Reset()
        {
            checkbox_MinimizeOnGameLaunch.IsChecked = false;
            radioButton_Compact.IsChecked = true;
        }

        private void OnMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
                this.DragMove();
        }

        private void OnQuitButtonClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            AddGameWindow ag_window = new AddGameWindow();
            ag_window.ShowDialog();
        }

        private void OnDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            GameUnit gu = (GameUnit)gameList.CurrentCell.Item;
            MessageBoxResult res = MessageBox.Show($"Are you sure you want to delete {gu.Title}","Confirm action", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                MainWindow.Context.Games.Remove(gu);
                MainWindow.Context.SaveChanges();
            }
        }
        private void OnDetailsButtonClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(gameList.CurrentCell.Item.ToString());
        }
        private void OnModifyButtonClicked(object sender, RoutedEventArgs e)
        {
            UpdateGameWindow ug_window = new UpdateGameWindow((GameUnit)gameList.CurrentCell.Item);
            ug_window.ShowDialog();
            gameList.Items.Refresh();
        }

        private void OnLaunchButtonClicked(object sender, RoutedEventArgs e)
        {
            string target = ((GameUnit)gameList.CurrentCell.Item).TargetExe;
            if (target == "")
                    MessageBox.Show("No executable target for this game has been selected! Select it by modifying the game!");
                else
                { try
                    {
                        Process.Start(target);
                    if (checkbox_MinimizeOnGameLaunch.IsChecked == true)
                        Application.Current.MainWindow.WindowState = WindowState.Minimized;
                    GameUnit gu = (GameUnit)gameList.CurrentCell.Item;
                    gu.LastPlayed = DateTime.Now;
                    gameList.Items.Refresh();
                    } catch (Exception ex)
                    {
                        MessageBox.Show("Houston, we have a problem! Perhaps the target has been deleted?");
                    MessageBox.Show(ex.StackTrace.ToString());
                    MessageBox.Show(target);
                    }   
                }
        }
        private void OnWebButtonClicked(object sender, RoutedEventArgs e)
        {
            string target = ((GameUnit)gameList.CurrentCell.Item).WebpageURL;
            if (target == "")
                MessageBox.Show("No webpage was set for this game has been selected! Select it by modifying the game!");
            else
            {
                try
                {
                    Process.Start("chrome.exe", target);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Houston, we have a problem! Perhaps chrome has been deleted?");
                    MessageBox.Show(ex.StackTrace.ToString());
                    MessageBox.Show(target);
                }
            }
        }

        private void OnDeleteDevButtonClicked(object sender, RoutedEventArgs e)
        {
            Developer dev = (Developer)developersList.CurrentCell.Item;
            MessageBoxResult res = MessageBox.Show($"Are you sure you want to delete {dev.Name}?\nIf their games are in the database, worldwide riots might break out, and we will descent into anarchy - Continue?", "Confirm action", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                MainWindow.Context.Developers.Remove(dev);
                MainWindow.Context.SaveChanges();
            }
        }
        private void OnDetailsDevButtonClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(developersList.CurrentCell.Item.ToString());
        }
        private void OnModifyDevButtonClicked(object sender, RoutedEventArgs e)
        {
            UpdateDevWindow ud_window = new UpdateDevWindow((Developer)developersList.CurrentCell.Item);
            ud_window.ShowDialog();
        }

        private void ChangeViewFull(object sender, RoutedEventArgs e)
        {
            for (int i = 2; i <gameList.Columns.Count()-1; i++)
            {
                gameList.Columns[i].Visibility = Visibility.Visible;
            }
        }
        private void ChangeViewCompact(object sender, RoutedEventArgs e)
        {
            for(int i = 2; i<gameList.Columns.Count()-1; i++)
            {
                gameList.Columns[i].Visibility = Visibility.Collapsed;
            }
        }

        private void OnAddDeveloperButtonClicked (object sender, RoutedEventArgs e)
        {
            AddDevWindow ad_window = new AddDevWindow();
            ad_window.ShowDialog();
        }

        private void WindowClosing(Object sender, CancelEventArgs e)
        {
            Context.Dispose();
        }
    }
}
