#region Namespace Declarations

using System;
using System.IO;
using System.Globalization;
using System.Threading;

using Axiom.Demos;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Demos.Configuration;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Axiom.Demos.Browser.CommandLine;

#endregion Namespace Declarations

namespace Axiom.SoundSystems.Demos
{

    /// <summary>
    /// Demo command line browser entry point.
    /// </summary>
    /// <remarks>
    /// This demo browser is implemented using a commandline interface. 
    /// </remarks>
    public class Program : IDisposable
    {
        protected const string CONFIG_FILE = @"EngineConfig.xml";

        private Axiom.Core.Root engine;

        private bool _configure()
        {
            // instantiate the Root singleton
            engine = new Axiom.Core.Root("AxiomDemos.log");

            _setupResources();

            // HACK: Temporary
            ConfigDialog dlg = new ConfigDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Axiom.Core.Root.Instance.Dispose();
                engine = null;
                return false;
            }

            return true;
        }

        /// <summary>
        ///		Loads default resource configuration if one exists.
        /// </summary>
        private void _setupResources()
        {
            string resourceConfigPath = Path.GetFullPath(CONFIG_FILE);

            if (File.Exists(resourceConfigPath))
            {
                EngineConfig config = new EngineConfig();

                // load the config file
                // relative from the location of debug and releases executables
                config.ReadXml(CONFIG_FILE);

                // interrogate the available resource paths
                foreach (EngineConfig.FilePathRow row in config.FilePath)
                {
                    ResourceGroupManager.Instance.AddResourceLocation(row.src, row.type);
                }
            }
        }

        public void Run()
        {
            try
            {
                if (_configure())
                {
                    Type demoType = SelectDemo(
                        Assembly.GetExecutingAssembly(),
                        typeof(TechDemo).AssemblyQualifiedName,
                        typeof(SoundDemoAttribute).AssemblyQualifiedName);

                    if (demoType != null)
                    {
                        using (TechDemo demo = (TechDemo)Activator.CreateInstance(demoType))
                        {
                            // select and pass-in a sound system
                            MemberInfo[] minfo = demo.GetType().FindMembers(
                                MemberTypes.Field,
                                BindingFlags.Instance | BindingFlags.NonPublic,
                                Type.FilterName, "SelectedSoundSystem");
                            
                            if (minfo.Length > 0)
                            {
                                // demo has the appropriate member
                                Console.WriteLine();
                                string soundSystem = SelectSoundSystem();

                                minfo[0].ReflectedType.InvokeMember(
                                    "SelectedSoundSystem",
                                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField,
                                    Type.DefaultBinder, demo, new object[]{soundSystem});
                            }

                            demo.Start();//show and start rendering
                        }
                    }
                }
            }
            catch (Exception caughtException)
            {
                LogManager.Instance.Write(BuildExceptionString(caughtException));
            }
        }

        private Type SelectDemo(Assembly assembly, string demoTypeName, string demoTypeAttributeName)
        {
            SortedList<string, string> demoList = new SortedList<string, string>();
            Type[] demoTypes = assembly.GetTypes();
            Type techDemo = Type.GetType(demoTypeName);
            
            Attribute demoAttribute;
            if (demoTypeAttributeName == null)
                demoAttribute = null;
            else
                demoAttribute = (Attribute)Activator.CreateInstance(Type.GetType(demoTypeAttributeName));

            foreach (Type demoType in demoTypes)
            {
                if (demoType.IsSubclassOf(techDemo))
                {
                    bool isDemo = false;
                    if (demoAttribute == null)
                        isDemo = true;

                    else
                    {
                        object[] attrs = demoType.GetCustomAttributes(demoAttribute.GetType(), true);
                        foreach (object o in attrs)
                            if (o.GetType() == demoAttribute.GetType() || o.GetType().IsSubclassOf(demoAttribute.GetType()))
                            {
                                isDemo = true;
                                break;
                            }
                    }

                    if (isDemo)
                        demoList.Add(demoType.Name, demoType.AssemblyQualifiedName);
                }
            }

            string next = "";

            int i = 1;
            foreach (KeyValuePair<string, string> typeName in demoList)
            {
                Console.WriteLine("{0}) {1}", i++, typeName.Key);
            }
            Console.WriteLine();

            while (true)
            {
                Console.Write("Select demo:");
                string line = Console.ReadLine();
                int number;
                if (!int.TryParse(line.Trim(), out number))
                    number = -1;

                if (number < 1 || number > demoList.Count)
                    Console.WriteLine("The number must be between 1 and {0}", demoList.Count);

                else
                {
                    next = (string)demoList.Values[number - 1];
                    break;
                }
            }

            Type type = Type.GetType(next);

            return type;
        }

        private string SelectSoundSystem()
        {        	
            // select a subsystem, this is hardcoded for now, sorry
            Console.WriteLine("1) Xna.Simple");
            Console.WriteLine("2) OpenAL.Tao");
            Console.WriteLine("3) OpenAL.OpenTK");
            Console.WriteLine();

            int number;
            while (true)
            {
                Console.Write("Select sound system:");
                string line = Console.ReadLine();
                if (!int.TryParse(line.Trim(), out number))
                    number = -1;

                if (number < 1 || number > 3)
                    Console.WriteLine("Invalid input");
                else
                    break;
            }

            switch (number)
            {
                case 1:
                    return "Axiom.SoundSystems.Xna.Simple";
                case 2:
                    return "Axiom.SoundSystems.OpenAL.Tao";
                case 3:
                    return "Axiom.SoundSystems.OpenAL.OpenTK";
            }

            return null;
        }

        #region Main

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                using (Program main = new Program())
                {
                    main.Run();//show and start rendering
                }//dispose of it when done
            }
            catch (Exception ex)
            {
                Console.WriteLine(BuildExceptionString(ex));
                Console.WriteLine("An exception has occurred.  Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static string BuildExceptionString(Exception exception)
        {
            string errMessage = string.Empty;

            errMessage += exception.Message + Environment.NewLine + exception.StackTrace;

            while (exception.InnerException != null)
            {
                errMessage += BuildInnerExceptionString(exception.InnerException);
                exception = exception.InnerException;
            }

            return errMessage;
        }

        private static string BuildInnerExceptionString(Exception innerException)
        {
            string errMessage = string.Empty;

            errMessage += Environment.NewLine + " InnerException ";
            errMessage += Environment.NewLine + innerException.Message + Environment.NewLine + innerException.StackTrace;

            return errMessage;
        }

        #endregion Main

        #region IDisposable Members

        public void Dispose()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}