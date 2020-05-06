﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;

namespace PrePoMax
{
    [Serializable]
    public class GeneralSettings : ISettings
    {
        // Variables                                                                                                                        
        private bool _openLastFile;
        private string _lastFileName;
        private bool _saveResultsInPmx;
        private LinkedList<string> _recentFiles;

        // Properties                                                                                                               
        public bool OpenLastFile { get { return _openLastFile; } set { _openLastFile = value; } }
        public string LastFileName { get { return _lastFileName; } set { _lastFileName = value; } }
        public bool SaveResultsInPmx { get { return _saveResultsInPmx; } set { _saveResultsInPmx = value; } }


        // Constructors                                                                                                             
        public GeneralSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _openLastFile = true;
            _lastFileName = null;
            _saveResultsInPmx = true;
            _recentFiles = null;
        }
        public string[] GetRecentFiles()
        {
            if (_recentFiles != null) return _recentFiles.ToArray();
            else return null;
        }
        public void AddRecentFile(string fileNameWithpPath)
        {
            if (_recentFiles == null) _recentFiles = new LinkedList<string>();
            //
            if (_recentFiles.Count == 0) _recentFiles.AddFirst(fileNameWithpPath);
            else
            {
                if (!_recentFiles.Contains(fileNameWithpPath))
                {
                    int _maxRecentFiles = 15;
                    while (_recentFiles.Count >= _maxRecentFiles) _recentFiles.RemoveLast();
                    _recentFiles.AddFirst(fileNameWithpPath);
                }
                else
                {
                    _recentFiles.Remove(fileNameWithpPath);
                    _recentFiles.AddFirst(fileNameWithpPath);
                }
            }
        }

        public void ClearRecentFiles()
        {
            _recentFiles.Clear();
        }
        



    }
}
