using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.SkyX
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using ( var game = new Game() )
            {
                game.Start();
            }
        }
    }
}
