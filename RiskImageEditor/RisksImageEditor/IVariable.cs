using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisksImageEditor
{
    interface IVariable
    {
        event Action EndOfEdit;
        event Action<IVariable> DeleteVariable; 
        double Value { get; }
        
        
    }
}
