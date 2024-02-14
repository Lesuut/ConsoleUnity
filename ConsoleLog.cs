using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class ConsoleLog : MonoBehaviour
{
    [System.Serializable]
    public struct Command
    {
        public Command(string name, UnityAction action, string nameInHelpList = "")
        {
            this.name = name;
            this.action = action;
            this.nameInHelpList = nameInHelpList == "" ? name : nameInHelpList;
        }
        public string name;
        public string nameInHelpList;
        public UnityAction action;
    }
    public struct DebugLogsGraph
    {
        public string info;
        public int scale;
        public string time;

        public DebugLogsGraph(string info, int scale, string time)
        {
            this.info = info;
            this.scale = scale;
            this.time = time;
        }
    }

    public static ConsoleLog instance; 
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private static string consoleInformationHistory;

    [SerializeField] private GameObject[] objsConsole;
    [SerializeField] private Transform scrollContent;
    [Space]
    [SerializeField] private TextMeshProUGUI helperText;
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private TextMeshProUGUI mainTextDisplay;
    [SerializeField] private TextMeshProUGUI graphText;
    [Header("System Component")]
    // Write the dependencies you need here
    [Header("Commands")]
    [SerializeField] private List<Command> commands = new List<Command>();

    private string[,] availableCommands;
    private int maxWordsInCommand = 5;
    private string graphTextStr = "";
    private List<DebugLogsGraph> graphDebugLogs = new List<DebugLogsGraph>();

    private void CommandHelp()
    {
        string strAllCommand = "[green:All command:]\n";
        for (int i = 0; i < commands.Count; i++)
        {
            strAllCommand += $"  '{commands[i].nameInHelpList}'";
            if (i != commands.Count - 1)
            {
                strAllCommand += "\n";
            }
        }
        WriteLog(strAllCommand);
    }
    private void CommandClear()
    {
        consoleInformationHistory = "";
        WriteLogWithoutFormatting("");
    }
    private void CommandDef()
    {
        scrollContent.transform.localPosition = new Vector3(0, 0, 0);
    }
    public void CommandAllColors()
    {
        string allColors = "\n[red:red]\n" +
                       "[carrot:carrot]\n" +
                       "[green:green]\n" +
                       "[greenWarm:greenWarm]\n" +
                       "[greenGrass:greenGrass]\n" +
                       "[blue:blue]\n" +
                       "[blueSea:blueSea]\n" +
                       "[blueSky:blueSky]\n" +
                       "[cyan:cyan]\n" +
                       "[tgs:tgs]\n" +
                       "[yellow:yellow]\n" +
                       "[orange:orange]\n" +
                       "[orangeLight:orangeLight]\n" +
                       "[purple:purple]\n" +
                       "[pink:pink]\n" +
                       "[white:white]\n\n"
                       +
                       "[red:red] " +
                       "[carrot:carrot] " +
                       "[green:green] " +
                       "[greenWarm:greenWarm] " +
                       "[greenGrass:greenGrass] " +
                       "[blue:blue] " +
                       "[blueSea:blueSea] " +
                       "[blueSky:blueSky] " +
                       "[cyan:cyan] " +
                       "[tgs:tgs] " +
                       "[yellow:yellow] " +
                       "[orange:orange] " +
                       "[orangeLight:orangeLight] " +
                       "[purple:purple] " +
                       "[pink:pink] " +
                       "[white:white] ";

        WriteLog(FormatColoredText(allColors));
    }

    private void Start()
    {
        commands.Add(new Command("help", CommandHelp));
        commands.Add(new Command("clear", CommandClear));
        commands.Add(new Command("def", CommandDef));
        commands.Add(new Command("debug color", CommandAllColors));

        //commands.Add(new Command("log", CommandWriteGraphDebugLogs));

        InitConsoleTextSize();
        Init();
    }
    private void Init()
    {
        int commandCount = commands.Count;

        availableCommands = new string[commandCount, maxWordsInCommand];

        for (int i = 0; i < commandCount; i++)
        {
            string[] words = commands[i].name.Split(' ');
            for (int j = 0; j < maxWordsInCommand; j++)
            {
                if (j < words.Length && words[j].Length > 0)
                {
                    availableCommands[i, j] = words[j][0].ToString().ToLower();
                }
                else
                {
                    availableCommands[i, j] = "&";
                }
            }
        }
    }
    private string lastInput = "";
    private void Update()
    {
        if (inputText.text != lastInput)
        {
            if (inputText.text != "")
            {
                helperText.text = commands[ProcessInput()].name;
                lastInput = inputText.text;
            }
        }

        if (Input.GetKey(KeyCode.C))
        {
            ActiveConsoleWindowButtonUseButton();
        }
        if (Input.GetKey(KeyCode.Tab))
        {
            EnterButton();
        }
        if (Input.touchCount > 3)
        {
            ActiveConsoleWindowButtonUseButton();
        }
    }
    public void EnterButton()
    {
        string inputCommand = helperText.text.ToLower();

        if (string.IsNullOrEmpty(inputCommand))
            return;

        bool commandFound = false;

        foreach (Command command in commands)
        {
            if (command.name.Equals(inputCommand))
            {
                command.action.Invoke();
                commandFound = true;
                break;
            }
        }

        if (!commandFound)
        {
            WriteLog("[red:Incorrect command!] Use [green: help] to view a list of available commands.");
        }

        inputText.text = "";
        helperText.text = "";
    }
    /*public void AutoHelpWriteButton()
    {
        string inputCommand = helperText.text.ToLower();

        if (string.IsNullOrEmpty(inputCommand))
            return;

        foreach (Command command in commands)
        {
            if (command.name.Equals(inputCommand))
            {
                command.action.Invoke();
                break;
            }
        }

        helperText.text = "";
        inputText.text = "";
    }*/
    public void WriteLog(string str)
    {
        consoleInformationHistory += $"\n[{System.DateTime.Now.ToString("HH:mm")}] {FormatColoredText(str)}";
        mainTextDisplay.text = consoleInformationHistory;
    }
    private void WriteLogWithoutFormatting(string str)
    {
        consoleInformationHistory += "\n" + str;
        mainTextDisplay.text = consoleInformationHistory;
    }
    public void WriteLogError(string str)
    {
        Debug.LogError(str + "!");
        consoleInformationHistory += "\n" + FormatColoredText("[red:" + str + "!]");
        mainTextDisplay.text = consoleInformationHistory;
    }
    public void WriteLogWarning(string str)
    {
        //Debug.LogWarning(str + "!");
        consoleInformationHistory += "\n" + FormatColoredText("[yellow:" + str + "!]");
        mainTextDisplay.text = consoleInformationHistory;
    }
    public string FormatColoredText(string input)
    {
        string formattedText = input;

        formattedText = formattedText.Replace("[red:", "<color=red>");
        formattedText = formattedText.Replace("[carrot:", "<color=#F13704>");

        formattedText = formattedText.Replace("[green:", "<color=green>");
        formattedText = formattedText.Replace("[greenWarm:", "<color=#9FB112>");
        formattedText = formattedText.Replace("[greenGrass:", "<color=#529314>");

        formattedText = formattedText.Replace("[blue:", "<color=blue>");
        formattedText = formattedText.Replace("[blueSea:", "<color=#115FCB>");
        formattedText = formattedText.Replace("[blueSky:", "<color=#068FBD>");
        formattedText = formattedText.Replace("[cyan:", "<color=#00ECFF>");
        formattedText = formattedText.Replace("[tgs:", "<color=#84E7FE>");

        formattedText = formattedText.Replace("[yellow:", "<color=yellow>");

        formattedText = formattedText.Replace("[orange:", "<color=orange>");
        formattedText = formattedText.Replace("[orangeLight:", "<color=#FAA103>");

        formattedText = formattedText.Replace("[purple:", "<color=purple>");

        formattedText = formattedText.Replace("[pink:", "<color=#FF00D6>");

        formattedText = formattedText.Replace("[white:", "<color=#FFFFFF>");


        formattedText = formattedText.Replace("]", "</color>");
        return formattedText;
    }
    private int ProcessInput()
    {
        string userInput = inputText.text.ToLower();

        int bestCommandId = 0;
        int bestMatchLength = 0;

        for (int i = 0; i < commands.Count; i++)
        {
            string commandName = commands[i].name.ToLower();

            int matchLength = GetMatchingPrefixLength(userInput, commandName);

            if (matchLength > bestMatchLength)
            {
                bestMatchLength = matchLength;
                bestCommandId = i;
            }
        }

        return bestCommandId;
    }
    private int GetMatchingPrefixLength(string s, string t)
    {
        int minLength = Mathf.Min(s.Length, t.Length);
        int matchLength = 0;

        for (int i = 0; i < minLength; i++)
        {
            if (s[i] == t[i])
            {
                matchLength++;
            }
            else
            {
                break;
            }
        }

        return matchLength;
    }
    public void ActiveConsoleWindowButton()
    {
        foreach (var item in objsConsole)
            item.SetActive(!item.activeSelf);
    }
    public void ActiveConsoleWindowButtonUseButton()
    {
        foreach (var item in objsConsole)
        {
            if (!item.activeSelf)
                item.SetActive(true);
        }
    }
    public void WriteLogGraph(int scale)
    {
        graphTextStr += GetColorGraphLine(scale);
        graphTextStr = TrimString(graphTextStr);
        graphText.text = graphTextStr;
    }
    public void WriteLogGraph(int scale, string info)
    {
        graphTextStr += GetColorGraphLine(scale);
        graphTextStr = TrimString(graphTextStr);
        graphText.text = graphTextStr;

        graphDebugLogs.Add(new DebugLogsGraph(info, scale, System.DateTime.Now.ToString("mm:ss:fff")));
    }
    public void WriteLogGraph(string info)
    {
        graphDebugLogs.Add(new DebugLogsGraph(info, -1, System.DateTime.Now.ToString("mm:ss:fff")));
    }
    private string GetColorGraphLine(int scale)
    {
        switch (scale)
        {
            case 9:
                return FormatColoredText($"[red:{scale}]");
            case 8:
                return FormatColoredText($"[carrot:{scale}]");
            case 7:
                return FormatColoredText($"[orange:{scale}]");
            case 6:
                return FormatColoredText($"[yellow:{scale}]");
            case 5:
                return FormatColoredText($"[greenWarm:{scale}]");
            case 4:
                return FormatColoredText($"[greenGrass:{scale}]");
            case 3:
                return FormatColoredText($"[blueSky:{scale}]");
            case 2:
                return FormatColoredText($"[blueSea:{scale}]");
            case 1:
                return FormatColoredText($"[blue:{scale}]");
            case 0:
                return FormatColoredText($"[purple:{1}]");
            default:
                return FormatColoredText($"[pink:{scale}]");
        }
    }
    private string TrimString(string input)
    {
        if (input.Length > 3500)
        {
            return input.Substring(input.Length - 3500);
        }
        return input;
    }
    private string[] ExtractValues(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new string[0]; 
        }

        string[] result = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        result = result.Concat(new string[] { "", "", "" }).ToArray();

        return result;
    }
    private bool IsNotNull(object obj)
    {
        if (obj != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool IsNotNull(object[] objs)
    {
        foreach (var item in objs)
        {
            if (item == null)
            {
                return false;
            }
        }
        return true;
    }
    private void InitConsoleTextSize(int newTextSize = 0)
    {
        string saveKey = "ConsoleTextSize";
        int defSize = 36;

        if (PlayerPrefs.GetInt(saveKey) == 0)
        {
            PlayerPrefs.SetInt(saveKey, defSize);
        }

        if (newTextSize != 0)
        {
            PlayerPrefs.SetInt(saveKey, newTextSize);
        }

        mainTextDisplay.fontSize = PlayerPrefs.GetInt(saveKey);       
    }
}
