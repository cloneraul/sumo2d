using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Controle de Rounds")]
    public int roundsVitóriaJogador1 = 0;
    public int roundsVitóriaJogador2 = 0;
    
    [HideInInspector] public int indexBolinhaJogador1;
    [HideInInspector] public int indexBolinhaJogador2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
            return;
        }
    }

    private void Start()
    {
        // Vai direto para a sua cena de gameplay atual, onde seus jogadores estão fixos
        CarregarCena("CenaGameplay");
    }

    public void CarregarCena(string nomeCena)
    {
        SceneManager.LoadScene(nomeCena);
    }

    public void RegistrarPonto(int jogadorQuePontuou)
    {
        if (jogadorQuePontuou == 1) roundsVitóriaJogador1++;
        else if (jogadorQuePontuou == 2) roundsVitóriaJogador2++;

        Debug.Log($"Placar: J1 ({roundsVitóriaJogador1}) vs J2 ({roundsVitóriaJogador2})");

        if (roundsVitóriaJogador1 >= 2 || roundsVitóriaJogador2 >= 2)
        {
            CarregarCena("CenaVitoria");
        }
        else
        {
            CarregarCena("CenaGameplay");
        }
    }

    public void ResetarPartida()
    {
        roundsVitóriaJogador1 = 0;
        roundsVitóriaJogador2 = 0;
    }
}