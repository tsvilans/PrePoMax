using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    public class FOFieldNames
    {
        public const string None = "NONE";                              // none
        public const string Default = "DEFAULT";                        // node: used for default scaling of nodes
        //
        public const string Disp = "DISP";                              // scalar
        public const string DispR = "DISPR";                            // scalar
        public const string DispI = "DISPI";                            // scalar
        public const string DispMag = "DISPMAG";                        // scalar
        public const string DispPha = "DISPPHA";                        // scalar
        public const string DispMax = "DISPMAX";                        // scalar
        public const string DispMaxAng = "DISPMAXANG";                  // scalar
        public const string DispMin = "DISPMIN";                        // scalar
        public const string DispMinAng = "DISPMINANG";                  // scalar
        public const string PDisp = "PDISP";                            // scalar - kind of: mag, pha
        // Velocity
        public const string Velo = "VELO";                              // vector
        // Stress
        public const string Stress = "STRESS";                          // tensor
        public const string StressR = "STRESSR";                        // tensor
        public const string StressI = "STRESSI";                        // tensor
        public const string StressMag = "STRESSMAG";                    // tensor
        public const string StressPha = "STRESSPHA";                    // tensor
        public const string StressMax = "STRESSMAX";                    // tensor
        public const string StressMaxAng = "STRESSMAXANG";              // tensor
        public const string StressMin = "STRESSMIN";                    // tensor
        public const string StressMinAng = "STRESSMINANG";              // tensor
        public const string PStress = "PSTRESS";                        // scalar - kind of: mag, pha
        public const string ZZStr = "ZZSTR";                            // tensor
        public const string ZZStrR = "ZZSTRR";                          // tensor
        public const string ZZStrI = "ZZSTRI";                          // tensor
        public const string ZZStrMag = "ZZSTRMAG";                      // tensor
        public const string ZZStrPha = "ZZSTRPHA";                      // tensor
        public const string ZZStrMax = "ZZSTRMAX";                      // tensor
        public const string ZZStrMaxAng = "ZZSTRMAXANG";                // tensor
        public const string ZZStrMin = "ZZSTRMIN";                      // tensor
        public const string ZZStrMinAng = "ZZSTRMINANG";                // tensor
        // Strain
        public const string ToStrain = "TOSTRAIN";                      // tensor
        public const string ToStraiR = "TOSTRAIR";                      // tensor
        public const string ToStraiI = "TOSTRAII";                      // tensor
        public const string ToStraiMag = "TOSTRAIMAG";                  // tensor
        public const string ToStraiPha = "TOSTRAIPHA";                  // tensor
        public const string ToStraiMax = "TOSTRAIMAX";                  // tensor
        public const string ToStraiMaxAng = "TOSTRAIMAXANG";            // tensor
        public const string ToStraiMin = "TOSTRAIMIN";                  // tensor
        public const string ToStraiMinAng = "TOSTRAIMINANG";            // tensor
        public const string MeStrain = "MESTRAIN";                      // tensor
        public const string MeStraiR = "MESTRAIR";                      // tensor
        public const string MeStraiI = "MESTRAII";                      // tensor
        public const string MeStraiMag = "MESTRAIMAG";                  // tensor
        public const string MeStraiPha = "MESTRAIPHA";                  // tensor
        public const string MeStraiMax = "MESTRAIMAX";                  // tensor
        public const string MeStraiMaxAng = "MESTRAIMAXANG";            // tensor
        public const string MeStraiMin = "MESTRAIMIN";                  // tensor
        public const string MeStraiMinAng = "MESTRAIMINANG";            // tensor
        public const string Pe = "PE";                                  // scalar
        //
        public const string Forc = "FORC";                              // vector
        public const string ForcR = "FORCR";                            // vector
        public const string ForcI = "FORCI";                            // vector
        public const string ForcMag = "FORCMAG";                        // vector
        public const string ForcPha = "FORCPHA";                        // vector
        public const string ForcMax = "FORCMAX";                        // vector
        public const string ForcMaxAng = "FORCMAXANG";                  // vector
        public const string ForcMin = "FORCMIN";                        // vector
        public const string ForcMinAng = "FORCMINANG";                  // vector
        public const string Ener = "ENER";                              // scalar
        public const string Contact = "CONTACT";                        // scalar
        // Thermal
        public const string NdTemp = "NDTEMP";                          // scalar
        public const string PNdTemp = "PNDTEMP";                        // scalar - kind of: mag, pha
        public const string Flux = "FLUX";                              // vector
        public const string Rfl = "RFL";                                // scalar
        public const string HError = "HERROR";                          // scalar
        public const string HErrorR = "HERRORR";                        // scalar
        public const string HErrorI = "HERRORI";                        // scalar
        public const string HErrorMag = "HERRORMAG";                    // scalar
        public const string HErrorPha = "HERRORPHA";                    // scalar
        public const string HErrorMax = "HERRORMAX";                    // scalar
        public const string HErrorMaxAng = "HERRORMAXANG";              // scalar
        public const string HErrorMin = "HERRORMIN";                    // scalar
        public const string HErrorMinAng = "HERRORMINANG";              // scalar
        // Sensitivity
        public const string Norm = "NORM";                              // vector
        public const string SenFreq = "SENFREQ";                        // scalar
        // Wear
        public const string SlidingDistance = "SLIDING_DISTANCE";       // scalar
        public const string SurfaceNormal = "SURFACE_NORMAL";           // vector
        public const string WearDepth = "WEAR_DEPTH";                   // vector
        public const string MeshDeformation = "MESH_DEF";               // vector
        public const string DispDeformationDepth = "DISP_DEF_DEPTH";    // vector
        // Imported pressure
        public const string Distance = "DISTANCE";                      // vector
        public const string Imported = "IMPORTED";                      // scalar
        //
        public const string Error = "ERROR";                            // scalar
        public const string ErrorR = "ERRORR";                          // scalar
        public const string ErrorI = "ERRORI";                          // scalar
        public const string ErrorMag = "ERRORMAG";                      // scalar
        public const string ErrorPha = "ERRORPHA";                      // scalar
        public const string ErrorMax = "ERRORMAX";                      // scalar
        public const string ErrorMaxAng = "ERRORMAXANG";                // scalar
        public const string ErrorMin = "ERRORMIN";                      // scalar
        public const string ErrorMinAng = "ERRORMINANG";                // scalar


        // Methods
        public static bool IsVisible(string fieldName)
        {
            switch (fieldName)
            {
                case DispR:
                case DispI:
                case DispMag:
                case DispPha:
                case DispMax:
                case DispMaxAng:
                case DispMin:
                case DispMinAng:
                case StressR:
                case StressI:
                case StressMag:
                case StressPha:
                case StressMax:
                case StressMaxAng:
                case StressMin:
                case StressMinAng:
                case ZZStrR:
                case ZZStrI:
                case ZZStrMag:
                case ZZStrPha:
                case ZZStrMax:
                case ZZStrMaxAng:
                case ZZStrMin:
                case ZZStrMinAng:
                case ToStraiR:
                case ToStraiI:
                case ToStraiMag:
                case ToStraiPha:
                case ToStraiMax:
                case ToStraiMaxAng:
                case ToStraiMin:
                case ToStraiMinAng:
                case MeStraiR:
                case MeStraiI:
                case MeStraiMag:
                case MeStraiPha:
                case MeStraiMax:
                case MeStraiMaxAng:
                case MeStraiMin:
                case MeStraiMinAng:
                case ForcR:
                case ForcI:
                case ForcMag:
                case ForcPha:
                case ForcMax:
                case ForcMaxAng:
                case ForcMin:
                case ForcMinAng:
                case HErrorR:
                case HErrorI:
                case HErrorMag:
                case HErrorPha:
                case HErrorMax:
                case HErrorMaxAng:
                case HErrorMin:
                case HErrorMinAng:
                case ErrorR:
                case ErrorI:
                case ErrorMag:
                case ErrorPha:
                case ErrorMax:
                case ErrorMaxAng:
                case ErrorMin:
                case ErrorMinAng:
                    return false;
                default:
                    return true;
            }
        }
        public static DataTypeEnum GetDataType(string fieldName)
        {
            switch (fieldName)
            {
                case None:
                case Default:
                    return DataTypeEnum.None;
                //
                case PDisp:
                case PStress:
                case Pe:
                case Ener:
                case Contact:
                case NdTemp:
                case PNdTemp:
                case Rfl:
                case HError:
                case HErrorR:
                case HErrorI:
                case HErrorMag:
                case HErrorPha:
                case HErrorMax:
                case HErrorMaxAng:
                case HErrorMin:
                case HErrorMinAng:
                case SenFreq:
                case Imported:
                case Error:
                case ErrorR:
                case ErrorI:
                case ErrorMag:
                case ErrorPha:
                case ErrorMax:
                case ErrorMaxAng:
                case ErrorMin:
                case ErrorMinAng:
                    return DataTypeEnum.Scalar;
                //
                case Disp:
                case DispR:
                case DispI:
                case DispMag:
                case DispPha:
                case DispMax:
                case DispMaxAng:
                case DispMin:
                case DispMinAng:
                case Velo:
                case Forc:
                case ForcR:
                case ForcI:
                case ForcMag:
                case ForcPha:
                case ForcMax:
                case ForcMaxAng:
                case ForcMin:
                case ForcMinAng:
                case Flux:
                case Norm:
                case SurfaceNormal:
                case WearDepth:
                case MeshDeformation:
                case DispDeformationDepth:
                case Distance:
                    return DataTypeEnum.Vector;
                //
                case Stress:
                case StressR:
                case StressI:
                case StressMag:
                case StressPha:
                case StressMax:
                case StressMaxAng:
                case StressMin:
                case StressMinAng:
                case ZZStr:
                case ZZStrR:
                case ZZStrI:
                case ZZStrMag:
                case ZZStrPha:
                case ZZStrMax:
                case ZZStrMaxAng:
                case ZZStrMin:
                case ZZStrMinAng:
                case ToStrain:
                case ToStraiR:
                case ToStraiI:
                case ToStraiMag:
                case ToStraiPha:
                case ToStraiMax:
                case ToStraiMaxAng:
                case ToStraiMin:
                case ToStraiMinAng:
                case MeStrain:
                case MeStraiR:
                case MeStraiI:
                case MeStraiMag:
                case MeStraiPha:
                case MeStraiMax:
                case MeStraiMaxAng:
                case MeStraiMin:
                case MeStraiMinAng:
                    return DataTypeEnum.Tensor;
                default:
                    return DataTypeEnum.None;
            }
        }
        public static Dictionary<string, string[]> GetSafetyFactorFieldComponentNames()
        {
            Dictionary<string, string[]> fieldNameComponentName = new Dictionary<string, string[]>
            {
                //{Disp, new string[] { FOComponentNames.All, FOComponentNames.U1, FOComponentNames.U2, FOComponentNames.U3 } },
                { Stress, new string[] { FOComponentNames.Mises, FOComponentNames.Tresca,
                                         FOComponentNames.S11, FOComponentNames.S22, FOComponentNames.S33,
                                         FOComponentNames.S12, FOComponentNames.S23, FOComponentNames.S13,
                                         FOComponentNames.SgnMaxAbsPri,
                                         FOComponentNames.PrincipalMax,
                                         FOComponentNames.PrincipalMid,
                                         FOComponentNames.PrincipalMin} },
                //{ Pe, new string[] { FOComponentNames.PE } }
            };
            return fieldNameComponentName;
        }
    }

    public class FOComponentNames
    {
        public const string None = "NONE";
        //
        public const string All = "ALL";
        //
        public const string U1 = "U1";
        public const string U2 = "U2";
        public const string U3 = "U3";
        //
        public const string MAG1 = "MAG1";
        public const string MAG2 = "MAG2";
        public const string MAG3 = "MAG3";
        public const string PHA1 = "PHA1";
        public const string PHA2 = "PHA2";
        public const string PHA3 = "PHA3";
        //
        public const string V1 = "V1";
        public const string V2 = "V2";
        public const string V3 = "V3";
        //
        public const string F1 = "F1";
        public const string F2 = "F2";
        public const string F3 = "F3";
        // Stress
        public const string Mises = "MISES";
        public const string Tresca = "TRESCA";
        public const string S11 = "S11";
        public const string S22 = "S22";
        public const string S33 = "S33";
        public const string S12 = "S12";
        public const string S23 = "S23";
        public const string S13 = "S13";
        public const string SgnMaxAbsPri = "SGN-MAX-ABS-PRI";
        public const string PrincipalMax = "PRINCIPAL-MAX";
        public const string PrincipalMid = "PRINCIPAL-MID";
        public const string PrincipalMin = "PRINCIPAL-MIN";
        //
        public const string MAGXX = "MAGXX";
        public const string MAGYY = "MAGYY";
        public const string MAGZZ = "MAGZZ";
        public const string MAGXY = "MAGXY";
        public const string MAGYZ = "MAGYZ";
        public const string MAGZX = "MAGZX";
        public const string PHAXX = "PHAXX";
        public const string PHAYY = "PHAYY";
        public const string PHAZZ = "PHAZZ";
        public const string PHAXY = "PHAXY";
        public const string PHAYZ = "PHAYZ";
        public const string PHAZX = "PHAZX";
        // Strain
        public const string ME11 = "ME11";
        public const string ME22 = "ME22";
        public const string ME33 = "ME33";
        public const string ME12 = "ME12";
        public const string ME23 = "ME23";
        public const string ME13 = "ME13";
        //
        public const string E11 = "E11";
        public const string E22 = "E22";
        public const string E33 = "E33";
        public const string E12 = "E12";
        public const string E23 = "E23";
        public const string E13 = "E13";
        //
        public const string PE = "PE";
        // Thermal
        public const string T = "T";
        // Contact
        public const string COpen = "COPEN";
        public const string CSlip1 = "CSLIP1";
        public const string CSlip2 = "CSLIP2";
        //
        public const string CPress = "CPRESS";
        public const string CShear1 = "CSHEAR1";
        public const string CShear2 = "CSHEAR2";
        //
        public const string N1 = "N1";
        public const string N2 = "N2";
        public const string N3 = "N3";
        //
        public const string H1 = "H1";
        public const string H2 = "H2";
        public const string H3 = "H3";
        // Imported pressure
        public const string D1 = "D1";
        public const string D2 = "D2";
        public const string D3 = "D3";
        public const string PRESS = "PRESS";
        //
        public const string Ratio = "RATIO";
        public const string SafetyFactor = "SAFETY_FACTOR";
        public const string Min = "MIN";
        public const string Max = "MAX";
        public const string Average = "AVERAGE";
        //
        //public const string UH1 = "UH1";
        //public const string UH2 = "UH2";
        //public const string UH3 = "UH3";
    }
}

