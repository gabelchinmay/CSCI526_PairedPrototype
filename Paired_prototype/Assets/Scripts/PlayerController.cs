using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // Import the System.Collections.Generic namespace for dictionaries


using TMPro; // Import TextMeshPro namespace


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 10.0f;
    private float jumpForce = 7.0f;
    private int jumpCount;
    private int maxJumps = 2;
    private bool isGameOver = false;
    private bool canJump = true;
    private bool isOnPlatform = false;
    private Color currentColour;
    private PlatformController CurrPlatform = null;
    public int maxHealth = 100;
    private int currentHealth;
    public TMP_Text healthText; // Reference to the TMP Text component
    public TMP_Text gameOverText;
    public TMP_Text InventoryText;
    public Image HealthSkeleton;
    public Image HealthBar;
    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    private Vector3 playerPreviousPosition; // Add this variable
    public GameObject placeholderPrefab;
    private bool isOnDefrost = false;



    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(InflictDamages());

    }

    void Update()
    {
        if (isOnPlatform)
        {
            //Debug.Log("On platform");

        }
        
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps && canJump)
        {
            Jump();
            jumpCount++;
            if(jumpCount == maxJumps){
                canJump = false;
                StartCoroutine(ResetJumpCooldown());
            }
        }


        if(!isGameOver){
            float horizontalInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
            

        }
        // Handle horizontal movement

        if (inventory.Count != 0)
        {
            InventoryText.text = "";
            InventoryText.text += "INVENTORY";

            if (inventory.ContainsKey("HealthUp"))
            {
                InventoryText.text += "\nHealthUp: " + inventory["HealthUp"];

            }
            if (inventory.ContainsKey("JumpHigher"))
            {
                InventoryText.text += "\nJumpHighers: " + inventory["JumpHigher"];

            }

            if (inventory.ContainsKey("Defrost"))
            {
                InventoryText.text += "\nDefrosts: " + inventory["Defrost"]; 
            }
            if (inventory.ContainsKey("Placeholder"))
            {
                InventoryText.text += "\nPlaceholders: " + inventory["Placeholder"];
            }
        }
        else
        {
            InventoryText.text = "";

        }

        if (Input.GetKeyDown(KeyCode.K) && inventory.ContainsKey("Placeholder")){

                playerPreviousPosition = transform.position;
                Vector3 placeholderPosition = playerPreviousPosition - Vector3.up * 0.7f;
                transform.Translate(Vector2.right * 1.1f); // Move 0.1 units to the right
                Instantiate(placeholderPrefab, placeholderPosition, Quaternion.identity);

                if (inventory.ContainsKey("Placeholder"))
                {
                    inventory["Placeholder"]--;
                    if (inventory["Placeholder"] <= 0)
                    {
                        inventory.Remove("Placeholder");
                    }
                }

            }


        if (Input.GetKeyDown(KeyCode.J) && inventory.ContainsKey("JumpHigher"))
        {
            StartCoroutine(JumpHigherPowerUp());

            if (inventory.ContainsKey("JumpHigher"))
            {
                inventory["JumpHigher"]--;
                if (inventory["JumpHigher"] <= 0)
                {
                    inventory.Remove("JumpHigher");
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.H) && inventory.ContainsKey("HealthUp"))
        {
            if(currentHealth<= maxHealth-10)
            {
                currentHealth += 10;

            }
            else
            {
                currentHealth = maxHealth;
            }
            UpdateHealthUI();

            if (inventory.ContainsKey("HealthUp"))
            {
                inventory["HealthUp"]--;
                if (inventory["HealthUp"] <= 0)
                {
                    inventory.Remove("HealthUp");
                }
            }

        }


        if (Input.GetKeyDown(KeyCode.F) && inventory.ContainsKey("Defrost"))
        {
            StartCoroutine(DefrostPowerUp());

            if (inventory.ContainsKey("Defrost"))
            {
                inventory["Defrost"]--;
                if (inventory["Defrost"] <= 0)
                {
                    inventory.Remove("Defrost");
                }
            }

        }


    }




    public void freeze(){
        isGameOver = true;
    }

    private void Jump()
    {
        if(!isGameOver){
            rb.velocity = new Vector2(rb.velocity.x, 0); // Reset vertical velocity before jump.
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        }

    }

    private System.Collections.IEnumerator ResetJumpCooldown()
    {
        yield return new WaitForSeconds(1.7f); // Adjust the cooldown duration as needed
        canJump = true;
        jumpCount=0;
    }

    public void OnPlatformEnter(PlatformController platform)
    {
        isOnPlatform = true;
        CurrPlatform = platform;
    }

    public void OnPlatformExit(PlatformController platform)
    {
        isOnPlatform = false;
        CurrPlatform = null;

    }

    private IEnumerator JumpHigherPowerUp()
    {
        jumpForce = 14.0f;

        yield return new WaitForSeconds(5.0f);

        jumpForce = 7.0f;

    }

    private IEnumerator DefrostPowerUp()
    {
        isOnDefrost = true;

        yield return new WaitForSeconds(5.0f);

        isOnDefrost = false;

    }



    private IEnumerator InflictDamages()
    {
        while (true){

            if(!isOnPlatform ){
                speed = 10.0f;
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            currentColour = Color.black;

            if(CurrPlatform != null){
                currentColour = CurrPlatform.getSpriteColor();
            }

            if(isOnPlatform)
            {
                if(currentColour == Color.red){
                    Debug.Log("Inflicting Damage.....");
                    this.TakeDamage(2);
                    speed = 10.0f;
                    yield return new WaitForSeconds(0.5f);
                }

                else if(currentColour == Color.cyan && !isOnDefrost)
                {
                    Debug.Log("Inflicting Damage.....");
                    this.TakeDamage(1);
                    speed = 1.0f;
                    yield return new WaitForSeconds(0.5f);
                }
                else{
                    speed = 10.0f;
                    yield return new WaitForSeconds(0.5f);
                }

            }

            yield return new WaitForSeconds(0.5f);
            
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            gameOverText.gameObject.SetActive(true);
            this.freeze();
        }
    }
    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            HealthBar.fillAmount = (float)currentHealth / (float)maxHealth;
            healthText.text = "Health: " + currentHealth.ToString()+"/100";
        }
        else
        {
            Debug.LogError("TMP Text component not assigned in the Inspector.");
        }
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collided with a pickup item
        if (collision.CompareTag("HealthUp") || collision.CompareTag("SpeedUp") || collision.CompareTag("Defrost") || collision.CompareTag("Placeholder") || collision.CompareTag("JumpHigher"))
        {

                string itemName = collision.tag;

                if (inventory.ContainsKey(itemName))
                {
                    inventory[itemName]++;
                }
                else
                {
                    inventory[itemName] = 1;
                }

                HandlePickupEffects(itemName);

                Destroy(collision.gameObject);
            }


    }

    // Function to handle the effects of collected items
    private void HandlePickupEffects(string itemName)
    {
        switch (itemName)
        {
            case "Health":
                break;
            case "Ability":
                break;
        }
    }


}
