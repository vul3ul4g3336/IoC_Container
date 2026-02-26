using IoC_Container.CarFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Car
{
    public class Tesla : ICar
    {
        public Tesla(IFactory car)
        {

        }
        public void ShowInfo()
        {
            Console.WriteLine("這是一台Tesla的自動駕駛電動車車");
        }

        public void Drive()
        {
            Console.WriteLine("發動電門引擎，啟動自動駕駛!");
        }
    }
}
