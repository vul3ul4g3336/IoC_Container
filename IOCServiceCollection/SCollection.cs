using IOCServiceCollection.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IOCServiceCollection
{
    public class SCollection : IServiceCollection
    {
        private bool _isReadOnly = false;
        private Type[] attrArr = { typeof(TransientAttribute), typeof(ScopeAttribute), typeof(SingletonAttribute) };
        public Dictionary<Type, List<ServiceDescriptor>> dictiontry = new Dictionary<Type, List<ServiceDescriptor>>();


        public int Count => dictiontry.Count;

        public bool IsReadOnly => _isReadOnly;

        public Microsoft.Extensions.DependencyInjection.ServiceDescriptor this[int index] { get => GetServiceDescriptorByIndex(index); set => throw new NotImplementedException(); }

        private ServiceDescriptor GetServiceDescriptorByIndex(int index)
        {
            int current = 0;
            ServiceDescriptor descriptor = null;
            foreach (var dict in dictiontry)
            {
                if (current == index)
                {
                    descriptor = dict.Value.Last();
                    break;
                }
                current++;
            }
            return descriptor;
        }

        public IServiceCollection AddSingleton<Ttype>()
        {
            return AddSingleton(typeof(Ttype), typeof(Ttype));
        }

        public IServiceCollection AddSingleton<Ttype, TService>()
        {
            return AddSingleton(typeof(Ttype), typeof(TService));
        }

        public IServiceCollection AddSingleton(Type serviceType, Type implementationType)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton);
            Add(descriptor);
            return this;
        }

        public IServiceCollection AddSingleton(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationFactory, ServiceLifetime.Singleton);
            Add(descriptor);
            return this;
        }

        public IServiceCollection AddScoped<Ttype, TService>()
        {
            return AddScoped(typeof(Ttype), typeof(TService));
        }

        public IServiceCollection AddScoped(Type serviceType, Type implementationType)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Scoped);
            Add(descriptor);
            return this;
        }

        public IServiceCollection AddScoped(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationFactory, ServiceLifetime.Scoped);
            Add(descriptor);
            return this;
        }

        public IServiceCollection AddTransient<Ttype, TService>()
        {
            return AddTransient(typeof(Ttype), typeof(TService));
        }

        public IServiceCollection AddTransient(Type serviceType, Type implementationType)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Transient);
            Add(descriptor);
            return this;
        }

        public IServiceCollection AddTransient(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationFactory, ServiceLifetime.Transient);
            Add(descriptor);
            return this;
        }

        public IServiceCollection AddTransient<Ttype>(Func<IServiceProvider, object> implementationFactory)
        {
            var descriptor = new ServiceDescriptor(typeof(Ttype), implementationFactory, ServiceLifetime.Transient);
            Add(descriptor);
            return this;
        }

        public SProvider BuildServiceProvider()
        {
            SProvider serviceProvider = InitinalServiceProvider();
            return serviceProvider;
        }

        private SProvider InitinalServiceProvider()
        {
            SProvider serviceProvider = new SProvider(this);

            // 要做到自動註冊PresenterFactory，但因為 PresenterFactory 需要有一個 serviceProvider
            // 因此在dictiontry容器先加入一個 serviceProvider

            List<ServiceDescriptor> descriptors = new List<ServiceDescriptor>();
            descriptors.Add(ServiceDescriptor.Singleton(typeof(SProvider), serviceProvider));

            dictiontry.Add(typeof(SProvider), descriptors);
            AddSingleton<PresenterFactory, PresenterFactory>();
            return serviceProvider;
        }

        public void AutoRegister(Assembly assembly)
        {
            // 必須標記 Attribute 是哪一種才能加進collection
            List<TypeInfo> definedtypes = assembly.DefinedTypes.Where(definedtype => definedtype.CustomAttributes.Any(attr => attrArr.Contains(attr.AttributeType)))
                                                   .ToList();
            RegistorTypes(definedtypes);
        }

        public void AutoRegisterMVP(Assembly assembly)
        {
            List<TypeInfo> definedtypes = assembly.DefinedTypes.ToList();
            RegistorTypes(definedtypes);
        }

        private void RegistorTypes(List<TypeInfo> definedtypes)
        {
            //1.優先找繼承的父類別 (因為物件導向不允許多重繼承)
            //2.如果沒有繼承，改找interface => 因為他是ImplementedInterfaces
            //  2-1 預設先找跟她同名的內容 用contains比對名子去找
            //  2-2 如果沒有，則找第一個intetface
            //3.都沒有就把自己當父類別
            foreach (TypeInfo implementationType in definedtypes)
            {
                Type serviceType = implementationType;

                if (implementationType.ImplementedInterfaces.Count() > 0)
                    serviceType = implementationType.ImplementedInterfaces.FirstOrDefault(x => x.Name.EndsWith(implementationType.Name)) ?? implementationType.ImplementedInterfaces.First();

                else if (implementationType.BaseType != null && implementationType.BaseType != typeof(object))
                    serviceType = implementationType.BaseType;

                ServiceLifetime serviceLifetime = implementationType.CustomAttributes.Any(x => x.AttributeType == typeof(SingletonAttribute)) ? ServiceLifetime.Singleton : ServiceLifetime.Transient;
                ServiceDescriptor descriptor = new ServiceDescriptor(serviceType, implementationType, serviceLifetime);
                Add(descriptor);
            }
        }
        public int IndexOf(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(ServiceDescriptor item)
        {
            if (!dictiontry.ContainsKey(item.ServiceType))
            {
                List<ServiceDescriptor> descriptorList = new List<ServiceDescriptor>();
                descriptorList.Add(item);
                dictiontry.Add(item.ServiceType, descriptorList);
            }
            else
            {
                // 如果有存這個的 ServiceType，取出 List 加上 這個 ServiceDescriptor
                dictiontry[item.ServiceType].Add(item);
            }

        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Microsoft.Extensions.DependencyInjection.ServiceDescriptor[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Microsoft.Extensions.DependencyInjection.ServiceDescriptor> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
