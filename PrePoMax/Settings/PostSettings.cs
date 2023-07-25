using System;
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
    public enum UndeformedModelTypeEnum
    {
        [StandardValue("None", Description = "None")]
        None,
        //
        [StandardValue("WireframeBody", DisplayName = "Wireframe body", Description = "Wireframe body")]
        WireframeBody,
        //
        [StandardValue("SolidBody", DisplayName = "Solid body", Description = "Solid body")]
        SolidBody
    }


    [Serializable]
    public class PostSettings : ISettings
    {
        // Variables                                                                                                                
        private string _deformationFieldOutputName;
        private UndeformedModelTypeEnum _undeformedModelType;
        private Color _undeformedWireModelColor;
        private Color _undeformedSolidModelColor;
        private bool _showMinValueLocation;
        private bool _showMaxValueLocation;
        private int _maxHistoryEntriesToShow;


        // Properties                                                                                                               
        public UndeformedModelTypeEnum UndeformedModelType
        {
            get { return _undeformedModelType; }
            set { _undeformedModelType = value; }
        }
        public Color UndeformedModelColor
        {
            get
            {
                // White color is used for times where no undeformed geometry is displayed
                if (_undeformedModelType == UndeformedModelTypeEnum.None) return Color.White;
                else if (_undeformedModelType == UndeformedModelTypeEnum.WireframeBody) return _undeformedWireModelColor;
                else if (_undeformedModelType == UndeformedModelTypeEnum.SolidBody) return _undeformedSolidModelColor;
                else throw new NotSupportedException();
            }
            set
            {
                if (_undeformedModelType == UndeformedModelTypeEnum.None) { }
                else if (_undeformedModelType == UndeformedModelTypeEnum.WireframeBody) _undeformedWireModelColor = value;
                else if (_undeformedModelType == UndeformedModelTypeEnum.SolidBody) _undeformedSolidModelColor = value;
                else throw new NotSupportedException();
            }
        }
        public Color UndeformedWireModelColor
        {
            get { return _undeformedWireModelColor; }
            set { _undeformedWireModelColor = value; }
        }
        public Color UndeformedSolidModelColor
        {
            get { return _undeformedSolidModelColor; }
            set { _undeformedSolidModelColor = value; }
        }
        public bool ShowMinValueLocation { get { return _showMinValueLocation; } set { _showMinValueLocation = value; } }
        public bool ShowMaxValueLocation { get { return _showMaxValueLocation; } set { _showMaxValueLocation = value; } }
        public int MaxHistoryEntriesToShow
        {
            get { return _maxHistoryEntriesToShow; }
            set
            {
                _maxHistoryEntriesToShow = value;
                if (_maxHistoryEntriesToShow < 1) _maxHistoryEntriesToShow = 1;
            } 
        }


        // Constructors                                                                                                             
        public PostSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _deformationFieldOutputName = CaeResults.FeResults.GetPossibleDeformationFieldOutputNames()[0];
            _undeformedModelType = UndeformedModelTypeEnum.WireframeBody;
            _undeformedWireModelColor = Color.Black;
            _undeformedSolidModelColor = Color.FromArgb(128, Color.Gray);
            //
            _showMinValueLocation = false;
            _showMaxValueLocation = true;
            //
            _maxHistoryEntriesToShow = 100;
        }
        
    }
}
