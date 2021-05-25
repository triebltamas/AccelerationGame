using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AccelerationMVVM.Model;
using AccelerationMVVM.Persistence;
using Moq;

namespace AccelerationMVVM.Test
{
    [TestClass]
    public class AccelerationGameModelTest
    {

        private AccelerationModel _model;
        private Mock<IPersistence> _mock;
        int[] _mockedTable;

        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new int[13 * 7 + 5];
            for (int i = 0; i < _mockedTable.Length; i++)
            {
                _mockedTable[i] = 0;
            }
            //placing fuel cells, and motorbike
            //the first 91 elements of the array represents the 13X7 grid of the gametable
            _mockedTable[87] = 1;
            _mockedTable[65] = 2;
            _mockedTable[26] = 2;
            //the last 5 element of the array represents metadata
            _mockedTable[91] = 8;    //_elapsedTime
            _mockedTable[92] = 9;    //_score
            _mockedTable[93] = 16;   //_fuel
            _mockedTable[94] = 406;  //_speed
            _mockedTable[95] = 1;    //_alreadyStepped



            _mock = new Mock<IPersistence>();
            _mock.Setup(mock => mock.Load(It.IsAny<String>()))
                .Returns(() => _mockedTable);

            _model = new AccelerationModel(_mock.Object);

            _model.TableChanged += new EventHandler(Model_TableChanged);
            _model.FuelChanged += new EventHandler(Model_FuelChanged);
            _model.PauseChanged += new EventHandler(Model_PauseChanged);
            _model.GameOver += new EventHandler(Model_GameOver);

        }

        [TestMethod]
        public void AccelerationModelNewGameTest()
        {
            _model.NewGame();
            Assert.AreEqual(500, _model.GetSpeed());
            Assert.AreEqual(25, _model.GetFuel());
            Assert.AreEqual(0, _model.GetScore());
            Assert.AreEqual(1, _model.GetTable()[12, 3]);
        }

        [TestMethod]
        public void AccelerationModelSteppingTest()
        {
            _model.NewGame();


            Assert.AreEqual(1, _model.GetTable()[12, 3]);

            _model.StepRight();

            Assert.AreEqual(1, _model.GetTable()[12, 4]);


            _model.StepLeft();

            Assert.AreEqual(1, _model.GetTable()[12, 4]);

            System.Threading.Thread.Sleep(550);
            _model.StepLeft();

            Assert.AreEqual(1, _model.GetTable()[12, 3]);


        }

        [TestMethod]
        public void AccelerationModelOnTickTest()
        {
            _model.NewGame();

            _model.LoadGame(String.Empty);

            Assert.AreEqual(16, _model.GetFuel());

            Assert.AreEqual(1, _model.GetTable()[12, 3]);
            Assert.AreEqual(2, _model.GetTable()[9, 2]);
            Assert.AreEqual(2, _model.GetTable()[3, 5]);

            System.Threading.Thread.Sleep((int)(_model.GetSpeed() * 1.1));

            Assert.AreEqual(15, _model.GetFuel());

            Assert.AreEqual(1, _model.GetTable()[12, 3]);
            Assert.AreEqual(2, _model.GetTable()[10, 2]);
            Assert.AreEqual(2, _model.GetTable()[4, 5]);

        }

        [TestMethod]
        public void AccelerationModelTogglePauseTest()
        {
            _model.NewGame();
            Assert.AreEqual(25, _model.GetFuel());
            Assert.AreEqual(0, _model.GetScore());

            _model.TogglePaused();

            System.Threading.Thread.Sleep(2 * _model.GetSpeed());
            Assert.AreEqual(25, _model.GetFuel());
            Assert.AreEqual(0, _model.GetScore());

        }

        [TestMethod]
        public void AccelerationModelLoadTest()
        {
            // kezdünk egy új játékot
            _model.NewGame();

            // majd betöltünk egy játékot
            _model.LoadGame(String.Empty);

            for (Int32 i = 0; i < 13; i++)
                for (Int32 j = 0; j < 7; j++)
                {
                    Assert.AreEqual(_mockedTable.GetValue(i * 7 + j), _model.GetTable().GetValue(i, j));
                    // ellenõrizzük, valamennyi mezõ értéke megfelelõ-e
                }

            // a lépésszám 0-ra áll vissza
            Assert.AreEqual(406, _model.GetSpeed());
            Assert.AreEqual(9, _model.GetScore());
            Assert.AreEqual(16, _model.GetFuel());

            // ellenõrizzük, hogy meghívták-e a Load mûveletet a megadott paraméterrel
            _mock.Verify(dataAccess => dataAccess.Load(String.Empty), Times.Once());
        }


        private void Model_TableChanged(Object sender, EventArgs e)
        {

        }
        private void Model_FuelChanged(Object sender, EventArgs e)
        {

        }
        private void Model_PauseChanged(Object sender, EventArgs e)
        {

        }
        private void Model_GameOver(Object sender, EventArgs e)
        {

        }
    }
}
