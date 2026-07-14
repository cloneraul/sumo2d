using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // Necessário para controlar o componente Image do soco

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

    [Header("Referências de Moedas UI (Arraste aqui)")]
    public TextMeshProUGUI textoMoedasJ1;
    public TextMeshProUGUI textoMoedasJ2;

    [Header("Referências de Cooldown UI (Arraste aqui)")]
    public TextMeshProUGUI textoCooldownJ1; // Arraste o "Texto_Cooldown_J1"
    public TextMeshProUGUI textoCooldownJ2; // Arraste o "Texto_Cooldown_J2"
    
    [Header("Imagens das Teclas de Cooldown (Arraste aqui)")]
    public Image imagemTeclaJ1; // Arraste o "Imagem_Tecla_J1" (Soco do J1)
    public Image imagemTeclaJ2; // Arraste o "Imagem_Tecla_J2" (Soco do J2)

    // Cores para o efeito de liga/desliga do soco
    private Color corNormal = Color.white; // Claro / Ativo
    private Color corEscura = new Color(0.25f, 0.25f, 0.25f, 1f); // Escuro / Em Cooldown

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Instance.textoPlacar = this.textoPlacar;
            Instance.painelVitoria = this.painelVitoria;
            Instance.textoVencedor = this.textoVencedor;
            Instance.textoMoedasJ1 = this.textoMoedasJ1;
            Instance.textoMoedasJ2 = this.textoMoedasJ2;
            
            // Garante as novas referências de Cooldown ao reiniciar o round
            Instance.textoCooldownJ1 = this.textoCooldownJ1;
            Instance.textoCooldownJ2 = this.textoCooldownJ2;
            Instance.imagemTeclaJ1 = this.imagemTeclaJ1;
            Instance.imagemTeclaJ2 = this.imagemTeclaJ2;

            Instance.ConfigurarBotaoVoltar();
            Instance.AtualizarInterfacePlacar();
            Instance.ResetarInterfaceMoedas(); 
            Instance.ResetarInterfaceCooldowns(); 

            Destroy(gameObject); 
            return;
        }
    }

    private void Start()
    {
        ConfigurarBotaoVoltar();
        AtualizarInterfacePlacar();
        ResetarInterfaceMoedas();
        ResetarInterfaceCooldowns();
        
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
            if (painelVitoria != null) painelVitoria.SetActive(true);

            if (textoVencedor != null)
            {
                int vencedor = (roundsVitóriaJogador1 >= 2) ? 1 : 2;
                textoVencedor.text = $"O JOGADOR {vencedor} VENCEU A PARTIDA!";
            }
        }
        else
        {
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

    public void AtualizarMoedasInterface(int idJogador, int totalMoedas)
    {
        if (idJogador == 1 && textoMoedasJ1 != null)
        {
            textoMoedasJ1.text = $"J1 Moedas: {totalMoedas}";
        }
        else if (idJogador == 2 && textoMoedasJ2 != null)
        {
            textoMoedasJ2.text = $"J2 Moedas: {totalMoedas}";
        }
    }

    public void ResetarInterfaceMoedas()
    {
        if (textoMoedasJ1 != null) textoMoedasJ1.text = "J1 Moedas: 0";
        if (textoMoedasJ2 != null) textoMoedasJ2.text = "J2 Moedas: 0";
    }

    /// <summary>
    /// Controla visualmente o tempo e a cor do botão de soco de cada jogador.
    /// </summary>
    public void AtualizarCooldownInterface(int idJogador, float tempoRestante)
    {
        if (idJogador == 1)
        {
            if (tempoRestante > 0)
            {
                if (textoCooldownJ1 != null) textoCooldownJ1.text = tempoRestante.ToString("F1") + "s";
                if (imagemTeclaJ1 != null) imagemTeclaJ1.color = corEscura; // Escurece o soco
            }
            else
            {
                if (textoCooldownJ1 != null) textoCooldownJ1.text = ""; // Remove o texto
                if (imagemTeclaJ1 != null) imagemTeclaJ1.color = corNormal; // Reacende o soco
            }
        }
        else if (idJogador == 2)
        {
            if (tempoRestante > 0)
            {
                if (textoCooldownJ2 != null) textoCooldownJ2.text = tempoRestante.ToString("F1") + "s";
                if (imagemTeclaJ2 != null) imagemTeclaJ2.color = corEscura; // Escurece o soco
            }
            else
            {
                if (textoCooldownJ2 != null) textoCooldownJ2.text = ""; // Remove o texto
                if (imagemTeclaJ2 != null) imagemTeclaJ2.color = corNormal; // Reacende o soco
            }
        }
    }

    public void ResetarInterfaceCooldowns()
    {
        if (textoCooldownJ1 != null) textoCooldownJ1.text = "";
        if (textoCooldownJ2 != null) textoCooldownJ2.text = "";
        if (imagemTeclaJ1 != null) imagemTeclaJ1.color = corNormal;
        if (imagemTeclaJ2 != null) imagemTeclaJ2.color = corNormal;
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