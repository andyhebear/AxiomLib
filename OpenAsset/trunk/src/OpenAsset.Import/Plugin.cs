using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Axiom.Core;

namespace Axiom.Component.OpenAsset
{
    class Plugin: IPlugin
    {
        private List<IAssetPlugin> _AssetImporters = new List<IAssetPlugin>();

        #region IPlugin Members

        public void Initialize()
        {
            LoadAssetPlugins();
        }

        public void Shutdown()
        {
            UnloadPluginAssemblies();
        }

        #endregion

        public void LoadAssetPlugins()
        {
            if (null == _AssetImporters)
                _AssetImporters = new List<IAssetPlugin>();
            else
            _AssetImporters.Clear();

            List<Assembly> plugInAssemblies = LoadPlugInAssemblies();
            List<IAssetPlugin> plugIns = GetPlugIns(plugInAssemblies);

            foreach (IAssetPlugin assetImporter in plugIns)
            {
                assetImporter.Initialize();
                _AssetImporters.Add(assetImporter);
            }
        }

        private void UnloadPluginAssemblies()
        {
            foreach (IAssetPlugin assetImporter in _AssetImporters)
            {
                assetImporter.Shutdown();
            }
        }

        private List<Assembly> LoadPlugInAssemblies()
        {
            DirectoryInfo dInfo = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] files = dInfo.GetFiles("*.dll");
            List<Assembly> plugInAssemblyList = new List<Assembly>();

            if (null != files)
            {
                foreach (FileInfo file in files)
                {
                    plugInAssemblyList.Add(Assembly.LoadFile(file.FullName));
                }
            }

            return plugInAssemblyList;

        }

        private List<IAssetPlugin> GetPlugIns(List<Assembly> assemblies)
        {
            List<Type> availableTypes = new List<Type>();

            foreach (Assembly currentAssembly in assemblies)
                availableTypes.AddRange(currentAssembly.GetTypes());

            // get a list of objects that implement the IAssetPlugin interface AND 
            // have the AssetPlugInAttribute
            List<Type> AssetLoaderList = availableTypes.FindAll(delegate(Type t)
            {
                List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
                object[] arr = t.GetCustomAttributes(typeof(AssetPlugInAttribute), true);
                return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(IAssetPlugin));
            });

            // convert the list of Objects to an instantiated list of IAssetPlugin
            return AssetLoaderList.ConvertAll<IAssetPlugin>(delegate(Type t) { return Activator.CreateInstance(t) as IAssetPlugin; });

        }
    }
}
