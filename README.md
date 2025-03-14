# 🛠 Unity Script Template Modifier Service

**Unity Script Template Modifier Service** is a Windows Service that **automatically detects new Unity Editor installations** and **modifies the default C# script template** inside the `ScriptTemplates` folder. This ensures that every new Unity version includes a customized script template without manual intervention.

## 🚀 Features
- 🔍 **Automatic Unity Version Detection** – Monitors Unity Hub for new installations.
- ✍ **Custom Script Templates** – Automatically updates the default `NewBehaviourScript.cs.txt`.
- 📂 **Log File Support** – Keeps a log of all modifications for tracking.
- ⚡ **Runs in the Background** – Works as a Windows Service without user interaction.

## 📌 How It Works  
1. **Detects new Unity versions** in `path\to\UnityEditor\Unity\Hub\Editor\`.
2. **Waits for installation completion** before making modifications.
3. **Locates the `ScriptTemplates` folder** inside the new Unity installation.
4. **Updates the default C# script template**, adding a custom header and modifications.

## 🛠 Installation & Usage
1. Build the project in **Release** mode.
2. Open **Command Prompt as Administrator**.
3. Install the service and set to Auto-Start with Windows:
```sh
    sc create UnityScriptTemplateModifierService binPath= "path\to\Unity-Script-Template-Modifier-Service.exe" start auto
```
4. Start the service:
```sh
    net start UnityScriptTemplateModifierService
```
5. Check logs at C:\UnityScriptTemplateModifierService\service.log to verify modifications.

## 🔄 Uninstalling
```sh
    net stop UnityScriptTemplateModifierService
    sc delete UnityScriptTemplateModifierService
```

## 📜 License
This project is open-source and available under the **MIT License**.