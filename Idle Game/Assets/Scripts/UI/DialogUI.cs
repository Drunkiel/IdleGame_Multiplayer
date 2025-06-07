using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public TMP_InputField answerField;
    //public EntityPreview _npcPreview;
    public Transform parent;
    public Button optionBTN;

    public bool finishedSpelling;

    public void UpdateDialog(Dialog _dialog)
    {
        //Reset current text and remove buttons
        dialogText.text = "";
        for (int i = 0; i < parent.childCount; i++)
            Destroy(parent.GetChild(i).gameObject);

        //Set new text and buttons
        for (int i = 0; i < _dialog.endOptions.Count; i++)
        {
            //Create new button and add listener
            Button newOptionButton = Instantiate(optionBTN, parent);
            int a = i;
            newOptionButton.onClick.AddListener(() => _dialog.endOptions[a].actionToDo.Invoke());
            
            //Set text to button
            newOptionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = _dialog.endOptions[i].displayText;
        }

        if (gameObject.activeSelf)
            StartCoroutine(nameof(TextWriting), _dialog.text);
    }

    public void SpeedUpDialog(string dialog)
    {
        dialogText.text = dialog;
        finishedSpelling = true;
    }

    IEnumerator TextWriting(string dialog)
    {
        finishedSpelling = false;

        foreach (char singleCharacter in dialog)
        {
            if (finishedSpelling)
                yield break;

            dialogText.text += singleCharacter;
            yield return new WaitForSeconds(0.02f);
        }

        finishedSpelling = true;
    }
}
