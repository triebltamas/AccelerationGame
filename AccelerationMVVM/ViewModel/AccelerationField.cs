using System;
using System.Collections.Generic;
using System.Text;

namespace AccelerationMVVM.ViewModel
{
    public class AccelerationField : ViewModelBase
    {
        private Int32 _fieldType;

        /// <summary>
        /// Zároltság lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 FieldType
        {
            get { return _fieldType; }
            set
            {
                if (_fieldType != value)
                {
                    _fieldType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Vízszintes koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Függőleges koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 Y { get; set; }
    }
}
