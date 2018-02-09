using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisksImageEditor
{
    interface ICalculate
    {
        void Recalculate();
        void Calculate();
        event Action<ICalculate> DeleteControl;
        void RemoveVariable(Edge sender);

    }
}
