using UnityEngine;
using TMPro;

public class HashcodeManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI codeText;       // drag the TMP text (digit display) here
    //public ProgramManager programManager;  // drag the ProgramManager object here

    void Start()
    {
        if (codeText != null)
            codeText.text = "";
    }

    public void AddNum(string num)
    {
        if (codeText != null)
            codeText.text += num;
    }

    public void ClearCode()
    {
        if (codeText != null)
            codeText.text = "";
    }

    //public void SubmitCode()
    //{
    //    if (programManager != null && codeText != null)
    //    {
    //        // pass code to ProgramManager
    //        programManager.presentationCode = codeText.text;
    //        programManager.codeText = codeText; // keep UI reference

    //        // start ProgramManager's coroutine
    //        programManager.StartCoroutine(programManager.LoadPresentation(codeText.text));
    //    }
    //}
}
