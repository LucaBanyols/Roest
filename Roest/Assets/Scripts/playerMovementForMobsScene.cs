using UnityEngine;

public class PlayerMovementForMobsScene : MonoBehaviour
{
    public float speed = 5f; // Vitesse de déplacement du joueur

    void Update()
    {
        // Récupérer les entrées de l'axe horizontal et vertical
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculer le vecteur de mouvement en fonction des entrées
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * speed * Time.deltaTime;

        // Appliquer le mouvement au transform du joueur
        transform.Translate(movement);

        // Vous pouvez également utiliser Rigidbody pour des mouvements physiques plus réalistes
        // Rigidbody rb = GetComponent<Rigidbody>();
        // rb.MovePosition(rb.position + movement);
    }
}
