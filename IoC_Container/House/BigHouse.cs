using IoC_Container.CarFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.House
{
    public class BigHouse : IHouse
    {
        public BigHouse(IFactory house)
        {
            

        }
        public void Live()
        {
            Console.WriteLine("爽啊!刺阿");
        }

        public void ShowInfo()
        {
            Console.WriteLine("這是一棟大房子");
        }
    }
}
