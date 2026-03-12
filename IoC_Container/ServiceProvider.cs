using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class ServiceProvider : System.IServiceProvider

    {
        Dictionary<Type, object> _singletonInstances = new Dictionary<Type, object>();
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

        private object GetImplementationInstance(List<ServiceDescriptor> descriptors, bool IsEnumerable = false) // IPeople -   People<Car>
        {

            var array = Array.CreateInstance(descriptors[0].ServiceType, descriptors.Count);

            for (int i = 0; i < descriptors.Count; i++)
            {
                var desc = descriptors[i];
                object instance = null;

                if (desc.Lifetime == ServiceLifetime.Singleton && desc.ImplementationInstance != null)
                {

                    instance = desc.ImplementationInstance;

                }
                else if (desc.Lifetime == ServiceLifetime.Singleton  && (descriptors[i].ImplementationType!= null && 
                    _singletonInstances.ContainsKey(descriptors[i].ImplementationType) ))
                {
                   
                    instance = _singletonInstances[descriptors[i].ImplementationType];
                }


                else if (desc.ImplementationFactory != null)
                {
                    instance = desc.ImplementationFactory.Invoke(this);

                    if (desc.Lifetime == ServiceLifetime.Singleton)
                    {
                        _singletonInstances[descriptors[i].ServiceType] = instance;
                    }
                }
                else
                {
                    instance = CreateInstance(desc.ImplementationType);

                    if (desc.Lifetime == ServiceLifetime.Singleton)
                    {
                        _singletonInstances[desc.ImplementationType] = instance;
                    }
                }

                array.SetValue(instance, i);
            }


            return IsEnumerable ? array : array.GetValue(0);
        }
        private object CreateInstance(Type type)  // 
        {
            var constructorInfos = type.GetConstructors().OrderByDescending(x => x.GetParameters().Length);
            foreach (var constructorInfo in constructorInfos)
            {
                bool haveNullInjection = false;
                ParameterInfo[] parameters = constructorInfo.GetParameters();
                if (parameters.Length == 0)
                {
                    return Activator.CreateInstance(type);
                }

                object[] parameterInstances = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    MethodInfo method = typeof(IServiceProvider).GetMethod(nameof(GetService), new Type[] { typeof(Type) });
                    object parameterInstance = method.Invoke(this, new object[] { parameters[i].ParameterType });
                    if (parameterInstance == null)
                    {
                        haveNullInjection = true;
                        break;
                    }
                    parameterInstances[i] = parameterInstance;
                }
                if (!haveNullInjection)
                {
                    return constructorInfo.Invoke(parameterInstances);
                }

            }

            return null;


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
            //else if (targetType.IsGenericType && collections.dicts.ContainsKey(targetType.GetGenericTypeDefinition()))
            //{
            //    var genericTypeDefinition = targetType.GetGenericTypeDefinition();
            //    var originDescriptors = collections.dicts[genericTypeDefinition];

            //    descriptors = new List<ServiceDescriptor>();
            //    foreach (var originDesc in originDescriptors)
            //    {
            //        descriptors.Add(new ServiceDescriptor(
            //                originDesc.ServiceType.MakeGenericType(targetType.GetGenericArguments()),
            //                originDesc.ImplementationType.MakeGenericType(targetType.GetGenericArguments()),
            //                originDesc.Lifetime));
            //        //{;
            //        //    type = originDesc.type.MakeGenericType(targetType.GetGenericArguments()),
            //        //    serviceLifetime = originDesc.Lifetime,
            //        //    func = originDesc.ImplementationFactory,
            //        //    ServiceType = type
            //        //})
            //    }
            else if (targetType.IsGenericType && collections.dicts.ContainsKey(targetType.GetGenericTypeDefinition()))
            {
                var genericTypeDefinition = targetType.GetGenericTypeDefinition();
                var originDescriptors = collections.dicts[genericTypeDefinition];

                descriptors = new List<ServiceDescriptor>();

                // 決定要處理哪些 descriptor
                var targets = isEnumerable ? originDescriptors : new List<ServiceDescriptor> { originDescriptors.Last() };

                foreach (var originDesc in targets)
                {
                    if (originDesc.ImplementationType != null)
                    {
                        descriptors.Add(new ServiceDescriptor(
                            targetType,
                            originDesc.ImplementationType.MakeGenericType(targetType.GetGenericArguments()),
                            originDesc.Lifetime));
                    }
                    else if (originDesc.ImplementationFactory != null)
                    {
                        descriptors.Add(new ServiceDescriptor(
                            targetType,
                            originDesc.ImplementationFactory,
                            originDesc.Lifetime));
                    }
                    else if (originDesc.ImplementationInstance != null)
                    {
                        descriptors.Add(new ServiceDescriptor(
                            targetType,
                            originDesc.ImplementationInstance));
                    }
                }
                collections.dicts[targetType] = descriptors;
            }
            else
            {
                return null;
            }
            List<ServiceDescriptor> targetDescriptors = isEnumerable ? descriptors : new List<ServiceDescriptor> { descriptors.Last() };






            return GetImplementationInstance(targetDescriptors, isEnumerable);
        }
    }
}
