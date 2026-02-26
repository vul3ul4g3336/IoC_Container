using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Game.Games
{
    public interface IGame
    {
        String Name { get; }
        void Render();
    }
}
