using IoC_Container.Game.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Game.Platform
{
    internal class PS5<TGame> : IGamePlatform<TGame> where TGame : class,IGame
    {
        public PS5(TGame game) : base(game)
        {
        }


        public override void Launch()
        {
            Console.WriteLine($"正在啟動PS5遊戲:{game.Name}");
        }
    }
}
