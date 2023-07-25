using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    class HOSetNames
    {
        public const string ContactWear = "CONTACT_WEAR";
        public const string AllContactElements = "ALL_CONTACT_ELEMENTS";
    }
    class HOFieldNames
    {
        public const string ComplexRealSuffix = " RE";
        public const string ComplexImaginarySuffix = " IM";
        public const string ComplexMagnitudeSuffix = " MAG";
        public const string ComplexPhaseSuffix = " PHA";
        //
        public const string Time = "TIME";
        public const string Frequency = "FREQUENCY";
        // Nodal
        public const string Coordinates = "COORDINATES";
        public const string Displacements = "DISPLACEMENTS";
        public const string Velocities = "VELOCITIES";
        public const string Forces = "FORCES";
        public const string TotalForce = "TOTAL FORCE";
        public const string Stresses = "STRESSES";
        public const string Strains = "STRAINS";
        public const string MechanicalStrains = "MECHANICAL STRAINS";
        public const string EquivalentPlasticStrains = "EQUIVALENT PLASTIC STRAIN";
        public const string InternalEnergyDensity = "INTERNAL ENERGY DENSITY";
        // Thermal
        public const string Temperatures = "TEMPERATURES";
        public const string HeatGeneration = "HEAT GENERATION";
        public const string TotalHeatGeneration = "TOTAL HEAT GENERATION";
        // Frequency
        public const string EigenvalueOutput = "EIGENVALUE OUTPUT";
        public const string ParticipationFactors = "PARTICIPATION FACTORS";
        public const string EffectiveModalMass = "EFFECTIVE MODAL MASS";
        public const string TotalEffectiveModalMass = "TOTAL EFFECTIVE MODAL MASS";
        public const string TotalEffectiveMass = "TOTAL EFFECTIVE MASS";
        public const string RelativeEffectiveModalMass = "RELATIVE EFFECTIVE MODAL MASS";
        public const string RelativeTotalEffectiveModalMass = "RELATIVE TOTAL EFFECTIVE MODAL MASS";
        // Contact
        public const string RelativeContactDisplacement = "RELATIVE CONTACT DISPLACEMENT";
        public const string ContactStress = "CONTACT STRESS";
        public const string ContactPrintEnergy = "CONTACT PRINT ENERGY";
        public const string TotalNumberOfContactElements = "TOTAL NUMBER OF CONTACT ELEMENTS";
        public const string StatisticsForSlaveSet = "STATISTICS FOR SLAVE SET";
        public const string TotalSurfaceForce = "TOTAL SURFACE FORCE";
        public const string MomentAboutOrigin = "MOMENT ABOUT ORIGIN";
        public const string CenterOgGravityCG = "CENTER OF GRAVITY CG";
        public const string MeanSurfaceNormal = "MEAN SURFACE NORMAL";
        public const string MomentAboutCG = "MOMENT ABOUT CG";
        public const string SurfaceArea = "SURFACE AREA";
        public const string NormalSurfaceForce = "NORMAL SURFACE FORCE";
        public const string ShearSurfaceForce = "SHEAR SURFACE FORCE";
        // Element
        public const string Volume = "VOLUME";
        public const string TotalVolume = "TOTAL VOLUME";
        public const string InternalEnergy = "INTERNAL ENERGY";
        public const string TotalInternalEnergy = "TOTAL INTERNAL ENERGY";
        // Thermal
        public const string HeatFlux = "HEAT FLUX";
        public const string BodyHeating = "BODY HEATING";
        public const string TotalBodyHeating = "TOTAL BODY HEATING";
        // Wear
        public const string SlidingDistance = "SLIDING DISTANCE";
        public const string SurfaceNormal = "SURFACE NORMAL";
        // Error
        public const string Error = "ERROR";


        // Methods
        public static string GetNoSuffixName(string name)
        {
            if (name.EndsWith(ComplexRealSuffix))
                return name.Substring(0, name.Length - ComplexRealSuffix.Length);
            else if (name.EndsWith(ComplexImaginarySuffix))
                return name.Substring(0, name.Length - ComplexImaginarySuffix.Length);
            else if (name.EndsWith(ComplexMagnitudeSuffix))
                return name.Substring(0, name.Length - ComplexMagnitudeSuffix.Length);
            else if (name.EndsWith(ComplexPhaseSuffix))
                return name.Substring(0, name.Length - ComplexPhaseSuffix.Length);
            else return name;
        }
        public static bool HasRealComplexSuffix(string name)
        {
            if (name.EndsWith(ComplexRealSuffix)) return true;
            else return false;
        }
        public static bool HasComplexSuffix(string name)
        {
            if (name.EndsWith(ComplexRealSuffix) ||
                name.EndsWith(ComplexImaginarySuffix) ||
                name.EndsWith(ComplexMagnitudeSuffix) ||
                name.EndsWith(ComplexPhaseSuffix))
                return true;
            else return false;
        }
    }
    //
    class HOComponentNames
    {
        public const string All = "ALL";
        //
        public const string Mises = "MISES";
        public const string Tresca = "TRESCA";
        public const string S11 = "S11";
        public const string S22 = "S22";
        public const string S33 = "S33";
        public const string S12 = "S12";
        public const string S23 = "S23";
        public const string S13 = "S13";
        //
        public const string SgnMaxAbsPri = "SGN-MAX-ABS-PRI";
        public const string PrincipalMax = "PRINCIPAL-MAX";
        public const string PrincipalMid = "PRINCIPAL-MID";
        public const string PrincipalMin = "PRINCIPAL-MIN";
        //
        public const string E11 = "E11";
        public const string E22 = "E22";
        public const string E33 = "E33";
        public const string E12 = "E12";
        public const string E23 = "E23";
        public const string E13 = "E13";
        //
        public const string Tang1 = "TANG1";
        public const string Tang2 = "TANG2";
        public const string S1 = "S1";
        public const string S2 = "S2";
        //
        public const string N1 = "N1";
        public const string N2 = "N2";
        public const string N3 = "N3";
        //
        public const string EIGENVALUE = "EIGENVALUE";
        public const string OMEGA = "OMEGA";
        public const string FREQUENCY = "FREQUENCY";
        public const string FREQUENCY_IM = "FREQUENCY IM";
        //
        public const string XCOMPONENT = "X COMPONENT";
        public const string YCOMPONENT = "Y COMPONENT";
        public const string ZCOMPONENT = "Z COMPONENT";
        public const string XROTATION = "X ROTATION";
        public const string YROTATION = "Y ROTATION";
        public const string ZROTATION = "Z ROTATION";
        //
        public const string X = "X";
        public const string Y = "Y";
        public const string Z = "Z";

    }


}
