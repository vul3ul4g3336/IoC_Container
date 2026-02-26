using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Game.Games
{
    internal class GTA :IGame
    {
        public string Name => "GTA";

        public void Render()
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAA");
        }
    }
}
