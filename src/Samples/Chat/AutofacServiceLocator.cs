using System;
using System.Collections;
using System.Collections.Generic;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace AspComet.Samples.Chat
{
    public sealed class AutofacServiceLocator : ServiceLocatorImplBase 
    {
        private readonly IContainer container;

        public AutofacServiceLocator(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            this.container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null)
                return this.container.Resolve(key, serviceType);
            return this.container.Resolve(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(serviceType);

            object instance;
            if (this.container.TryResolve(type, out instance))
            {
                return ((IEnumerable)instance).Cast<object>();
            }

            return Enumerable.Empty<object>();
        }
    }
}