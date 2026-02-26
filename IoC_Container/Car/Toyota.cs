using IoC_Container.CarFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Car
{
    internal class Toyota : ICar
    {
        public Toyota(IFactory car)
        {

        }
        public void ShowInfo()
        {
            Console.WriteLine("這是一台Toyota的國民車款，便宜好用又耐撞");
        }

        public void Drive()
        {
            Console.WriteLine("發動油門引擎，啟動駕駛!");
        }
    }
}
