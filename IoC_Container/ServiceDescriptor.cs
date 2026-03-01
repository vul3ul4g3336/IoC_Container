using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class ServiceDescriptor
    {
        public Type ServiceType;
        public Type type { get; set; }
        public ServiceLifetime serviceLifetime { get; set; }
        public object _singletonInstance;
        public Func<object> func { get; set; }


    }
}
