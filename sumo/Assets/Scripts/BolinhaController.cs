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
            // Assina o evento do botão de Empurrão (Observer)
            controles.Jogador1.Empurrar.started += AoTentarEmpurrar;
        }
        else if (idJogador == 2)
        {
            controles.Jogador2.Enable();
            controles.Jogador2.Mover.performed += AoMover;
            controles.Jogador2.Mover.canceled += AoMover;
            // Assina o evento do botão de Empurrão (Observer)
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

    // Método chamado pelo evento do botão do Input System
    private void AoTentarEmpurrar(InputAction.CallbackContext contexto)
    {
        // Verifica se o tempo de cooldown já passou
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

        // Guarda o momento do ataque para o Cooldown
        tempoUltimoEmpurrao = Time.time;

        // 1. Calcula a distância entre as duas bolinhas
        float distancia = Vector2.Distance(transform.position, inimigo.transform.position);

        // 2. Calcula a direção oposta (da minha bolinha em direção ao inimigo)
        Vector2 direcao = (inimigo.transform.position - transform.position).normalized;

        // 3. Regra de proximidade: quanto mais perto, mais forte o empurrão
        // Se a distância for muito pequena, travamos em 0.5f para não multiplicar por infinito
        float fatorDistancia = 1f / Mathf.Max(distancia, 0.5f);
        float forcaFinal = forcaEmpurraoAtual * fatorDistancia;

        // 4. Aplica a força do tipo 'Impulse' (impacto imediato) no Rigidbody do inimigo
        Rigidbody2D rbInimigo = inimigo.GetComponent<Rigidbody2D>();
        if (rbInimigo != null)
        {
            rbInimigo.AddForce(direcao * forcaFinal, ForceMode2D.Impulse);
            Debug.Log($"Jogador {idJogador} empurrou com força: {forcaFinal}");
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = inputMovimento * velocidadeAtual;
    }
}