using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOCServiceCollection.Test
{
    internal class DogFood : Food
    {
        public override void ShowFood()
        {
            Console.WriteLine("DogFood");
        }
    }
}
