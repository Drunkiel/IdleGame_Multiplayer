using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class Dialog
{
    public string text;
    public List<DialogEndOptions> endOptions = new();
}

[Serializable]
public class DialogEndOptions
{
    public string displayText;
    public UnityEvent actionToDo;
}
