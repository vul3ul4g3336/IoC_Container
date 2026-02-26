using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.CarFactory
{
    public class HouseFactory : IFactory
    {
        public void showInfo()
        {
            Console.WriteLine("賣房的");
        }
    }
}
