using UnityEngine;

[CreateAssetMenu(fileName = "TeleprompterCellText", menuName = "ScriptableObjects/TeleprompterCellText", order = 1)]
public class TeleprompterCellScriptableObject : ScriptableObject
{
    [TextArea(10,10)]
    public string text;
}
