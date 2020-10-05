using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private IDisposable _startButtonStream;
    private IDisposable _quitButtonStream;
    
    public void StartGame()
    {
        SceneManager.LoadScene(1);
        
        DisposeButtonsStreams();
    }

    private void DisposeButtonsStreams()
    {
        _startButtonStream.Dispose();
        _quitButtonStream.Dispose();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Awake()
    {
        _startButtonStream = Observable.EveryUpdate()
            .Where(_ => Input.GetButton("A Button"))
            .Subscribe(_ => StartGame());

        _quitButtonStream = Observable.EveryUpdate()
            .Where(_ => Input.GetButton("B Button"))
            .Subscribe(_ => Quit());
    }
}