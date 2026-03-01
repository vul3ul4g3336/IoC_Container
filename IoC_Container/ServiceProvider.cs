using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly ServiceCollection collections;
        public ServiceProvider(ServiceCollection collections) 
        { 
            this.collections = collections;
        }
        
        //1. 檢查T 是哪一種情況?  一般的class / 泛型類型/IEnumerable/Func
        //2. 每一個類型到最後都需要從容器中找出對應的ServiceDescriptor
        //3. 根據ServiceDescriptor 中的 LiftTime 來決定如何生成
        // - Transient 直接無腦創建/呼叫Func
        // - Signleton 檢查該物件是否曾經生成過，若生成過從容器中拿取，沒生成過則進一步生成並放回容器中
        //4. 針對指定的類型逐一檢查建構元內的參數並透過遞迴完成生成
        
        
        public T GetService<T>() // GetService<IGamePlatform<>>()
        {
            return (T)GetService(typeof(T));
        }

        private object GetImplementationInstance(List<ServiceDescriptor> descriptors , bool IsEnumerable = false) // IPeople -   People<Car>
        {
            
            var array = Array.CreateInstance(descriptors[0].ServiceType, descriptors.Count);

            for (int i = 0; i < descriptors.Count; i++)
            {
                var desc = descriptors[i];
                object instance;

                if (desc.serviceLifetime == ServiceLifetime.Singleton && desc._singletonInstance != null)
                {
                    instance = desc._singletonInstance;
                }
                else if (desc.func != null)
                {
                    instance = desc.func.Invoke();

                    if (desc.serviceLifetime == ServiceLifetime.Singleton)
                    {
                        desc._singletonInstance = instance;
                    }
                }
                else
                {
                    instance = CreateInstance(desc.type);

                    if (desc.serviceLifetime == ServiceLifetime.Singleton)
                    {
                        desc._singletonInstance = instance;
                    }
                }

                array.SetValue(instance, i);
            }


            return IsEnumerable ? array : array.GetValue(0);
        }
        private object CreateInstance(Type type)  // 
        {
            ConstructorInfo constructorInfo = type.GetConstructors()[0];
            ParameterInfo[] parameters = constructorInfo.GetParameters();

            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(type);
            }


            object[] parameterInstances = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                MethodInfo method = typeof(ServiceProvider).GetMethod(nameof(GetService), new Type[] { typeof(Type) });
                object aa = method.Invoke(this, new object[] { parameters[i].ParameterType });

                parameterInstances[i] = aa;
            }

            return constructorInfo.Invoke(parameterInstances);

        }

        public object GetService(Type type) // IEnumerble<ICar>
        {


            bool isEnumerable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
            Type targetType = isEnumerable ? type.GetGenericArguments()[0] : type;  //

            List<ServiceDescriptor> descriptors = null;
            if (collections.dicts.ContainsKey(targetType))
            {
                descriptors = collections.dicts[targetType];
            }
            else if (targetType.IsGenericType && collections.dicts.ContainsKey(targetType.GetGenericTypeDefinition()))
            {
                var genericTypeDefinition = targetType.GetGenericTypeDefinition();
                var originDescriptors = collections.dicts[genericTypeDefinition];

                descriptors = new List<ServiceDescriptor>();
                foreach (var originDesc in originDescriptors)
                {
                    descriptors.Add(new ServiceDescriptor()
                    {
                        type = originDesc.type.MakeGenericType(targetType.GetGenericArguments()),
                        serviceLifetime = originDesc.serviceLifetime,
                        func = originDesc.func,
                        ServiceType = type
                    });
                }

                collections.dicts[targetType] = descriptors;
            }

            List<ServiceDescriptor> targetDescriptors = isEnumerable ? descriptors : new List<ServiceDescriptor> { descriptors.Last() };

            




            return GetImplementationInstance(targetDescriptors, isEnumerable);
        }
    }
}
