using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : SingletonBehaviour<DialogueManager>
{
    [SerializeField] CanvasGroup _dialoguePanel;
    [SerializeField] TextMeshProUGUI _dialogueText;

    [SerializeField] TextMeshProUGUI _speakerText;

    public bool IsDialogueRunning { get; private set; }
    private readonly Queue<DialogueLine> _dialogueQueue = new();
    private DialogueLine CurrentLine => _dialogueQueue.Peek();

    bool _canContinue = false;

    private void Start()
    {
        _dialoguePanel.gameObject.SetActive(false);
        WorldManager.Instance.OnWorldChange += (WorldBrain brain) =>
        PlayerInputs.Instance.UI.Next.performed += OnNext;
    }

    public void RunDialogue(DialogueData data)
    {
        IsDialogueRunning = true;
        GameManager.Instance.IsFrozen = true;
        _speakerText.gameObject.SetActive(data.ShowSpeakerName);

        // TODO tell camera to focus on source
        _dialoguePanel.gameObject.SetActive(true);
        _dialogueText.text = "";
        _speakerText.text = data.SpeakerName;
        _dialogueQueue.Clear();
        foreach (var line in data.Lines)
            _dialogueQueue.Enqueue(line);

        NextLine();
    }

    void NextLine()
    {
        if (_dialogueQueue.TryPeek(out _))
        {
            _dialogueText.text = "";
            StartCoroutine(PlayDialogue(_dialogueQueue.Dequeue()));
        }
        else
        {
            EndConversation();
        }
    }

    IEnumerator PlayDialogue(DialogueLine line)
    {
        while (_dialogueText.text != line.Line)
        {
            _canContinue = false;
            foreach (var letter in line.Line.ToCharArray())
            {
                _dialogueText.text += letter;
                yield return new WaitForSeconds(line.CharacterSpeed / 60);
            }
        }
        _canContinue = true;
    }

    void OnNext(InputAction.CallbackContext ctx)
    {
        if (!_canContinue) return;
        NextLine();
        _canContinue = false;
    }

    void EndConversation()
    {
        _dialogueText.text = "";
        // TODO remove focus
        IsDialogueRunning = false;
        GameManager.Instance.IsFrozen = false;
        _dialoguePanel.gameObject.SetActive(false);
    }

}
