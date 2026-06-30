using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;

    [Header("Jogador")]
    public bool jogador1 = true;

    [Header("Empurrão")]
    public BallController inimigo;
    public float forcaEmpurrao = 15f;
    public float distanciaMaxima = 3f;

    private PlayerControls controls;
    private Rigidbody2D rb;
    private Vector2 movimento;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();

        if (jogador1)
        {
            controls.Player1.Move.performed += OnMove;
            controls.Player1.Move.canceled += OnMove;
            controls.Player1.Push.performed += OnPush;
        }
        else
        {
            controls.Player2.Move.performed += OnMove;
            controls.Player2.Move.canceled += OnMove;
            controls.Player2.Push.performed += OnPush;
        }
    }

    private void OnEnable()
    {
        if (jogador1)
            controls.Player1.Enable();
        else
            controls.Player2.Enable();
    }

    private void OnDisable()
    {
        if (jogador1)
            controls.Player1.Disable();
        else
            controls.Player2.Disable();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movimento * velocidade;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movimento = context.ReadValue<Vector2>();
    }

    private void OnPush(InputAction.CallbackContext context)
    {
        if (inimigo == null)
            return;

        float distancia = Vector2.Distance(transform.position, inimigo.transform.position);

        // Está muito longe
        if (distancia > distanciaMaxima)
            return;

        // Direção do empurrão
        Vector2 direcao = (inimigo.transform.position - transform.position).normalized;

        // Quanto mais perto, maior a força
        float intensidade = 1f - (distancia / distanciaMaxima);

        float forcaFinal = forcaEmpurrao * intensidade;

        Rigidbody2D rbInimigo = inimigo.GetComponent<Rigidbody2D>();

        rbInimigo.AddForce(direcao * forcaFinal, ForceMode2D.Impulse);

        Debug.Log(jogador1 ? "Player 1 empurrou!" : "Player 2 empurrou!");
    }
}