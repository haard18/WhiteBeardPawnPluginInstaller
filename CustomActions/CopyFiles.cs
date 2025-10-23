using System;
using System.IO;
using Microsoft.Win32;
using Microsoft.Deployment.WindowsInstaller;

namespace WhiteBeardPawnPlugin.CustomActions
{
    public class CopyFiles
    {
        /// <summary>
        /// Custom Action: Copy Plugin Files
        /// Copies PawnPlugin64.dll to MT5 Plugins directory and license file to ProgramData
        /// </summary>
        [CustomAction]
        public static ActionResult CopyPluginFiles(Session session)
        {
            session.Log("Begin CopyPluginFiles Custom Action");

            try
            {
                // Get paths from session properties
                string mt5Path = session["MT5_INSTALL_PATH"];
                string licenseFilePath = session["LICENSE_FILE_PATH"];
                string installerPath = session["INSTALLFOLDER"];

                session.Log($"MT5 Path: {mt5Path}");
                session.Log($"License File: {licenseFilePath}");
                session.Log($"Installer Path: {installerPath}");

                if (string.IsNullOrEmpty(mt5Path))
                {
                    session.Log("ERROR: MT5_INSTALL_PATH is empty");
                    return ActionResult.Failure;
                }

                // Step 1: Create Plugins directory if it doesn't exist
                string pluginsPath = Path.Combine(mt5Path, "Plugins");
                if (!Directory.Exists(pluginsPath))
                {
                    session.Log($"Creating Plugins directory: {pluginsPath}");
                    Directory.CreateDirectory(pluginsPath);
                }

                // Step 2: Copy PawnPlugin64.dll to Plugins directory
                string sourceDll = Path.Combine(installerPath, "PawnPlugin64.dll");
                string destDll = Path.Combine(pluginsPath, "PawnPlugin64.dll");

                session.Log($"Copying DLL from {sourceDll} to {destDll}");
                
                if (File.Exists(sourceDll))
                {
                    File.Copy(sourceDll, destDll, true);
                    session.Log("DLL copied successfully");
                }
                else
                {
                    session.Log($"ERROR: Source DLL not found: {sourceDll}");
                    return ActionResult.Failure;
                }

                // Step 3: Create WhiteBeard directory in ProgramData if it doesn't exist
                string programDataPath = @"C:\ProgramData\WhiteBeard";
                if (!Directory.Exists(programDataPath))
                {
                    session.Log($"Creating directory: {programDataPath}");
                    Directory.CreateDirectory(programDataPath);
                }

                // Step 4: Copy license file to ProgramData\WhiteBeard
                if (!string.IsNullOrEmpty(licenseFilePath) && File.Exists(licenseFilePath))
                {
                    string destLicense = Path.Combine(programDataPath, Path.GetFileName(licenseFilePath));
                    session.Log($"Copying license from {licenseFilePath} to {destLicense}");
                    File.Copy(licenseFilePath, destLicense, true);
                    session.Log("License file copied successfully");
                }
                else
                {
                    session.Log("WARNING: License file not found or path is empty");
                }

                // Step 5: Create registry entries
                CreateRegistryEntries(session, mt5Path, licenseFilePath);

                session.Log("CopyPluginFiles completed successfully");
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log($"ERROR in CopyPluginFiles: {ex.Message}");
                session.Log($"Stack Trace: {ex.StackTrace}");
                return ActionResult.Failure;
            }
        }

        /// <summary>
        /// Create registry entries for the plugin installation
        /// </summary>
        private static void CreateRegistryEntries(Session session, string mt5Path, string licenseFilePath)
        {
            try
            {
                session.Log("Creating registry entries");

                // Create registry key: HKLM\Software\WhiteBeard\PawnPlugin
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"Software\WhiteBeard\PawnPlugin"))
                {
                    if (key != null)
                    {
                        key.SetValue("InstallPath", mt5Path, RegistryValueKind.String);
                        key.SetValue("LicensePath", licenseFilePath ?? "", RegistryValueKind.String);
                        key.SetValue("Version", "1.0.0", RegistryValueKind.String);
                        key.SetValue("InstallDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), RegistryValueKind.String);
                        key.SetValue("CompanyName", session["COMPANY_NAME"] ?? "", RegistryValueKind.String);
                        key.SetValue("CompanyEmail", session["COMPANY_EMAIL"] ?? "", RegistryValueKind.String);

                        session.Log("Registry entries created successfully");
                    }
                    else
                    {
                        session.Log("ERROR: Could not create registry key");
                    }
                }
            }
            catch (Exception ex)
            {
                session.Log($"ERROR in CreateRegistryEntries: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Custom Action: Cleanup on Uninstall
        /// Removes plugin files and registry entries
        /// </summary>
        [CustomAction]
        public static ActionResult CleanupFiles(Session session)
        {
            session.Log("Begin CleanupFiles Custom Action");

            try
            {
                // Read registry to get installation paths
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\WhiteBeard\PawnPlugin"))
                {
                    if (key != null)
                    {
                        string mt5Path = key.GetValue("InstallPath")?.ToString();
                        
                        if (!string.IsNullOrEmpty(mt5Path))
                        {
                            // Remove DLL from Plugins directory
                            string dllPath = Path.Combine(mt5Path, "Plugins", "PawnPlugin64.dll");
                            if (File.Exists(dllPath))
                            {
                                session.Log($"Removing DLL: {dllPath}");
                                File.Delete(dllPath);
                            }
                        }
                    }
                }

                // Remove registry key
                session.Log("Removing registry key");
                Registry.LocalMachine.DeleteSubKeyTree(@"Software\WhiteBeard\PawnPlugin", false);

                session.Log("Cleanup completed successfully");
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log($"ERROR in CleanupFiles: {ex.Message}");
                session.Log($"Stack Trace: {ex.StackTrace}");
                // Don't fail uninstall if cleanup fails
                return ActionResult.Success;
            }
        }
    }
}
