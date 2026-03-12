using IOCServiceCollection.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace IOCServiceCollection
{
    internal class Dog : Animal
    {
        Food food;
        public Dog(Food food)
        {
            this.food = food;
        }

        public override void ShowInfo()
        {
            Console.WriteLine("Dog");
            food.ShowFood();
        }
    }
}
