using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace UnityScriptTemplateModifierService
{
    public partial class UnityScriptTemplateModifierService : ServiceBase
    {
        private FileSystemWatcher _watcher;
        //Change to the Unity Editors path in your computer
        private readonly string unityHubPath = @"D:\Program Files\Unity";

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
            WriteLog("Unity Monitor Service started.");
        }

        protected override void OnStop()
        {
            _watcher?.Dispose();
            WriteLog("Unity Monitor Service stopped.");
        }

        private async void OnNewUnityInstall(object sender, FileSystemEventArgs e)
        {
            string newUnityPath = e.FullPath;
            string scriptTemplatesPath = Path.Combine(newUnityPath, "Editor", "Data", "Resources", "ScriptTemplates");

            if (Directory.Exists(scriptTemplatesPath))
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

                if (File.Exists(scriptFile))
                {
                    ModifyMonoBehaviourScriptTemplate(scriptFile);
                }
            }
        }

        private void ModifyMonoBehaviourScriptTemplate(string filePath)
        {
            string customContent = "using UnityEngine;\r\n\r\n    #ROOTNAMESPACEBEGIN#\r\npublic class #SCRIPTNAME# : MonoBehaviour\r\n{\r\n    private void Start()\r\n    {\r\n        #NOTRIM#\r\n    }\r\n}\r\n#ROOTNAMESPACEEND#\r\n";
            File.WriteAllText(filePath, customContent);
            WriteLog($"Modified script template: {filePath}");
        }

        private void WriteLog(string message)
        {
            string logFile = @"C:\UnityScriptTemplateModifierService\service.log";
            Directory.CreateDirectory(Path.GetDirectoryName(logFile));
            File.AppendAllText(logFile, $"{DateTime.Now}: {message}\n");
        }
    }
}
