using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    public class FOFieldNames
    {
        public const string None = "NONE";
        public const string Default = "DEFAULT";
        //
        public const string Disp = "DISP";
        public const string Dispi = "DISPI";
        // Stress
        public const string Stress = "STRESS";
        public const string Stressi = "STRESSI";
        public const string ZZStr = "ZZSTR";
        public const string ZZStri = "ZZSTRI";
        // Strain
        public const string ToStrain = "TOSTRAIN";
        public const string ToStraii = "TOSTRAII";
        public const string MeStrain = "MESTRAIN";
        public const string MeStraii = "MESTRAII";
        public const string Pe = "PE";
        //
        public const string Forc = "FORC";
        public const string Forci = "FORCI";
        public const string Ener = "ENER";
        public const string Contact = "CONTACT";
        // Thermal
        public const string NdTemp = "NDTEMP";
        public const string Flux = "FLUX";
        public const string Rfl = "RFL";
        public const string HError = "HERROR";
        public const string HErrori = "HERRORI";
        // Sensitivity
        public const string Norm = "NORM";
        public const string SenFreq = "SENFREQ";
        // Wear
        public const string SlidingDistance = "SLIDING_DISTANCE";
        public const string SurfaceNormal = "SURFACE_NORMAL";
        public const string WearDepth = "WEAR_DEPTH";
        public const string MeshDeformation = "MESH_DEF";
        public const string DispDeformationDepth = "DISP_DEF_DEPTH";
        // Imported pressure
        public const string Distance = "DISTANCE";
        public const string Imported = "IMPORTED";
        //
        public const string Error = "ERROR";
        public const string Errori = "ERRORI";
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
        //public const string UH1 = "UH1";
        //public const string UH2 = "UH2";
        //public const string UH3 = "UH3";
    }
}
