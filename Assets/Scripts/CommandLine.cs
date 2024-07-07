using System.Collections.Generic;
using UnityEngine;
// ReSharper disable InconsistentNaming
// this script is based on the following tutorial: https://www.youtube.com/watch?v=VzOEM-4A2OM

public class CommandLine : MonoBehaviour
{
    private bool _showCommandLine;
    private bool _showHelp;
    
    private Vector2 scroll;
    
    private string _input;
    private List<object> _commandList;

    private static Command<int> INCREASE_REQUEST_INTERVAL;
    private static Command<int> DECREASE_REQUEST_INTERVAL;
    private static Command HELP;
    
    private void Awake()
    {
        var gameManager = GameManager.Singleton;

        INCREASE_REQUEST_INTERVAL = new Command<int>("increase_request_interval",
            "Increases the interval the client request data from the Adlete Service.",
            "increase_request_interval <value>", value =>
            {
                gameManager.IncreaseRequestInterval(value);
            });
        DECREASE_REQUEST_INTERVAL = new Command<int>("decrease_request_interval",
            "Decreases the interval the client request data from the Adlete Service.",
            "decrease_request_interval <value>", (value) =>
            {
                gameManager.DecreaseRequestInterval(value);
            });
        
        HELP = new Command("help", "Shows all available commands.", "help", () =>
        {
            _showHelp = true;
        });
        
        _commandList = new List<object>
        {
            INCREASE_REQUEST_INTERVAL,
            DECREASE_REQUEST_INTERVAL,
            HELP
        };
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _showCommandLine = !_showCommandLine;
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (_showCommandLine)
            {
                HandleInput();
                _input = "";
            }
        }
    }
    
    private void OnGUI()
    {
        if (!_showCommandLine)
        {
            return;
        }
        
        var y = 0;
        
        if (_showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");
            var viewPort = new Rect(0, 0, Screen.width -30, 20 * _commandList.Count);
            
            scroll = GUI.BeginScrollView(new Rect(0, y + 5, Screen.width, 90), scroll, viewPort);

            var i = 0;
            foreach (var command in _commandList)
            {
                if (command is CommandBase commandBase)
                {
                    var label = $"{commandBase.CommandFormat} - {commandBase.CommandDescription}";
                    var rectLabel = new Rect(5, 20 * i, viewPort.width - 100, 20);
                    GUI.Label(rectLabel, label);
                    i++;
                }
            }
            
            GUI.EndScrollView();
            
            y += 100;
        }
        
        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        _input = GUI.TextField(new Rect(10, y + 5, Screen.width - 20, 20), _input);
    }

    private void HandleInput()
    {
        var properties = _input.Split(' ');
        foreach (var cmd in _commandList)
        {
            if (cmd is CommandBase commandBase && _input.Contains(commandBase.CommandId))
            {
                switch (cmd)
                {
                    case Command command:
                        command.InvokeCommand();
                        break;
                    case Command<int> intCommand:
                        intCommand.InvokeCommand(int.Parse(properties[1]));
                        break;
                }
            }
        }
    }
}