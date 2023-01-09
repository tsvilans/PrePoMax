using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class StepCollection
    {
        // Variables                                                                                                                
        private List<Step> _steps;


        // Properties                                                                                                               
        public List<Step> StepsList { get { return _steps; } }


        // Constructors                                                                                                             
        public StepCollection()
        {
            _steps = new List<Step>();
        }


        // Static Methods                                                                                                           
        public bool MulitRegionSelectionExists(string stepName, IMultiRegion newRegion)
        {
            List<Step> prevSteps = new List<Step>();
            foreach (var step in _steps)
            {
                if (step.Name != stepName) prevSteps.Add(step);
                else break;
            }
            //
            List<IMultiRegion> existingRegions = new List<IMultiRegion>();
            foreach (var step in prevSteps)
            {
                if (newRegion is HistoryOutput) existingRegions.AddRange(step.HistoryOutputs.Values);
                else if (newRegion is BoundaryCondition) existingRegions.AddRange(step.BoundaryConditions.Values);
                else if (newRegion is Load) existingRegions.AddRange(step.Loads.Values);
                else if (newRegion is DefinedField) existingRegions.AddRange(step.DefinedFields.Values);
                else throw new NotSupportedException();
            }
            //
            return MulitRegionSelectionExists(existingRegions, newRegion);
        }
        private bool MulitRegionSelectionExists(List<IMultiRegion> existingRegions, IMultiRegion newRegion)
        {
            foreach (var existingRegion in existingRegions)
            {
                if (IsSelectionRegionEqual(existingRegion, newRegion))
                {
                    newRegion.RegionName = existingRegion.RegionName;
                    newRegion.RegionType = existingRegion.RegionType;
                    return true;
                }
            }
            return false;
        }
        public static bool MultiRegionChanged(IMultiRegion oldRegion, IMultiRegion newRegion)
        {
            if (IsSelectionRegionEqual(oldRegion, newRegion))
            {
                // Region remained the same
                newRegion.RegionName = oldRegion.RegionName;
                newRegion.RegionType = oldRegion.RegionType;
                return false;
            }
            else
            {
                // Region changed
                return true;
            }
        }
        private static bool IsSelectionRegionEqual(IMultiRegion existingRegion, IMultiRegion newRegion)
        {
            // IsRegionNameSelection(newRegion.RegionName) is used for Propagate
            if ((newRegion.RegionType == RegionTypeEnum.Selection || IsRegionNameSelection(newRegion.RegionName)) &&
                existingRegion.CreationIds != null && newRegion.CreationIds != null &&
                IsRegionNameSelection(existingRegion.RegionName) &&
                newRegion.CreationIds.Length == existingRegion.CreationIds.Length &&
                newRegion.GetType() == existingRegion.GetType() &&
                newRegion.CreationIds.Except(existingRegion.CreationIds).Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool IsRegionNameSelection(string regionName)
        {
            return regionName.StartsWith(CaeMesh.Globals.InternalSelectionName);
        }


        // Methods                                                                                                                  
        public void AddStep(Step step, bool copyBCsAndLoads)
        {
            if (copyBCsAndLoads && _steps.Count >= 1)
            {
                Step lastStep = _steps.Last();
                //
                foreach (var entry in lastStep.BoundaryConditions)
                {
                    step.AddBoundaryCondition(entry.Value.DeepClone());
                }
                foreach (var entry in lastStep.Loads)
                {
                    step.AddLoad(entry.Value.DeepClone());
                }
                foreach (var entry in lastStep.DefinedFields)
                {
                    step.AddDefinedField(entry.Value.DeepClone());
                }
            }
            _steps.Add(step);
        }
        public Step GetStep(string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName) return step;
            }
            return null;
        }
        public string[] GetStepNames()
        {
            List<string> names = new List<string>();
            foreach (var step in _steps) names.Add(step.Name);
            return names.ToArray();
        }
        public void ReplaceStep(string oldStepName, Step newStep)
        {
            List<Step> newSteps = new List<Step>();
            foreach (var step in _steps)
            {
                if (step.Name == oldStepName) newSteps.Add(newStep);
                else newSteps.Add(step);
            }
            _steps = newSteps;
        }
        public Step RemoveStep(string stepName)
        {
            Step stepToRemove = null;
            foreach (var step in _steps)
            {
                if (step.Name == stepName)
                {
                    stepToRemove = step;
                    break;
                }
            }
            if (stepToRemove != null) _steps.Remove(stepToRemove);
            //
            return stepToRemove;
        }
        public string[] GetNextStepNames(string stepName)
        {
            List<string> stepNames = null;
            foreach (var step in _steps)
            {
                if (stepNames != null) stepNames.Add(step.Name);
                if (step.Name == stepName) stepNames = new List<string>();
            }
            if (stepNames.Count > 0) return stepNames.ToArray();
            else return new string[0];
        }
        public OrderedDictionary<int, double> GetStepIdDuration()
        {
            int count = 1;  // start at 1
            OrderedDictionary<int, double> stepIdDuration =
                new OrderedDictionary<int, double>("Step id - duration");
            //
            foreach (var step in _steps)
            {
                if (step is StaticStep ss) stepIdDuration.Add(count++, ss.TimePeriod);
                else if (step is BoundaryDisplacementStep) continue;
                else if (step is FrequencyStep fs) stepIdDuration.Add(count++, 0);
                else if (step is BuckleStep bs) stepIdDuration.Add(count++, 0);
                else throw new NotImplementedException();
            }
            //
            return stepIdDuration;
        }
        // Run or check analysis
        public void SetRunAnalysis()
        {
            foreach (Step step in _steps) step.RunAnalysis = true;
        }
        public void SetCheckModel()
        {
            foreach (Step step in _steps) step.RunAnalysis = false;
        }
        // History
        public void AddHistoryOutput(HistoryOutput historyOutput, string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName) step.AddHistoryOutput(historyOutput);
            }
        }
        public Dictionary<string, int> GetHistoryOutputRegionsCount()
        {
            Dictionary<string, int> regionsCount = new Dictionary<string, int>();
            foreach (var step in _steps)
            {
                foreach (var entry in step.HistoryOutputs)
                {
                    if (entry.Value.RegionName != null)
                    {
                        if (regionsCount.ContainsKey(entry.Value.RegionName)) regionsCount[entry.Value.RegionName]++;
                        else regionsCount.Add(entry.Value.RegionName, 1);
                    }
                }
            }
            return regionsCount;
        }
        // Field
        public void AddFieldOutput(FieldOutput fieldOutput, string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName) step.AddFieldOutput(fieldOutput);
            }
        }
        // Boundary condition
        public string[] GetAllBoundaryConditionNames()
        {
            HashSet<string> allNames = new HashSet<string>();
            foreach (var step in _steps) allNames.UnionWith(step.BoundaryConditions.Keys);
            return allNames.ToArray();
        }
        public void AddBoundaryCondition(BoundaryCondition boundaryCondition, string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName) step.AddBoundaryCondition(boundaryCondition);
            }
        }
        public Dictionary<string, int> GetBoundaryConditionRegionsCount()
        {
            Dictionary<string, int> regionsCount = new Dictionary<string, int>();
            foreach (var step in _steps)
            {
                foreach (var entry in step.BoundaryConditions)
                {
                    if (entry.Value.RegionName != null)
                    {
                        if (regionsCount.ContainsKey(entry.Value.RegionName)) regionsCount[entry.Value.RegionName]++;
                        else regionsCount.Add(entry.Value.RegionName, 1);
                    }
                }
            }
            return regionsCount;
        }
        public Step GetBoundaryConditionStep(BoundaryCondition boundaryCondition)
        {
            BoundaryCondition existing;
            foreach (var step in _steps)
            {
                if (step.BoundaryConditions.TryGetValue(boundaryCondition.Name, out existing) && existing == boundaryCondition)
                    return step;
            }
            return null;
        }
        public Dictionary<int, bool[]> GetAllZeroDisplacements(FeModel model)
        {
            int[] nodeIds;
            bool isZero;
            bool[] bcDsplacement;
            bool[] displacement;
            string nodeSetName;
            Dictionary<int, bool[]> nodeIdZeroDisplacements = new Dictionary<int, bool[]>();
            //
            foreach (var step in _steps)
            {
                foreach (var entry in step.BoundaryConditions)
                {
                    if (entry.Value.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        nodeIds = model.Mesh.NodeSets[entry.Value.RegionName].Labels;
                    }
                    else if (entry.Value.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        nodeSetName = model.Mesh.Surfaces[entry.Value.RegionName].NodeSetName;
                        nodeIds = model.Mesh.NodeSets[nodeSetName].Labels;
                    }
                    else if (entry.Value.RegionType == RegionTypeEnum.ReferencePointName)
                    {
                        nodeIds = new int[0];
                    }
                    else throw new NotSupportedException();
                    //
                    if (entry.Value is FixedBC fbc)
                    {
                        foreach (var nodeId in nodeIds)
                        {
                            if (nodeIdZeroDisplacements.TryGetValue(nodeId, out displacement))
                            {
                                displacement[0] = true;
                                displacement[1] = true;
                                displacement[2] = true;
                            }
                            else
                            {
                                nodeIdZeroDisplacements.Add(nodeId, new bool[] { true, true, true });
                            }
                        }
                    }
                    else if (entry.Value is DisplacementRotation dr)
                    {
                        isZero = false;
                        bcDsplacement = new bool[3];
                        for (int i = 0; i < 3; i++)
                        {
                            bcDsplacement[i] = dr.GetDofType(i + 1) == DOFType.Zero; // || dr.GetDofType(i + 1) == DOFType.Fixed;
                            isZero |= bcDsplacement[i];
                        }
                        if (isZero)
                        {
                            foreach (var nodeId in nodeIds)
                            {
                                if (nodeIdZeroDisplacements.TryGetValue(nodeId, out displacement))
                                {
                                    if (bcDsplacement[0]) displacement[0] = true;
                                    if (bcDsplacement[1]) displacement[1] = true;
                                    if (bcDsplacement[2]) displacement[2] = true;
                                }
                                else
                                {
                                    nodeIdZeroDisplacements.Add(nodeId, bcDsplacement.ToArray());   // copy
                                }
                            }
                        }
                    }
                }
            }
            //
            return nodeIdZeroDisplacements;
        }
        // Load
        public string[] GetAllLoadNames()
        {
            HashSet<string> allNames = new HashSet<string>();
            foreach (var step in _steps) allNames.UnionWith(step.Loads.Keys);
            return allNames.ToArray();
        }
        public void AddLoad(Load load, string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName)  step.AddLoad(load);
            }
        }
        public Dictionary<string, int> GetLoadRegionsCount()
        {
            Dictionary<string, int> regionsCount = new Dictionary<string, int>();
            foreach (var step in _steps)
            {
                foreach (var entry in step.Loads)
                {
                    if (entry.Value.RegionName != null)
                    {
                        if (regionsCount.ContainsKey(entry.Value.RegionName)) regionsCount[entry.Value.RegionName]++;
                        else regionsCount.Add(entry.Value.RegionName, 1);
                    }
                }
            }
            return regionsCount;
        }
        public Step GetLoadStep(Load load)
        {
            Load existing;
            foreach (var step in _steps)
            {
                if (step.Loads.TryGetValue(load.Name, out existing) && existing == load)
                    return step;
            }
            return null;
        }
        public HashSet<Type> GetAllLoadTypes()
        {
            HashSet<Type> loadTypes = new HashSet<Type>();
            foreach (var _entry in _steps)
            {
                foreach (var loadEntry in _entry.Loads) loadTypes.Add(loadEntry.Value.GetType());
            }
            return loadTypes;
        }
        public bool IsActiveRadiationLoadDefined()
        {
            foreach (var _entry in _steps)
            {
                foreach (var loadEntry in _entry.Loads)
                {
                    if (loadEntry.Value is RadiationHeatTransfer rht && rht.Active) return true;
                }
            }
            //
            return false;
        }
        // Defined field
        public string[] GetAllDefinedFieldNames()
        {
            HashSet<string> allNames = new HashSet<string>();
            foreach (var step in _steps) allNames.UnionWith(step.DefinedFields.Keys);
            return allNames.ToArray();
        }
        public void AddDefinedField(DefinedField definedField, string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name == stepName) step.AddDefinedField(definedField);
            }
        }
        public Dictionary<string, int> GetDefinedFieldRegionsCount()
        {
            Dictionary<string, int> regionsCount = new Dictionary<string, int>();
            foreach (var step in _steps)
            {
                foreach (var entry in step.DefinedFields)
                {
                    if (entry.Value.RegionName != null)
                    {
                        if (regionsCount.ContainsKey(entry.Value.RegionName)) regionsCount[entry.Value.RegionName]++;
                        else regionsCount.Add(entry.Value.RegionName, 1);
                    }
                }
            }
            return regionsCount;
        }
        public Step GetDefinedFieldStep(DefinedField definedField)
        {
            DefinedField existing;
            foreach (var step in _steps)
            {
                if (step.DefinedFields.TryGetValue(definedField.Name, out existing) && existing == definedField)
                    return step;
            }
            return null;
        }
        // Wear
        public int[] GetSlipWearStepIds()
        {
            int count = 1;  // start at 1
            List<int> stepIds = new List<int>();
            foreach (var step in _steps)
            {
                if (step is SlipWearStep sws) stepIds.Add(count);
                count++;
            }
            return stepIds.ToArray();
        }
        public BoundaryDisplacementStep GetBoundaryDisplacementStep()
        {
            int count = 0;
            BoundaryDisplacementStep boundaryDisplacementStep = null;
            foreach (var step in _steps)
            {
                if (step is BoundaryDisplacementStep bds)
                {
                    boundaryDisplacementStep = bds;
                    count++;
                }
            }
            //
            if (count > 1) throw new CaeException("More than one boundary displacement step defined.");
            else if (count == 1) return boundaryDisplacementStep;
            else return null;
        }
        public bool AreContactHistoryOutputsDefined()
        {
            bool defined;
            foreach (var step in _steps)
            {
                if (step is BoundaryDisplacementStep) continue;
                //
                defined = false;
                foreach (var entry in step.HistoryOutputs)
                {
                    if (entry.Value.Active &&
                        entry.Value is ContactHistoryOutput cho &&
                        cho.Variables.HasFlag(ContactHistoryVariable.CDIS))
                    {
                        defined = true;
                        break;
                    }
                }
                if (!defined) return false;
            }
            return true;
        }
    }
}
