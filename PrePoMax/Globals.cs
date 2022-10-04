using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PrePoMax
{
    public class Globals
    {
        public static string HomePage = "https://prepomax.fs.um.si/";
        //
        public static string ProgramName = "PrePoMax v1.3.5";
        //
        public static string ReadyText = "Ready";
        public static string OpeningText = "Opening...";
        public static string ImportingText = "Importing...";
        public static string SavingText = "Saving...";
        public static string SavingAsText = "Saving As...";
        public static string ExportingText = "Exporting...";
        public static string TransformingText = "Transforming...";
        public static string PreviewText = "Preview...";
        public static string MeshingText = "Meshing...";
        public static string UndoingText = "Undoing...";
        public static string CreatingCompoundText = "Creating compound...";
        public static string RegeneratingCompoundText = "Regenerating compound...";
        public static string RegeneratingText = "Regenerating history...";
        public static string FlippingNormalsText = "Flipping normals...";
        public static string SplittingFacesText = "Splitting faces...";
        public static string ExplodePartsText = "Explode parts...";
        //
        public static string SettingsFileName = "settings.bin";
        public static string MaterialLibraryFileName = "materials.lib";
        //
        public static string HistoryFileName = "history";
        // Settings
        public static string GeneralSettingsName = "General";
        public static string GraphicsSettingsName = "Graphics";
        public static string ColorSettingsName = "Default Colors";
        public static string AnnotationSettingsName = "Annotations";
        public static string MeshingSettingsName = "Meshing";
        public static string PreSettingsName = "Pre-processing";
        public static string CalculixSettingsName = "Calculix";
        public static string PostSettingsName = "Post-processing";
        public static string LegendSettingsName = "Legend";
        public static string StatusBlockSettingsName = "Status block";
        public static string UnitSystemSettingsName = "Unit system";
        //
        public static string NetGenMesher = @"\NetGen\NetGenMesher.exe";
        public static string MmgsMesher = @"\NetGen\mmgs.exe";
        public static string VisFileName = "geometry.vis";
        public static string BrepFileName = "geometry.brep";
        public static string StlFileName = "geometry.stl";        
        public static string MeshParametersFileName = "meshParameters";
        public static string MeshRefinementFileName = "meshRefinement";
        public static string VolFileName = "geometry.vol";
        public static string MmgMeshFileName = "geometry.mesh";
        public static string EdgeNodesFileName = "edgeNodes";
        // Names
        public static string NameSeparator = ":";        
        public static string MissingSectionName = "Missing_section";
        // Graphics
        public static int BeamNodeSize = 5;
    }
}
