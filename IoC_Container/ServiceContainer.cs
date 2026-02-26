using IoC_Container.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class ServiceContainer
    {
        Dictionary<Type, List<ServiceDescriptor>> dicts = new Dictionary<Type, List<ServiceDescriptor>>(); // IXXX , XXX        


        public void AddTransient<T, T1>() where T1 : T
        {
            AddTransient(typeof(T), typeof(T1));
        }
        public void AddSingleton<T, T1>() where T1 : T
        {
            AddSingleton(typeof(T), typeof(T1));
        }
        public void AddTransient<T>(Func<T> func) where T : class
        {
            if (!dicts.ContainsKey(typeof(T)))
            {
                dicts[typeof(T)] = new List<ServiceDescriptor>();
            }
            dicts[typeof(T)].Add(new ServiceDescriptor()
            {
                serviceLifetime = ServiceLifetime.Transient,
                func = func
            });
        }

        public void AddTransient(Type T, Type T1)
        {

            if (!dicts.ContainsKey(T))
            {
                dicts[T] = new List<ServiceDescriptor>();
            }
            dicts[T].Add(new ServiceDescriptor()
            {
                serviceLifetime = ServiceLifetime.Transient,
                type = T1
            });
        }
        public void AddSingleton(Type T, Type T1)
        {
            if (!dicts.ContainsKey(T))
            {
                dicts[T] = new List<ServiceDescriptor>();
            }
            dicts[T].Add(new ServiceDescriptor()
            {
                serviceLifetime = ServiceLifetime.Singleton,
                type = T1
            });
        }


        //1.GetService: 對外呼叫的方法,並且用來分類是 Type/GenericType/Enumerable 來做分流
        //2.GetImplemetionInstance: 根據GetService給予的不同類型,統一類型從Dictionary中取出ServiceDescriptor 
        //3.CreateInstance: 從ServiceDescriptor中找出對應的Type並且滿足該類型的所有建構元參數,並建構她。 其中建構元的每一個參數預設都應該會存在在容器中,所以需要返回第一步

        public T GetService<T>() // GetService<IGamePlatform<>>()
        {
            return (T)GetImplementationInstance(typeof(T));
        }
        private object GetImplementationInstance(Type type) // IPeople -   People<Car>
        {

            bool isEnumerable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
            Type targetType = isEnumerable ? type.GetGenericArguments()[0] : type;  //

            List<ServiceDescriptor> descriptors = null;
            if (dicts.ContainsKey(targetType))
            {
                descriptors = dicts[targetType];
            }
            else if (targetType.IsGenericType && dicts.ContainsKey(targetType.GetGenericTypeDefinition()))
            {
                var genericTypeDefinition = targetType.GetGenericTypeDefinition();
                var originDescriptors = dicts[genericTypeDefinition];

                descriptors = new List<ServiceDescriptor>();
                foreach (var originDesc in originDescriptors)
                {
                    descriptors.Add(new ServiceDescriptor()
                    {
                        type = originDesc.type.MakeGenericType(targetType.GetGenericArguments()),
                        serviceLifetime = originDesc.serviceLifetime,
                        func = originDesc.func
                    });
                }

                dicts[targetType] = descriptors;
            }

            List<ServiceDescriptor> targetDescriptors = isEnumerable ? descriptors : new List<ServiceDescriptor> { descriptors.Last() };

            var array = Array.CreateInstance(targetType, targetDescriptors.Count);
           
            for (int i = 0; i < targetDescriptors.Count; i++)
            {
                var desc = targetDescriptors[i];
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
            return isEnumerable ? array : array.GetValue(0);

        }
        private object CreateInstance(Type type)  // Tesla
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
                MethodInfo method = typeof(ServiceContainer).GetMethod(nameof(GetService));
                MethodInfo genericMethod = method.MakeGenericMethod(parameters[i].ParameterType);


                parameterInstances[i] = genericMethod.Invoke(this, null);
            }

            return constructorInfo.Invoke(parameterInstances);

        }
        //private T Resolve<T>(ServiceDescriptor desciptor)
        //{

        //    ConstructorInfo constructorInfo = desciptor.type.GetConstructors()[0];
        //    ParameterInfo[] parameters = constructorInfo.GetParameters();
        //    object[] parameterInstances = new object[parameters.Length];


        //    for (int i = 0; i < parameters.Length; i++)
        //    {

        //        if (parameters[i].ParameterType.IsGenericType)
        //        {
        //            MethodInfo methodGeneric = typeof(ServiceContainer).GetMethod("ResolveGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
        //            var temp = methodGeneric.MakeGenericMethod(parameters[i].ParameterType);
        //            parameterInstances[i] = temp.Invoke(this, new object[] { });
        //        }
        //        ServiceDescriptor desciptor1 = dicts[parameters[i].ParameterType];


        //        if (desciptor._singletonInstance != null && desciptor.serviceLifetime == ServiceLifetime.Singleton)
        //        {
        //            parameterInstances[i] = desciptor._singletonInstance;
        //            continue;
        //        }

        //        MethodInfo method = typeof(ServiceContainer).GetMethod("Resolve", BindingFlags.NonPublic | BindingFlags.Instance);
        //        parameterInstances[i] = method.MakeGenericMethod(parameters[i].ParameterType).Invoke(this, new object[] { desciptor1 });


        //        if (desciptor.serviceLifetime == ServiceLifetime.Singleton)
        //        {
        //            desciptor._singletonInstance = parameterInstances[i];
        //        }
        //    }
        //    T instance = (T)constructorInfo.Invoke(parameterInstances);


        //    return instance;

        //}

        //private T ResolveGeneric<T>() //  IGamePlatform<IGame>
        //{
        //    var genericTypeDefinition = typeof(T).GetGenericTypeDefinition(); //  IGamePlatform<>
        //    var genericArgs = typeof(T).GetGenericArguments(); //  <IGame>

        //    ConstructorInfo constructorInfo = genericArgs.GetConstructors()[0];
        //    ParameterInfo[] parameters = constructorInfo.GetParameters();
        //    object[] parameterInstances = new object[parameters.Length];

        //    for (int i = 0; i < parameters.Length; i++)
        //    {
        //        Type parameterType = parameters[i].ParameterType;

        //        ServiceDescriptor genericDescriptor = dicts[genericTypeDefinition];

        //        Type[] resolvedGenericArgs = new Type[genericArgs.Length];
        //        for (int j = 0; j < genericArgs.Length; j++)
        //        {

        //            if (!dicts.ContainsKey(genericArgs[j]))
        //            {
        //                continue;
        //            }
        //            resolvedGenericArgs[j] = dicts[genericArgs[j]].type;
        //        }

        //        var genericType = genericDescriptor.type.MakeGenericType(resolvedGenericArgs);


        //        ServiceDescriptor concreteDescriptor = new ServiceDescriptor()
        //        {
        //            serviceLifetime = genericDescriptor.serviceLifetime,
        //            type = genericType
        //        };
        //        parameterInstances[i] = Activator.CreateInstance(genericType);
        //    }
        //    T instance = (T)constructorInfo.Invoke(parameterInstances);


        //    return instance;
        //    //MethodInfo method = typeof(ServiceContainer).GetMethod("Resolve", BindingFlags.NonPublic | BindingFlags.Instance);

        //}
        //[Flags]
        //Read = 1 => 0001
        //Create = 2 => 0010
        //Read+Create => 0011 => Read+Create & Read !=0 => 擁有權限
        //Execute = 4 => 0100
        //Modify = 8 => 1000


    }
}
