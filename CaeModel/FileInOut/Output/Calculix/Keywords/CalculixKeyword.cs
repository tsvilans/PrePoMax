using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    public abstract class CalculixKeyword
    {
        // Variables                                                                                                                
        protected List<CalculixKeyword> _keywords;


        // Properties                                                                                                               
        public List<CalculixKeyword> Keywords { get { return _keywords; } }


        // Constructor                                                                                                              
        public CalculixKeyword()
        {
            _keywords = new List<CalculixKeyword>();
        }


        // Methods                                                                                                                  
        public void AddKeyword(CalculixKeyword keyword)
        {
            _keywords.Add(keyword);
        }
        public void ClearKeywords()
        {
            _keywords.Clear();
        }
        public abstract string GetKeywordString();
        public abstract string GetDataString();
        //
        protected string GetComplexLoadCase(ComplexLoadTypeEnum complexLoadType)
        {
            string loadCase;
            if (complexLoadType == ComplexLoadTypeEnum.None) loadCase = "";
            else if (complexLoadType == ComplexLoadTypeEnum.Real) loadCase = ", Load case=1";
            else if (complexLoadType == ComplexLoadTypeEnum.Imaginary) loadCase = ", Load case=2";
            else throw new NotImplementedException();
            //
            return loadCase;
        }
        protected double GetComplexRatio(ComplexLoadTypeEnum complexLoadType, double angleDeg)
        {
            double ratio;
            if (complexLoadType == ComplexLoadTypeEnum.None)
            {
                ratio = 1;
            }
            else if (complexLoadType == ComplexLoadTypeEnum.Real)
            {
                if (angleDeg == 0) ratio = 1;
                else if (angleDeg == 90) ratio = 0;
                else if (angleDeg == 180) ratio = -1;
                else if (angleDeg == 270) ratio = 0;
                else ratio = Math.Cos(angleDeg * Math.PI / 180);
            }
            else if (complexLoadType == ComplexLoadTypeEnum.Imaginary)
            {
                if (angleDeg == 0) ratio = 0;
                else if (angleDeg == 90) ratio = 1;
                else if (angleDeg == 180) ratio = 0;
                else if (angleDeg == 270) ratio = -1;
                else ratio = Math.Sin(angleDeg * Math.PI / 180);
            }
            else throw new NotSupportedException();
            //
            return ratio;
        }


    }
}
