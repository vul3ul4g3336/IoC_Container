using IOCServiceCollection.Test;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IOCServiceCollection
{
    public class SProvider : IServiceProvider
    {
        public SCollection _services;
        private Dictionary<Type, object> singletonDic = new Dictionary<Type, object>();

        public SProvider(SCollection services)
        {
            _services = services;
        }

        public object CreateInstance(Type type)
        {
            // 建構元參數最多的排在最前面
            var ctors = type.GetConstructors().OrderByDescending(x => x.GetParameters().Length);

            foreach (var ctor in ctors)
            {
                bool haveNullResult = false;
                // 沒有建構元的話就Create 
                if (ctor.GetParameters().Length == 0)
                {
                    return Activator.CreateInstance(type);
                }

                // 取得所有的參數並放入List
                var parms = ctor.GetParameters();

                List<object> parmsInstanceList = new List<object>();

                foreach (var param in parms)
                {
                    MethodInfo getServiceMethod = typeof(SProvider).GetMethod("GetService");
                    var result = getServiceMethod.Invoke(this, new object[] { param.ParameterType });

                    if (result == null)
                    {
                        haveNullResult = true;
                        break;
                    }
                    parmsInstanceList.Add(result);
                }

                if (!haveNullResult)
                {
                    return Activator.CreateInstance(type, parmsInstanceList.ToArray());
                }

            }
            return null;
        }

        public object GetService(Type serviceType)
        {
            //找不到時，可能為泛型，Enum，或是不存在
            if (!_services.dictiontry.TryGetValue(serviceType, out List<ServiceDescriptor> descriptors))
            {
                // 檢查是否為IEnumerable
                if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    // 取得類型 IEnumerable 內的型別 ILoggerProvider
                    Type itemType = serviceType.GetGenericArguments()[0];

                    // 找到有注入在 ILoggerProvider 的類型(先抓第一個)
                    if (_services.dictiontry.TryGetValue(itemType, out List<ServiceDescriptor> genericDescriptors))
                    {
                        //取得List<>
                        Type listType = typeof(List<>);
                        //製作List<T>
                        Type listGenericType = listType.MakeGenericType(itemType);
                        //建立類型 IList
                        IList listGenericTypeInstance = (IList)Activator.CreateInstance(listGenericType);


                        foreach (ServiceDescriptor genericDescriptor in genericDescriptors)
                        {
                            object implemenInstance = GetImplementationInstance(genericDescriptor);
                            listGenericTypeInstance.Add(implemenInstance);
                        }


                        // List<ShowA>
                        return listGenericTypeInstance;
                    }
                    else
                    {
                        return null;
                    }

                }

                //檢查是否為泛型
                else if (serviceType.IsGenericType) //ILogger<Form1>
                {
                    //取得泛型類型 ILogger<T>
                    Type genericType = serviceType.GetGenericTypeDefinition();

                    //取得泛型的實作類別 Logger<T> 
                    if (_services.dictiontry.TryGetValue(genericType, out List<ServiceDescriptor> genericDescriptors))
                    {
                        //取得泛型內的類型Form1
                        Type gt = serviceType.GetGenericArguments()[0];

                        ServiceDescriptor genericDescriptor = genericDescriptors.Last();

                        //組裝泛型 Logger<Form1>
                        Type genericWithImplementationType = genericDescriptor.ImplementationType.MakeGenericType(gt);

                        //製作ServiceDescriptor
                        ServiceDescriptor serviceDescriptor = new ServiceDescriptor(genericDescriptor.ServiceType, genericWithImplementationType, genericDescriptor.Lifetime);

                        return GetImplementationInstance(serviceDescriptor);
                    }

                    // 沒有找到，代表注入的可能是 ILogger<Form1>，但目前找到的是 Logger<Form1>

                }
                else
                {
                    return null;
                }
            }

            return GetImplementationInstance(descriptors.Last());

        }


        public object GetImplementationInstance(ServiceDescriptor descriptor)
        {
            switch (descriptor.Lifetime)
            {
                // 1. Transient: 物件的話就 new 出來，Func的話就執行
                case ServiceLifetime.Transient:

                    if (descriptor.ImplementationFactory != null)
                    {
                        return descriptor.ImplementationFactory.Invoke(this);
                    }
                    else
                    {
                        return CreateInstance(descriptor.ImplementationType);
                    }

                // 3. Singleton: 
                // 物件 先找有沒有 instance 存在，沒有的話 new 出來並存到 instance
                // Func 先找有沒有 instance 存在，沒有的話 執行並存到 instance              
                case ServiceLifetime.Singleton:
                    if (this.singletonDic.ContainsKey(descriptor.ServiceType))
                    {
                        return this.singletonDic[descriptor.ServiceType];
                    }
                    else if (descriptor.ImplementationInstance != null)
                    {
                        return descriptor.ImplementationInstance;
                    }
                    else if (descriptor.ImplementationFactory != null)
                    {
                        // 把 ImplementationFactory.Invoke(this) 用一個物件容器存起來，存在ServiceProvider
                        object implemenInstance = descriptor.ImplementationFactory.Invoke(this);
                        this.singletonDic.Add(descriptor.ServiceType, implemenInstance);
                        return implemenInstance;
                    }
                    else
                    {
                        object implemenInstance = CreateInstance(descriptor.ImplementationType);
                        this.singletonDic.Add(descriptor.ServiceType, implemenInstance);
                        return implemenInstance;
                    }
                default:
                    return null;
            }
        }
    }

}
 