namespace GameStore.App.StaticData.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    public static class DefaultImplementation
    {
        /// <summary>
        /// When an interface has more than one implementation, the default implementation eligible for dependency injection should be put in this dictionary - {key: typeof(interface), value: typeof(default implementation)}
        /// </summary>
        public static readonly IDictionary<Type, Type> Defaults = new Dictionary<Type, Type>()
        {

        };
    }
}
