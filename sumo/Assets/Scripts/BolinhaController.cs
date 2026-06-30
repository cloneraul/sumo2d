using UnityEngine;
using UnityEngine.InputSystem;

public class BolinhaController : MonoBehaviour
{
    [Header("Configurações Base")]
    public BolinhaData dadosBase; // O Scriptable Object com os status
    public int idJogador; // 1 para J1, 2 para J2

    private Rigidbody2D rb;
    private Vector2 inputMovimento;
    private float velocidadeAtual;

    // Referência para a classe de controles gerada pelo Input System
    private ControlesSumo controles;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controles = new ControlesSumo();
    }

    private void Start()
    {
        // Se já tiver um dado inserido para testes, inicializa
        if (dadosBase != null)
        {
            InicializarBolinha(dadosBase);
        }
    }

    // Método que configura a bolinha com os dados do Scriptable Object correspondente
    public void InicializarBolinha(BolinhaData novosDados)
    {
        dadosBase = novosDados;
        
        // Aplica os status do Scriptable Object à física do Rigidbody2D
        rb.mass = dadosBase.massaBase;
        transform.localScale = Vector3.one * dadosBase.tamanhoEscala;
        velocidadeAtual = dadosBase.velocidadeInicial;

        // Aplica a cor correta dependendo de qual jogador assumiu esta bolinha
        SpriteRenderer renderizador = GetComponent<SpriteRenderer>();
        if (renderizador != null)
        {
            renderizador.color = (idJogador == 1) ? dadosBase.corJogador1 : dadosBase.corJogador2;
        }
    }

    // Ativa os mapas de input por eventos (Padrão Observer)
    private void OnEnable()
    {
        if (idJogador == 1)
        {
            controles.Jogador1.Enable();
            controles.Jogador1.Mover.performed += AoMover;
            controles.Jogador1.Mover.canceled += AoMover;
        }
        else if (idJogador == 2)
        {
            controles.Jogador2.Enable();
            controles.Jogador2.Mover.performed += AoMover;
            controles.Jogador2.Mover.canceled += AoMover;
        }
    }

    private void OnDisable()
    {
        // Desativa os controles para evitar erros de memória
        controles.Jogador1.Disable();
        controles.Jogador2.Disable();
    }

    private void AoMover(InputAction.CallbackContext contexto)
    {
        // Salva a direção das teclas (WASD ou Setas) enviadas pelo Input System
        inputMovimento = contexto.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Move o Rigidbody2D diretamente nas 4 direções (X e Y)
        rb.linearVelocity = inputMovimento * velocidadeAtual;
    }
}