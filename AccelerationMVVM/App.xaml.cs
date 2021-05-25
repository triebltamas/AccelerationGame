using AccelerationMVVM.Model;
using AccelerationMVVM.Persistence;
using AccelerationMVVM.View;
using AccelerationMVVM.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AccelerationMVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private AccelerationModel _model;
        private AccelerationViewModel _viewModel;
        private MainWindow _view;

        #endregion

        #region Constructors

        /// <summary>
        /// Alkalmazás példányosítása.
        /// </summary>
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);

            
        }

        #endregion

        #region Application event handlers

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // modell létrehozása
            _model = new AccelerationModel(new TextFilePersistence());
            _model.GameOver += new EventHandler(Model_GameOver);
            _model.NewGame();

            // nézemodell létrehozása
            _viewModel = new AccelerationViewModel(_model);
            _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);


            // nézet létrehozása
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.KeyDown += new KeyEventHandler(View_KeyDown);
            _view.Show();
        }

        #endregion

        #region View event handlers

        /// <summary>
        /// Nézet bezárásának eseménykezelője.
        /// </summary>
        private void View_Closing(object sender, CancelEventArgs e)
        {
            Boolean restartTimer = _model.IsTimerEnabled();

            if (MessageBox.Show("Are you sure you want to quit?", "Acceleration Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást

                if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                    _model.StartTimers();
            }
        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left)
            {
                _model.StepLeft();
            }
            if(e.Key == Key.Right)
            {
                _model.StepRight();
            }
            if (e.Key == Key.P)
            {
                _model.TogglePaused();
            }
        }

        #endregion

        #region ViewModel event handlers

        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private void ViewModel_NewGame(object sender, EventArgs e)
        {
            _model.NewGame();
        }

        /// <summary>
        /// Játék betöltésének eseménykezelője.
        /// </summary>
        private void ViewModel_LoadGame(object sender, EventArgs e)
        {
            Boolean restartTimer = _model.IsTimerEnabled();

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog(); // dialógusablak
                openFileDialog.Title = "Loading Acceleration gametable";
                openFileDialog.Filter = "Acceleration gametable|*.stl";
                if (openFileDialog.ShowDialog() == true)
                {
                    // játék betöltése
                    _model.LoadGame(openFileDialog.FileName);
                }
            }
            catch (Persistence.DataException)
            {
                MessageBox.Show("Error when loading game.", "Acceleration Game", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _model.StartTimers();
            
        }

        /// <summary>
        /// Játék mentésének eseménykezelője.
        /// </summary>
        private void ViewModel_SaveGame(object sender, EventArgs e)
        {
            Boolean restartTimer = _model.IsTimerEnabled();

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
                saveFileDialog.Title = "Saving Acceleration gametable";
                saveFileDialog.Filter = "Acceleration gametable|*.stl";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        // játéktábla mentése
                        _model.SaveGame(saveFileDialog.FileName);
                    }
                    catch (Persistence.DataException)
                    {
                        MessageBox.Show("Error when saving game." + Environment.NewLine + "Wrong path, or directory is not writeable.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Unsuccessful save!", "AccelerationGame", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _model.StartTimers();
        }

        /// <summary>
        /// Játékból való kilépés eseménykezelője.
        /// </summary>
        private void ViewModel_ExitGame(object sender, EventArgs e)
        {
            _view.Close(); // ablak bezárása
        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver(object sender, EventArgs e)
        {
            MessageBox.Show("Game over, your fuel ran out! Score: " + _model.GetScore().ToString(),
                                "Acceleration",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
        }

        #endregion
    }
}
