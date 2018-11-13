using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphGame.Logic;

namespace GraphGame.Client
{
public class Bootstrap
    : MonoBehaviour
{

    private Game Game;
    private void Awake()
    {
        this.Game = new Game();
        this.Game.OnGameOver = () => Debug.Log(string.Format("Game Over!\n{0}", this.Game.ToString()));
    }

    private void OnEnable()
    {
        this.Game.Start("xyz");
    }

    // Update is called once per frame
    void Update ()
    {
        this.Game.Update(Time.deltaTime);
	}
}
}
