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
        private Color _undeformedModelColor;
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
            get { return _undeformedModelColor; }
            set { _undeformedModelColor = value; }
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
            _undeformedModelColor = Color.Black;
            //
            _showMinValueLocation = false;
            _showMaxValueLocation = true;
            //
            _maxHistoryEntriesToShow = 100;
        }
        
    }
}
