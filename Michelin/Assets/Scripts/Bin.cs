using UnityEngine;

public class Bin : MonoBehaviour
{
    public AudioSource source;
    private void OnTriggerStay2D(Collider2D other)
    {
        GameIngredient ingredient = other.GetComponent<GameIngredient>();
		source.Play();
		if (ingredient != null)
        {

            if (ingredient.selected == false)
            {
                ingredient.selected = false; // Stop draggi

                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.gravityScale = 2f; // Enable gravity so it falls
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }

                Debug.Log("Ingredient binned: " + other.name);
            }
           
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameIngredient ingredient = collision.GetComponent<GameIngredient>();

        if (ingredient != null)
        {
			source.Play();
			ingredient.isBeingBinned = true;
           

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameIngredient ingredient = collision.GetComponent<GameIngredient>();

        if (ingredient != null)
        {

            ingredient.isBeingBinned = false;
            source.Play();

        }
    }
}
