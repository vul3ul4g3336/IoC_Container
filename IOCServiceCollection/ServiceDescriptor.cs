using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOCServiceCollection
{
    public class ServiceDescriptor
    {
        public ServiceLifetime Lifetime { get; }

        public Type ServiceType { get; }
        private Type _implementationType;
        public Type ImplementationType
        {
            get
            {
                return _implementationType;
            }
        }

        private object _implementationFactory;
        public Func<ServiceProvider, object> ImplementationFactory
        {
            get
            {
                return (Func<ServiceProvider, object>)_implementationFactory;
            }
        }

        private object _implementationInstance;
        public object ImplementationInstance
        {
            get
            {
                return _implementationInstance;
            }
            set
            {
                _implementationInstance = value;
            }
        }

        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            _implementationType = implementationType;
            Lifetime = lifetime;
            ServiceType = serviceType;
        }

        public ServiceDescriptor(Type serviceType, Func<ServiceProvider, object> factory, ServiceLifetime lifetime)
        {
            _implementationFactory = factory;
            Lifetime = lifetime;
            ServiceType = serviceType;
        }
    }
}
