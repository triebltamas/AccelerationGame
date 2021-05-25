using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AccelerationMVVM.Persistence
{
    public class TextFilePersistence : IPersistence
    {
        public int[] Load(string path)
        {
            if (path == null)
                throw new ArgumentException("path");
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String[] numbers = reader.ReadToEnd().Split();

                    return numbers.Select(number => Int32.Parse(number)).ToArray();
                }
            }
            catch
            {
                throw new DataException("Error occurred during reading.");
            }
        }

        public void Save(string path, int[] values)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (values == null)
                throw new ArgumentNullException("values");

            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    // a mezőket szöveggé konvertáljuk, végül aggregáljuk őket szóközökkel közrezárva
                    writer.Write(values.Select(value => value.ToString()).Aggregate((value1, value2) => value1 + " " + value2));
                }
            }
            catch
            {
                throw new DataException("Error occurred during writing.");
            }
        }
    }
}
