using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Instância estática para o padrão Singleton
    public static GameManager Instance { get; private set; }

    [Header("Controle de Rounds")]
    public int roundsVitóriaJogador1 = 0;
    public int roundsVitóriaJogador2 = 0;
    
    // Armazenará qual bolinha cada jogador escolheu (resolveremos no Passo 2 e 7)
    [HideInInspector] public int indexBolinhaJogador1;
    [HideInInspector] public int indexBolinhaJogador2;

    private void Awake()
    {
        // Implementação do Singleton: garante que só exista UM GameManager no jogo todo
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Não destrói ao mudar de cena
        }
        else
        {
            Destroy(gameObject); // Destrói duplicatas se voltar para a cena _Boot
            return;
        }
    }

    private void Start()
    {
        // Assim que o jogo abre na cena _Boot, ele pula para a tela de seleção
        // NOTA: Certifique-se de criar e adicionar as cenas no Build Settings depois!
        CarregarCena("CenaSelecao");
    }

    public void CarregarCena(string nomeCena)
    {
        SceneManager.LoadScene("CenaGameplay");
    }

    // Método que será chamado quando uma bolinha cair da arena
    public void RegistrarPonto(int jogadorQuePontuou)
    {
        if (jogadorQuePontuou == 1) roundsVitóriaJogador1++;
        else if (jogadorQuePontuou == 2) roundsVitóriaJogador2++;

        Debug.Log($"Placar: J1 ({roundsVitóriaJogador1}) vs J2 ({roundsVitóriaJogador2})");

        // Verifica a condição de vitória (Melhor de 3: quem fizer 2 pontos ganha)
        if (roundsVitóriaJogador1 >= 2 || roundsVitóriaJogador2 >= 2)
        {
            CarregarCena("CenaVitoria");
        }
        else
        {
            // Reinicia o round atual recarregando a cena de gameplay
            CarregarCena("CenaGameplay");
        }
    }

    public void ResetarPartida()
    {
        roundsVitóriaJogador1 = 0;
        roundsVitóriaJogador2 = 0;
    }
}
