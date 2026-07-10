using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Controle de Rounds")]
    public int roundsVitóriaJogador1 = 0;
    public int roundsVitóriaJogador2 = 0;
    
    [HideInInspector] public int indexBolinhaJogador1;
    [HideInInspector] public int indexBolinhaJogador2;

    [Header("Referências de UI (Arraste aqui)")]
    public TextMeshProUGUI textoPlacar;
    public GameObject painelVitoria;
    public TextMeshProUGUI textoVencedor;

    private void Awake()
    {
        // Se já existir um GameManager (do DontDestroyOnLoad) e entrarmos na Gameplay,
        // precisamos atualizar as referências de UI dele na cena nova!
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // O GameManager antigo herda as coisas visuais da cena atualizada
            Instance.textoPlacar = this.textoPlacar;
            Instance.painelVitoria = this.painelVitoria;
            Instance.textoVencedor = this.textoVencedor;
            Instance.ConfigurarBotaoVoltar();
            Instance.AtualizarInterfacePlacar();

            Destroy(gameObject); 
            return;
        }
    }

    private void Start()
    {
        ConfigurarBotaoVoltar();
        AtualizarInterfacePlacar();
        
        // Se o GameManager nasceu no Boot/Menu, ele segue o fluxo normal
        if (SceneManager.GetActiveScene().name == "_Boot")
        {
            CarregarCena("MenuSelecao");
        }
    }

    public void CarregarCena(string nomeCena)
    {
        SceneManager.LoadScene(nomeCena);
    }

    private void ConfigurarBotaoVoltar()
    {
        if (painelVitoria != null)
        {
            UnityEngine.UI.Button botao = painelVitoria.transform.Find("BotaoVoltar")?.GetComponent<UnityEngine.UI.Button>();
            if (botao != null)
            {
                botao.onClick.RemoveAllListeners();
                botao.onClick.AddListener(VoltarParaOMenu);
            }
        }
    }

    public void RegistrarPonto(int jugadorQuePontuou)
    {
        if (painelVitoria != null && painelVitoria.activeSelf) return;

        if (jugadorQuePontuou == 1) roundsVitóriaJogador1++;
        else if (jugadorQuePontuou == 2) roundsVitóriaJogador2++;

        Debug.Log($"Placar: J1 ({roundsVitóriaJogador1}) vs J2 ({roundsVitóriaJogador2})");
        AtualizarInterfacePlacar();

        if (roundsVitóriaJogador1 >= 2 || roundsVitóriaJogador2 >= 2)
        {
            // FUNÇÃO DA VITÓRIA: Só aparece aqui quando alguém bate 2 pontos!
            if (painelVitoria != null) painelVitoria.SetActive(true);

            if (textoVencedor != null)
            {
                int vencedor = (roundsVitóriaJogador1 >= 2) ? 1 : 2;
                textoVencedor.text = $"O JOGADOR {vencedor} VENCEU A PARTIDA!";
            }
        }
        else
        {
            // Reinicia a cena para o próximo round
            CarregarCena("CenaGameplay");
        }
    }

    void AtualizarInterfacePlacar()
    {
        if (textoPlacar != null)
        {
            textoPlacar.text = $"J1: {roundsVitóriaJogador1}  |  J2: {roundsVitóriaJogador2}";
        }
    }

    public void VoltarParaOMenu()
    {
        ResetarPartida();
        CarregarCena("MenuSelecao");
    }

    public void ResetarPartida()
    {
        roundsVitóriaJogador1 = 0;
        roundsVitóriaJogador2 = 0;
    }
}