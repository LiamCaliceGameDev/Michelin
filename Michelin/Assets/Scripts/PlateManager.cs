using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Make sure to include this to access UI components
using static Ingredient;
using TMPro;

public class PlateManager : MonoBehaviour
{
    [Header("Rule System")]
    public List<PlateRule> currentRules = new List<PlateRule>();
    public List<PlateRule> availableRules; // Assign via Inspector (add your rule assets here)
    public int minRules = 1;
    public int maxRules = 1;

    public RuleUIManager ruleUIManager; // Assign via Inspector


    [Header("Plate Settings")]
    public Vector3 plateCenter = Vector3.zero;
    public float plateRadius = 1.0f;

    [Header("Gizmo Settings")]
    public Color gizmoColor = Color.green;

    public static PlateManager instance;

    // List of ingredients currently on the plate
    public List<GameIngredient> ingredientsOnPlate = new List<GameIngredient>();

    public List<Ingredient.IngredientType> allIngredientTypes; // List of all ingredient types in your game

    public Slider timeSlider; // Assign in Inspector
    public float maxTimePerPlate = 60f; // Max time for each plate (in seconds)
    private float timer;
    private bool timerRunning = false;

    public int totalLives = 3;          // Total number of lives
    private int remainingLives;         // Remaining lives
    public List<GameObject> badMarkers; // Bad markers for UI (3 bad markers in your UI)

    public TMP_Text scoreText; // UI text to display the score (assign in Inspector)
    public int score = 0; // To keep track of the player's score
    public int bonusPointsFor5Seconds = 50;  // Points for completing within 5 seconds
    public int bonusPointsFor10Seconds = 30; // Points for completing within 10 seconds
    public int basePoints;
    private float plateStartTime; // To track when the plate started (for bonus points)

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        remainingLives = totalLives;
        UpdateLivesUI();  // Update UI to show initial lives
        NewPlate();
    }

    public void NewPlate()
    {
        ResetPlate();
        currentRules.Clear();

        int ruleCount = Random.Range(minRules, maxRules + 1);
        List<PlateRule> selectedRules = new List<PlateRule>();

        var shuffled = availableRules.OrderBy(r => Random.value).ToList();

        // STEP 1: Force-add a MustHaveIngredientCountRule first (if available)
        PlateRule countRule = shuffled.FirstOrDefault(r => r is IngredientCountRule);
        if (countRule != null)
        {
            selectedRules.Add(countRule);
        }

        bool chainRuleAdded = false;

        // STEP 2: Add compatible rules
        foreach (var rule in shuffled)
        {
            if (selectedRules.Contains(rule)) continue;

            // Skip additional chain rules if one is already added
            if (chainRuleAdded && rule is MustHaveChainRule)
                continue;

            if (selectedRules.Count < ruleCount && rule.IsCompatibleWith(selectedRules))
            {
                selectedRules.Add(rule);

                if (rule is MustHaveChainRule)
                {
                    chainRuleAdded = true;
                }
            }
        }

        currentRules = selectedRules;
        ruleUIManager?.DisplayRules(currentRules);

        timer = maxTimePerPlate;
        timerRunning = true;

        plateStartTime = Time.time; // Record the time when the plate starts

        if (timeSlider != null)
        {
            timeSlider.maxValue = maxTimePerPlate;
            timeSlider.value = maxTimePerPlate;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position + plateCenter, plateRadius);
    }

    /// <summary>
    /// Check if a transform is within the plate area.
    /// </summary>
    public bool IsWithinPlate(Transform objTransform)
    {
        return IsWithinPlate(objTransform.position);
    }

    /// <summary>
    /// Check if a world position is within the plate area.
    /// </summary>
    public bool IsWithinPlate(Vector2 position)
    {
        float distance = Vector2.Distance(position, transform.position + plateCenter);
        return distance <= plateRadius;
    }

    /// <summary>
    /// Call this when an ingredient is placed on the plate.
    /// </summary>
    public void RegisterIngredient(GameIngredient ingredient)
    {
        if (!ingredientsOnPlate.Contains(ingredient))
        {
            ingredientsOnPlate.Add(ingredient);
        }
    }

    public void UnRegisterIngrediennt(GameIngredient ingredient)
    {
        if (ingredientsOnPlate.Contains(ingredient))
        {
            ingredientsOnPlate.Remove(ingredient);
        }
    }

    /// <summary>
    /// Call this when an ingredient is removed/destroyed.
    /// </summary>
    public void UnregisterIngredient(GameIngredient ingredient)
    {
        ingredientsOnPlate.Remove(ingredient);
    }

    public int GetTotalIngredients()
    {
        return ingredientsOnPlate.Count;
    }

    public float GetTotalWeight()
    {
        return ingredientsOnPlate.Sum(i => i.ingredientData.weight);
    }

    public int GetIngredientCountByType(IngredientType type)
    {
        return ingredientsOnPlate.Count(i => i.ingredientData.ingredientType == type);
    }

    public void ResetPlate()
    {
        // Create a copy to avoid modifying the list while iterating
        var ingredientsToRemove = new List<GameIngredient>(ingredientsOnPlate);

        foreach (var ingredient in ingredientsToRemove)
        {
            if (ingredient != null)
            {
                Destroy(ingredient.gameObject);
            }
        }

        ingredientsOnPlate.Clear();
    }

    public void SendOutPlate()
    {
        bool allRulesSatisfied = currentRules.All(rule => rule.IsRuleSatisfied(this));

        if (allRulesSatisfied)
        {
            Debug.Log("✅ Success! All rules are satisfied.");

            // Add points for successfully completing the plate
            AddPointsForSuccess();
        }
        else
        {
            Debug.Log("❌ Wrong! Some rules are not satisfied.");
            LoseLife();  // Subtract a life if the order is wrong
        }

        NewPlate();
    }

    public int GetLongestIngredientChain()
    {
        int maxChain = 0;
        HashSet<GameIngredient> visited = new HashSet<GameIngredient>();

        foreach (var ingredient in ingredientsOnPlate)
        {
            if (!visited.Contains(ingredient))
            {
                int chainLength = DepthFirstChain(ingredient, visited);
                if (chainLength > maxChain)
                {
                    maxChain = chainLength;
                }
            }
        }

        return maxChain;
    }

    private int DepthFirstChain(GameIngredient ingredient, HashSet<GameIngredient> visited)
    {
        if (visited.Contains(ingredient)) return 0;

        visited.Add(ingredient);
        int count = 1;

        foreach (var neighbor in ingredient.touchingIngredients)
        {
            if (ingredientsOnPlate.Contains(neighbor)) // Only count plate ingredients
            {
                count += DepthFirstChain(neighbor, visited);
            }
        }

        return count;
    }

    private void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;

            // Update slider value
            if (timeSlider != null)
            {
                timeSlider.value = timer;
            }

            // Check if the time is up
            if (timer <= 0f)
            {
                timerRunning = false;
                Debug.Log("⏰ You took too long!");
                LoseLife();  // Subtract a life if the order is wrong
                NewPlate(); // Automatically reset plate
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            print(GetLongestIngredientChain());
        }
    }

    private void UpdateLivesUI()
    {
        // Update Bad Markers (set active depending on remaining lives)
        for (int i = 0; i < badMarkers.Count; i++)
        {
            if (i < totalLives - remainingLives)
            {
                badMarkers[i].SetActive(true); // Show the bad marker
            }
            else
            {
                badMarkers[i].SetActive(false); // Hide the bad marker
            }
        }

    }

    private void LoseLife()
    {
        remainingLives--;
        UpdateLivesUI();  // Update the UI

        if (remainingLives <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void AddPointsForSuccess()
    {
        float timeTaken = Time.time - plateStartTime; // Calculate time taken to complete the plate

        int pointsToAdd = basePoints;  // Base points for a successful plate

        // Award bonus points based on how fast the player completed the plate
        if (timeTaken <= 5f)
        {
            pointsToAdd += bonusPointsFor5Seconds;
            Debug.Log("Bonus: 5 seconds! Added " + bonusPointsFor5Seconds + " points.");
        }
        else if (timeTaken <= 10f)
        {
            pointsToAdd += bonusPointsFor10Seconds;
            Debug.Log("Bonus: 10 seconds! Added " + bonusPointsFor10Seconds + " points.");
        }

        score += pointsToAdd; // Add the points to the score
        Debug.Log("Added " + pointsToAdd + " points. Total score: " + score);

        UpdateScoreUI();  // Update the UI with the new score
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score; // Display the score
        }
    }
}
