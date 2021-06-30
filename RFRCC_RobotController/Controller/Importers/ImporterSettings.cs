using CopingLineModels;
using ABB.Robotics.Math;
using System;


namespace RFRCC_RobotController.Controller.Importers
{
    /// <summary>
    /// Settings used to specify import characteristics and requirments 
    /// </summary>
    public class ImporterSettings
    {
        /// <summary>
        /// Main settings used directly by parsing intity to decode and generate information from ASCII file
        /// </summary>
        public ImportSettings Import = new ImportSettings();
        /// <summary>
        /// Main settings used directly in the generation of robot data from processing of job
        /// </summary>
        public GenerationSettings Generation = new GenerationSettings();
        /// <summary>
        /// Initialise object with default of relevant requirements
        /// </summary>
        public ImporterSettings()
        {
            Update();
        }
        /// <summary>
        /// Update all settings to default or relevant requirements
        /// </summary>
        public void Update()
        {
            UpdateImportSettings();
            UpdateGenerationSettings();
        }
        /// <summary>
        /// Update Generation settings to default or relevant requirements
        /// </summary>
        public void UpdateGenerationSettings()
        {
            Properties.Settings settings = Properties.Settings.Default;

            foreach (OperationType type in Enum.GetValues(typeof(OperationType)))
            {

                LeadInOutSettings leadinout = new LeadInOutSettings(false, 0, 0, false); // not enabled by default
                PathSettings pathsettings = new PathSettings(1, 1, 1, leadinout);
                switch (type)
                {
                    case OperationType.BoltHoleCut:
                    case OperationType.OneDSlottedHoleCut:
                    case OperationType.TwoDSlottedHoleCut:
                        leadinout = new LeadInOutSettings(settings.GenerationHoleLeadInOutEnabled,
                                                    (double)settings.GenerationHoleLeadInDist / 1000.0,
                                                    (double)settings.GenerationHoleLeadOutDist / 1000 / 0,
                                                    settings.GenerationHoleLinearLeadIn);
                        pathsettings = new PathSettings((int)settings.GenerationOpSettingsHoleApproxDist,
                                                    (double)settings.GenerationOpSettingsHoleAccel,
                                                    (double)settings.GenerationOpSettingsHoleAccel,
                                                    leadinout);
                        break;
                    case OperationType.ExternalContour:
                        leadinout = new LeadInOutSettings(settings.GenerationExtContourLeadInOutEnabled,
                                                    (double)settings.GenerationExtContourLeadInDist / 1000.0,
                                                    (double)settings.GenerationExtContourLeadOutDist / 1000.0,
                                                    settings.GenerationExtContourLinearLeadIn);
                        pathsettings = new PathSettings((int)settings.GenerationOpSettingsExtContourApproxDist,
                                                    (double)settings.GenerationOpSettingsExtContourAccel,
                                                    (double)settings.GenerationOpSettingsExtContourAccel,
                                                    leadinout);
                        break;
                    case OperationType.InternalContour:
                        leadinout = new LeadInOutSettings(settings.GenerationIntContourLeadInOutEnabled,
                                                    (double)settings.GenerationIntContourLeadInDist / 1000.0,
                                                    (double)settings.GenerationIntContourLeadOutDist / 1000.0,
                                                    settings.GenerationIntContourLinearLeadIn);
                        pathsettings = new PathSettings((int)settings.GenerationOpSettingsIntContourApproxDist,
                                                    (double)settings.GenerationOpSettingsIntContourAccel,
                                                    (double)settings.GenerationOpSettingsIntContourAccel,
                                                    leadinout);
                        break;
                    case OperationType.Marking:
                    default:
                        // as default
                        break;
                }


                if (Generation.OperationSettings.ContainsKey(type)) Generation.OperationSettings[type] = pathsettings;
                else Generation.OperationSettings.Add(type, pathsettings);
            }

            // Manoeuvre controls
            Generation.LoadSequenceEnabled = settings.LoadSequenceEnabled;
            Generation.IHSEnabled = settings.IHSEnabled;
            Generation.HitchfeedingEnabled = settings.HitchfeedingEnabled;
            Generation.EGMEnabled = settings.EGMEnabled;
            Generation.LocationEnabled = settings.LocationSequenceEnabled;
            Generation.PathCorrectionEnabled = settings.PathCorrectionEnabled;

        }
        /// <summary>
        /// Update Import settings to default or relevant requirements
        /// </summary>
        public void UpdateImportSettings()
        {
            Properties.Settings settings = Properties.Settings.Default;

            // Defaults
            Import.DefaultStockLength = (double)settings.ImportDefaultStockLength;

            // Face definitions
            Import.FaceDefinitions.Clear();
            int number_of_faces_desired = (int)settings.ImportNumberOfSectors; // default 4
            bool start_aligned_with_face = settings.ImportStartAlignedWithFace; // default false
            bool custom_angles = settings.ImportAssignCustomSectors;   // default false
            double quadrant_angle = 2 * Math.PI / number_of_faces_desired;
            double start_angle;
            if (start_aligned_with_face) start_angle = 0;
            else start_angle = quadrant_angle / 2;

            // public enum PlateFace { Unknown = -1, Behind, BehindTop, Top, TopFront, Front, FrontBottom, Bottom, BottomBehind };
            if (!custom_angles)
            {
                for (int i = 0; i < number_of_faces_desired; i++)
                {
                    double min = start_angle - (i + 1) * quadrant_angle;
                    double max = start_angle - i * quadrant_angle;
                    if (min < -start_angle) min += 2 * Math.PI;
                    if (max <= -start_angle) max += 2 * Math.PI;
                    int closest_face = (int)Math.Round((double)i * (8.0 / (double)number_of_faces_desired));
                    Import.FaceDefinitions.Add((PlateFace)closest_face, new FaceQuadrant(min, max));
                }
            }
            else
            {
                double min = Globals.DegToRad((double)settings.ImportSMin);
                double max = Globals.DegToRad((double)settings.ImportSMax);
                Import.FaceDefinitions.Add(PlateFace.Behind, new FaceQuadrant(min, max));

                min = Globals.DegToRad((double)settings.ImportSEMin);
                max = Globals.DegToRad((double)settings.ImportSEMax);
                Import.FaceDefinitions.Add(PlateFace.BottomBehind, new FaceQuadrant(min, max));

                min = Globals.DegToRad((double)settings.ImportEMin);
                max = Globals.DegToRad((double)settings.ImportEMax);
                Import.FaceDefinitions.Add(PlateFace.Bottom, new FaceQuadrant(min, max));

                min = Globals.DegToRad((double)settings.ImportNEMin);
                max = Globals.DegToRad((double)settings.ImportNEMax);
                Import.FaceDefinitions.Add(PlateFace.FrontBottom, new FaceQuadrant(min, max));

                min = Globals.DegToRad((double)settings.ImportNMin);
                max = Globals.DegToRad((double)settings.ImportNMax);
                Import.FaceDefinitions.Add(PlateFace.Front, new FaceQuadrant(min, max));

                min = Globals.DegToRad((double)settings.ImportNWMin);
                max = Globals.DegToRad((double)settings.ImportNWMax);
                Import.FaceDefinitions.Add(PlateFace.TopFront, new FaceQuadrant(min, max));

                min = Globals.DegToRad((double)settings.ImportWMin);
                max = Globals.DegToRad((double)settings.ImportWMax);
                Import.FaceDefinitions.Add(PlateFace.Top, new FaceQuadrant(min, max));

                min = Globals.DegToRad((double)settings.ImportSWMin);
                max = Globals.DegToRad((double)settings.ImportSWMax);
                Import.FaceDefinitions.Add(PlateFace.BehindTop, new FaceQuadrant(min, max));
            }
        }
    }
}
