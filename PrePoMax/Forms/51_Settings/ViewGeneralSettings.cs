using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Settings
{
    [Serializable]
    public class ViewGeneralSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private GeneralSettings _generalSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [Category("General")]
        [OrderedDisplayName(0, 10, "Open last file")]
        [Description("When the program starts open the last file Saved/Opened.")]
        public bool OpenLastFile { get { return _generalSettings.OpenLastFile; } set { _generalSettings.OpenLastFile = value; } }
        //
        [Category("General")]
        [OrderedDisplayName(1, 10, "Last file name")]
        [Description("The name of the last file Saved/Opened.")]
        [ReadOnly(true)]
        public string LastFileName { get { return _generalSettings.LastFileName; } set { _generalSettings.LastFileName = value; } }
        //
        [Category("General")]
        [OrderedDisplayName(2, 10, "Save results in .pmx files")]
        [Description("Save the results in the PrePoMax .pmx file.")]
        public bool SaveResultsInPmx
        {
            get { return _generalSettings.SaveResultsInPmx; }
            set { _generalSettings.SaveResultsInPmx = value; }
        }
        //
        [Category("General")]
        [OrderedDisplayName(3, 10, "Default unit system")]
        [Description("Select the default unit system for new models.")]
        public string UnitSystemType
        {
            get
            {
                return _generalSettings.UnitSystemType.GetDescription();
            }
            set
            {
                foreach (UnitSystemType unitSystemType in Enum.GetValues(typeof(UnitSystemType)))
                {
                    if (unitSystemType.GetDescription() == value)
                    {
                        _generalSettings.UnitSystemType = unitSystemType;
                        return;
                    }
                }
                throw new NotSupportedException();
            }
        }
        //
        [Category("Import mesh")]
        [OrderedDisplayName(0, 10, "Edge angle")]
        [Description("Select the edge angle for the detection of model edges. The angle will be used for future imports.")]
        [TypeConverter(typeof(StringAngleDegConverter))]
        public double EdgeAngle { get { return _generalSettings.EdgeAngle; } set { _generalSettings.EdgeAngle = value; } }


        // Constructors                                                                                                             
        public ViewGeneralSettings(GeneralSettings generalSettings)
        {
            _generalSettings = generalSettings;
            _dctd = ProviderInstaller.Install(this);
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(OpenLastFile));
            _dctd.RenameBooleanPropertyToYesNo(nameof(SaveResultsInPmx));
            // Add unit system types as description strings
            List<string> descriptions = new List<string>();
            foreach (UnitSystemType unitSystemType in Enum.GetValues(typeof(UnitSystemType)))
                descriptions.Add(unitSystemType.GetDescription());
            _dctd.PopulateProperty(nameof(UnitSystemType), descriptions.ToArray());
        }


        // Methods                                                                                                                  
        public ISettings GetBase()
        {
            return _generalSettings;
        }
        public void Reset()
        {
            _generalSettings.Reset();
        }
    }

}
