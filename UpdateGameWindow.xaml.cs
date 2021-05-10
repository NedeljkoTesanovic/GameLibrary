using GameLibrary.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
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
    /// Interaction logic for UpdateGameWindow.xaml
    /// </summary>
    public partial class UpdateGameWindow : Window
    {
        public GameUnit NewGame = new GameUnit();
        public GameUnit OldGame = new GameUnit();
        public UpdateGameWindow(GameUnit SelectedGame)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();

            NewGame = SelectedGame;
            OldGame = new GameUnit(SelectedGame);
            this.DataContext = NewGame;
            this.combobox_Developer.ItemsSource = MainWindow.Context.Developers.ToList();


            Reset(OldGame);
        }

        public void Reset(GameUnit SelectedGame)
        {
            ResetColours();
            textbox_Title.Text = SelectedGame.Title;
            textbox_Description.Text = SelectedGame.Description;
            textbox_Rating.Text = SelectedGame.Rating.ToString();
            textbox_MaxPlayers.Text = SelectedGame.MaxPlayers.ToString();
            textbox_TargetExecutable.Text = SelectedGame.TargetExe;
            if (SelectedGame.PlayMode == PlayMode.Both)
            {
                checkbox_Singleplayer.IsChecked = true;
                checkbox_Multiplayer.IsChecked = true;
            }
            else if (SelectedGame.PlayMode == PlayMode.Multiplayer)
            {
                checkbox_Singleplayer.IsChecked = false;
                checkbox_Multiplayer.IsChecked = true;
            }                
            else
            {
                checkbox_Singleplayer.IsChecked = true;
                checkbox_Multiplayer.IsChecked = false;
            }
            checkbox_Action.IsChecked = SelectedGame.Genres.Contains(Genre.Action) ? true:false;
            checkbox_Adventure.IsChecked = SelectedGame.Genres.Contains(Genre.Adventure) ? true : false;
            checkbox_RPG.IsChecked = SelectedGame.Genres.Contains(Genre.RPG) ? true : false;
            checkbox_Simulation.IsChecked = SelectedGame.Genres.Contains(Genre.Simulation) ? true : false;
            checkbox_Sport.IsChecked = SelectedGame.Genres.Contains(Genre.Sport) ? true : false;
            checkbox_Strategy.IsChecked = SelectedGame.Genres.Contains(Genre.Strategy) ? true : false;
            combobox_Developer.SelectedItem = SelectedGame.Developer;
            if (SelectedGame.WebpageURL == "")
            {
                checkbox_Webpage.IsChecked = false;
                textbox_Webpage.Text = "";
                textbox_Webpage.IsEnabled = false;
            }
            else
            {
                checkbox_Webpage.IsChecked = true;
                textbox_Webpage.Text = SelectedGame.WebpageURL;
                textbox_Webpage.IsEnabled = true;
            }

            datepicker_ReleaseDate.SelectedDate = SelectedGame.ReleaseDate;
        }
        public void ResetColours()
        {
            textbox_Title.BorderBrush = Brushes.GhostWhite;
            textbox_Description.BorderBrush = Brushes.GhostWhite;
            textbox_Rating.BorderBrush = Brushes.GhostWhite;
            textbox_MaxPlayers.BorderBrush = Brushes.GhostWhite;
            checkbox_Singleplayer.BorderBrush = Brushes.GhostWhite;
            checkbox_Multiplayer.BorderBrush = Brushes.GhostWhite;
            stackPanel_Genres.Background = Brushes.GhostWhite;
            combobox_Developer.BorderBrush = Brushes.GhostWhite;
            textbox_Webpage.BorderBrush = Brushes.GhostWhite;
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.DragMove();
        }

        private void OnQuitButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private List<Genre> PackGenres()
        {
            List<Genre> genres = new List<Genre>();
            if (checkbox_Action.IsChecked == true)
                genres.Add(Genre.Action);
            if (checkbox_Adventure.IsChecked == true)
                genres.Add(Genre.Adventure);
            if (checkbox_RPG.IsChecked == true)
                genres.Add(Genre.RPG);
            if (checkbox_Simulation.IsChecked == true)
                genres.Add(Genre.Simulation);
            if (checkbox_Sport.IsChecked == true)
                genres.Add(Genre.Sport);
            if (checkbox_Strategy.IsChecked == true)
                genres.Add(Genre.Strategy);
            return genres;
        }

        private PlayMode PackPlayMode()
        {
            if (checkbox_Multiplayer.IsChecked == true && checkbox_Singleplayer.IsChecked == true)
                return PlayMode.Both;
            if (checkbox_Singleplayer.IsChecked == true)
                return PlayMode.Singleplayer;
            return PlayMode.Multiplayer;
        }

        private bool ValidationCheck()
        {
            ResetColours();
            string msg = "";
            if (textbox_Title.Text == "")
            {
                textbox_Title.BorderBrush = Brushes.Red;
                msg = "Title field left empty!";
            }
            if (textbox_Rating.Text == "" || Double.Parse(textbox_Rating.Text) > 5 || Double.Parse(textbox_Rating.Text) < 1)
            {
                textbox_Rating.BorderBrush = Brushes.Red;
                msg += "\nRating field requires numerical (type double) input within [1.0-5.0] range!";
            }
            if (textbox_MaxPlayers.Text.Length == 0 || textbox_MaxPlayers.Text.Any(c => c < '0' || c > '9') || textbox_MaxPlayers.Text == "0")
            {
                textbox_MaxPlayers.BorderBrush = Brushes.Red;
                msg += "\nMaxPlayers requires a positive integer number as input!";
            } else if (textbox_MaxPlayers.Text=="1" && checkbox_Singleplayer.IsChecked == false)
            {
                textbox_MaxPlayers.BorderBrush = Brushes.Red;
                checkbox_Multiplayer.BorderBrush = Brushes.Red;
                msg += "\nMultiplayer implies more than 1 player!";
            }
            if (checkbox_Singleplayer.IsChecked == false && checkbox_Multiplayer.IsChecked == false)
            {
                checkbox_Singleplayer.BorderBrush = Brushes.Red;
                msg += "\nAt least one play mode box must be checked!";
            }
            if ((checkbox_Action.IsChecked == false) && (checkbox_Adventure.IsChecked == false) && (checkbox_RPG.IsChecked == false) && (checkbox_Simulation.IsChecked == false) && (checkbox_Sport.IsChecked == false) && (checkbox_Strategy.IsChecked == false))
            {
                stackPanel_Genres.Background = Brushes.Red;
                msg += "\nAt least one genre box must be checked!";
            }
            if (combobox_Developer.SelectedItem == null)
            {
                combobox_Developer.BorderBrush = Brushes.Red;
                msg += "\nNo Developer selected!";
            }
            if (checkbox_Webpage.IsChecked == true)
            {
                Uri uriResult;
                bool result = Uri.TryCreate(textbox_Webpage.Text, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (!result)
                {
                    textbox_Webpage.BorderBrush = Brushes.Red;
                    msg += "\nInvalid trailer URL!\nAccepted format: https://www.abcdefg.com \n(Make sure https:// is not omitted)";
                }
            }

            if (msg == "")
                return true;

            MessageBox.Show(msg);
            return false;

        }

        private void OnUpdateButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ValidationCheck())
            {
                OldGame = MainWindow.Context.Games.Find(OldGame.Id);
                {
                    NewGame.Genres = PackGenres();
                    NewGame.PlayMode = PackPlayMode();

                    OldGame = MainWindow.Context.Games.Find(OldGame.Id);
                    OldGame = NewGame;
                    MainWindow.Context.SaveChanges();
                    this.Close();
                }
            }
        }
        private void OnAddTargetButtonClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Executables (*.exe)|*.exe"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.CheckPathExists)
                textbox_TargetExecutable.Text = openFileDialog.FileName;
            NewGame.TargetExe = openFileDialog.FileName; //posvadjao sam se sa ovim bindingom pa cu da dodam ovako rucno
        }
        private void OnResetButtonClicked(object sender, RoutedEventArgs e)
        {
            Reset(OldGame);
        }

        private void Checkbox_Webpage_Checked(object sender, RoutedEventArgs e)
        {
            textbox_Webpage.IsEnabled = true;
        }
        private void Checkbox_Webpage_Unchecked(object sender, RoutedEventArgs e)
        {
            textbox_Webpage.IsEnabled = false;
        }
    }
}
