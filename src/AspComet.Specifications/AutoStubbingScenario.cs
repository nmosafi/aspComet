using System;
using System.Collections.Generic;
using System.Reflection;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications
{
    public abstract class AutoStubbingScenario<TSut> 
        where TSut : class
    {
        static Dictionary<Type, object> dependencies;
        static TSut sut;

        Establish context = () =>
                            dependencies = new Dictionary<Type, object>();

        Cleanup stuff = () =>
        {
            dependencies = null;
            sut = null;
        };

        protected static T Dependency<T>() where T : class
        {
            Type type = typeof(T);
            T dependency = (T) Dependency(type);
            return dependency;
        }

        protected static void SetDependency<T>(T dependency)
        {
            Type type = typeof(T);
            if (dependencies.ContainsKey(type))
            {
                throw new Exception("Type " + type + " already registered.");
            }

            dependencies.Add(type, dependency);
        }

        private static object Dependency(Type type)
        {
            if (dependencies.ContainsKey(type))
            {
                return dependencies[type];
            }

            if (!type.IsInterface)
            {
                throw new Exception("Can only auto generate stubs on interfaces.  Add the dependency manually");
            }

            if (dependencies.ContainsKey(type))
            {
                throw new Exception("Type " + type + " already registered.");
            }

            object stub = MockRepository.GenerateStub(type);
            dependencies.Add(type, stub);
            return stub;
        }

        protected static TSut SUT
        {
            get
            {
                if (sut == null)
                {
                    sut = GenerateSut();
                }
                return sut;
            }
        }

        private static TSut GenerateSut()
        {
            Type type = typeof(TSut);
            object[] constructorParams = ResolveConstructorParams(type);
            TSut generatedSut = (TSut) Activator.CreateInstance(type, constructorParams);
            return generatedSut;
        }

        private static object[] ResolveConstructorParams(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors()[0];

            ParameterInfo[] parameters = constructor.GetParameters();
            List<object> constructorParams = new List<object>(parameters.Length);
            foreach (ParameterInfo parameter in parameters)
            {
                object constructorParam = Dependency(parameter.ParameterType);
                constructorParams.Add(constructorParam);
            }

            return constructorParams.ToArray();
        }
    }
}