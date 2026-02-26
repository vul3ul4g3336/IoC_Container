using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Game.Games
{
    internal class LOL :IGame
    {

        public string Name => "LOL";
        public void Render()
        {
            Console.WriteLine("BBBBBBBBBBBBBBBBB");
        }
    }
}
