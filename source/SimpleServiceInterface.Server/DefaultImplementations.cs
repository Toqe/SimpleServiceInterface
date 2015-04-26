using SimpleServiceInterface.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Server
{
    public class DefaultImplementations
    {
        private static Type ISimpleServiceType = typeof(ISimpleService);

        public static Func<string, string> UrlModifier = (string url) => url;

        public static Func<Type, object> InstanceBuilder = (Type type) => type.GetConstructor(new Type[0]).Invoke(new object[0]);

        public static Func<string, Type> TypeFinder =
            (string name) =>
                AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => ISimpleServiceType.IsAssignableFrom(t) && t.Name == name);
    }
}
