using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;
using System.Configuration;
using Octree;

namespace CaeResults
{
    [Serializable]
    public static class DatFileReader
    {
        private static readonly string[] spaceSplitter = new string[] { " " };
        private static readonly string[] commaSplitter = new string[] { "," };
        private static readonly string[] underscoreSplitter = new string[] { "_" };
        private static readonly string[] parenthesesSplitter = new string[] { "(", ")" };
        private static readonly string[] componentsSplitter = new string[] { " ", "," };
        private static readonly string[] dataSplitter = new string[] { " ", "for set", "and time" };
        private static readonly string[] signSplitter = new string[] { "-", "+" };
        private static readonly string steadyStateDynamicsKey =
            "P A R T I C I P A T I O N   F A C T O R S   F O R   F R E Q U E N C Y";
        public const string EigenvalueOutputKey = "E I G E N V A L U E   O U T P U T";
        public const string ParticipationFactorsKey = "P A R T I C I P A T I O N   F A C T O R S";
        public const string EffectiveModalMassKey = "E F F E C T I V E   M O D A L   M A S S";
        public const string TotalEffectiveMassKey = "T O T A L   E F F E C T I V E   M A S S";
        //
        private static readonly Dictionary<string, string> compMapRP = new Dictionary<string, string>()
        {
            { "Id", "Id" },
            //
            { FOComponentNames.U1, "UR1" },
            { FOComponentNames.U2, "UR2" },
            { FOComponentNames.U3, "UR3" },
            //
            { "RF1", "RM1" },
            { "RF2", "RM2" },
            { "RF3", "RM3" },
            //
            { "T", "T" }
        };


        // Methods                                                                                                                  
        static public HistoryResults Read(string fileName)
        {
            if (fileName != null && File.Exists(fileName))
            {
                string[] lines = Tools.ReadAllLines(fileName, true);
                //
                List<string> dataSetNames = new List<string>();
                // Nodal                                                                
                dataSetNames.Add(HOFieldNames.Displacements);
                dataSetNames.Add(HOFieldNames.Velocities);
                dataSetNames.Add(HOFieldNames.Forces);
                dataSetNames.Add(HOFieldNames.TotalForce);
                dataSetNames.Add(HOFieldNames.Stresses);
                dataSetNames.Add(HOFieldNames.Strains);
                dataSetNames.Add(HOFieldNames.MechanicalStrains);
                dataSetNames.Add(HOFieldNames.EquivalentPlasticStrains);
                dataSetNames.Add(HOFieldNames.InternalEnergyDensity);
                // Thermal
                dataSetNames.Add(HOFieldNames.Temperatures);
                dataSetNames.Add(HOFieldNames.HeatGeneration);
                dataSetNames.Add(HOFieldNames.TotalHeatGeneration);
                // Frequency                                                            
                dataSetNames.Add(EigenvalueOutputKey);
                dataSetNames.Add(ParticipationFactorsKey);
                dataSetNames.Add(EffectiveModalMassKey);
                dataSetNames.Add(TotalEffectiveMassKey);
                //dataSetNames.Add(HOFieldNames.EigenvalueOutput);
                //dataSetNames.Add(HOFieldNames.ParticipationFactors);
                //dataSetNames.Add(HOFieldNames.EffectiveModalMass);
                //dataSetNames.Add(HOFieldNames.TotalEffectiveMass);
                // Contact                                                              
                dataSetNames.Add(HOFieldNames.RelativeContactDisplacement);
                dataSetNames.Add(HOFieldNames.ContactStress);
                dataSetNames.Add(HOFieldNames.ContactPrintEnergy);
                dataSetNames.Add(HOFieldNames.TotalNumberOfContactElements);
                dataSetNames.Add(HOFieldNames.StatisticsForSlaveSet);
                dataSetNames.Add(HOFieldNames.TotalSurfaceForce);
                dataSetNames.Add(HOFieldNames.MomentAboutOrigin);
                dataSetNames.Add(HOFieldNames.CenterOgGravityCG);
                dataSetNames.Add(HOFieldNames.MeanSurfaceNormal);
                dataSetNames.Add(HOFieldNames.MomentAboutCG);
                dataSetNames.Add(HOFieldNames.SurfaceArea);
                dataSetNames.Add(HOFieldNames.NormalSurfaceForce);
                dataSetNames.Add(HOFieldNames.ShearSurfaceForce);
                // Element                                                              
                dataSetNames.Add(HOFieldNames.Volume);
                dataSetNames.Add(HOFieldNames.TotalVolume);
                dataSetNames.Add(HOFieldNames.InternalEnergy);
                dataSetNames.Add(HOFieldNames.TotalInternalEnergy);
                // Thermal
                dataSetNames.Add(HOFieldNames.HeatFlux);
                dataSetNames.Add(HOFieldNames.BodyHeating);
                dataSetNames.Add(HOFieldNames.TotalBodyHeating);
                //                                                                      
                Dictionary<string, string> repairedSetNames = new Dictionary<string, string>();
                //
                bool steadyStateDynamics;
                List<string[]> dataSetLinesList = SplitToDataSetLinesList(dataSetNames, lines, repairedSetNames,
                                                                          out steadyStateDynamics);
                Repair(dataSetLinesList, dataSetNames);
                //
                DatDataSet newDataSet;
                DatDataSet existingDataSet;
                OrderedDictionary<string, DatDataSet> dataSets = new OrderedDictionary<string, DatDataSet>("DataSets");
                //
                HistoryResults allHistoryResults = new HistoryResults("HistoryOutput");
                HistoryResults frequencyHistoryOutput = new HistoryResults("HistoryOutput");
                foreach (string[] dataSetLines in dataSetLinesList)
                {
                    // Frequency
                    if (dataSetLines[0] == EigenvalueOutputKey ||
                        dataSetLines[0] == ParticipationFactorsKey ||
                        dataSetLines[0] == EffectiveModalMassKey ||
                        dataSetLines[0] == TotalEffectiveMassKey)
                    {
                        frequencyHistoryOutput.AppendSets(GetFrequencyDatDataSets(dataSetLines));
                    }
                    else
                    {
                        newDataSet = GetDatDataSet(dataSetNames, dataSetLines, repairedSetNames);
                        if (newDataSet.SetName == steadyStateDynamicsKey) continue;
                        // Steady state dynamics
                        if (steadyStateDynamics && dataSets.TryGetValue(newDataSet.GetHashKey(), out existingDataSet))
                        {
                            existingDataSet.FieldName += HOFieldNames.ComplexRealSuffix;
                            dataSets.Replace(newDataSet.GetHashKey(), existingDataSet.GetHashKey(), existingDataSet);
                            //
                            newDataSet.FieldName += HOFieldNames.ComplexImaginarySuffix;
                        }
                        //
                        if (newDataSet.FieldName != HOFieldNames.Error) dataSets.Add(newDataSet.GetHashKey(), newDataSet);
                    }
                }
                //
                AddTotalEffectiveModalMassToMassRatio(frequencyHistoryOutput);
                allHistoryResults.AppendSets(frequencyHistoryOutput);
                //
                HistoryResults historyOutput = GetHistoryOutput(dataSets.Values);
                AddComplexFields(historyOutput);
                AddStressComponents(historyOutput);
                AddStrainComponents(historyOutput);
                //
                allHistoryResults.AppendSets(historyOutput);
                return allHistoryResults;
            }
            //
            return null;
        }
        static private List<string[]> SplitToDataSetLinesList(List<string> dataSetNames, string[] lines,
                                                              Dictionary<string, string> repairedSetNames,
                                                              out bool steadyStateDynamics)
        {
            // displacements (vx, vy, vz) for set DISP and time  0.1000000E+00
            //         5   5.080202E+00  0.000000E+00  0.000000E+00
            // forces (fx, fy, fz) for set FORCE and time  0.1000000E+00
            //         69 -2.678823E-05  6.029691E+01  1.673592E+01
            //         47  7.091853E-05  1.246206E+01  3.973111E+00
            //         48 -1.268813E-04 -5.285929E+01 -1.336375E+01
            // stresses (elem, integ.pnt.,sxx,syy,szz,sxy,sxz,syz) for set ELEMENTSET-1 and time  0.1000000E+01
            //         2030   1 -1.824212E-11 -1.000000E-01 -1.062898E-10 -6.614340E-11 -5.298272E-10  3.899325E-10
            //         2030   2 -2.853204E-09 -1.000000E-01 -3.195191E-11  1.074865E-09 -1.638492E-10 -3.539078E-10
            Dictionary<string, HashSet<string>> existingNames = new Dictionary<string, HashSet<string>>();
            List<string> dataSet;
            List<string[]> dataSets = new List<string[]>();
            //
            string theName;
            string firstLine;
            steadyStateDynamics = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length == 0) continue;
                if (!steadyStateDynamics && lines[i].ToUpper().StartsWith(steadyStateDynamicsKey))
                {
                    steadyStateDynamics = true;
                    foreach (var dataSetToRemove in dataSets.ToArray())
                    {
                        firstLine = dataSetToRemove[0].ToUpper();
                        if (!firstLine.StartsWith(EigenvalueOutputKey) &&
                            !firstLine.StartsWith(ParticipationFactorsKey) &&
                            !firstLine.StartsWith(EffectiveModalMassKey) &&
                            !firstLine.StartsWith(TotalEffectiveMassKey))
                        {
                            dataSets.Remove(dataSetToRemove);
                        }
                    }
                }
                //
                theName = null;
                foreach (var name in dataSetNames)
                {
                    if (lines[i].ToUpper().StartsWith(name.ToUpper()))
                    {
                        theName = name;
                        break;
                    }
                }
                // Frequency
                if (theName == EigenvalueOutputKey ||
                    theName == ParticipationFactorsKey ||
                    theName == EffectiveModalMassKey ||
                    theName == TotalEffectiveMassKey)
                {
                    int emptyLinesCounter = 0;
                    dataSet = new List<string> { lines[i] };
                    i++;
                    //
                    while (i < lines.Length && emptyLinesCounter <= 2)
                    {
                        if (lines[i].Length == 0) emptyLinesCounter++;
                        else dataSet.Add(lines[i]);
                        if (emptyLinesCounter >= 3) break;
                        //
                        i++;
                    }
                    //
                    dataSets.Add(dataSet.ToArray());
                }
                // Contact statistics
                else if (theName == HOFieldNames.StatisticsForSlaveSet)
                {
                    dataSet = new List<string>();
                    for (int j = 0; j <= 8 && i < lines.Length; j++)
                    {
                        if (lines[i].Length != 0) dataSet.Add(lines[i]);
                        i+=2;
                    }
                    // At the end reduce the i for 1 since it will be increased next time in the loop
                    i--;
                    //
                    List<string[]> repairedSets = RepairContactStatistics(dataSet.ToArray(), ref existingNames, repairedSetNames);
                    //
                    if (repairedSets != null) dataSets.AddRange(repairedSets);
                }
                else if (theName != null)
                {
                    dataSet = new List<string> { lines[i] };
                    i++;
                    //
                    while (i < lines.Length && lines[i].Length == 0) i++;    // skip empty lines
                    //
                    while (i < lines.Length)
                    {
                        if (lines[i].Length == 0 || lines[i].Contains("time")) break;    // last line is empty
                        else dataSet.Add(lines[i]);
                        //
                        i++;
                    }
                    //
                    dataSets.Add(dataSet.ToArray());
                }
            }
            //
            return dataSets;
        }
        static private List<string[]> RepairContactStatistics(string[] lines, ref Dictionary<string, HashSet<string>> existingNames,
                                                              Dictionary<string, string> repairedSetNames)
        {            
            //statistics for slave set INTERNAL_SELECTION - 2_CONTACTPAIR - 1_SLAVE, master set INTERNAL_SELECTION - 1_CONTACTPAIR - 1_MASTER and time  0.5000000E+00
            //total surface force (fx, fy, fz) and moment about the origin (mx, my, mz)
            //3.821561E+02  1.814803E+02 - 5.224860E+03  4.075417E+05  3.873634E+03  2.996193E+04
            //center of gravity and mean normal
            //- 6.862848E+00 - 8.165902E+01  1.040585E+02  0.000000E+00  0.000000E+00  1.000000E+00
            //moment about the center of gravity(mx, my, mz)
            //- 2.306952E+02 - 3.552590E+01  9.056343E-01
            //area, normal force(+ = tension) and shear force(size)
            //7.729843E+03 - 5.224860E+03  4.230584E+02
            //
            string[] dataSet;
            List<string[]> repairedDataSets = new List<string[]>();
            //
            if (lines.Length != 9) return repairedDataSets;
            //
            string[] tmp = lines[0].Split(new string[] { "slave set", "master set", "and time"}, 
                                          StringSplitOptions.RemoveEmptyEntries);
            string slaveName = RepairSetName(tmp[1].Trim(new char[] { ' ', ','}), repairedSetNames);
            string masterName = RepairSetName(tmp[2].Trim(), repairedSetNames);
            string time = tmp[3].Trim();
            //
            if (slaveName.EndsWith(CaeMesh.Globals.SlaveNameSuffix.ToUpper()))
                slaveName = slaveName.Replace(CaeMesh.Globals.SlaveNameSuffix.ToUpper(), "");
            if (masterName.EndsWith(CaeMesh.Globals.MasterNameSuffix.ToUpper()))
                masterName = masterName.Replace(CaeMesh.Globals.MasterNameSuffix.ToUpper(), "");
            //
            string name = slaveName == masterName ? slaveName : slaveName + "_" + masterName;
            //
            HashSet<string> existingNamesAtTime;
            if (existingNames.TryGetValue(time, out existingNamesAtTime))
            {
                if (existingNamesAtTime.Contains(name)) return null;
            }
            else
            {
                existingNames.Add(time, new HashSet<string>() { name });
            }
            // Total surface force
            tmp = lines[2].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
            dataSet = new string[2];
            dataSet[0] = "Total surface force (FX, FY, FZ) for set " + name + " and time " + time;
            dataSet[1] = string.Format("{0} {1} {2}", tmp[0], tmp[1], tmp[2]);
            repairedDataSets.Add(dataSet);
            // Moment about origin
            dataSet = new string[2];
            dataSet[0] = "Moment about origin (MX, MY, MZ) for set " + name + " and time " + time;
            dataSet[1] = string.Format("{0} {1} {2}", tmp[3], tmp[4], tmp[5]);
            repairedDataSets.Add(dataSet);
            // Center of gravity CG
            tmp = lines[4].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
            dataSet = new string[2];
            dataSet[0] = "Center of gravity CG (X, Y, Z) for set " + name + " and time " + time;
            dataSet[1] = string.Format("{0} {1} {2}", tmp[0], tmp[1], tmp[2]);
            repairedDataSets.Add(dataSet);
            // Mean normal
            dataSet = new string[2];
            dataSet[0] = "Mean surface normal (NX, NY, NZ) for set " + name + " and time " + time;
            dataSet[1] = string.Format("{0} {1} {2}", tmp[3], tmp[4], tmp[5]);
            repairedDataSets.Add(dataSet);
            // Moment about CG
            tmp = lines[6].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
            dataSet = new string[2];
            dataSet[0] = "Moment about CG (MX, MY, MZ) for set " + name + " and time " + time;
            dataSet[1] = string.Format("{0} {1} {2}", tmp[0], tmp[1], tmp[2]);
            repairedDataSets.Add(dataSet);
            // Area
            tmp = lines[8].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
            dataSet = new string[2];
            dataSet[0] = "Surface area (A) for set " + name + " and time " + time;
            dataSet[1] = tmp[0];
            repairedDataSets.Add(dataSet);
            //
            // Normal surface force
            dataSet = new string[2];
            dataSet[0] = "Normal surface force (FN) for set " + name + " and time " + time;
            dataSet[1] = tmp[1];
            repairedDataSets.Add(dataSet);
            // Shear surface force
            dataSet = new string[2];
            dataSet[0] = "Shear surface force (FS) for set " + name + " and time " + time;
            dataSet[1] = tmp[2];
            repairedDataSets.Add(dataSet);
            //
            return repairedDataSets;
        }
        static private void Repair(List<string[]> dataSetLinesList, List<string> dataSetNames)
        {
            foreach (var lines in dataSetLinesList)
            {
                if (lines.Length > 0)
                {
                    foreach (var name in dataSetNames)
                    {
                        if (lines[0].ToUpper().StartsWith(name.ToUpper()))
                        {
                            if (name == HOFieldNames.Displacements)
                            {
                                //displacements (vx,vy,vz) for set NODESET-1 and time  0.1000000E+01
                                //       310 -2.462709E-03 -6.331758E-04 -4.384750E-05
                                lines[0] = lines[0].Replace("(vx,vy,vz)", "(Id,U1,U2,U3)");
                            }
                            else if (name == HOFieldNames.Velocities)
                            {
                                //velocities (vx,vy,vz) for set INTERNAL_SELECTION-1_NH_OUTPUT-1 and time  0.8102399E-05
                                //       40  1.162032E-17 -7.948453E-02 -8.204195E-18
                                lines[0] = lines[0].Replace("(vx,vy,vz)", "(Id,V1,V2,V3)");
                            }
                            else if (name ==HOFieldNames.Forces)
                            {
                                //forces (fx,fy,fz) for set NODESET-1 and time  0.1000000E+01
                                //       310 -2.582430E-13 -1.013333E-01  6.199805E-14
                                lines[0] = lines[0].Replace("(fx,fy,fz)", "(Id,RF1,RF2,RF3)");
                            }
                            else if (name == HOFieldNames.TotalForce)
                            {
                                //total force (fx,fy,fz) for set NODESET-1 and time  0.1000000E+01
                                //       -5.868470E-13 -1.000000E+00 -1.028019E-13
                                lines[0] = lines[0].Replace("(fx,fy,fz)", "(RF1,RF2,RF3)");
                            }
                            else if (name == HOFieldNames.Stresses)
                            {
                                //stresses (elem, integ.pnt.,sxx,syy,szz,sxy,sxz,syz) for set SOLID_PART-1 and time  0.1000000E+01
                                //      1655   1  1.186531E-02 -3.997792E-02 -3.119545E-03  1.104426E-02  2.740127E-03 -9.467634E-03
                                lines[0] = lines[0].Replace("(elem, integ.pnt.,sxx,syy,szz,sxy,sxz,syz)",
                                                            "(Id,Int.Pnt.,S11,S22,S33,S12,S13,S23)");
                            }
                            else if (name == HOFieldNames.Strains)
                            {
                                //strains (elem, integ.pnt.,exx,eyy,ezz,exy,exz,eyz) forset SOLID_PART-1 and time  0.1000000E+01
                                //      1655   1  1.180693E-07 -2.028650E-07  2.530589E-08  6.836925E-08  1.696269E-08 -5.860917E-08
                                lines[0] = lines[0].Replace("(elem, integ.pnt.,exx,eyy,ezz,exy,exz,eyz)",
                                                            "(Id,Int.Pnt.,E11,E22,E33,E12,E13,E23)");
                                lines[0] = lines[0].Replace(" forset ", " for set ");
                            }
                            else if (name == HOFieldNames.MechanicalStrains)
                            {
                                // mechanical strains (elem, integ.pnt.,exx,eyy,ezz,exy,exz,eyz) forset ELEMENTSET-1 and time  0.1000000E+01
                                //      2030   1  1.428572E-07 -4.761905E-07  1.428571E-07 -1.951987E-14  2.100533E-15  1.625910E-15
                                lines[0] = lines[0].Replace("(elem, integ.pnt.,exx,eyy,ezz,exy,exz,eyz)",
                                                            "(Id,Int.Pnt.,E11,E22,E33,E12,E13,E23)");
                                lines[0] = lines[0].Replace(" forset ", " for set ");
                            }
                            else if (name == HOFieldNames.EquivalentPlasticStrains)
                            {
                                // equivalent plastic strain (elem, integ.pnt.,pe)for set ELEMENTSET-1 and time  0.6250000E-01
                                //      1682   1  0.000000E+00
                                lines[0] = lines[0].Replace("(elem, integ.pnt.,pe)",
                                                            "(Id,Int.Pnt.,PEEQ)");
                                lines[0] = lines[0].Replace(")for set ", ") for set ");
                            }
                            // Thermal                                                                                              
                            else if (name == HOFieldNames.Temperatures)
                            {
                                // temperatures for set INTERNAL_SELECTION-1_NH_OUTPUT-1 and time  0.1000000E+01
                                //      3450  1.000000E+02
                                lines[0] = lines[0].Replace("temperatures for set", "temperatures (Id,T) for set");
                            }
                            else if (name == HOFieldNames.HeatGeneration)
                            {
                                // heat generation for set INTERNAL_SELECTION-4_NH_OUTPUT-1 and time  0.1000000E+01
                                //       793  6.764684E+02
                                lines[0] = lines[0].Replace("heat generation for set", "heat generation (Id,RFL) for set");
                            }
                            else if (name == HOFieldNames.TotalHeatGeneration)
                            {
                                // total heat generation for set INTERNAL_SELECTION-4_NH_OUTPUT-1 and time  0.5750000E+01
                                //        9.890290E+02
                                lines[0] = lines[0].Replace("total heat generation for set", "total heat generation (RFL) for set");
                            }
                            //                                                                                                      
                            // Contact                                                                                              
                            //                                                                                                      
                            else if (name == HOFieldNames.RelativeContactDisplacement)
                            {
                                // relative contact displacement (slave element+face,normal,tang1,tang2) for all contact elements and time 0.1000000E+01
                                //     84102          4 -1.111983E-07 -2.300226E-07  1.142343E-07
                                lines[0] = lines[0].Replace("(slave element+face,normal,tang1,tang2)",
                                                            "(Id,Int.Pnt.,NORMAL,TANG1,TANG2)");
                                lines[0] = lines[0].Replace("for all contact elements", "for set ALL_CONTACT_ELEMENTS");
                            }
                            else if (name == HOFieldNames.ContactStress)
                            {
                                // contact stress (slave element+face,press,tang1,tang2) for all contact elements and time 0.5000000E+00
                                //     97837          4  9.511105E-01 -2.082501E-01 -2.605988E-02
                                lines[0] = lines[0].Replace("(slave element+face,press,tang1,tang2)",
                                                            "(Id,Int.Pnt.,PRESS,TANG1,TANG2)");
                                lines[0] = lines[0].Replace("for all contact elements", "for set ALL_CONTACT_ELEMENTS");
                            }
                            else if (name == HOFieldNames.ContactPrintEnergy)
                            {
                                // contact print energy (slave element+face,energy)for all contact elements and time 0.5000000E+00
                                //     98823          4  6.898953E-06
                                lines[0] = lines[0].Replace("(slave element+face,energy)for", "(Id,Int.Pnt.,ENERGY) for");
                                lines[0] = lines[0].Replace("for all contact elements", "for set ALL_CONTACT_ELEMENTS");
                            }
                            else if (name == HOFieldNames.TotalNumberOfContactElements)
                            {
                                // total number of contact elements for time  0.5000000E+00
                                // 560
                                lines[0] = lines[0].Replace("elements for time", "elements (NUM) for set ALL_CONTACT_ELEMENTS and time");
                            }
                            //                                                                                                      
                            // Element                                                                                              
                            //                                                                                                      
                            else if (name == HOFieldNames.InternalEnergyDensity)
                            {
                                // internal energy density (elem, integ.pnt.,eneset ELEMENTSET-1 and time  0.6250000E-01
                                //      3068   1  4.313000E-01
                                lines[0] = lines[0].Replace("(elem, integ.pnt.,eneset ",
                                                            "(Id,Int.Pnt.,ENER) for set ");
                                // internal energy density (elem, integ.pnt.,energy) for set INTERNAL_SELECTION-1_EH_OUTPUT-1 and time  0.6000000E+04
                                lines[0] = lines[0].Replace("(elem, integ.pnt.,energy",
                                                            "(Id,Int.Pnt.,ENER");
                            }
                            else if (name == HOFieldNames.InternalEnergy)
                            {
                                //internal energy (element, energy) for set SOLID_PART-1 and time  0.1000000E+01
                                //      1655  9.342906E-09
                                lines[0] = lines[0].Replace("(element, energy)", "(Id,ELSE)");
                            }
                            else if (name == HOFieldNames.TotalInternalEnergy)
                            {
                                //total internal energy for set SOLID_PART-1 and time  0.1000000E+01
                                //        3.249095E-04
                                lines[0] = lines[0].Replace("total internal energy for set",
                                                            "total internal energy (SE) for set");
                            }
                            else if (name == HOFieldNames.Volume)
                            {
                                //volume (element, volume) for set SOLID_PART-1 and time  0.1000000E+01
                                //      1655  1.538557E+00
                                lines[0] = lines[0].Replace("(element, volume)", "(Id,EVOL)");
                            }
                            else if (name == HOFieldNames.TotalVolume)
                            {
                                //total volume for set SOLID_PART-1 and time  0.1000000E+01
                                //        2.322033E+03
                                lines[0] = lines[0].Replace("total volume for set", "total volume (VOL) for set");
                            }
                            // Thermal                                                                                              
                            else if (name == HOFieldNames.HeatFlux)
                            {
                                // heat flux (elem, integ.pnt.,qx,qy,qz) for set INTERNAL_SELECTION-3_EH_OUTPUT-1 and time  0.1000000E+01
                                //       477   1 -6.733427E+00  1.076158E+01 -1.030648E+01
                                lines[0] = lines[0].Replace("(elem, integ.pnt.,qx,qy,qz)", "(Id,Int.Pnt.,Q1,Q2,Q3)");
                            }
                            else if (name == HOFieldNames.BodyHeating)
                            {
                                // body heating (element, volume) for set INTERNAL_SELECTION-3_EH_OUTPUT-1 and time  0.1000000E+01
                                //       477  0.000000E+00
                                lines[0] = lines[0].Replace("(element, volume)", "(Id,EBHE)");
                            }
                            else if (name == HOFieldNames.TotalBodyHeating)
                            {
                                // total body heating for set INTERNAL_SELECTION-3_EH_OUTPUT-1 and time  0.1000000E+01
                                //        0.126000E+00
                                lines[0] = lines[0].Replace("total body heating for set", "total body heating (BHE) for set");
                            }
                        }
                    }
                    
                }
            }
        }
        static private string RepairSetName(string setName, Dictionary<string, string> repairedSetNames)
        {
            string[] tmp;
            string newName = setName;
            if (setName.StartsWith(CaeMesh.Globals.InternalSelectionName.ToUpper()))
            {
                if (!repairedSetNames.TryGetValue(setName, out newName))
                {
                    tmp = setName.Split(underscoreSplitter, StringSplitOptions.None);
                    newName = "";
                    for (int i = 2; i < tmp.Length; i++)
                    {
                        newName += tmp[i];
                        if (i < tmp.Length - 1) newName += "_";
                    }
                    // Check if an existing set was already renamed to an existing new name
                    if (repairedSetNames.Values.Contains(newName))
                    {
                        newName = NamedClass.GetNameWithoutLastValue(newName);
                        newName = new HashSet<string>(repairedSetNames.Values).GetNextNumberedKey(newName);
                    }
                    repairedSetNames.Add(setName, newName);
                }
            }
            else if (setName.StartsWith(CaeMesh.Globals.InternalName.ToUpper()))
            {
                if (!repairedSetNames.TryGetValue(setName, out newName))
                {
                    tmp = setName.Split(underscoreSplitter, StringSplitOptions.None);
                    newName = "";
                    for (int i = 1; i < tmp.Length; i++)
                    {
                        newName += tmp[i];
                        if (i < tmp.Length - 1) newName += "_";
                    }
                    // Check if an existing set was already renamed to an existing new name
                    if (repairedSetNames.Values.Contains(newName))
                    {
                        newName = NamedClass.GetNameWithoutLastValue(newName);
                        newName = new HashSet<string>(repairedSetNames.Values).GetNextNumberedKey(newName);
                    }
                    repairedSetNames.Add(setName, newName);
                }
            }
            return newName;
        }
        static private HistoryResults GetFrequencyDatDataSets(string[] dataSetLines)
        {
            int firstDataRowId = 1;
            string entryName = null;
            string totalEntryName = HOFieldNames.TotalEffectiveModalMass;
            string[] componentNames = null;
            double time = 0;
            List<double[]> valuesList = null;
            double[] rowValues = null;
            List<DatDataSet> dataSets = new List<DatDataSet>();
            //
            if (dataSetLines[0] == EigenvalueOutputKey)
            {
                firstDataRowId = 4;
                entryName = HOFieldNames.EigenvalueOutput;
                componentNames = new string[] { HOComponentNames.EIGENVALUE, HOComponentNames.OMEGA, HOComponentNames.FREQUENCY,
                                                HOComponentNames.FREQUENCY_IM};
            }
            else if (dataSetLines[0] == ParticipationFactorsKey)
            {
                firstDataRowId = 2;
                entryName = HOFieldNames.ParticipationFactors;
                componentNames = new string[] { HOComponentNames.XCOMPONENT,HOComponentNames.YCOMPONENT, HOComponentNames.ZCOMPONENT,
                                                HOComponentNames.XROTATION,HOComponentNames.YROTATION, HOComponentNames.ZROTATION };
            }
            else if (dataSetLines[0] == EffectiveModalMassKey)
            {
                firstDataRowId = 2;
                entryName = HOFieldNames.EffectiveModalMass;
                componentNames = new string[] { HOComponentNames.XCOMPONENT,HOComponentNames.YCOMPONENT, HOComponentNames.ZCOMPONENT,
                                                HOComponentNames.XROTATION,HOComponentNames.YROTATION, HOComponentNames.ZROTATION };
            }
            else if (dataSetLines[0] == TotalEffectiveMassKey)
            {
                firstDataRowId = 2;
                entryName = HOFieldNames.TotalEffectiveMass;
                componentNames = new string[] { HOComponentNames.XCOMPONENT,HOComponentNames.YCOMPONENT, HOComponentNames.ZCOMPONENT,
                                                HOComponentNames.XROTATION,HOComponentNames.YROTATION, HOComponentNames.ZROTATION };
            }
            //
            string[] tmp;
            HistoryResults historyResults = new HistoryResults("HistoryResults");
            HistoryResultSet historyResultSet = new HistoryResultSet("ENTIRE_MODEL");
            HistoryResultField historyResultField = new HistoryResultField(HOFieldNames.Frequency);
            HistoryResultComponent historyResultComponent = new HistoryResultComponent(entryName);
            HistoryResultComponent totalHistoryResultComponent = new HistoryResultComponent(totalEntryName);
            HistoryResultEntries historyResultEntries = null;
            HistoryResultEntries totalHistoryResultEntries = null;
            valuesList = new List<double[]>();
            //
            for (int i = firstDataRowId; i < dataSetLines.Length; i++)
            {
                tmp = dataSetLines[i].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length >= 2)
                {
                    rowValues = new double[tmp.Length];
                    for (int j = 0; j < rowValues.Length; j++) rowValues[j] = ParseStringToDouble(tmp[j]);
                    valuesList.Add(rowValues);
                }
            }
            double[][] values = valuesList.ToArray();   // [row][col]
            if (values != null && values.Length > 0)
            {
                int delta = values[0].Length - componentNames.Length;
                for (int col = delta; col < values[0].Length; col++)  // first value in a row is time
                {
                    historyResultEntries = new HistoryResultEntries(componentNames[col - delta], false);
                    totalHistoryResultEntries = new HistoryResultEntries(componentNames[col - delta], false);
                    //
                    for (int row = 0; row < values.Length; row++)
                    {
                        if (delta == 1) time = values[row][0];
                        else time = row + 1;
                        //
                        if (double.IsNaN(time)) totalHistoryResultEntries.Add(1, values[row][col]);
                        else historyResultEntries.Add(time, values[row][col]);
                    }
                    historyResultComponent.Entries.Add(historyResultEntries.Name, historyResultEntries);
                    if (totalHistoryResultEntries.Time.Count > 0)
                        totalHistoryResultComponent.Entries.Add(totalHistoryResultEntries.Name, totalHistoryResultEntries);
                }
                historyResultField.Components.Add(historyResultComponent.Name, historyResultComponent);
                if (totalHistoryResultComponent.Entries.Count > 0)
                    historyResultField.Components.Add(totalHistoryResultComponent.Name, totalHistoryResultComponent);
                historyResultSet.Fields.Add(historyResultField.Name, historyResultField);
                historyResults.Sets.Add(historyResultSet.Name, historyResultSet);
            }
            //
            return historyResults;
        }
        static private DatDataSet GetDatDataSet(List<string> dataSetNames, string[] dataSetLines,
                                                Dictionary<string, string> repairedSetNames)
        {
            try
            {
                // displacements (vx, vy, vz) for set DISP and time  0.1000000E+00
                List<string> componentNames = new List<string>();
                DatDataSet dataSet = new DatDataSet();
                //
                string firstLine = dataSetLines[0];
                if (firstLine.ToUpper().StartsWith(steadyStateDynamicsKey))
                {
                    dataSet.SetName = steadyStateDynamicsKey;
                    return dataSet;
                }
                //
                foreach (var name in dataSetNames)
                {
                    if (firstLine.ToLower().StartsWith(name.ToLower()))
                    {
                        dataSet.FieldName = name;
                        break;
                    }
                }
                //
                string[] tmp = firstLine.Split(parenthesesSplitter, StringSplitOptions.RemoveEmptyEntries);
                //
                string[] tmp2 = tmp[1].Split(componentsSplitter, StringSplitOptions.RemoveEmptyEntries);
                componentNames = tmp2.ToList();
                //
                tmp2 = tmp[2].Split(dataSplitter, StringSplitOptions.RemoveEmptyEntries);
                dataSet.SetName = RepairSetName(tmp2[0].Trim(), repairedSetNames);
                dataSet.Time = double.Parse(tmp2[1]);
                //
                int length;
                double[] values;
                List<bool> locals = new List<bool>();
                List<double[]> allValues = new List<double[]>();
                for (int i = 1; i < dataSetLines.Length; i++)
                {
                    tmp = dataSetLines[i].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
                    // Local result
                    if (tmp[tmp.Length - 1].ToUpper() == "L")
                    {
                        locals.Add(true);
                        length = tmp.Length - 1;
                    }
                    else
                    {
                        locals.Add(false);
                        length = tmp.Length;
                    }
                    //
                    values = new double[length];
                    //
                    for (int j = 0; j < length; j++) values[j] = ParseStringToDouble(tmp[j]);
                    //
                    allValues.Add(values);
                }
                dataSet.Locals = locals.ToArray();
                dataSet.Values = allValues.ToArray();
                dataSet.ComponentNames = componentNames.ToArray();
                //
                return dataSet;
            }
            catch
            {
                return new DatDataSet() { FieldName = HOFieldNames.Error };
            }
        }
        static private double ParseStringToDouble(string valueAsString)
        {
            double value;
            int count;
            string tmp3;
            string[] tmp2;
            if (!double.TryParse(valueAsString, out value))
            {
                // Try to repair values as:
                // 1.090361-282  8.468565-284 -8.171595-285
                tmp3 = valueAsString;
                tmp2 = tmp3.Split(signSplitter, StringSplitOptions.RemoveEmptyEntries);
                if (tmp2.Length == 2)
                {
                    count = tmp3.Length - (tmp2[1].Length + 1);
                    valueAsString = tmp3.Substring(0, count) + "E" + tmp3.Substring(count, tmp2[1].Length + 1);
                    //
                    if (!double.TryParse(valueAsString, out value)) value = double.NaN;
                }
                else value = double.NaN;
            }
            return value;
        }
        static private HistoryResults GetHistoryOutput(IEnumerable<DatDataSet> dataSets)
        {
            HistoryResults historyOutput = new HistoryResults("HistoryOutput");
            HistoryResultSet set;
            HistoryResultField field;
            HistoryResultComponent component = null;
            HistoryResultEntries entries;
            //
            DatDataSet repairedDataSet;
            int offset;
            string valueId;
            string id;
            double time;
            double[] values;
            string noSuffixName;
            //
            foreach (var dataSet in dataSets)
            {
                repairedDataSet = RepairReferencePointDataSet(dataSet);
                //
                time = repairedDataSet.Time;
                // Get or create a set
                if (!historyOutput.Sets.TryGetValue(repairedDataSet.SetName, out set))
                {
                    set = new HistoryResultSet(repairedDataSet.SetName);
                    historyOutput.Sets.Add(set.Name, set);
                }
                // Get or create a field
                if (!set.Fields.TryGetValue(repairedDataSet.FieldName, out field))
                {
                    field = new HistoryResultField(repairedDataSet.FieldName);
                    set.Fields.Add(field.Name, field);
                }
                // For each value line in data set: id x y z
                for (int i = 0; i < repairedDataSet.Values.Length; i++)
                {
                    values = repairedDataSet.Values[i];
                    //
                    if (repairedDataSet.ComponentNames.Length > 0 && repairedDataSet.ComponentNames[0] == "Id")
                    {
                        // The first column is id column
                        if (repairedDataSet.ComponentNames[1] == "Int.Pnt.")
                        {
                            // The second column is In.Pnt. column
                            valueId = values[0].ToString() + "_" + values[1].ToString();
                            offset = 2;
                        }
                        else
                        {
                            valueId = values[0].ToString();
                            offset = 1;
                        }
                    }
                    // There is no id
                    else
                    {
                        valueId = null;
                        offset = 0;
                    }                    
                    //                                                                                  
                    // For ecah component
                    for (int j = 0; j < values.Length - offset; j++)
                    {
                        // Get or create a component
                        if (!field.Components.TryGetValue(repairedDataSet.ComponentNames[j + offset], out component))
                        {
                            component = new HistoryResultComponent(repairedDataSet.ComponentNames[j + offset]);
                            field.Components.Add(component.Name, component);
                        }
                        // For the case of total forces
                        if (valueId == null) id = component.Name;
                        else id = valueId;
                        // Get or create historyValues as component entries
                        if (!component.Entries.TryGetValue(id, out entries))
                        {
                            entries = new HistoryResultEntries(id, repairedDataSet.Locals[i]);
                            component.Entries.Add(entries.Name, entries);
                        }
                        // Sum - If the same Id exists for the same time: sum them together
                        if ((field.Name == HOFieldNames.RelativeContactDisplacement ||
                             field.Name == HOFieldNames.ContactStress ||
                             field.Name == HOFieldNames.ContactPrintEnergy) && entries.Time.Contains(time))
                        {
                            entries.SumValue(values[j + offset]);
                        }
                        // Skip repeating
                        else if (field.Name == HOFieldNames.TotalNumberOfContactElements && entries.Time.Contains(time)) 
                        { }
                        // Add
                        else
                        {
                            entries.Add(time, values[j + offset]);
                        }
                    }
                }
                //
            }
            // Averege the summed values
            foreach (var setEntry in historyOutput.Sets)
            {
                foreach (var fieldEntry in setEntry.Value.Fields)
                {
                    foreach (var componentEntry in fieldEntry.Value.Components)
                    {
                        foreach (var entry in componentEntry.Value.Entries)
                        {
                            entry.Value.ComputeAverage();
                        }
                    }
                }
            }
            //
            return historyOutput;
        }
        static private DatDataSet RepairReferencePointDataSet(DatDataSet dataSet)
        {
            string setName = dataSet.SetName;
            string[] tmp;
            // Ref node
            tmp = setName.ToUpper().Split(new string[] { FeReferencePoint.RefName.ToUpper() }, 
                                          StringSplitOptions.RemoveEmptyEntries);
            if (tmp.Length == 2) dataSet.SetName = tmp[0];
            // Rot node
            tmp = setName.ToUpper().Split(new string[] { FeReferencePoint.RotName.ToUpper() },
                                          StringSplitOptions.RemoveEmptyEntries);
            if (tmp.Length == 2)
            {
                dataSet.SetName = tmp[0];
                for (int i = 0; i < dataSet.ComponentNames.Length; i++)
                {
                    dataSet.ComponentNames[i] = compMapRP[dataSet.ComponentNames[i]];
                }
            }
            //
            return dataSet;
        }

        //
        static private void AddComplexFields(HistoryResults historyOutput)
        {
            HistoryResultField fieldR;
            HistoryResultField fieldI;
            HistoryResultField fieldMag;
            HistoryResultField fieldPha;
            HistoryResultComponent compR;
            HistoryResultComponent compI;
            HistoryResultComponent compMag;
            HistoryResultComponent compPha;
            HistoryResultEntries entryR;
            HistoryResultEntries entryI;
            HistoryResultEntries entryMag;
            HistoryResultEntries entryPha;
            double[] real;
            double[] imaginary;
            double[] mag;
            double[] pha;
            HashSet<string> complexFieldNames = new HashSet<string>();
            foreach (var setsEntry in historyOutput.Sets)
            {
                complexFieldNames.Clear();
                foreach (var fieldEntry in setsEntry.Value.Fields)
                {
                    if (HOFieldNames.HasComplexSuffix(fieldEntry.Key))
                        complexFieldNames.Add(HOFieldNames.GetNoSuffixName(fieldEntry.Key));
                }
                //
                foreach (var complexFieldName in complexFieldNames)
                {
                    if (setsEntry.Value.Fields.TryGetValue(complexFieldName + HOFieldNames.ComplexRealSuffix, out fieldR) &&
                        setsEntry.Value.Fields.TryGetValue(complexFieldName + HOFieldNames.ComplexImaginarySuffix, out fieldI))
                    {
                        fieldMag = new HistoryResultField(complexFieldName + HOFieldNames.ComplexMagnitudeSuffix);
                        fieldPha = new HistoryResultField(complexFieldName + HOFieldNames.ComplexPhaseSuffix);
                        //
                        foreach (var componentName in fieldR.Components.Keys)
                        {
                            compMag = new HistoryResultComponent(componentName);
                            compPha = new HistoryResultComponent(componentName);
                            compPha.Unit = StringAngleDegConverter.GetUnitAbbreviation();
                            //
                            if (fieldR.Components.TryGetValue(componentName, out compR) &&
                                fieldI.Components.TryGetValue(componentName, out compI))
                            {
                                foreach (var entryName in compR.Entries.Keys)
                                {
                                    entryMag = new HistoryResultEntries(entryName, false);
                                    entryPha = new HistoryResultEntries(entryName, false);
                                    //
                                    if (compR.Entries.TryGetValue(entryName, out entryR) &&
                                        compI.Entries.TryGetValue(entryName, out entryI))
                                    {
                                        real = entryR.Values.ToArray();
                                        imaginary = entryI.Values.ToArray();
                                        mag = new double[Math.Min(real.Length, imaginary.Length)];
                                        pha = new double[mag.Length];
                                        for (int i = 0; i < mag.Length; i++)
                                        {
                                            mag[i] = Tools.GetComplexMagnitude(real[i], imaginary[i]);
                                            pha[i] = Tools.GetComplexPhaseDeg(real[i], imaginary[i]);
                                        }
                                        //
                                        entryMag.Time = entryR.Time;
                                        entryMag.Values = mag.ToList();
                                        compMag.Entries.Add(entryMag.Name, entryMag);
                                        //
                                        entryPha.Time = entryR.Time;
                                        entryPha.Values = pha.ToList();
                                        compPha.Entries.Add(entryPha.Name, entryPha);
                                    }
                                }
                            }
                            fieldMag.Components.Add(compMag.Name, compMag);
                            fieldPha.Components.Add(compPha.Name, compPha);
                        }
                        //
                        setsEntry.Value.Fields.Add(fieldMag.Name, fieldMag);
                        setsEntry.Value.Fields.Add(fieldPha.Name, fieldPha);
                    }
                }
            }
        }
        static private void AddStressComponents(HistoryResults historyOutput)
        {
            HistoryResultComponent vonMisesCom;
            HistoryResultComponent trescaCom;
            HistoryResultComponent sgnMaxAbsPrinCom;
            HistoryResultComponent prinMaxCom;
            HistoryResultComponent prinMidCom;
            HistoryResultComponent prinMinCom;
            //
            foreach (var setsEntry in historyOutput.Sets)
            {
                foreach (var fieldEntry in setsEntry.Value.Fields)
                {
                    string noSuffixName = HOFieldNames.GetNoSuffixName(fieldEntry.Key);
                    //
                    if (noSuffixName == HOFieldNames.Stresses && HOFieldNames.HasRealComplexSuffix(fieldEntry.Key))
                    {
                        vonMisesCom = new HistoryResultComponent(HOComponentNames.Mises);
                        trescaCom = new HistoryResultComponent(HOComponentNames.Tresca);
                        sgnMaxAbsPrinCom = new HistoryResultComponent(HOComponentNames.SgnMaxAbsPri);
                        prinMaxCom = new HistoryResultComponent(HOComponentNames.PrincipalMax);
                        prinMidCom = new HistoryResultComponent(HOComponentNames.PrincipalMid);
                        prinMinCom = new HistoryResultComponent(HOComponentNames.PrincipalMin);
                        //
                        string[] entryNames = fieldEntry.Value.Components[HOComponentNames.S11].Entries.Keys.ToArray();
                        double[][] values = new double[6][];
                        double[] vmArray;
                        double[] trescaArray;
                        double[] sgnMaxAbsPrinArray;
                        double[] prinMaxArray;
                        double[] prinMidArray;
                        double[] prinMinArray;
                        HistoryResultEntries hrEntries;
                        //
                        double s11;
                        double s22;
                        double s33;
                        double s12;
                        double s23;
                        double s31;
                        double I1;
                        double I2;
                        double I3;
                        double sp1, sp2, sp3;                        
                        //
                        foreach (var entryName in entryNames)
                        {
                            values[0] = fieldEntry.Value.Components[HOComponentNames.S11].Entries[entryName].Values.ToArray();
                            values[1] = fieldEntry.Value.Components[HOComponentNames.S22].Entries[entryName].Values.ToArray();
                            values[2] = fieldEntry.Value.Components[HOComponentNames.S33].Entries[entryName].Values.ToArray();
                            values[3] = fieldEntry.Value.Components[HOComponentNames.S12].Entries[entryName].Values.ToArray();
                            values[4] = fieldEntry.Value.Components[HOComponentNames.S23].Entries[entryName].Values.ToArray();
                            values[5] = fieldEntry.Value.Components[HOComponentNames.S13].Entries[entryName].Values.ToArray();
                            //
                            vmArray = new double[values[0].Length];
                            trescaArray = new double[values[0].Length];
                            sgnMaxAbsPrinArray = new double[values[0].Length];
                            prinMaxArray = new double[values[0].Length];
                            prinMidArray = new double[values[0].Length];
                            prinMinArray = new double[values[0].Length];
                            //
                            for (int i = 0; i < vmArray.Length; i++)
                            {
                                vmArray[i] = Math.Sqrt(0.5 * (
                                                               Math.Pow(values[0][i] - values[1][i], 2)
                                                             + Math.Pow(values[1][i] - values[2][i], 2)
                                                             + Math.Pow(values[2][i] - values[0][i], 2)
                                                             + 6 * (
                                                                      Math.Pow(values[3][i], 2)
                                                                    + Math.Pow(values[4][i], 2)
                                                                    + Math.Pow(values[5][i], 2)
                                                                   )
                                                            )
                                                     );
                                //
                                s11 = values[0][i];
                                s22 = values[1][i];
                                s33 = values[2][i];
                                s12 = values[3][i];
                                s23 = values[4][i];
                                s31 = values[5][i];
                                //
                                I1 = s11 + s22 + s33;
                                I2 = s11 * s22 + s22 * s33 + s33 * s11 -
                                     Math.Pow(s12, 2.0) - Math.Pow(s23, 2.0) - Math.Pow(s31, 2.0);
                                I3 = s11 * s22 * s33 - s11 * Math.Pow(s23, 2.0) -
                                     s22 * Math.Pow(s31, 2.0) - s33 * Math.Pow(s12, 2.0) + 2.0 * s12 * s23 * s31;
                                //
                                sp1 = sp2 = sp3 = 0;
                                Tools.SolveQubicEquationDepressedCubic(1.0, -I1, I2, -I3, ref sp1, ref sp2, ref sp3);
                                Tools.Sort3_descending(ref sp1, ref sp2, ref sp3);
                                //
                                if (double.IsNaN(sp1)) sp1 = 0;
                                if (double.IsNaN(sp2)) sp2 = 0;
                                if (double.IsNaN(sp3)) sp3 = 0;
                                //
                                sgnMaxAbsPrinArray[i] = Math.Abs(sp1) > Math.Abs(sp3) ? (float)sp1 : (float)sp3;
                                prinMaxArray[i] = sp1;
                                prinMidArray[i] = sp2;
                                prinMinArray[i] = sp3;
                                //
                                trescaArray[i] = sp1 - sp3;
                            }
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.S11].Entries[entryName].Time;
                            hrEntries.Values = vmArray.ToList();
                            vonMisesCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.S11].Entries[entryName].Time;
                            hrEntries.Values = trescaArray.ToList();
                            trescaCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.S11].Entries[entryName].Time;
                            hrEntries.Values = sgnMaxAbsPrinArray.ToList();
                            sgnMaxAbsPrinCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.S11].Entries[entryName].Time;
                            hrEntries.Values = prinMaxArray.ToList();
                            prinMaxCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.S11].Entries[entryName].Time;
                            hrEntries.Values = prinMidArray.ToList();
                            prinMidCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.S11].Entries[entryName].Time;
                            hrEntries.Values = prinMinArray.ToList();
                            prinMinCom.Entries.Add(entryName, hrEntries);
                        }
                        //
                        fieldEntry.Value.Components.Insert(0, vonMisesCom.Name, vonMisesCom);
                        fieldEntry.Value.Components.Insert(1, trescaCom.Name, trescaCom);
                        fieldEntry.Value.Components.Add(sgnMaxAbsPrinCom.Name, sgnMaxAbsPrinCom);
                        fieldEntry.Value.Components.Add(prinMaxCom.Name, prinMaxCom);
                        fieldEntry.Value.Components.Add(prinMidCom.Name, prinMidCom);
                        fieldEntry.Value.Components.Add(prinMinCom.Name, prinMinCom);
                    }
                }
            }
        }
        static private void AddStrainComponents(HistoryResults historyOutput)
        {
            foreach (var setsEntry in historyOutput.Sets)
            {
                foreach (var fieldEntry in setsEntry.Value.Fields)
                {
                    string noSuffixName = HOFieldNames.GetNoSuffixName(fieldEntry.Key);
                    //
                    if ((noSuffixName == HOFieldNames.Strains || noSuffixName == HOFieldNames.MechanicalStrains) &&
                        HOFieldNames.HasRealComplexSuffix(fieldEntry.Key))
                    {
                        HistoryResultComponent vonMisesCom = new HistoryResultComponent(HOComponentNames.Mises);
                        HistoryResultComponent trescaCom = new HistoryResultComponent(HOComponentNames.Tresca);
                        HistoryResultComponent sgnMaxAbsPrinCom = new HistoryResultComponent(HOComponentNames.SgnMaxAbsPri);
                        HistoryResultComponent prinMaxCom = new HistoryResultComponent(HOComponentNames.PrincipalMax);
                        HistoryResultComponent prinMidCom = new HistoryResultComponent(HOComponentNames.PrincipalMid);
                        HistoryResultComponent prinMinCom = new HistoryResultComponent(HOComponentNames.PrincipalMin);
                        //
                        string[] entryNames = fieldEntry.Value.Components[HOComponentNames.E11].Entries.Keys.ToArray();
                        double[][] values = new double[6][];
                        double[] vmArray;
                        double[] trescaArray;
                        double[] sgnMaxAbsPrinArray;
                        double[] prinMaxArray;
                        double[] prinMidArray;
                        double[] prinMinArray;
                        HistoryResultEntries hrEntries;
                        //
                        double e11;
                        double e22;
                        double e33;
                        double e12;
                        double e23;
                        double e31;
                        double I1;
                        double I2;
                        double I3;
                        double ep1, ep2, ep3;
                        //
                        foreach (var entryName in entryNames)
                        {
                            values[0] = fieldEntry.Value.Components[HOComponentNames.E11].Entries[entryName].Values.ToArray();
                            values[1] = fieldEntry.Value.Components[HOComponentNames.E22].Entries[entryName].Values.ToArray();
                            values[2] = fieldEntry.Value.Components[HOComponentNames.E33].Entries[entryName].Values.ToArray();
                            values[3] = fieldEntry.Value.Components[HOComponentNames.E12].Entries[entryName].Values.ToArray();
                            values[4] = fieldEntry.Value.Components[HOComponentNames.E23].Entries[entryName].Values.ToArray();
                            values[5] = fieldEntry.Value.Components[HOComponentNames.E13].Entries[entryName].Values.ToArray();
                            //
                            vmArray = new double[values[0].Length];
                            trescaArray = new double[values[0].Length];
                            sgnMaxAbsPrinArray = new double[values[0].Length];
                            prinMaxArray = new double[values[0].Length];
                            prinMidArray = new double[values[0].Length];
                            prinMinArray = new double[values[0].Length];
                            //
                            for (int i = 0; i < values[0].Length; i++)
                            {
                                // Calculate Von Mises values
                                vmArray[i] = Math.Sqrt(0.5 * (
                                                              Math.Pow(values[0][i] - values[1][i], 2)
                                                            + Math.Pow(values[1][i] - values[2][i], 2)
                                                            + Math.Pow(values[2][i] - values[0][i], 2)
                                                            + 6 * (
                                                                     Math.Pow(values[3][i], 2)
                                                                   + Math.Pow(values[4][i], 2)
                                                                   + Math.Pow(values[5][i], 2)
                                                                  )
                                                           )
                                                    );
                                //
                                e11 = values[0][i];
                                e22 = values[1][i];
                                e33 = values[2][i];
                                e12 = values[3][i];
                                e23 = values[4][i];
                                e31 = values[5][i];
                                //
                                I1 = e11 + e22 + e33;
                                I2 = e11 * e22 + e22 * e33 + e33 * e11 -
                                     Math.Pow(e12, 2.0) - Math.Pow(e23, 2.0) - Math.Pow(e31, 2.0);
                                I3 = e11 * e22 * e33 - e11 * Math.Pow(e23, 2.0) -
                                     e22 * Math.Pow(e31, 2.0) - e33 * Math.Pow(e12, 2.0) + 2.0 * e12 * e23 * e31;
                                //
                                ep1 = ep2 = ep3 = 0;
                                Tools.SolveQubicEquationDepressedCubic(1.0, -I1, I2, -I3, ref ep1, ref ep2, ref ep3);
                                Tools.Sort3_descending(ref ep1, ref ep2, ref ep3);
                                //
                                if (double.IsNaN(ep1)) ep1 = 0;
                                if (double.IsNaN(ep2)) ep2 = 0;
                                if (double.IsNaN(ep3)) ep3 = 0;
                                //
                                sgnMaxAbsPrinArray[i] = Math.Abs(ep1) > Math.Abs(ep3) ? (float)ep1 : (float)ep3;
                                prinMaxArray[i] = ep1;
                                prinMidArray[i] = ep2;
                                prinMinArray[i] = ep3;
                                //
                                trescaArray[i] = ep1 - ep3;
                            }
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.E11].Entries[entryName].Time;
                            hrEntries.Values = vmArray.ToList();
                            vonMisesCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.E11].Entries[entryName].Time;
                            hrEntries.Values = trescaArray.ToList();
                            trescaCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.E11].Entries[entryName].Time;
                            hrEntries.Values = sgnMaxAbsPrinArray.ToList();
                            sgnMaxAbsPrinCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.E11].Entries[entryName].Time;
                            hrEntries.Values = prinMaxArray.ToList();
                            prinMaxCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.E11].Entries[entryName].Time;
                            hrEntries.Values = prinMidArray.ToList();
                            prinMidCom.Entries.Add(entryName, hrEntries);
                            //
                            hrEntries = new HistoryResultEntries(entryName, false);
                            hrEntries.Time = fieldEntry.Value.Components[HOComponentNames.E11].Entries[entryName].Time;
                            hrEntries.Values = prinMinArray.ToList();
                            prinMinCom.Entries.Add(entryName, hrEntries);
                        }
                        //
                        fieldEntry.Value.Components.Insert(0, vonMisesCom.Name, vonMisesCom);
                        fieldEntry.Value.Components.Insert(1, trescaCom.Name, trescaCom);
                        fieldEntry.Value.Components.Add(sgnMaxAbsPrinCom.Name, sgnMaxAbsPrinCom);
                        fieldEntry.Value.Components.Add(prinMaxCom.Name, prinMaxCom);
                        fieldEntry.Value.Components.Add(prinMidCom.Name, prinMidCom);
                        fieldEntry.Value.Components.Add(prinMinCom.Name, prinMinCom);
                    }
                }
            }
        }
        static private void AddTotalEffectiveModalMassToMassRatio(HistoryResults historyOutput)
        {
            double[] effectiveModelMassTimes;
            double[] effectiveModelMassValues;
            double value;
            double totalModalMassValue;
            double totalMassValue;
            HistoryResultEntries entry;
            HistoryResultComponent effectiveModalMass;
            HistoryResultComponent totalModalMass;
            HistoryResultComponent totalMass;
            HistoryResultComponent relativeEffectiveModalMass = new HistoryResultComponent(HOFieldNames.RelativeEffectiveModalMass);
            HistoryResultComponent relativeTotalMass = new HistoryResultComponent(HOFieldNames.RelativeTotalEffectiveModalMass);
            string[] entryNames = new string[] { HOComponentNames.XCOMPONENT,
                                                 HOComponentNames.YCOMPONENT,
                                                 HOComponentNames.ZCOMPONENT,
                                                 HOComponentNames.XROTATION,
                                                 HOComponentNames.YROTATION,
                                                 HOComponentNames.ZROTATION };
            //
            foreach (var setsEntry in historyOutput.Sets)
            {
                foreach (var fieldEntry in setsEntry.Value.Fields)
                {
                    if (fieldEntry.Key == HOFieldNames.Frequency)
                    {
                        if (fieldEntry.Value.Components.TryGetValue(HOFieldNames.EffectiveModalMass, out effectiveModalMass) &&
                            fieldEntry.Value.Components.TryGetValue(HOFieldNames.TotalEffectiveModalMass, out totalModalMass) &&
                            fieldEntry.Value.Components.TryGetValue(HOFieldNames.TotalEffectiveMass, out totalMass))
                        {
                            foreach (var entryName in entryNames)
                            {
                                effectiveModelMassValues = effectiveModalMass.Entries[entryName].Values.ToArray();
                                effectiveModelMassTimes = effectiveModalMass.Entries[entryName].Time.ToArray();
                                totalModalMassValue = totalModalMass.Entries[entryName].Values.ToArray()[0];
                                totalMassValue = totalMass.Entries[entryName].Values.ToArray()[0];
                                // Relative effective modal mass
                                entry = new HistoryResultEntries(entryName, false);
                                for (int i = 0; i < effectiveModelMassValues.Length; i++)
                                {
                                    if (totalMassValue != 0) value = effectiveModelMassValues[i] / totalMassValue;
                                    else value = 0;
                                    value = Tools.RoundToSignificantDigits(value, 6);
                                    entry.Add(effectiveModelMassTimes[i],value);
                                }
                                relativeEffectiveModalMass.Entries.Add(entry.Name, entry);
                                // Ratio
                                if (totalMassValue != 0) value = totalModalMassValue / totalMassValue;
                                else value = 0;
                                value = Tools.RoundToSignificantDigits(value, 6);
                                //
                                entry = new HistoryResultEntries(entryName, false);
                                entry.Add(1, value);
                                //
                                relativeTotalMass.Entries.Add(entry.Name, entry);
                            }
                            //
                            fieldEntry.Value.Components.Add(relativeEffectiveModalMass.Name, relativeEffectiveModalMass);
                            fieldEntry.Value.Components.Add(relativeTotalMass.Name, relativeTotalMass);
                        }
                    }
                }
            }
        }



    }
}
