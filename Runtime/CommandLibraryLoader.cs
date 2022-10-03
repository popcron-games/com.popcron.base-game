#nullable enable
using System;
using System.Reflection;
using Popcron.CommandRunner;

namespace BaseGame
{
    public class CommandLibraryLoader
    {
        private static readonly Log log = new(nameof(CommandLibraryLoader));

        [OnEnable(true)]
        private static void DoIt()
        {
            Library library = Library.Singleton;
            foreach (Type type in TypeCache.GetTypesAssignableFrom<IBaseCommand>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                try
                {
                    IBaseCommand prefab = (IBaseCommand)Activator.CreateInstance(type);
                    library.Add(prefab);
                }
                catch (Exception e)
                {
                    if (e is not MissingMethodException)
                    {
                        log.LogError(e.Message);
                    }
                }
            }

            foreach ((MethodInfo method, CommandAttribute attribute) in TypeCache.GetMethodsWithAttribute<CommandAttribute>())
            {
                if (method.IsStatic)
                {
                    MethodCommand command = new(attribute.Path, method);
                    library.Add(command);
                }
                else
                {
                    log.LogErrorFormat("Method {0} is not static and cant be registered a command", method.Name);
                }
            }
        }
    }
}