using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    GameplayActions _actions;
    [SerializeField] Animator _anim;

    private void Awake()
    {
        _actions = new();
        _actions.Enable();

        _actions.UI.Any.performed += StartGame;
    }

    private void OnDisable()
    {
        _actions.Disable();
        _actions.UI.Any.performed -= StartGame;
    }

    private void StartGame(InputAction.CallbackContext context)
    {
        _anim.SetTrigger("Fade");
        SceneManager.LoadScene("SCN_Core");
    }

    public void Quit()
    {
        print("Quitting... If you see this... HELLO");
        Application.Quit();
    }
}
