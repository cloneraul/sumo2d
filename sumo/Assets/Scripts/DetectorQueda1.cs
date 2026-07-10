using UnityEngine;

public class DetectorQueda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se o objeto que encostou no sensor começar com o nome "Teste_J1"
        if (collision.name.StartsWith("Teste_J1"))
        {
            // O Jogador 1 caiu, então o ponto vai para o Jogador 2!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(2);
            }
        }
        // Se o objeto que encostou no sensor começar com o nome "Teste_J2"
        else if (collision.name.StartsWith("Teste_J2"))
        {
            // O Jogador 2 caiu, então o ponto vai para o Jogador 1!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(1);
            }
        }
    }
}