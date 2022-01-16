using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using StarterAssets;


public class DebugConsole : Singleton<DebugConsole>
{
    const string bindingsGroup = "devConsole";
    public Canvas dbgCanvas;
    public GameObject parentObj;
    public GameObject animationParent;
    public Text txt;
    public Text inputTxt;
    public Image bg;
    int maxChars = 2000;
    bool shown = true;

    bool animating = false;
    float animationTime = 0;
    Vector3 start;
    Vector3 end;

    Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
    List<string> commandNames = new List<string>();

    int LogLevel = 0;

    public void RegisterCommand(string commandName, ConsoleCommand newCommand)
    {
        commandName = commandName.Trim().ToLower();

        if (commandName.Contains(" "))
        {
            Log("[ERROR] Tried to register command containing whitespace.");
        }
        else if (!commands.ContainsKey(commandName))
        {
            commands.Add(commandName, newCommand);
            commandNames.Add(commandName);
            commandNames.Sort();
        }
        else
        {
            Log("[ERROR] Tried to register command \"" + commandName + "\" but it is already registered.");
        }
    }

    void Toggle()
    {
        Vector3 up = new Vector3(0, Screen.height, 0);
        Vector3 down = new Vector3(0, Screen.height / 2, 0);

        shown = !shown;

        if (shown)
        {
            InputBinding.MaskByGroup(bindingsGroup);
        }
        else
        {
            InputBinding.MaskByGroup("default");
        }

        start = shown ? up : down;
        end = shown ? down : up;

        if (!animating)
        {
            animating = true;
            StartCoroutine(Animate());
        }
        else
        {
            animationTime = 1 - animationTime;
            start = shown ? down : up;
            end = shown ? up : down;
        }
    }

    void Awake()
    {
    }

    void Start()
    {
        RegisterBasicCommands();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (!parentObj.active)
            {
                parentObj.SetActive(true);
            }
            else
            {
                Toggle();
            }
        }
        if (shown)
        {
            if (Input.inputString.Contains("\n") || Input.inputString.Contains("\r"))
            {
                ProcessInput();
            }
            else if (Input.inputString.Contains("\b"))
            {
                inputTxt.text = inputTxt.text.Substring(0, Mathf.Max(inputTxt.text.Length - 1, 1));
            }
            else if (Input.inputString != "")
            {
                inputTxt.text += Input.inputString;
            }
        }
    }

    public void Log(System.Object obj)
    {
        if (txt.text.Length > maxChars)
        {
            txt.text = txt.text.Substring(1500);
        }

        txt.text += "\n" + obj.ToString();
    }

    IEnumerator Animate()
    {
        animationTime = 0;
        while (animationTime < 1)
        {
            animationTime += Time.deltaTime * 4;
            if (animationTime > 1) animationTime = 1;
            animationParent.transform.localPosition = Vector3.LerpUnclamped(start, end, animationTime * animationTime * (3f - 2f * animationTime)); //smooth step

            yield return null;
        }
        animating = false;
    }

    void ProcessInput()
    {
        string[] inputArr = inputTxt.text.Substring(1).Split(' ');
        List<string> parameters = new List<string>();
        string command = inputArr[0].ToLower();
        Log("]" + inputTxt.text.Substring(1));

        for (int i = 1; i < inputArr.Length; i++)
        {
            if (inputArr[i] == "") continue;
            else if (inputArr[i].StartsWith("\""))
            {
                string quoteParam = inputArr[i];
                while (!inputArr[i].EndsWith("\"") && i < inputArr.Length)
                {
                    quoteParam += " " + inputArr[i];
                    i++;
                }
                parameters.Add(quoteParam);
            }
            else
            {
                parameters.Add(inputArr[i]);
            }

        }

        if (commands.ContainsKey(command.ToLower()))
        {
            if (parameters.Count > 0 && parameters[0] == "?")
            {
                Log(commands[command].HelpMessage);
            }
            else
            {
                Log(commands[command].Execute(parameters.ToArray()));
            }
        }
        else
        {
            Log("Unrecognized command \"" + command + "\"");
        }

        inputTxt.text = ">";
    }

    void RegisterBasicCommands()
    {
        RegisterCommand("jumphigher", new ConsoleCommand(
                (x) =>
                {
                    GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
                    float idx = 0;
                    if (x.Length > 0)
                    {
                        float.TryParse(x[0], out idx);
                    }
                    else
                    {
                        return "type jumphigher <number> to set height.";
                    }
                    foreach (GameObject obj in objs)
                    {
                        var controller = obj.GetComponent<ThirdPersonController>();
                        if (controller.IsOwner)
                        {
                            controller.JumpHeight = idx;
                            
                        }
                    }
                    return "Your jump height has updated.";
                }, "jumphigher <number> - set your jump height."));
                    RegisterCommand("help", new ConsoleCommand(
                (x) =>
                {
                    int idx = 0;
                    if (x.Length > 0)
                    {
                        int.TryParse(x[0], out idx);
                        idx -= 1;
                    }

                    int commandCount = commands.Count;

                    if (idx > commandCount)
                    {
                        idx = commandCount - 10;
                    }
                    else if (idx < 0)
                    {
                        idx = 0;
                    }

                    string helpString = "Showing commands " + (idx + 1) + " - " + Mathf.Min(idx + 10, commandCount) + " of " + commandCount + ":";

                    for (int i = idx; i < idx + 10 && i < commandCount; i++)
                    {
                        helpString += "\n  " + (i + 1) + " - " + commandNames[i];
                    }

                    helpString += "\nType \"?\" after a command for more details.";

                    return helpString;
                },
                "help <number> - Displays a list of commands."
        ));
       // RegisterCommand("Logging", new ConsoleCommand(
       //     (x) =>
       //     {
       //         int logLevel = -1;

       //         if (x.Length > 0)
       //         {
       //             if (int.TryParse(x[0], out logLevel))
       //             {
       //                 LogLevel = Mathf.Clamp(logLevel, 0, 3);

       //                 if (LogLevel == 0)
       //                 {
       //                     Application.logMessageReceived -= Application_logMessageReceived;
       //                 }
       //                 else
       //                 {
       //                     Application.logMessageReceived += Application_logMessageReceived;
       //                 }

       //                 return "Logging level set to " + LogLevel;
       //             }
       //         }

       //         return "Logging level is " + LogLevel;
       //     },
       //     "Sets logging level for the console.\n 0 - No logging.\n1 - Errors only.\n2 - Errors and Warnings.\n3 - Errors, Warnings and Debugs."
       //));
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Log && LogLevel < 3) return;
        if (type == LogType.Warning && LogLevel < 2) return;

        Log(type + "\n" + condition + "\n" + stackTrace);
    }
}
