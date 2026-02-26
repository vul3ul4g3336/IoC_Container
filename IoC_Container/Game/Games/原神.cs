using IoC_Container.Game.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Game.Games
{
    public class 原神 :IGame
    {

        public string Name => "原神";
        public void Render()
        {
            Console.WriteLine("CCCCCCCCCCCCCCCC");
        }
    }
}
