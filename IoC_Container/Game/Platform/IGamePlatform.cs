using IoC_Container.Game.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Game
{
    public abstract class IGamePlatform<TGame> where TGame : class,IGame
    {
        protected TGame game;
        public IGamePlatform(TGame game)
        {
            this.game = game;
        }
        public abstract void Launch();
    }
}
