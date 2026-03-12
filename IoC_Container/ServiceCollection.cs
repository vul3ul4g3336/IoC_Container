using IoC_Container.Game;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class ServiceCollection : IServiceCollection
    {
        public Dictionary<Type, List<ServiceDescriptor>> dicts = new Dictionary<Type, List<ServiceDescriptor>>(); // IXXX , XXX        

        public int Count => dicts.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public Microsoft.Extensions.DependencyInjection.ServiceDescriptor this[int index]
        {
            get  {
                int num = 0;
                foreach (var dict in dicts)
                {
                    if (index == num)
                    {
                        return dict.Value.Last();
                    }
                    num++;
                }
                return null;
            }
            set => throw new NotImplementedException();
        }

        public IServiceCollection AddTransient<T, T1>() where T1 : T
        {
            AddTransient(typeof(T), typeof(T1));
            return this;
        }
        public IServiceCollection AddSingleton<T, T1>() where T1 : T
        {
            AddSingleton(typeof(T), typeof(T1));
            return this;
        }
        public IServiceCollection AddTransient<T>(Func<System.IServiceProvider, T> func) where T : class
        {
            if (!dicts.ContainsKey(typeof(T)))
            {
                dicts[typeof(T)] = new List<ServiceDescriptor>();
            }
            dicts[typeof(T)].Add(new ServiceDescriptor(typeof(T), func, ServiceLifetime.Transient));
            return this;
        }
        public IServiceCollection AddTransient<T>()
        {
            AddTransient(typeof(T), typeof(T));
            return this;
        }
        public IServiceCollection AddSingleton<T>()
        {
            AddSingleton(typeof(T), typeof(T));
            return this;
        }
        public IServiceCollection AddTransient(Type T, Type T1)
        {

            if (!dicts.ContainsKey(T))
            {
                dicts[T] = new List<ServiceDescriptor>();
            }
            dicts[T].Add(new ServiceDescriptor(T, T1, ServiceLifetime.Transient));
            return this;
        }
        public IServiceCollection AddSingleton(Type T, Type T1)
        {
            if (!dicts.ContainsKey(T))
            {
                dicts[T] = new List<ServiceDescriptor>();
            }
            dicts[T].Add(new ServiceDescriptor(T, T1, ServiceLifetime.Singleton));
            return this;
        }
        //1.GetService: 對外呼叫的方法,並且用來分類是 Type/GenericType/Enumerable 來做分流
        //2.GetImplemetionInstance: 根據GetService給予的不同類型,統一類型從Dictionary中取出ServiceDescriptor 
        //3.CreateInstance: 從ServiceDescriptor中找出對應的Type並且滿足該類型的所有建構元參數,並建構她。 其中建構元的每一個參數預設都應該會存在在容器中,所以需要返回第一步
        public int IndexOf(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            int num = 0;
            foreach (var dict in dicts)
            {
                if (dict.Value.Contains(item))
                {
                    break;
                }
                num++;
            }
            return num;
        }
        public IServiceProvider BuildServiceProvider()
        {
            IServiceProvider serviceProvider = new ServiceProvider(this);
            return serviceProvider;
        }
        public void Insert(int index, Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            if (!dicts.ContainsKey(item.ServiceType))
            {
                dicts[item.ServiceType] = new List<ServiceDescriptor>();
            }
            dicts[item.ServiceType].Add(item);
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
