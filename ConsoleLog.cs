using TMPro;
using UnityEngine;

public class ConsoleLog : MonoBehaviour
{
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

    private string[] availableCommandsData = {
        "",
        "help",
        "clear",
        "def",
        "" };
    private string[,] availableCommands;
    private int maxWordsInCommand = 5;

    private void Start()
    {
        int commandCount = availableCommandsData.Length;

        availableCommands = new string[commandCount, maxWordsInCommand];

        for (int i = 0; i < commandCount; i++)
        {
            string[] words = availableCommandsData[i].Split(' ');
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
                helperText.text = availableCommandsData[ProcessInput()];
                lastInput = inputText.text;
            }
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            EnterButton();
        }
        if (Input.GetKey(KeyCode.C))
        {
            ActiveConsoleWindowButtonUseButton();
        }
    }
    public void EnterButton()
    {
        switch (inputText.text)
        {
            case (""):
                break;

            case ("help"):
                CommandHelp();
                break;
            case ("clear"):
                CommandClear();
                break;
            case ("def"):
                CommandDef();
                break;

            default:
                WriteLog("{red:Incorrect command!}");
                break;
        }
        inputText.text = "";        
    }

    private void CommandHelp()
    {
        string strAllCommand = "{cyan:All command:}\n";
        for (int i = 1; i < availableCommandsData.Length - 1; i++)
        {
            strAllCommand += availableCommandsData[i] + "\n";
        }
        WriteLog(strAllCommand);
    }
    private void CommandClear()
    {
        consoleInformationHistory = "";
        WriteLog("");
    }
    private void CommandDef()
    {
        scrollContent.transform.localPosition = Vector3.zero;
    }

    public void WriteLog(string str)
    {
        consoleInformationHistory += FormatColoredText(str) + "\n";
        mainTextDisplay.text = consoleInformationHistory;
    }
    private string FormatColoredText(string input)
    {
        string formattedText = input;

        formattedText = formattedText.Replace("{red:", "<color=red>");
        formattedText = formattedText.Replace("{green:", "<color=green>");
        formattedText = formattedText.Replace("{blue:", "<color=blue>");
        formattedText = formattedText.Replace("{yellow:", "<color=yellow>");
        formattedText = formattedText.Replace("{orange:", "<color=orange>");
        formattedText = formattedText.Replace("{purple:", "<color=purple>");
        formattedText = formattedText.Replace("{pink:", "<color=#FF00D6>");
        formattedText = formattedText.Replace("{cyan:", "<color=#00ECFF>");

        formattedText = formattedText.Replace("}", "</color>");

        return formattedText;
    }
    private int ProcessInput()
    {
        string userInput = inputText.text.ToLower();
        string[] userInputwords = userInput.Split(' ');
        for (int i = 0; i < userInputwords.Length; i++)
        {
            userInputwords[i] = userInputwords[i][0].ToString().ToLower();
        }

        int[,] comandCoficient = new int[availableCommandsData.Length, 1];

        for (int i = 0; i < availableCommandsData.Length; i++)
        {
            for (int q = 0; q < maxWordsInCommand; q++)
            {
                if (q < userInputwords.Length && availableCommands[i, q] == userInputwords[q])
                {
                    comandCoficient[i, 0] += 1;
                }
            }
        }

        int bestIdComand = 0;
        int highestCoefficient = 0;

        for (int i = 0; i < availableCommandsData.Length; i++)
        {
            if (highestCoefficient < comandCoficient[i, 0])
            {
                highestCoefficient = comandCoficient[i, 0];
                bestIdComand = i;
            }
        }

        return bestIdComand;
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
}
