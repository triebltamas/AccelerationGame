using AccelerationMVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace AccelerationMVVM.ViewModel
{
    public class AccelerationViewModel : ViewModelBase
    {
        #region Fields
        private AccelerationModel _model;

        #endregion

        #region Properties

        /// <summary>
        /// Új játék kezdése parancs lekérdezése.
        /// </summary>
        public DelegateCommand NewGameCommand { get; private set; }

        /// <summary>
        /// Játék betöltése parancs lekérdezése.
        /// </summary>
        public DelegateCommand LoadGameCommand { get; private set; }

        /// <summary>
        /// Játék mentése parancs lekérdezése.
        /// </summary>
        public DelegateCommand SaveGameCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand ExitCommand { get; private set; }

        /// <summary>
        /// Játékmező gyűjtemény lekérdezése.
        /// </summary>
        public ObservableCollection<AccelerationField> Fields { get; set; }

        /// <summary>
        /// Score értékének lekérdezése.
        /// </summary>
        public Int32 GameScoreCount { get { return _model.GetScore(); } }

        /// <summary>
        /// Fennmaradt Fuel lekérdezése.
        /// </summary>
        public Int32 FuelCounter { get { return _model.GetFuel(); } }

        /// <summary>
        /// A Játék folyásának lekérdezése.
        /// </summary>
        public String IsPaused { get { return _model.GetIsPaused()?"Paused":"Playing"; } }
        
        #endregion

        #region Events

        /// <summary>
        /// Új játék eseménye.
        /// </summary>
        public event EventHandler NewGame;

        /// <summary>
        /// Játék betöltésének eseménye.
        /// </summary>
        public event EventHandler LoadGame;

        /// <summary>
        /// Játék mentésének eseménye.
        /// </summary>
        public event EventHandler SaveGame;

        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler ExitGame;

        #endregion

        #region Constructors

        /// <summary>
        /// A nézetmodell példányosítása.
        /// </summary>
        /// <param name="model">A modell típusa.</param>
        public AccelerationViewModel(AccelerationModel model)
        {
            // játék csatlakoztatása
            _model = model;
            _model.GameOver += new EventHandler(Model_GameOver);
            _model.FuelChanged += new EventHandler(Model_FuelChanged);
            _model.PauseChanged += new EventHandler(Model_PauseChanged);
            _model.TableChanged += new EventHandler(Model_TableChanged);

            // parancsok kezelése
            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());

            // játéktábla létrehozása
            Fields = new ObservableCollection<AccelerationField>();
            for (Int32 i = 0; i < 13; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < 7; j++)
                {
                    Fields.Add(new AccelerationField
                    {
                        FieldType = 0,
                        X = i,
                        Y = j,
                    });
                }
            }


            RefreshTable();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Tábla frissítése.
        /// </summary>
        private void RefreshTable()
        {
            foreach (AccelerationField field in Fields) // inicializálni kell a mezőket is
            {
                field.FieldType = _model.GetTable()[field.X,field.Y];
            }

            OnPropertyChanged("GameTable");
        }

        #endregion

        #region Game event handlers

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver(object sender, EventArgs e)
        {
            NewGame(this, EventArgs.Empty);
        }

        /// <summary>
        /// Az üzemanyag változásának eseménykezelője.
        /// </summary>
        private void Model_FuelChanged(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke( new Action(() => { 
                OnPropertyChanged("FuelCounter");
                OnPropertyChanged("GameScoreCount");
            }));
        }

        /// <summary>
        /// A játék folyásának eseménykezelője.
        /// </summary>
        private void Model_PauseChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("IsPaused");
        }


        /// <summary>
        /// A játéktábla változásának eseménykezelője.
        /// </summary>
        private void Model_TableChanged(object sender, EventArgs e)
        {
            RefreshTable();
        }


        #endregion

        #region Event methods

        /// <summary>
        /// Új játék indításának eseménykiváltása.
        /// </summary>
        private void OnNewGame()
        {
            if (NewGame != null)
                NewGame(this, EventArgs.Empty);
        }



        /// <summary>
        /// Játék betöltése eseménykiváltása.
        /// </summary>
        private void OnLoadGame()
        {
            if (LoadGame != null)
                LoadGame(this, EventArgs.Empty);
        }

        /// <summary>
        /// Játék mentése eseménykiváltása.
        /// </summary>
        private void OnSaveGame()
        {
            if (SaveGame != null)
                SaveGame(this, EventArgs.Empty);
        }

        /// <summary>
        /// Játékból való kilépés eseménykiváltása.
        /// </summary>
        private void OnExitGame()
        {
            if (ExitGame != null)
                ExitGame(this, EventArgs.Empty);
        }

        #endregion
    }
}
