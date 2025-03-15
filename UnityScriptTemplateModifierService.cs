using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace UnityScriptTemplateModifierService
{
    public partial class UnityScriptTemplateModifierService : ServiceBase
    {
        /// <summary>
        /// Path to Unity Hub installations
        /// </summary>
        private readonly string unityHubPath = @"D:\Program Files\Unity";
        /// <summary>
        /// Path to service files
        /// </summary>
        private readonly string serviceFilesPath = @"E:\Program Files\UnityScriptTemplateModifierService";

        /// <summary>
        /// File system watcher to detect new Unity installations
        /// </summary>
        private FileSystemWatcher _watcher;

        public UnityScriptTemplateModifierService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _watcher = new FileSystemWatcher
            {
                Path = unityHubPath,
                NotifyFilter = NotifyFilters.DirectoryName,
                Filter = "*",
                EnableRaisingEvents = true
            };

            _watcher.Created += OnNewUnityInstall;
            WriteLog("Unity Script Template Modifier Service started.");
        }

        protected override void OnStop()
        {
            _watcher?.Dispose();
            WriteLog("Unity Script Template Modifier Service stopped.");
        }

        /// <summary>
        /// Event handler for new Unity installations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnNewUnityInstall(object sender, FileSystemEventArgs e)
        {
            string newUnityPath = e.FullPath;
            string scriptTemplatesPath = Path.Combine(newUnityPath, "Editor", "Data", "Resources", "ScriptTemplates");

            bool success;
            int remainingAttempts = 5;
            do
            {
                remainingAttempts--;
                success = Directory.Exists(scriptTemplatesPath);
                if (success)
                {
                    WriteLog($"New Unity version detected: {newUnityPath}");
                    await Task.Delay(5000); // Wait to ensure the files are available

                    var scriptTemplateFile = "81-C# Script-NewBehaviourScript.cs.txt";

                    if (int.TryParse(Path.GetFileName(newUnityPath).Split('.').First(), out int unityVersion))
                    {
                        scriptTemplateFile = unityVersion <= 2023 ? "81-C# Script-NewBehaviourScript.cs.txt" :
                            "1-Scripting__MonoBehaviour Script-NewMonoBehaviourScript.cs.txt";
                    }

                    string scriptFile = Path.Combine(scriptTemplatesPath, scriptTemplateFile);

                    success = File.Exists(scriptFile);
                    if (success)
                    {
                        ModifyMonoBehaviourScriptTemplate(scriptFile);
                    }
                    else
                    {
                        WriteLog($"Error: File path [{scriptFile}] not found. Remaining Attempts: {remainingAttempts}. Trying again in 10 minutes...");
                    }
                }
                else
                {
                    WriteLog($"Error: Templates path: [{scriptTemplatesPath}] not found. Remaining Attempts: {remainingAttempts}. Trying again in 10 minutes...");
                }

                if (!success)
                {
                    await Task.Delay(600000);
                }

            } while (!success && remainingAttempts > 0);
        }

        /// <summary>
        /// Modifies the MonoBehaviour script template
        /// </summary>
        /// <param name="filePath"></param>
        private void ModifyMonoBehaviourScriptTemplate(string filePath)
        {
            File.WriteAllText(filePath, GetBaseMonoBehaviourTemplateContent("BaseMonoBehaviour.txt"));
            WriteLog($"Modified script template: {filePath}");
        }

        /// <summary>
        /// Gets the content of the template file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetBaseMonoBehaviourTemplateContent(string filePath)
        {
            string fullPath = Path.Combine(serviceFilesPath, "Templates", filePath);
            if (File.Exists(fullPath))
            {
                return File.ReadAllText(fullPath);
            }
            else
            {
                WriteLog($"Error: File path [{fullPath}] not found.");
                // Default template content
                return "using UnityEngine;\r\n\r\n    #ROOTNAMESPACEBEGIN#\r\npublic class #SCRIPTNAME# : MonoBehaviour\r\n{\r\n    private void Start()\r\n    {\r\n        #NOTRIM#\r\n    }\r\n}\r\n#ROOTNAMESPACEEND#\r\n";
            }
        }

        /// <summary>
        /// Writes a message to the log file
        /// </summary>
        /// <param name="message"></param>
        private void WriteLog(string message)
        {
            string logFile = Path.Combine(serviceFilesPath, "service.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logFile));
            File.AppendAllText(logFile, $"{DateTime.Now}: {message}\n");
        }
    }
}
