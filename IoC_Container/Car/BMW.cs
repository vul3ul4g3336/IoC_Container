using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Car
{
    internal class BMW : ICar
    {
        public void Drive()
        {
            Console.WriteLine("開BMW");
        }

        public void ShowInfo()
        {
            Console.WriteLine("介紹BMW");
        }
    }
}
