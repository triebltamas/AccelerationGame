using System;
using System.Collections.Generic;
using System.Text;

namespace AccelerationMVVM.Persistence
{
    public interface IPersistence
    {
        int[] Load(String path);

        void Save(String path, int[] values);
    }
}
