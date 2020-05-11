﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax
{
    [Serializable]
    public class SettingsContainer
    {
        // Variables                                                                                                                
        private GeneralSettings _general;
        private GraphicsSettings _graphics;
        private PreSettings _pre;
        private CalculixSettings _calculix;
        private PostSettings _post;


        // Properties                                                                                                               
        public GeneralSettings General { get { return _general; } set { _general = value; } }
        public GraphicsSettings Graphics { get { return _graphics; } set { _graphics = value; } }
        public PreSettings Pre { get { return _pre; } set { _pre = value; } }
        public CalculixSettings Calculix { get { return _calculix; } set { _calculix = value; } }
        public PostSettings Post { get { return _post; } set { _post = value; } }


        // Constructors                                                                                                             
        public SettingsContainer()
        {
            Initialize();
        }
        public SettingsContainer(Dictionary<string, ISettings> items)
        {
            Initialize();
            FromDictionary(items);
        }


        // Methods                                                                                                                          
        public void Initialize()
        {
            _general = new GeneralSettings();
            _graphics = new GraphicsSettings();
            _pre = new PreSettings();
            _calculix = new CalculixSettings();
            _post = new PostSettings();
        }
        public void Reset()
        {
            _general.Reset();
            _graphics.Reset();
            _pre.Reset();
            _calculix.Reset();
            _post.Reset();
        }
        public Dictionary<string, ISettings> ToDictionary()
        {
            Dictionary<string, ISettings> items = new Dictionary<string, ISettings>();
            items.Add(Globals.GeneralSettingsName, _general);
            items.Add(Globals.GraphicsSettingsName, _graphics);
            items.Add(Globals.PreSettingsName, _pre);
            items.Add(Globals.CalculixSettingsName, _calculix);
            items.Add(Globals.PostSettingsName, _post);
            return items;
        }
        public void FromDictionary(Dictionary<string, ISettings> items)
        {
            _general = (GeneralSettings)items[Globals.GeneralSettingsName];
            _graphics = (GraphicsSettings)items[Globals.GraphicsSettingsName];
            _pre = (PreSettings)items[Globals.PreSettingsName];
            _calculix = (CalculixSettings)items[Globals.CalculixSettingsName];
            _post = (PostSettings)items[Globals.PostSettingsName];
        }
        public void SaveToFile(string fileName)
        {
            ToDictionary().DumpToFile(fileName);
        }
        public void LoadFromFile(string fileName)
        {
            Dictionary<string, ISettings> items = CaeGlobals.Tools.LoadDumpFromFile<Dictionary<string, ISettings>>(fileName);
            FromDictionary(items);
           
        }
        //
        public string GetWorkDirectory()
        {
            string lastFileName = _general.LastFileName;
            if (_calculix.UsePmxFolderAsWorkDirectory && lastFileName != null && File.Exists(lastFileName) &&
                Path.GetExtension(lastFileName) == ".pmx")
            {
                return Path.GetDirectoryName(lastFileName);
            }
            else return _calculix.WorkDirectory;
        }
      
    }
}
