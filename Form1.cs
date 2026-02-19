using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * TODO:
 * Improve error catching
 */

// For now, localization will not be implemented due to the scarcity of non-English assets.
namespace FarmReplower
{
    public partial class frmSetup : Form
    {
        private sealed class InstallableComponent
        {
            private readonly Func<string> _installer;
            private readonly string _name;
            private readonly HashSet<string> _hashes;
            private readonly HashSet<Uri> _uris;

            public InstallableComponent(Func<string> installer, string name, IEnumerable<string> hashes, IEnumerable<Uri> uris)
            {
                _installer = installer ?? throw new ArgumentNullException(nameof(installer));
                _name = name ?? throw new ArgumentNullException(nameof(name));
                _hashes = new HashSet<string>(hashes);
                _uris = new HashSet<Uri>(uris);
            }

            public string Install() => _installer();
            public bool CanInstall(HashSet<string> hashes) => hashes != null && _hashes.IsSubsetOf(hashes);
            public Func<string> Installer => _installer;
            public string Name => _name;
            public IEnumerable<string> Hashes => _hashes;
            public IEnumerable<Uri> Uris => _uris;
        }

        private readonly string[] _defaultText = {
            "Farm Replower", "Configuration", "Username",
            "Password", "Confirm Password", "Database Name",
            "Installation Path", string.Empty, "Install", "Set Search Path"
        };

        private readonly InstallableComponent[] _installableComponents;
        private readonly Dictionary<string, HashSet<string>> _hashCache = new Dictionary<string, HashSet<string>>();
        private string _searchPath = Application.StartupPath;
        private string _stackPath;
        private string _repoPath;
        private string _schemaPath;
        private string _dehasherPath;
        private string _assetsPath;
        private string _composerPath;
        private string _nodePath;
        private int _currentComponentIndex;

        public frmSetup()
        {
            InitializeComponent();

            _installableComponents = new InstallableComponent[]{
                new InstallableComponent(InstallStack, "Stack", new string[]{
                    "ce3bdf852bd62c7363cb51d66e709b6a9bf5f3ea59bc1712ffda11d9238e5651"
                }, new Uri[]{
                    new Uri("https://sourceforge.net/projects/xampp/files/XAMPP Windows/8.2.12/xampp-portable-windows-x64-8.2.12-0-VS16.zip")
                }),
                
                new InstallableComponent(InstallComposer, "Composer", new string[]{
                    "471f2d857abf0ec18af7b055e61472214d91adb24f9bdbbb864c1c64faad7dd6"
                }, new Uri[]{
                    new Uri("https://getcomposer.org/download/2.9.2/composer.phar")
                }),

                new InstallableComponent(InstallRepo, "Repo", new string[]{
                    "fbc9fd0fcb8dc68c7a1a67c3bb179a211f0e43e0606ca6b71c32c0d01d3764d8"
                }, new Uri[]{
                    new Uri("https://github.com/FV-Replowed/fv-replowed/archive/4c349827936260e1f9dbc740fb74fc1c3edcc814.zip")
                }),

                new InstallableComponent(InstallNode, "Node", new string[]{
                    "3c624e9fbe07e3217552ec52a0f84e2bdc2e6ffa7348f3fdfb9fbf8f42e23fcf"
                }, new Uri[]{
                    new Uri("https://nodejs.org/dist/v22.21.1/node-v22.21.1-win-x64.zip")
                }),

                new InstallableComponent(InstallSchema, "Schema", new string[]{
                    "2f23e3a62662023d98f6f3dbf92992ade0aa568fc73a5d144f44b2baf165ba2d"
                }, new Uri[]{
                    new Uri("https://mega.nz/file/TrBlxSbJ#uBkzcoeMyPFd-TyCIEHiN5Ozqq4wMDjyBBGLGAc40jw")
                }),
                
                new InstallableComponent(InstallDehasher, "Native Dehasher", new string[]{
                    "743e6e6d6704e67dbee288d5ba5101fe33551dd65d90ac761bab1b336b116392"
                }, new Uri[]{
                    new Uri("https://github.com/PuccamiteTech/FVDehasher/releases/download/1.02-SNAPSHOT/windows-build.zip")
                }),

                new InstallableComponent(InstallAssets, "Assets", new string[]{
                    "218bace2dbebf7ba736a2d0542aeed190db985094ad6c0100aa345d9b3e64708",
                    "df5222d3508504f50640a9b2c37316599f3c52e347c301727eede21445cde57e",
                    "1c17dcd122ee63ad97f5559863b6cc86bc1caae078d2395217b7563fb1232ad6",
                    "7569055c0da5d1b3ed8d22117b7f8bb5cefd319dbd3bb69d88589e8c16cc9bb8"
                }, new Uri[]{
                    new Uri("https://archive.org/download/original-farmville/urls-bluepload.unstable.life-farmvilleassets.txt-shallow-20201225-045045-5762m-00000.warc.gz"),
                    new Uri("https://archive.org/download/original-farmville/urls-bluepload.unstable.life-farmvilleassets.txt-shallow-20201225-045045-5762m-00001.warc.gz"),
                    new Uri("https://archive.org/download/original-farmville/urls-bluepload.unstable.life-farmvilleassets.txt-shallow-20201225-045045-5762m-00002.warc.gz"),
                    new Uri("https://archive.org/download/original-farmville/urls-bluepload.unstable.life-farmvilleassets.txt-shallow-20201225-045045-5762m-00003.warc.gz")
                })
            };
        }

        private static string ComputeHash(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                using (SHA256 sha256 = SHA256.Create())
                {
                    foreach (byte b in sha256.ComputeHash(stream))
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    return sb.ToString();
                }
            }
            catch
            {
                return null;
            }
        }

        private static string[] ReplaceText(string path, Dictionary<string, string> replacements)
        {
            try
            {
                string[] lines = File.ReadAllLines(path);
                
                for (int index = 0; index < lines.Length; index++)
                {
                    KeyValuePair<string, string> match = replacements.FirstOrDefault(e => lines[index].Trim().StartsWith(e.Key));

                    if (match.Key != null)
                    {
                        lines[index] = match.Value;
                    }
                }

                return lines;
            }
            catch
            {
                return null;
            }
        }

        private static string FormatInstallationError(Exception err)
        {
            return "Installation Failed: " + err.Message;
        }

        // allowing uppercase hex characters would be illogical in this context
        private static bool IsHash(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Length == 64 &&
                input.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'));
        }

        private static void MergeContents(string sourceDirName, string destDirName, bool overwrite)
        {
            string relativePath;
            string destPath;

            sourceDirName = Path.GetFullPath(sourceDirName).TrimEnd(Path.DirectorySeparatorChar);
            destDirName = Path.GetFullPath(destDirName);

            foreach (string file in Directory.EnumerateFiles(sourceDirName, "*", SearchOption.AllDirectories))
            {
                relativePath = file.Substring(sourceDirName.Length).TrimStart(Path.DirectorySeparatorChar);
                destPath = Path.Combine(destDirName, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(destPath) ?? destDirName);

                if (overwrite && File.Exists(destPath))
                {
                    File.Delete(destPath);
                }

                File.Move(file, destPath);
            }
        }

        private string InstallStack()
        {
            _stackPath = Path.Combine(txtInstallationPath.Text, "xampp");
            _repoPath = Path.Combine(_stackPath, "htdocs");
            string controlIniPath = Path.Combine(_stackPath, "xampp-control.ini");
            string phpIniPath = Path.Combine(_stackPath, "php", "php.ini");
            string pmaConfPath = Path.Combine(_stackPath, "phpMyAdmin", "config.inc.php");
            string httpdConfPath = Path.Combine(_stackPath, "apache", "conf", "httpd.conf");
            string publicPath = Path.Combine(_repoPath, "public").Replace('\\', '/');

            Dictionary<string, string> controlIniReplacements = new Dictionary<string, string>
            {
                {"Tomcat=1", "Tomcat=0"}
            };

            Dictionary<string, string> phpIniReplacements = new Dictionary<string, string>
            {
                {"error_reporting =", "error_reporting = E_ALL & ~E_DEPRECATED & ~E_STRICT"},
                {"display_errors =", "display_errors = Off"},
                {";extension=zip", "extension=zip"}
            };

            Dictionary<string, string> httpdConfReplacements = new Dictionary<string, string>
            {
                {"DocumentRoot", $"DocumentRoot \"{publicPath}\""},
                {$"<Directory \"{_repoPath.Replace('\\', '/').Replace("C:", string.Empty)}\"", $"<Directory \"{publicPath}\">"}
            };

            Dictionary<string, string> pmaConfReplacements = new Dictionary<string, string>
            {
                {"$cfg['Servers'][$i]['password']", $"$cfg['Servers'][$i]['password'] = '{txtPassword.Text}';"},
                {"$cfg['Servers'][$i]['controlpass']", $"$cfg['Servers'][$i]['controlpass'] = '{txtPassword.Text}';"}
            };

            try
            {
                foreach (string hash in _installableComponents[_currentComponentIndex].Hashes)
                {
                    ZipFile.ExtractToDirectory(LocateFile(hash), txtInstallationPath.Text);
                }

                Directory.Delete(Path.Combine(_stackPath, "tomcat"), true);
                File.WriteAllLines(controlIniPath, ReplaceText(controlIniPath, controlIniReplacements));
                File.WriteAllLines(phpIniPath, ReplaceText(phpIniPath, phpIniReplacements));
                File.WriteAllLines(pmaConfPath, ReplaceText(pmaConfPath, pmaConfReplacements));

                Process process = new Process();

                process.StartInfo.FileName = Path.Combine(_stackPath, "php", "php.exe");
                process.StartInfo.Arguments = @"-n -d output_buffering=0 -q install\install.php usb";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.WorkingDirectory = _stackPath;
                process.Start();
                process.WaitForExit();

                File.WriteAllLines(httpdConfPath, ReplaceText(httpdConfPath, httpdConfReplacements));

                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/c mysql_start.bat";
                process.Start();

                process.StartInfo.FileName = Path.Combine(_stackPath, "mysql", "bin", "mysql.exe");
                process.StartInfo.Arguments = $"-u root";
                process.StartInfo.RedirectStandardInput = true;
                process.Start();

                using (StreamWriter writer = process.StandardInput)
                {
                    writer.WriteLine($"ALTER USER 'root'@'localhost' IDENTIFIED BY '{txtPassword.Text}';");
                    writer.WriteLine($"ALTER USER 'pma'@'localhost' IDENTIFIED BY '{txtPassword.Text}';");
                    writer.WriteLine($"CREATE USER '{txtUsername.Text}'@'localhost' IDENTIFIED BY '{txtPassword.Text}';");
                    writer.WriteLine($"CREATE DATABASE `{txtDatabaseName.Text}`;");
                    writer.WriteLine($"GRANT ALL PRIVILEGES ON `{txtDatabaseName.Text}`.* TO '{txtUsername.Text}'@'localhost';");
                    writer.WriteLine("FLUSH PRIVILEGES;");
                    writer.WriteLine("DROP USER ''@'%';");
                    writer.WriteLine("DROP USER 'root'@'127.0.0.1';");
                    writer.WriteLine("DROP USER 'root'@'::1';");
                }

                process.WaitForExit();
            }
            catch (Exception err)
            {
                return FormatInstallationError(err);
            }

            return null;
        }

        private string InstallComposer()
        {
            _composerPath = Path.Combine(_stackPath, "composer.phar");

            try
            {
                foreach (string hash in _installableComponents[_currentComponentIndex].Hashes)
                {
                    File.Copy(LocateFile(hash), _composerPath);
                }

                File.WriteAllText(Path.Combine(_stackPath, "composer.bat"), "@\"%~dp0php\\php.exe\" \"%~dp0composer.phar\" %*");
            }
            catch (Exception err)
            {
                return FormatInstallationError(err);
            }

            return null;
        }

        private string InstallRepo()
        {
            string envExamplePath = Path.Combine(_repoPath, ".env.example");
            string configPhpPath = Path.Combine(_repoPath, "public", "farmville", "flashservices", "amfphp", "Helpers", "config.php");

            Dictionary<string, string> envReplacements = new Dictionary<string, string>
            {
                {"DB_DATABASE=", $"DB_DATABASE='{txtDatabaseName.Text}'"},
                {"DB_USERNAME=", $"DB_USERNAME='{txtUsername.Text}'"},
                {"DB_PASSWORD=", $"DB_PASSWORD='{txtPassword.Text}'"}
            };

            Dictionary<string, string> configPhpReplacements = new Dictionary<string, string>
            {
                {"define('DB_USERNAME'", $"define('DB_USERNAME', getenv('DB_USERNAME') ?: '{txtUsername.Text}');"},
                {"define('DB_PASSWORD'", $"define('DB_PASSWORD', getenv('DB_PASSWORD') ?: '{txtPassword.Text}');"},
                {"define('DB_NAME'", $"define('DB_NAME', getenv('DB_NAME') ?: '{txtDatabaseName.Text}');"}
            };

            try
            {
                Directory.Delete(_repoPath, true);

                foreach (string hash in _installableComponents[_currentComponentIndex].Hashes)
                {
                    ZipFile.ExtractToDirectory(LocateFile(hash), _stackPath);
                }
                
                foreach (string path in Directory.GetDirectories(_stackPath, "fv-replowed-*"))
                {
                    Directory.Move(path, _repoPath);
                }

                File.WriteAllLines(envExamplePath, ReplaceText(envExamplePath, envReplacements));
                File.Move(envExamplePath, Path.Combine(_repoPath, ".env"));
                File.WriteAllLines(configPhpPath,  ReplaceText(configPhpPath, configPhpReplacements));

                Process process = new Process();

                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c \"{Path.Combine(Path.GetDirectoryName(_repoPath), "composer.bat")}\" update";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.WorkingDirectory = _repoPath;
                process.Start();
                process.WaitForExit();

                process.StartInfo.FileName = Path.Combine(_stackPath, "php", "php.exe");
                process.StartInfo.Arguments = "artisan key:generate";
                process.Start();
                process.WaitForExit();

                process.StartInfo.Arguments = "artisan migrate --seed";
                process.Start();
                process.WaitForExit();
            }
            catch (Exception err)
            {
                return FormatInstallationError(err);
            }

            return null;
        }

        private string InstallNode()
        {
            _nodePath = Path.Combine(txtInstallationPath.Text, "node");
            string npmPath = Path.Combine(_nodePath, "npm.cmd");

            try
            {
                foreach (string hash in _installableComponents[_currentComponentIndex].Hashes)
                {
                    ZipFile.ExtractToDirectory(LocateFile(hash), txtInstallationPath.Text);
                }

                foreach (string path in Directory.GetDirectories(txtInstallationPath.Text, "node-*"))
                {
                    Directory.Move(path, _nodePath);
                }

                Process process = new Process();

                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.WorkingDirectory = _repoPath;
                process.StartInfo.EnvironmentVariables["PATH"] = _nodePath + ";" + (Environment.GetEnvironmentVariable("PATH") ?? "");
                process.StartInfo.EnvironmentVariables["NODE_SKIP_PLATFORM_CHECK"] = "1";

                process.StartInfo.Arguments = $"/c \"{npmPath}\" install --include-workspace-root";
                process.Start();
                process.WaitForExit();

                process.StartInfo.Arguments = $"/c \"{npmPath}\" run build";
                process.Start();
                process.WaitForExit();
            }
            catch (Exception err)
            {
                return FormatInstallationError(err);
            }

            return null;
        }

        private string InstallSchema()
        {
            _schemaPath = Path.Combine(_repoPath, "items.sql");

            try
            {
                foreach (string hash in _installableComponents[_currentComponentIndex].Hashes)
                {
                    File.Copy(LocateFile(hash), _schemaPath);
                }

                Process process = new Process();

                process.StartInfo.FileName = Path.Combine(_stackPath, "mysql", "bin", "mysql.exe");
                process.StartInfo.Arguments = $"-u root -p\"{txtPassword.Text}\" -D \"{txtDatabaseName.Text}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.WorkingDirectory = _stackPath;

                process.Start();

                using (StreamWriter writer = process.StandardInput)
                {
                    foreach (string line in File.ReadLines(_schemaPath))
                    {
                        writer.WriteLine(line);
                    }
                }

                process.WaitForExit();

                process.StartInfo.FileName = Path.Combine(_stackPath, "xampp_stop.exe");
                process.StartInfo.Arguments = string.Empty;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception err)
            {
                return FormatInstallationError(err);
            }

            return null;
        }

        private string InstallDehasher()
        {
            _dehasherPath = Path.Combine(txtInstallationPath.Text, "dehasher.exe");

            try
            {
                foreach (string hash in _installableComponents[_currentComponentIndex].Hashes)
                {
                    ZipFile.ExtractToDirectory(LocateFile(hash), txtInstallationPath.Text);
                }

                foreach (string path in Directory.GetFiles(txtInstallationPath.Text, "FVDehasher-*.exe"))
                {
                    File.Move(path, _dehasherPath);
                }
            }
            catch (Exception err)
            {
                return FormatInstallationError(err);
            }

            return null;
        }

        private string InstallAssets()
        {
            _assetsPath = Path.Combine(_repoPath, "public", "farmville", "assets");
            string moreAssetsPath = Path.Combine(Directory.GetParent(_assetsPath).FullName, "more_assets");

            try
            {
                Process process = new Process();

                process.StartInfo.FileName = _dehasherPath;
                process.StartInfo.Arguments = string.Empty;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.WorkingDirectory = txtInstallationPath.Text;

                foreach (string hash in _installableComponents[_currentComponentIndex].Hashes)
                {
                    process.StartInfo.Arguments += $"\"{LocateFile(hash)}\" ";
                }

                if (Directory.Exists(_assetsPath))
                {
                    Directory.Move(_assetsPath, moreAssetsPath);
                }

                process.Start();
                process.WaitForExit();

                Directory.Move(Path.Combine(txtInstallationPath.Text, "farmville", "assets"), _assetsPath);
                Directory.Delete(Path.Combine(txtInstallationPath.Text, "farmville"), true);

                if (Directory.Exists(moreAssetsPath))
                {
                    MergeContents(moreAssetsPath, _assetsPath, true);
                    Directory.Delete(moreAssetsPath, true);
                }

                File.Delete(Path.Combine(process.StartInfo.WorkingDirectory, "entries.txt"));
            }
            catch (Exception err)
            {
                return FormatInstallationError(err);
            }

            return null;
        }

        private string LocateFile(string hash)
        {
            if (_hashCache.TryGetValue(hash, out HashSet<string> value))
            {
                return value.FirstOrDefault();
            }

            return null;
        }

        private InstallableComponent ParseInstallableComponent(InstallableComponent baseComponent, string line)
        {
            string[] splitLine = line.Trim().Split(' ');
            HashSet<string> hashes = new HashSet<string>();
            HashSet<Uri> uris = new HashSet<Uri>();

            if (baseComponent == null)
            {
                return null;
            }

            foreach (string token in splitLine)
            {
                string trimmedToken = token.Trim();

                if (trimmedToken.Length > 0)
                {
                    if (IsHash(trimmedToken))
                    {
                        hashes.Add(trimmedToken);
                    }
                    else
                    {
                        try
                        {
                            uris.Add(new Uri(trimmedToken));
                        }
                        catch
                        {
                            Log($"Invalid Token: '{trimmedToken}'");
                        }
                    }
                }
            }

            if (hashes.Count == 0 && uris.Count == 0)
            {
                return null;
            }

            return new InstallableComponent(baseComponent.Installer, baseComponent.Name,
                (hashes.Count == 0) ? baseComponent.Hashes : hashes,
                (uris.Count == 0) ? baseComponent.Uris : uris);
        }

        private void UpdateInstallableComponents()
        {
            string updateFileName = "ic";
            IEnumerable<string> updateFileLines;
            InstallableComponent updatedComponent;
            int index = 0;

            try
            {
                if (File.Exists(updateFileName))
                {
                    updateFileLines = File.ReadLines(updateFileName).Take(_installableComponents.Length);
                    Log("Applying Updates...");

                    foreach (string line in updateFileLines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            updatedComponent = ParseInstallableComponent(_installableComponents[index], line);

                            if (updatedComponent == null)
                            {
                                Log("Failed to Update " + _installableComponents[index].Name);
                            }
                            else
                            {
                                _installableComponents[index] = updatedComponent;
                                Log("Updated " + _installableComponents[index].Name);
                            }
                        }

                        index++;
                    }
                }
                else
                {
                    Log("No Update File Found");
                }
            }
            catch
            {
                Log("Update Failed");
            }
        }

        private void CleanupHashCache()
        {
            foreach (HashSet<string> set in _hashCache.Values)
            {
                set.RemoveWhere(path => !File.Exists(path));
            }
        }

        private async Task UpdateHashCacheAsync()
        {
            IEnumerable<string> paths;
            string hash;

            CleanupHashCache();

            try
            {
                paths = Directory.EnumerateFiles(_searchPath);
            }
            catch
            {
                Log("Search Failed");
                return;
            }

            foreach (string path in paths)
            {
                if (!_hashCache.Values.Any(set => set.Contains(path)))
                {
                    hash = await Task.Run(() => ComputeHash(path));

                    if (hash == null)
                    {
                        Log("Hashing Failed for " + Path.GetFileName(path));
                    }
                    else
                    {
                        if (!_hashCache.TryGetValue(hash, out var set))
                        {
                            set = new HashSet<string>();
                            _hashCache[hash] = set;
                        }

                        set.Add(path);
                        Log($"Hashed {Path.GetFileName(path)} ({hash})");
                    }
                }
            }
        }
        private void Log(string message)
        {
            message += Environment.NewLine;

            if (txtStatus.InvokeRequired)
            {
                txtStatus.Invoke(new Action(() => txtStatus.AppendText(message)));
            }
            else
            {
                txtStatus.AppendText(message);
            }
        }

        private void LogEnumerable<T>(IEnumerable<T> enumerable)
        {
            Log(string.Join(", ", enumerable.Select(
                item => item is Uri uri ? uri.AbsoluteUri : item?.ToString())));
        }

        private void LogSearchPath()
        {
            Log("Search Path: " + _searchPath);
        }

        private void ToggleControls()
        {
            grpConfiguration.Enabled = !btnInstall.Enabled;
            btnInstall.Enabled = !btnInstall.Enabled;
            btnSetSearchPath.Enabled = !btnSetSearchPath.Enabled;
        }

        private bool IsUsernameValid()
        {
            if (txtUsername.Text != _defaultText[txtUsername.TabIndex])
            {
                txtUsername.Text = txtUsername.Text.ToLower();
            }

            bool hasRequiredLength = txtUsername.TextLength >= 1 && txtUsername.TextLength <= 32;
            bool hasInvalidCharacters = !txtUsername.Text.All(
                c => (c == '_') || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z'));

            if (hasRequiredLength && !hasInvalidCharacters)
            {
                return true;
            }

            Log("Invalid Username");
            return false;
        }

        private bool IsPasswordValid()
        {
            bool hasRequiredLength = txtPassword.TextLength >= 8;
            bool hasInvalidCharacters = txtPassword.Text.Any(c => c == '\'' || c == '"' || c == '\\');
            bool hasChanged = txtPassword.Text != _defaultText[txtPassword.TabIndex];
            bool isIdentical = txtPassword.Text == txtConfirmPassword.Text;
            bool isWithinRange = txtPassword.Text.All(c => c >= 33 && c <= 126);
            bool isValid = hasRequiredLength && !hasInvalidCharacters && hasChanged && isIdentical && isWithinRange;

            if (!isValid)
            {
                Log("Invalid Password");
            }

            return isValid;
        }

        private bool IsDatabaseNameValid()
        {
            if (txtDatabaseName.Text != _defaultText[txtDatabaseName.TabIndex])
            {
                txtDatabaseName.Text = txtDatabaseName.Text.ToLower();
            }

            bool hasRequiredLength = txtDatabaseName.TextLength >= 1 && txtDatabaseName.TextLength <= 64;
            bool hasInvalidCharacters = !txtDatabaseName.Text.All(
                c => (c == '_') || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z'));

            if (hasRequiredLength && !hasInvalidCharacters)
            {
                return true;
            }

            Log("Invalid Database Name");
            return false;
        }

        // root directory installation is allowed
        private bool IsInstallationPathValid()
        {
            bool isValid = txtInstallationPath.Text != _defaultText[txtInstallationPath.TabIndex] &&
                (!txtInstallationPath.Text.Contains(':') || txtInstallationPath.Text.Contains(":/") || txtInstallationPath.Text.Contains(":\\"));

            try
            {
                if (isValid)
                {
                    DirectoryInfo info = Directory.CreateDirectory(txtInstallationPath.Text);
                    txtInstallationPath.Text = info.FullName;
                }
            }
            catch
            {
                isValid = false;
            }

            if (!isValid)
            {
                Log("Invalid Installation Path");
            }

            return isValid;
        }

        private bool IsTextInputValid()
        {
            bool isValid = true;

            if (!IsUsernameValid())
            {
                isValid = false;
            }

            if (!IsPasswordValid())
            {
                isValid = false;
            }

            if (!IsDatabaseNameValid())
            {
                isValid = false;
            }

            if (!IsInstallationPathValid())
            {
                isValid = false;
            }

            return isValid;
        }

        private void frmSetup_Load(object sender, EventArgs e)
        {
            foreach (Control parentControl in Controls)
            {
                parentControl.Text = _defaultText[parentControl.TabIndex];

                foreach (Control childControl in parentControl.Controls)
                {
                    childControl.Text = _defaultText[childControl.TabIndex];
                }
            }

            lblTitle.Select();
            searchPathDialog.ShowNewFolderButton = false;
            UpdateInstallableComponents();

            if (Environment.Is64BitOperatingSystem)
            {
                LogSearchPath();
            }
            else
            {
                Log("This tool requires a 64-bit OS.");
                ToggleControls();
            }
        }

        private void txt_Enter(object sender, EventArgs e)
        {
            TextBox box = (TextBox) sender;

            if (box.Text == _defaultText[box.TabIndex])
            {
                box.Text = string.Empty;
            }
        }

        private void txt_EnterSensitive(object sender, EventArgs e)
        {
            txt_Enter(sender, e);
            ((TextBox) sender).UseSystemPasswordChar = true;
        }

        private void txt_Leave(object sender, EventArgs e)
        {
            TextBox box = (TextBox) sender;

            box.Text = box.Text.Trim();

            if (box.Text.Length == 0)
            {
                box.Text = _defaultText[box.TabIndex];
                box.UseSystemPasswordChar = false;
            }
        }

        private async void btnInstall_Click(object sender, EventArgs e)
        {
            bool isInputValid = true;
            string error;

            _currentComponentIndex = 0;
            ToggleControls();
            txtStatus.Text = string.Empty;
            Log("Caching Hashes...");
            await UpdateHashCacheAsync();
            Log(Environment.NewLine + "Checking Hashes...");

            foreach (InstallableComponent ic in _installableComponents)
            {
                if (!ic.CanInstall(new HashSet<string>(_hashCache.Keys)))
                {
                    Log($"{Environment.NewLine}Missing {ic.Name}:");
                    LogEnumerable(ic.Uris);
                    LogEnumerable(ic.Hashes.Where(hash => LocateFile(hash) == null));
                    isInputValid = false;
                }
            }

            Log(Environment.NewLine + "Checking Text Input...");

            if (!IsTextInputValid())
            {
                isInputValid = false;
            }

            if (!isInputValid)
            {
                ToggleControls();
                return;
            }
            
            foreach (InstallableComponent ic in _installableComponents)
            {
                Log($"Installing {ic.Name}...");
                error = await Task.Run(() => ic.Install());

                if (error != null)
                {
                    Log(error);
                    ToggleControls();
                    return;
                }

                _currentComponentIndex++;
            }

            Log("Installation Complete");
            ToggleControls();
        }

        private void btnSetSearchPath_Click(object sender, EventArgs e)
        {
            if (searchPathDialog.ShowDialog() == DialogResult.OK)
            {
                _searchPath = searchPathDialog.SelectedPath;
                LogSearchPath();
            }
        }

        private void txtStatus_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.LinkText,
                UseShellExecute = true
            });
        }
    }
}
