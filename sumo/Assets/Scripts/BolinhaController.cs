using UnityEngine;
using UnityEngine.InputSystem;

public class BolinhaController : MonoBehaviour
{
    [Header("Configurações Base")]
    public BolinhaData dadosBase;
    public int idJogador;

    [Header("Alvo")]
    public BolinhaController inimigo; // Referência para a outra bolinha

    private Rigidbody2D rb;
    private Vector2 inputMovimento;
    private float velocidadeAtual;
    private float forcaEmpurraoAtual;

    [Header("Mecânica de Cooldown")]
    private float tempoUltimoEmpurrao = -10f; // Começa menor para poder usar de primeira
    public float tempoCooldown = 2.5f; // Tempo em segundos para usar de novo

    private ControlesSumo controles;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controles = new ControlesSumo();
    }

    private void Start()
    {
        if (dadosBase != null)
        {
            InicializarBolinha(dadosBase);
        }
    }

    public void InicializarBolinha(BolinhaData novosDados)
    {
        dadosBase = novosDados;
        
        rb.mass = dadosBase.massaBase;
        transform.localScale = Vector3.one * dadosBase.tamanhoEscala;
        velocidadeAtual = dadosBase.velocidadeInicial;
        forcaEmpurraoAtual = dadosBase.forcaEmpurraoBase;

        SpriteRenderer renderizador = GetComponent<SpriteRenderer>();
        if (renderizador != null)
        {
            renderizador.color = (idJogador == 1) ? dadosBase.corJogador1 : dadosBase.corJogador2;
        }
    }

    private void OnEnable()
    {
        if (idJogador == 1)
        {
            controles.Jogador1.Enable();
            controles.Jogador1.Mover.performed += AoMover;
            controles.Jogador1.Mover.canceled += AoMover;
            controles.Jogador1.Empurrar.started += AoTentarEmpurrar;
        }
        else if (idJogador == 2)
        {
            controles.Jogador2.Enable();
            controles.Jogador2.Mover.performed += AoMover;
            controles.Jogador2.Mover.canceled += AoMover;
            controles.Jogador2.Empurrar.started += AoTentarEmpurrar;
        }
    }

    private void OnDisable()
    {
        controles.Jogador1.Disable();
        controles.Jogador2.Disable();
    }

    private void AoMover(InputAction.CallbackContext contexto)
    {
        inputMovimento = contexto.ReadValue<Vector2>();
    }

    private void AoTentarEmpurrar(InputAction.CallbackContext contexto)
    {
        if (Time.time - tempoUltimoEmpurrao >= tempoCooldown)
        {
            ExecutarEmpurrao();
        }
        else
        {
            Debug.Log($"Jogador {idJogador} em Cooldown!");
        }
    }

    private void ExecutarEmpurrao()
    {
        if (inimigo == null) return;

        // 1. Calcula a distância entre as duas bolinhas
        float distancia = Vector2.Distance(transform.position, inimigo.transform.position);

        // 2. Verifica se o inimigo está dentro do alcance máximo de empurrão
        if (distancia > 3f) 
        {
            Debug.Log($"Jogador {idJogador} tentou empurrar, mas o inimigo está muito longe! Distância: {distancia}");
            return; 
        }

        // Se chegou aqui, o golpe acertou! Ativa o Cooldown:
        tempoUltimoEmpurrao = Time.time;

        // 3. Calcula a direção em que o inimigo será arremessado
        Vector2 direcao = (inimigo.transform.position - transform.position).normalized;

        // 4. Regra de proximidade com o multiplicador x5 para dar impacto real
        float fatorDistancia = 1f / Mathf.Max(distancia, 0.4f);
        float forcaFinal = forcaEmpurraoAtual * fatorDistancia * 5f;

        Rigidbody2D rbInimigo = inimigo.GetComponent<Rigidbody2D>();
        if (rbInimigo != null)
        {
            // Aplica a força de impacto imediato (Impulse)
            rbInimigo.AddForce(direcao * forcaFinal, ForceMode2D.Impulse);
            Debug.Log($"Jogador {idJogador} EMPURROU COM FORÇA: {forcaFinal}!");
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = inputMovimento * velocidadeAtual;
    }
}