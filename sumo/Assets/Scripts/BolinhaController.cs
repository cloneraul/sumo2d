using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller principal do jogador (bolinha). Gerencia movimento, empurrão e coleta de moedas.
/// 
/// PARA DIAGNOSTICAR PROBLEMAS COM TRIGGERS:
/// Se OnTriggerEnter2D não dispara quando o jogador pega moeda:
/// 1. Adicione o script DetectorFisico.cs a este GameObject
/// 2. Adicione DetectorFisico.cs também à moeda
/// 3. Consulte LAYER_COLLISION_GUIDE.md na pasta session-state
/// 
/// Possíveis problemas:
/// - Layer Collision Matrix bloqueando colisões (Physics 2D Settings)
/// - "Is Trigger" não ativado na moeda
/// - Collision Detection não em modo Continuous (já configurado aqui)
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
        if (dadosBase != null)
        {
            InicializarBolinha(dadosBase);
        }

        // FIX: Força detecção contínua de colisão para que OnTriggerEnter2D dispare corretamente
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
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

        float distancia = Vector2.Distance(transform.position, inimigo.transform.position);

        if (distancia > 3f) 
        {
            Debug.Log($"Jogador {idJogador} tentou empurrar, mas o inimigo está muito longe!");
            return; 
        }

        tempoUltimoEmpurrao = Time.time;

        Vector2 direcao = (inimigo.transform.position - transform.position).normalized;

        float fatorDistancia = 1f / Mathf.Max(distancia, 0.4f);
        
        // A força final usa a variável 'forcaEmpurraoAtual', que cresce a cada moeda!
        float forcaFinal = forcaEmpurraoAtual * fatorDistancia * 5f;

        Rigidbody2D rbInimigo = inimigo.GetComponent<Rigidbody2D>();
        if (rbInimigo != null)
        {
            rbInimigo.AddForce(direcao * forcaFinal, ForceMode2D.Impulse);
            Debug.Log($"Jogador {idJogador} EMPURROU com força total de: {forcaFinal}!");
        }
    }

    // REGRA DO DOCUMENTO: Modificadores Acumulativos das Moedas
    public void ColetarMoedaEvolutiva()
    {
        moedasColetadas++;

        // 1. Fica mais pesada (Aumenta a massa no Rigidbody em +0.5 por moeda)
        rb.mass += 0.5f;

        // 2. Dá mais força (Aumenta a força base de empurrão em +3 por moeda)
        forcaEmpurraoAtual += 3f;

        // 3. Fica mais lenta (Diminui a velocidade atual em -0.5 por moeda)
        // Colocamos um limite mínimo (ex: 2f) para a bolinha não parar de andar se pegar muitas moedas
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