using System;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;

namespace WhiteBeardPawnPlugin.CustomActions
{
    public class DetectMT5
    {
        /// <summary>
        /// Custom Action: Detect MetaTrader 5 Installation
        /// Checks default MT5 installation path and validates it
        /// </summary>
        [CustomAction]
        public static ActionResult DetectMT5Installation(Session session)
        {
            session.Log("Begin DetectMT5Installation Custom Action");

            try
            {
                // Default MT5 installation path
                string defaultMT5Path = @"C:\Program Files\MetaTrader 5 Platform\TradeMain";
                session.Log($"Checking default MT5 path: {defaultMT5Path}");

                // Check if MT5 is installed in default location
                if (Directory.Exists(defaultMT5Path))
                {
                    // Verify it's a valid MT5 installation by checking for key files
                    string mt5Executable = Path.Combine(defaultMT5Path, "terminal64.exe");
                    
                    if (File.Exists(mt5Executable))
                    {
                        session.Log("MetaTrader 5 found in default location");
                        session["MT5_INSTALL_PATH"] = defaultMT5Path;
                        session["MT5_FOUND"] = "1";
                        return ActionResult.Success;
                    }
                    else
                    {
                        session.Log("Directory exists but terminal64.exe not found");
                    }
                }

                // Check common alternative locations
                string[] alternativePaths = new string[]
                {
                    @"C:\Program Files (x86)\MetaTrader 5",
                    @"C:\Program Files\MetaTrader 5",
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\MetaTrader 5",
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\MetaTrader 5"
                };

                foreach (var altPath in alternativePaths)
                {
                    session.Log($"Checking alternative path: {altPath}");
                    
                    if (Directory.Exists(altPath))
                    {
                        string mt5Exe = Path.Combine(altPath, "terminal64.exe");
                        if (File.Exists(mt5Exe))
                        {
                            session.Log($"MetaTrader 5 found at: {altPath}");
                            session["MT5_INSTALL_PATH"] = altPath;
                            session["MT5_FOUND"] = "1";
                            return ActionResult.Success;
                        }
                    }
                }

                // MT5 not found - user will need to browse
                session.Log("MetaTrader 5 not found in any default location. User must browse.");
                session["MT5_FOUND"] = "0";
                session["MT5_INSTALL_PATH"] = "";
                
                return ActionResult.Success; // Continue to allow user to browse
            }
            catch (Exception ex)
            {
                session.Log($"ERROR in DetectMT5Installation: {ex.Message}");
                session.Log($"Stack Trace: {ex.StackTrace}");
                session["MT5_FOUND"] = "0";
                return ActionResult.Success; // Don't fail, just mark as not found
            }
        }

        /// <summary>
        /// Custom Action: Validate MT5 Path
        /// Validates that the user-selected path is a valid MT5 installation
        /// </summary>
        [CustomAction]
        public static ActionResult ValidateMT5Path(Session session)
        {
            session.Log("Begin ValidateMT5Path Custom Action");

            try
            {
                string mt5Path = session["MT5_INSTALL_PATH"];
                
                if (string.IsNullOrEmpty(mt5Path))
                {
                    session.Log("ERROR: MT5_INSTALL_PATH is empty");
                    return ActionResult.Failure;
                }

                session.Log($"Validating MT5 path: {mt5Path}");

                // Check if directory exists
                if (!Directory.Exists(mt5Path))
                {
                    session.Log($"ERROR: Directory does not exist: {mt5Path}");
                    return ActionResult.Failure;
                }

                // Check for terminal64.exe
                string mt5Executable = Path.Combine(mt5Path, "terminal64.exe");
                if (!File.Exists(mt5Executable))
                {
                    session.Log($"ERROR: terminal64.exe not found in: {mt5Path}");
                    return ActionResult.Failure;
                }

                // Check for Plugins directory or create it
                string pluginsPath = Path.Combine(mt5Path, "Plugins");
                if (!Directory.Exists(pluginsPath))
                {
                    session.Log($"Plugins directory does not exist. Will be created during installation.");
                }

                session.Log("MT5 path validated successfully");
                session["MT5_PATH_VALID"] = "1";
                
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log($"ERROR in ValidateMT5Path: {ex.Message}");
                session.Log($"Stack Trace: {ex.StackTrace}");
                return ActionResult.Failure;
            }
        }
    }
}
