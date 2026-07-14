using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller principal do jogador (bolinha). Gerencia movimento, empurrão, coleta de moedas e UI.
/// </summary>
public class BolinhaController : MonoBehaviour
{
    [Header("Configurações Base")]
    public BolinhaData dadosBase;
    public int idJogador;

    [Header("Alvo")]
    public BolinhaController inimigo; 

    private Rigidbody2D rb;
    private Vector2 inputMovimento;
    private float velocidadeAtual;
    private float forcaEmpurraoAtual;

    [Header("Mecânica de Cooldown")]
    private float tempoUltimoEmpurrao = -10f; 
    public float tempoCooldown = 2.5f; 

    [Header("Atributos Acumulados por Moedas")]
    public int moedasColetadas = 0;

    [Header("Ajuste Fino do Balanço (Moedas)")]
    [Tooltip("Quanto de massa a bolinha ganha por moeda.")]
    public float ganhoMassaPorMoeda = 0.2f;
    [Tooltip("Quanto de força de empurrão a bolinha ganha por moeda.")]
    public float ganhoForcaPorMoeda = 0.5f;
    [Tooltip("Quanto de velocidade a bolinha perde por moeda.")]
    public float perdaVelocidadePorMoeda = 0.2f;
    [Tooltip("Velocidade mínima que a bolinha pode chegar ao coletar muitas moedas.")]
    public float velocidadeMinimaPossivel = 3.0f;

    private ControlesSumo controles;
    private GameManager gameManager; // Referência para o GameManager da cena

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controles = new ControlesSumo();
    }

    private void Start()
    {
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        gameManager = FindFirstObjectByType<GameManager>();
        Invoke(nameof(BuscarInimigoAutomaticamente), 0.1f);
    }

    private void Update()
    {
        // Envia as informações do tempo de recarga para a HUD atualizar a cada frame
        EnviarCooldownParaUI();
    }

    /// <summary>
    /// Esta função é chamada pelo GerenciadorSpawn assim que a bolinha é instanciada.
    /// Ela injeta os Scriptable Objects corretos e define quem é o J1 ou J2.
    /// </summary>
    public void InicializarBolinha(BolinhaData novosDados, bool ehJogador1)
    {
        dadosBase = novosDados;
        idJogador = ehJogador1 ? 1 : 2;
        
        rb.mass = dadosBase.massaBase;
        transform.localScale = Vector3.one * dadosBase.tamanhoEscala;
        velocidadeAtual = dadosBase.velocidadeInicial;
        forcaEmpurraoAtual = dadosBase.forcaEmpurraoBase;

        SpriteRenderer renderizador = GetComponent<SpriteRenderer>();
        if (renderizador != null)
        {
            renderizador.color = ehJogador1 ? dadosBase.corJogador1 : dadosBase.corJogador2;
        }

        AtivarControlesPorID();
        NotificarUI();

        Debug.Log($"[BOLINHA] Jogador {idJogador} inicializado com sucesso usando dados de: {dadosBase.name}");
    }

    private void AtivarControlesPorID()
    {
        controles.Jogador1.Disable();
        controles.Jogador2.Disable();

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

    private void BuscarInimigoAutomaticamente()
    {
        BolinhaController[] todosJogadores = FindObjectsByType<BolinhaController>(FindObjectsSortMode.None);
        foreach (BolinhaController jogador in todosJogadores)
        {
            if (jogador != this)
            {
                inimigo = jogador; // CORREÇÃO AQUI: Removido o 'child:'
                break;
            }
        }
    }

    private void OnDisable()
    {
        if (controles != null)
        {
            controles.Jogador1.Disable();
            controles.Jogador2.Disable();
        }
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

        float distancia = Vector2.Distance(transform.position, inimigo.transform.position);

        if (distancia > 3f) 
        {
            Debug.Log($"Jogador {idJogador} tentou empurrar, mas o inimigo está muito longe!");
            return; 
        }

        tempoUltimoEmpurrao = Time.time;

        Vector2 direcao = (inimigo.transform.position - transform.position).normalized;

        float fatorDistancia = 1f / Mathf.Max(distancia, 0.4f);

        float forcaFinal = forcaEmpurraoAtual * fatorDistancia * 5f;
        
        if (float.IsNaN(forcaFinal) || float.IsInfinity(forcaFinal)) forcaFinal = 0f;
        float maxEmpurrao = 100f; 
        forcaFinal = Mathf.Clamp(forcaFinal, -maxEmpurrao, maxEmpurrao);

        Rigidbody2D rbInimigo = inimigo.GetComponent<Rigidbody2D>();
        if (rbInimigo != null)
        {
            rbInimigo.AddForce(direcao * forcaFinal, ForceMode2D.Impulse);
            Debug.Log($"Jogador {idJogador} EMPURROU com força total de: {forcaFinal}!");
        }
    }

    public void ColetarMoedaEvolutiva()
    {
        moedasColetadas++;

        rb.mass += ganhoMassaPorMoeda;
        forcaEmpurraoAtual += ganhoForcaPorMoeda;
        velocidadeAtual = Mathf.Max(velocidadeAtual - perdaVelocidadePorMoeda, velocidadeMinimaPossivel);

        NotificarUI();

        Debug.Log($"[MOEDA] Jogador {idJogador} coletou sua {moedasColetadas}ª moeda! " +
                  $"Nova Massa: {rb.mass} | Nova Força Base: {forcaEmpurraoAtual} | Nova Vel: {velocidadeAtual}");
    }

    private void NotificarUI()
    {
        if (gameManager != null)
        {
            gameManager.AtualizarMoedasInterface(idJogador, moedasColetadas);
        }
    }

    /// <summary>
    /// Calcula o tempo de cooldown restante e envia para o GameManager atualizar o Canvas.
    /// </summary>
    private void EnviarCooldownParaUI()
    {
        if (gameManager == null) return;

        float tempoPassado = Time.time - tempoUltimoEmpurrao;
        float tempoRestante = tempoCooldown - tempoPassado;

        if (tempoRestante > 0)
        {
            gameManager.AtualizarCooldownInterface(idJogador, tempoRestante);
        }
        else
        {
            gameManager.AtualizarCooldownInterface(idJogador, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (inputMovimento.magnitude > 0)
        {
            Vector2 velocidadeDesejada = inputMovimento * velocidadeAtual;
            Vector2 diferencaVelocidade = velocidadeDesejada - rb.linearVelocity;
            
            float aceleracao = 20f; 
            Vector2 forcaAplicar = diferencaVelocidade * aceleracao * rb.mass;
            
            float maxForceMagnitude = 50f;
            if (forcaAplicar.magnitude > maxForceMagnitude)
            {
                forcaAplicar = forcaAplicar.normalized * maxForceMagnitude;
            }

            rb.AddForce(forcaAplicar);
        }
    }
}