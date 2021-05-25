using AccelerationMVVM.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace AccelerationMVVM.Model
{
    public class AccelerationModel
    {
        #region Private fields

        private int _elapsedTime;
        private bool _alreadyStepped;
        private int _score;
        private bool _isPaused;
        private int _speed;
        private int _fuel;
        private int[,] _table;
        private Timer _timer;
        private Timer _sidewaysTimer;
        private IPersistence _persistence;

        #endregion

        #region Public properties

        public int[,] GetTable() { return _table; }
        public int GetScore() { return _score; }
        public int GetSpeed() { return _speed; }
        public int GetFuel() { return _fuel; }
        public bool GetIsPaused() { return _isPaused; }

        #endregion

        #region Events

        public event EventHandler TableChanged;
        public event EventHandler FuelChanged;
        public event EventHandler PauseChanged;
        public event EventHandler GameOver;

        #endregion

        #region Constructors

        public AccelerationModel() : this(null) { }

        public AccelerationModel(IPersistence persistence)
        {
            _isPaused = false;
            _timer = new Timer();
            _sidewaysTimer = new Timer();

            _timer.Elapsed += OnTick;
            _sidewaysTimer.Elapsed += SidewaysTimerOnTick;

            _persistence = persistence;
        }

        #endregion

        #region Public methods

        public void NewGame()
        {
            if (_isPaused)
            {
                TogglePaused();
            }


            _timer.Stop();
            _sidewaysTimer.Stop();

            _timer.Interval = 500;
            _sidewaysTimer.Interval = 500;

            _score = 0;
            _elapsedTime = 0;
            _speed = 500;
            _fuel = 25;
            _alreadyStepped = false;

            _table = new int[13, 7];
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    _table[i, j] = 0;
                }
            }
            _table[12, 3] = 1;
            _timer.Start();
            _sidewaysTimer.Start();


        }
        public void StepLeft()
        {
            for (int i = 1; i < 7 && !_isPaused && !_alreadyStepped; i++)
            {
                if (_table[12, i] == 1)
                {
                    if (_table[12, i - 1] != 2)
                    {
                        _table[12, i] = 0;
                        _table[12, i - 1] = 1;
                    }
                    else
                    {
                        _table[12, i] = 0;
                        _table[12, i - 1] = 1;
                        _fuel = 25;
                        FuelChanged(this, new EventArgs());
                    }
                }
            }
            _alreadyStepped = true;
        }
        public void StepRight()
        {
            for (int i = 0; i < 6 && !_isPaused && !_alreadyStepped; i++)
            {
                if (_table[12, i] == 1)
                {
                    if (_table[12, i + 1] != 2)
                    {
                        _table[12, i] = 0;
                        _table[12, i + 1] = 1;
                        break;
                    }
                    else
                    {
                        _table[12, i] = 0;
                        _table[12, i + 1] = 1;
                        _fuel = 25;
                        FuelChanged(this, new EventArgs());
                    }
                }
            }
            _alreadyStepped = true;
        }
        public void TogglePaused()
        {
            if (_isPaused)
            {
                _timer.Start();
                _sidewaysTimer.Start();
                _isPaused = false;

                PauseChanged(this, new EventArgs());
            }
            else
            {
                _timer.Stop();
                _sidewaysTimer.Stop();
                _isPaused = true;

                PauseChanged(this, new EventArgs());
            }
        }

        public void LoadGame(String path)
        {
            if (_persistence == null)
                return;

            // végrehajtjuk a betöltést
            int[] values = _persistence.Load(path);

            if (values.Length != _table.Length + 5)
                throw new DataException("Error occurred during game loading.");

            // beállítjuk az értékeket
            for (Int32 i = 0; i < _table.GetLength(0); i++)
                for (Int32 j = 0; j < _table.GetLength(1); j++)
                {
                    _table[i, j] = values[i * _table.GetLength(1) + j];
                }

            _elapsedTime = values[_table.Length];
            _score = values[_table.Length + 1];
            _fuel = values[_table.Length + 2];
            _speed = values[_table.Length + 3];

            if (values[_table.Length + 4] == 0)
                _alreadyStepped = true;
            else
                _alreadyStepped = false;

            _timer.Interval = _speed;

            TableChanged(this, new EventArgs());
            FuelChanged(this, new EventArgs());
        }

        public void SaveGame(String path)
        {
            if (_persistence == null)
                return;

            // az értékeket kimásoljuk egy új tömbbe
            int[] values = new int[13 * 7 + 5];
            for (Int32 i = 0; i < _table.GetLength(0); i++)
                for (Int32 j = 0; j < _table.GetLength(1); j++)
                {
                    values[i * _table.GetLength(1) + j] = _table[i, j];
                }

            values[_table.Length] = _elapsedTime;
            values[_table.Length + 1] = _score;
            values[_table.Length + 2] = _fuel;
            values[_table.Length + 3] = _speed;

            if (_alreadyStepped)
                values[_table.Length + 4] = 0;
            else
                values[_table.Length + 4] = 1;

            // végrehajtjuk a mentést
            _persistence.Save(path, values);
        }

        public bool IsTimerEnabled()
        {
            bool isEnabled = _timer.Enabled;
            _timer.Stop();
            _sidewaysTimer.Stop();
            _isPaused = true;

            PauseChanged(this, new EventArgs());

            return isEnabled;
        }

        public void StartTimers()
        {
            _timer.Start();
            _sidewaysTimer.Start();
            _isPaused = false;

            PauseChanged(this, new EventArgs());
        }

        #endregion

        #region Private methods

        private void OnTick(object sender, EventArgs e)
        {
            _score++;
            _fuel--;
            FuelChanged(this, new EventArgs());

            if (_speed >= 2 && _score % 2 == 0)
            {
                _speed = (int)(_speed * 0.95);
                _timer.Interval = _speed;
            }





            for (int i = 12; i >= 0; i--)
            {
                for (int j = 6; j >= 0; j--)
                {
                    if (i < 11 && _table[i, j] == 2)
                    {
                        _table[i, j] = 0;
                        _table[i + 1, j] = 2;
                    }
                    else if (i == 11 && _table[i, j] == 2)
                    {
                        if (_table[i + 1, j] != 1)
                        {
                            _table[i, j] = 0;
                            _table[i + 1, j] = 2;
                        }
                        else
                        {
                            _table[i, j] = 0;
                            _fuel = 25;
                            FuelChanged(this, new EventArgs());
                        }
                    }
                    else if (i == 12 && _table[i, j] == 2)
                    {
                        _table[i, j] = 0;
                    }
                }
            }


            TableChanged(this, new EventArgs());

            if (_fuel == 0)
            {
                _timer.Stop();
                _sidewaysTimer.Stop();
                GameOver(this, new EventArgs());
            }




        }

        private void SidewaysTimerOnTick(object sender, EventArgs e)
        {
            _elapsedTime++;
            _alreadyStepped = false;
            bool fuelOnScreen = false;
            foreach (int x in _table)
            {
                if (x == 2)
                {
                    fuelOnScreen = true;
                    break;
                }
            }

            if (!fuelOnScreen || _elapsedTime % 5 == 0)
            {
                Random random = new Random();
                int fuelIndex = random.Next(0, 7);
                _table[0, fuelIndex] = 2;
            }
            TableChanged(this, new EventArgs());

        }

        #endregion
    }
}
