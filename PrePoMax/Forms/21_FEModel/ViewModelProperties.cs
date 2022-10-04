using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeModel;
using CaeResults;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewModelProperties
    {
        // Variables                                                                                                                
        private string _name;
        private ModelProperties _modelProperties;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [Category("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [Description("Name of the model.")]
        [Id(1, 1)]
        public string Name { get { return _name; } set { _name = value; } }
        //
        [Category("Data")]
        [OrderedDisplayName(1, 10, "Model space")]
        [Description("Model space.")]
        [Id(2, 1)]
        public ModelSpaceEnum ModelSpace { get { return _modelProperties.ModelSpace; } set { _modelProperties.ModelSpace = value; } }//
        [Category("Data")]
        [OrderedDisplayName(2, 10, "Model type")]
        [Description("Model type.")]
        [Id(3, 1)]
        public ModelType ModelType 
        { 
            get { return _modelProperties.ModelType; } 
            set 
            { 
                _modelProperties.ModelType = value;
                //
                UpdateVisibility();
            }
        }
        // Submodel
        [Category("Submodel")]
        [OrderedDisplayName(0, 10, "Global results .frd")]
        [Description("Global results file name (.frd) without path.")]
        [EditorAttribute(typeof(FilteredFileNameEditor), typeof(UITypeEditor))]
        [Id(1, 2)]
        public string GlobalResultsFileName
        {
            get { return _modelProperties.GlobalResultsFileName; }
            set 
            {
                string fileName = System.IO.Path.GetFileName(value);
                //
                if (fileName.Contains(" ") || fileName.Contains(":") || fileName.Contains("\\") || fileName.Contains("/") 
                    || fileName.ToUTF8() != fileName)
                    throw new Exception("Enter the global results file name (.frd) without path. " + 
                                        "The results file name must not contain any special characters.");
                _modelProperties.GlobalResultsFileName = fileName; 
            }
        }
        // Slip wear model
        [Category("Slip wear model")]
        [OrderedDisplayName(0, 10, "Results")]
        [Description("Select the results of the slip wear model to be recorded.")]
        [Id(1, 3)]
        public SlipWearResultsEnum SlipWearResults
        {
            get { return _modelProperties.SlipWearResults; }
            set { _modelProperties.SlipWearResults = value; }
        }
        //
        [Category("Slip wear model")]
        [OrderedDisplayName(1, 10, "Number of cycles")]
        [Description("Set the number of slip wear cycles.")]
        [Id(2, 3)]
        public int NumberOfCycles
        {
            get { return _modelProperties.NumberOfCycles; }
            set { _modelProperties.NumberOfCycles = value; }
        }
        //
        [Category("Slip wear model")]
        [OrderedDisplayName(2, 10, "Cycles increment")]
        [Description("Set the increment of slip wear cycles.")]
        [Id(3, 3)]
        public int CyclesIncrement
        {
            get { return _modelProperties.CyclesIncrement; }
            set { _modelProperties.CyclesIncrement = value; }
        }
        //
        [Category("Slip wear model")]
        [OrderedDisplayName(3, 10, "BDM remeshing")]
        [Description("Use boundary displacement method (BDM) for remeshing due to the applied surface wear displacements " +
                     "after each wear cycle. Boundary displacement step must be used to define fixed model regions to prevent all " +
                     "rigid body motions.")]
        [Id(4, 3)]
        public bool BdmRemeshing { get { return _modelProperties.BdmRemeshing; } set { _modelProperties.BdmRemeshing = value; } }
        // Physical constants
        [Category("Physical constants")]
        [OrderedDisplayName(0, 10, "Absolute zero")]
        [Description("Value of the absolute zero temperature.")]
        [TypeConverter(typeof(StringTemperatureUndefinedConverter))]
        [Id(1, 4)]
        public double AbsoluteZero { get { return _modelProperties.AbsoluteZero; } set { _modelProperties.AbsoluteZero = value; } }
        //
        [Category("Physical constants")]
        [OrderedDisplayName(1, 10, "Stefan-Boltzmann const")]
        [Description("Value of the Stefan-Boltzmann constant.")]
        [TypeConverter(typeof(StringStefanBoltzmannUndefinedConverter))]
        [Id(2, 4)]
        public double StefanBoltzmann
        {
            get { return _modelProperties.StefanBoltzmann; }
            set { _modelProperties.StefanBoltzmann = value; }
        }
        //
        [Category("Physical constants")]
        [OrderedDisplayName(2, 10, "Gravitational const")]
        [Description("Value of the Newton gravitational constant.")]
        [TypeConverter(typeof(StringNewtonGravityUndefinedConverter))]
        [Id(3, 4)]
        public double NewtonGravity
        {
            get { return _modelProperties.NewtonGravity; }
            set { _modelProperties.NewtonGravity = value; }
        }


        // Constructors                                                                                                             
        public ViewModelProperties(ModelProperties modelProperties)
        {
            _name = "Empty";
            _modelProperties = modelProperties;
            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(BdmRemeshing));
            //
            FilteredFileNameEditor.Filter = "Result files|*.frd";
            //
            UpdateVisibility();
        }


        // Methods                                                                                                                  
        public ModelProperties GetBase()
        {
            return _modelProperties;
        }
        private void UpdateVisibility()
        {
            if (_modelProperties.ModelSpace.IsTwoD())
            {
                _dctd.GetProperty(nameof(ModelSpace)).RemoveStandardValues(
                    new HashSet<string>() { ModelSpaceEnum.ThreeD.ToString() });
            }
            else
            {
                _dctd.GetProperty(nameof(ModelSpace)).RemoveStandardValues(
                    new HashSet<string>() { ModelSpaceEnum.PlaneStress.ToString(),
                                            ModelSpaceEnum.PlaneStrain.ToString(),
                                            ModelSpaceEnum.Axisymmetric.ToString() });
            }
            //
            bool subModel = _modelProperties.ModelType == ModelType.Submodel;
            bool slipWearModel = _modelProperties.ModelType == ModelType.SlipWearModel;
            //
            _dctd.GetProperty(nameof(GlobalResultsFileName)).SetIsBrowsable(subModel);
            _dctd.GetProperty(nameof(SlipWearResults)).SetIsBrowsable(slipWearModel);
            _dctd.GetProperty(nameof(NumberOfCycles)).SetIsBrowsable(slipWearModel);
            _dctd.GetProperty(nameof(CyclesIncrement)).SetIsBrowsable(slipWearModel);
            _dctd.GetProperty(nameof(BdmRemeshing)).SetIsBrowsable(slipWearModel);
        }


    }
}
