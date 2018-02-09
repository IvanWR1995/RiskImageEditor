using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace RisksImageEditor
{
    interface ILinked
    {
        void IsLinked( Edge sender, Point Begin, Point End);

    }
}
