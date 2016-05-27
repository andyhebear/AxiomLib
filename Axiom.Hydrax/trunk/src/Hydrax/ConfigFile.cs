using System;
using System.ComponentModel;
using System.Collections.Generic;
using Axiom.Core;
using System.IO;
using Axiom.Math;
namespace Axiom.Hydrax
{
    /// <summary>
    /// this class represents an hydrax configuration file.
    /// </summary>
    public class ConfigFile
    {
        protected Dictionary<string, object> mCfgFileValues = new Dictionary<string, object>();
        protected string mHydraxVersion;
        /// <summary>
        /// Current hydrax version.
        /// </summary>
        public string HydraxVersion
        {
            set { mHydraxVersion = value; }
            get { return mHydraxVersion; }
        }
        protected string mModule;
        /// <summary>
        /// Name of the current geo module
        /// </summary>
        public string Module
        {
            set { mModule = value; }
            get { return mModule; }
        }
        protected string mNoise;
        /// <summary>
        /// Name of the current Noise module.
        /// </summary>
        public string Noise
        {
            set { mNoise = value; }
            get { return mNoise; }
        }
        /// <summary>
        /// Empty default constructor.
        /// </summary>
        public ConfigFile()
        {
        }
        /// <summary>
        /// Loads the hydrax cfg file.
        /// </summary>
        /// <param name="ConfigResource">stream from current hydrax cfg file.</param>
        public void Load(Stream ConfigResource)
        {
            StreamReader sr = new StreamReader(ConfigResource);
            string sLine = "";

            while ((sLine = sr.ReadLine()) != null)
            {
                //skip comments
                if (sLine.StartsWith("#"))
                    continue;
                //skip empty lines
                if (sLine.Equals(""))
                    continue;
                //Get hydrax version string.
                if (sLine.StartsWith("HydraxVersion"))
                {
                    string[] seperateLine = sLine.Split('=');
                    if (seperateLine.Length == 2)
                        mHydraxVersion = seperateLine[1];

                    continue;
                }
                //get the module type
                if (sLine.StartsWith("Module"))
                {
                    string[] seperateLine = sLine.Split('=');
                    if (seperateLine.Length == 2)
                        mModule = seperateLine[1];

                    continue;
                }
                //get noise
                if (sLine.StartsWith("Noise"))
                {
                    string[] seperateLine = sLine.Split('=');
                    if (seperateLine.Length == 2)
                       mNoise = seperateLine[1];

                    continue;
                }
                //Get Components
                if (sLine.StartsWith("Components"))
                {
                    string[] seperateLine = sLine.Split('=');
                    if (seperateLine.Length == 2)
                        AddConfigValue(GetComponents(seperateLine[1]), "Components");

                    continue;
                }

                //fetch values
                ParseLine(sLine);
            }
        }
        /// <summary>
        /// Interprets a line from cfg file.
        /// </summary>
        /// <param name="Line">line from cfg file</param>
        private void ParseLine(string Line)
        {
            string[] valueContainer = Line.Replace("<","").Split('>');
            string[] valParams;
            string Name;
            switch (valueContainer[0])
            {
                case "float":
                    valParams = valueContainer[1].Split('=');
                    Name = valParams[0];
                    float vFloat = float.Parse(valParams[1]);
                    AddConfigValue(vFloat, Name);
                    break;
                case "vector3":
                    valParams = valueContainer[1].Split('=');
                    Name = valParams[0];
                    string[] vector = valParams[1].Split('x');
                    Vector3 vVector = new Vector3(float.Parse(vector[0]), float.Parse(vector[1]), float.Parse(vector[2]));
                    AddConfigValue(vVector, Name);
                    break;
                case "int":
                    valParams = valueContainer[1].Split('=');
                    Name = valParams[0];
                    int vInt = int.Parse(valParams[1]);
                    AddConfigValue(vInt, Name);
                    break;
                case "size":
                    valParams = valueContainer[1].Split('=');
                    Name = valParams[0];
                    string[] vSizeSplit = valParams[1].Split('x');
                    System.Drawing.Size vSize = new System.Drawing.Size(int.Parse(vSizeSplit[0]), int.Parse(vSizeSplit[1]));
                    AddConfigValue(vSize, Name);
                    break;
                case "bool":
                    valParams = valueContainer[1].Split('=');
                    Name = valParams[0];
                    bool vBool = bool.Parse(valParams[1]);
                    AddConfigValue(vBool, Name);
                    break;
            }
        }

        /// <summary>
        /// Extracts components from cfg line.
        /// </summary>
        /// <param name="Components">cfg file components line.</param>
        /// <returns>valid hydraxcomponent</returns>
        private HydraxComponent GetComponents(string Components)
        {
            HydraxComponent ret = HydraxComponent.None;
            string[] components = Components.Split('|');
            for (int i = 0; i < components.Length; i++)
            {
                switch (components[i])
                {
                    case "Sun":
                        ret |= HydraxComponent.Sun;
                        break;
                    case "Foam":
                        ret |= HydraxComponent.Foam;
                        break;
                    case "Depth":
                        ret |= HydraxComponent.Depth;
                        break;
                    case "Caustics":
                        ret |= HydraxComponent.Caustics;
                        break;
                    case "Underwater":
                        ret |= HydraxComponent.Underwater;
                        break;
                    case "UnderwaterReflections":
                        ret |= HydraxComponent.UnderwaterReflections;
                        break;
                    case "UnderwaterGodRays":
                        ret |= HydraxComponent.UnderwaterGodRays;
                        break;
                    default:
                        Hydrax.HydraxLog("Unknown Component: " + components[i]);
                        break;
                }
            }
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HydraxComponent GetComponents()
        {
            return (HydraxComponent)GetValue("Components");
        }
        /// <summary>
        /// adds a cfg file value to the known cfg values.
        /// </summary>
        /// <param name="Value">valid value from cfg file.</param>
        /// <param name="ValueName">unique name for a cfg value from cfg file.</param>
        private void AddConfigValue(object Value, string ValueName)
        {
            if (!mCfgFileValues.ContainsKey(ValueName))
                mCfgFileValues.Add(ValueName, Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public MaterialManager.ShaderMode GetShaderMode(string Name)
        {
            return (MaterialManager.ShaderMode)GetValue(Name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Size GetSize(string Name)
        {
            return (Size)GetValue(Name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool GetBool(string Name)
        {
            return (bool)GetValue(Name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public float GetFloat(string Name)
        {
            return (float)GetValue(Name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public int GetInt(string Name)
        {
            return (int)GetValue(Name);
        }
        /// <summary>
        /// Get's value as Vector3.
        /// </summary>
        /// <param name="Name">Name of the value</param>
        /// <returns>Value as Vector3</returns>
        public Vector3 GetVector(string Name)
        {
            return (Vector3)GetValue(Name);
        }
        /// <summary>
        /// Reads a value from the Configfile.
        /// </summary>
        /// <param name="Name">Name of the configuration parameter.</param>
        /// <returns>Value as an object, simply cast it. If the value is unknow, value will be null</returns>
        private object GetValue(string Name)
        {
            Object obj = null;
            if (mCfgFileValues.TryGetValue(Name, out obj))
                return obj;

            return null;
        }
    }
}
