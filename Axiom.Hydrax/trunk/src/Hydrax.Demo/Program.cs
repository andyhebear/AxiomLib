using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrax.Demo
{
    static class Program
    {
        static void Main( string[] args )
        {
            using ( var demo = new Application())
            {
                demo.Run();
            }
        }
    }
}
