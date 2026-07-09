using UnityEngine;

public class DetectorQueda : MonoBehaviour
{
    private GerenciadorPartida gerenciador;

    void Start()
    {
        gerenciador = FindFirstObjectByType<GerenciadorPartida>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Só ativa se o objeto que caiu for um dos clones válidos do jogo
        if (collision.name == "Teste_J1" || collision.name == "Teste_J2")
        {
            if (gerenciador != null)
            {
                gerenciador.RegistrarQueda(collision.name);
            }
        }
    }
}