using System;
using System.Collections.Generic;

namespace AspComet
{
    /// <summary>
    ///     Describes the services which need to be instantiated for AspComet
    /// </summary>
    [Obsolete("This functionality has been replaced by fluent configuration")]
   public class ServiceMetadata
    {
        public Type ServiceType { get; private set; }
        public Type ActualType { get; private set; }
        public bool IsPerRequest { get; private set; }

        public Type ConstructorType { get; private set; }

        private ServiceMetadata()
        {
        }


    }
}