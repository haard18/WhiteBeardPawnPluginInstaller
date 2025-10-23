using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.Deployment.WindowsInstaller;

namespace WhiteBeardPawnPlugin.CustomActions
{
    public class VerifyLicense
    {
        /// <summary>
        /// Custom Action: Verify License File
        /// Searches for license file, decrypts it, verifies signature, and extracts company info
        /// </summary>
        [CustomAction]
        public static ActionResult VerifyLicenseFile(Session session)
        {
            session.Log("Begin VerifyLicenseFile Custom Action");

            try
            {
                // Step 1: Get license file path from property or search default location
                string licenseFilePath = session["LICENSE_FILE_PATH"];
                
                if (string.IsNullOrEmpty(licenseFilePath))
                {
                    // Search default location: C:\ProgramData\WhiteBeard\*_pawn_plugin.lic
                    string defaultPath = @"C:\ProgramData\WhiteBeard";
                    session.Log($"Searching for license file in: {defaultPath}");
                    
                    if (Directory.Exists(defaultPath))
                    {
                        var licenseFiles = Directory.GetFiles(defaultPath, "*_pawn_plugin.lic");
                        if (licenseFiles.Length > 0)
                        {
                            licenseFilePath = licenseFiles[0];
                            session.Log($"Found license file: {licenseFilePath}");
                        }
                    }
                }

                // Step 2: If still not found, prompt user (set property to trigger dialog)
                if (string.IsNullOrEmpty(licenseFilePath) || !File.Exists(licenseFilePath))
                {
                    session.Log("License file not found. User must browse for it.");
                    session["LICENSE_FOUND"] = "0";
                    return ActionResult.Success; // Continue to allow user to browse
                }

                // Step 3: Read and decrypt license file
                session.Log($"Reading license file: {licenseFilePath}");
                string licenseContent = File.ReadAllText(licenseFilePath);

                // Step 4: Verify and decrypt using public key
                var licenseData = DecryptAndVerifyLicense(licenseContent, session);
                
                if (licenseData == null)
                {
                    session.Log("ERROR: License verification failed");
                    session["LICENSE_VALID"] = "0";
                    return ActionResult.Failure;
                }

                // Step 5: Extract company information
                session["COMPANY_NAME"] = licenseData.CompanyName;
                session["COMPANY_EMAIL"] = licenseData.CompanyEmail;
                session["LICENSE_VALID"] = "1";
                session["LICENSE_FOUND"] = "1";
                session["LICENSE_FILE_PATH"] = licenseFilePath;

                session.Log($"License verified successfully for: {licenseData.CompanyName} ({licenseData.CompanyEmail})");
                
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log($"ERROR in VerifyLicenseFile: {ex.Message}");
                session.Log($"Stack Trace: {ex.StackTrace}");
                session["LICENSE_VALID"] = "0";
                return ActionResult.Failure;
            }
        }

        /// <summary>
        /// Verify the license signature using RSA public key
        /// </summary>
        private static LicenseData DecryptAndVerifyLicense(string licenseContent, Session session)
        {
            try
            {
                // Load public key from embedded resource or file
                string publicKeyPath = Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "PublicKey.xml"
                );

                if (!File.Exists(publicKeyPath))
                {
                    session.Log($"ERROR: Public key not found at {publicKeyPath}");
                    return null;
                }

                string publicKeyXml = File.ReadAllText(publicKeyPath);

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(publicKeyXml);

                    // Parse license file - extract data and signature
                    string[] lines = licenseContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    StringBuilder dataBuilder = new StringBuilder();
                    StringBuilder signatureBuilder = new StringBuilder();
                    bool inSignature = false;
                    bool inData = false;

                    foreach (string line in lines)
                    {
                        if (line.Contains("-----BEGIN WHITEBEARD LICENSE-----"))
                        {
                            inData = true;
                            continue;
                        }
                        if (line.Contains("-----BEGIN SIGNATURE-----"))
                        {
                            inData = false;
                            inSignature = true;
                            continue;
                        }
                        if (line.Contains("-----END WHITEBEARD LICENSE-----"))
                        {
                            break;
                        }

                        if (inData)
                            dataBuilder.Append(line);
                        else if (inSignature)
                            signatureBuilder.Append(line);
                    }

                    // Decode base64
                    byte[] dataBytes = Convert.FromBase64String(dataBuilder.ToString());
                    byte[] signature = Convert.FromBase64String(signatureBuilder.ToString());

                    // Verify signature
                    bool isValid = rsa.VerifyData(dataBytes, SHA256.Create(), signature);

                    if (!isValid)
                    {
                        session.Log("ERROR: License signature verification failed!");
                        return null;
                    }

                    session.Log("License signature verified successfully");

                    // Parse license data from verified JSON
                    string jsonData = Encoding.UTF8.GetString(dataBytes);
                    var licenseData = ParseLicenseData(jsonData, session);
                    
                    return licenseData;
                }
            }
            catch (Exception ex)
            {
                session.Log($"ERROR in DecryptAndVerifyLicense: {ex.Message}");
                session.Log($"Stack Trace: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Parse license data from JSON
        /// </summary>
        private static LicenseData ParseLicenseData(string jsonData, Session session)
        {
            try
            {
                // Simple JSON parser (or use Json.NET/System.Text.Json if available)
                var data = new LicenseData();
                
                // Extract values using simple string parsing
                // For production, use proper JSON deserialization
                var json = jsonData.Replace("{", "").Replace("}", "").Replace("\"", "");
                foreach (var line in json.Split(','))
                {
                    var parts = line.Split(':');
                    if (parts.Length < 2) continue;
                    
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    
                    if (key == "CompanyName")
                        data.CompanyName = value;
                    else if (key == "CompanyEmail")
                        data.CompanyEmail = value;
                    else if (key == "ExpiryDate" && !string.IsNullOrEmpty(value) && value != "null")
                    {
                        if (DateTime.TryParse(value, out DateTime expiry))
                        {
                            data.ExpiryDate = expiry;
                            if (expiry < DateTime.UtcNow)
                            {
                                session.Log($"WARNING: License expired on {expiry:yyyy-MM-dd}");
                                return null; // License expired
                            }
                        }
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                session.Log($"ERROR parsing license data: {ex.Message}");
                return null;
            }
        }

        private class LicenseData
        {
            public string CompanyName { get; set; }
            public string CompanyEmail { get; set; }
            public DateTime? ExpiryDate { get; set; }
        }
    }
}
