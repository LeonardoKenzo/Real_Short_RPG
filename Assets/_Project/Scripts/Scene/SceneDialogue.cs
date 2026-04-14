using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SceneDialogue : MonoBehaviour
{   
    public string[] dialogue;
    public int dialogueIndex;

    public Text dialogueText;
    public GameObject dialoguePanel;

   
    public bool startDialogue;
    private bool end;
    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void StartDialogue()
    {
        startDialogue = true;
        dialogueIndex = 0;
        dialoguePanel.SetActive(true);
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        dialogueText.text = "";
        foreach(char letter in dialogue[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(.1f);
        }
    }

    void Update()
    {
        if (!startDialogue && !end)
        {
            StartDialogue();
        }
        else if(dialogueText.text == dialogue[dialogueIndex] && Input.GetButtonDown("Submit"))
        {
            NextDialogue();
        }
    }
    void NextDialogue()
    {   
        dialogueIndex++;
        if(dialogueIndex < dialogue.Length)
        {
            StartCoroutine(ShowDialogue());
        }
        else
        {
            EndDialogue();
        }
    }
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        startDialogue = false;
        end = true;
        dialogueIndex = 0;
    }
}
