using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CUEAudioVisualizer.Plugin;

namespace CUEAudioVisualizer
{
    static class PluginLoader
    {
        private static string PluginsDirectory = "Plugins";

        internal static IPlugin[] LoadPlugins()
        {
            if (!Directory.Exists(PluginsDirectory)) return null;

            List<IPlugin> pluginList = new List<IPlugin>();
            //Attempt to load all plugins
            foreach (string currentPluginPath in Directory.EnumerateFiles(PluginsDirectory, "*.dll"))
            {
                Assembly pluginAssembly = TryLoadAssembly(currentPluginPath);
                if (pluginAssembly == null) continue; //Unable to load assembly

                //Load all IPlugin instances in the current assembly
                var iPluginTypes = pluginAssembly.GetTypes().Where(type => typeof(IPlugin).IsAssignableFrom(type) && type.IsClass); //Get all IPlugin classes defined in the plugin
                foreach (Type currentPlugin in iPluginTypes)
                {
                    try
                    {
                        IPlugin createdInstance = (IPlugin)Activator.CreateInstance(currentPlugin);
                        pluginList.Add(createdInstance);
                    }
                    catch { }
                }
            }

            return pluginList.ToArray();
        }

        private static Assembly TryLoadAssembly(string filePath)
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(Path.GetFullPath(filePath));
                return assembly;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
