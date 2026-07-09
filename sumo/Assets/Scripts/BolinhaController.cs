using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller principal do jogador (bolinha). Gerencia movimento, empurrão e coleta de moedas.
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

    private ControlesSumo controles;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controles = new ControlesSumo();
    }

    private void Start()
    {
        // Força detecção contínua de colisão para que os Triggers e Física funcionem perfeitamente
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // Busca automática pelo inimigo na cena após ambos nascerem
        Invoke(nameof(BuscarInimigoAutomaticamente), 0.1f);
    }

    /// <summary>
    /// Esta função é chamada pelo GerenciadorSpawn assim que a bolinha é instanciada.
    /// Ela injeta os Scriptable Objects corretos e define quem é o J1 ou J2.
    /// </summary>
    public void InicializarBolinha(BolinhaData novosDados, bool ehJogador1)
    {
        dadosBase = novosDados;
        idJogador = ehJogador1 ? 1 : 2;
        
        // Aplica os dados vindos do seu Scriptable Object específico
        rb.mass = dadosBase.massaBase;
        transform.localScale = Vector3.one * dadosBase.tamanhoEscala;
        velocidadeAtual = dadosBase.velocidadeInicial;
        forcaEmpurraoAtual = dadosBase.forcaEmpurraoBase;

        // Altera a cor dinamicamente com base no arquivo laranja (Scriptable Object)
        SpriteRenderer renderizador = GetComponent<SpriteRenderer>();
        if (renderizador != null)
        {
            renderizador.color = ehJogador1 ? dadosBase.corJogador1 : dadosBase.corJogador2;
        }

        // Ativa os controles do Input System baseados no ID definido
        AtivarControlesPorID();

        Debug.Log($"[BOLINHA] Jogador {idJogador} inicializado com sucesso usando dados de: {dadosBase.name}");
    }

    private void AtivarControlesPorID()
    {
        // Desativa primeiro para garantir que não haja duplicatas
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
                inimigo = jogador;
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

        // 1. Fica mais pesada (+0.5 por moeda)
        rb.mass += 0.5f;

        // 2. Dá mais força (+3 por moeda)
        forcaEmpurraoAtual += 3f;

        // 3. Fica mais lenta (-0.5 por moeda, limite mínimo de 2f)
        velocidadeAtual = Mathf.Max(velocidadeAtual - 0.5f, 2f);

        Debug.Log($"[MOEDA] Jogador {idJogador} coletou sua {moedasColetadas}ª moeda! " +
                  $"Nova Massa: {rb.mass} | Nova Força Base: {forcaEmpurraoAtual} | Nova Vel: {velocidadeAtual}");
    }

    private void FixedUpdate()
    {
        Vector2 velocidadeDesejada = inputMovimento * velocidadeAtual;
        Vector2 diferencaVelocidade = velocidadeDesejada - rb.linearVelocity;
        
        float aceleracao = 20f; 
        
        rb.AddForce(diferencaVelocidade * aceleracao * rb.mass);
    }
}