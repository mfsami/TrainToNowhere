using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ===== UI =====
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI crowbarText;

    public Button forwardButton;
    public Button backButton;
    public Button searchButton;
    public Button eatButton;
    public Button hideButton;   // NEW
    public Button dashButton;   // NEW

    // ===== Tuning =====
    const int START_STAMINA = 10;
    const int MAX_STAMINA = 12;
    const int MOVE_COST = 1;
    const int EAT_GAIN = 3;
    const int DASH_COST = 3;
    int numLocked = 1; // fewer locks while prototyping

    // Threat tuning
    const float AMBUSH_CHANCE_NEAR = 0.15f; // if within 2 cars
    const float WAIT_CHANCE_FAR = 0.20f; // if 4+ cars away
    const float NOISE_JUMP_AHEAD = 0.20f; // after noise, 20% land 1 ahead
    const float HIDE_PASS_ADJ = 0.70f; // 70% it passes when adjacent
    const float HIDE_SKIP_FAR = 0.60f; // 60% it does nothing when far
    const float DASH_SUCCESS = 0.70f; // 70% dash past success

    // ===== State =====
    bool gameOver = false;
    int playerPos = 3;
    int playerStamina = START_STAMINA;
    int totalCars = 10;

    bool[] locked;               // door into index i is locked

    // Items
    bool hasCrowbar = false;
    int food = 0;
    bool weapon = false;

    // ===== Threat State =====
    int threatPos = 0;
    int turn = 0;
    bool madeNoise = false;       // set true by actions like crowbar pry
    bool isHiding = false;       // NEW: affects threat behavior
    bool debugShowDistance = false;

    System.Random rng = new System.Random();

    void Start()
    {
        forwardButton.onClick.AddListener(MoveForward);
        backButton.onClick.AddListener(MoveBack);
        searchButton.onClick.AddListener(Search);
        eatButton.onClick.AddListener(EatFood);
        if (hideButton) hideButton.onClick.AddListener(Hide);       // NEW
        if (dashButton) dashButton.onClick.AddListener(DashPast);   // NEW

        // Init random locked doors (don’t lock start or engine)
        locked = new bool[totalCars];
        int placed = 0;
        while (placed < numLocked)
        {
            int idx = Random.Range(1, totalCars - 1); // can lock 1..engine-1
            if (idx == playerPos || locked[idx]) continue;
            locked[idx] = true;
            placed++;
        }

        // Threat spawn: 70% behind, 30% ahead, 2–4 cars away
        bool behind = Random.value < 0.7f;
        int d = Random.Range(2, 5); // 2..4
        threatPos = Mathf.Clamp(playerPos + (behind ? -d : +d), 0, totalCars - 1);
        if (threatPos == playerPos) threatPos = Mathf.Max(0, playerPos - 1);

        UpdateUI("Game Start\n" + BuildClue());
    }

    // ---------------- Player Actions ----------------

    void MoveForward()
    {
        if (gameOver) return;

        if (playerPos >= totalCars - 1)
        {
            EndRun("You have reached the engine. ESCAPED!");
            return;
        }

        if (playerStamina <= 0)
        {
            EndTurn("Too exhausted to move. Eat to recover.");
            return;
        }

        int nextCar = playerPos + 1;

        if (locked[nextCar])
        {
            if (hasCrowbar)
            {
                hasCrowbar = false;
                locked[nextCar] = false;
                ChangeStamina(-MOVE_COST); // effort to pry
                madeNoise = true;          // crowbar is loud
                EndTurn($"You pry the lock open. (Door to Car {nextCar} now unlocked)");
                return;
            }
            else
            {
                EndTurn($"The door to Car {nextCar} is LOCKED. (Find crowbar or backtrack)");
                return;
            }
        }

        playerPos = nextCar;
        ChangeStamina(-MOVE_COST);
        EndTurn($"You moved to Car {playerPos}");
    }

    void MoveBack()
    {
        if (gameOver) return;

        if (playerPos <= 0)
        {
            EndTurn("You are at the start of the train.");
            return;
        }

        if (playerStamina <= 0)
        {
            EndTurn("Too exhausted to move. Eat to recover.");
            return;
        }

        playerPos--;
        ChangeStamina(-MOVE_COST);
        EndTurn($"You moved to Car {playerPos}");
    }

    void EatFood()
    {
        if (gameOver) return;

        if (food < 1)
        {
            EndTurn("You have no food.");
            return;
        }

        food--;
        ChangeStamina(+EAT_GAIN);
        EndTurn("You eat a ration and feel stronger.");
    }

    // Simple search: free for now
    void Search()
    {
        if (gameOver) return;

        int roll = Random.Range(1, 5); // 1..4
        string msg;

        switch (roll)
        {
            case 1:
                msg = $"You search Car {playerPos} and find... nothing.";
                break;

            case 2: // crowbar
                if (!hasCrowbar)
                {
                    hasCrowbar = true;
                    msg = $"You search Car {playerPos} and find... a crowbar!";
                }
                else
                {
                    if (Random.value < 0.5f) { food++; msg = $"Duplicate crowbar roll → you found food instead."; }
                    else { weapon = true; msg = $"Duplicate crowbar roll → you found a weapon instead."; }
                }
                break;

            case 3:
                food++;
                msg = $"You search Car {playerPos} and find... food!";
                break;

            case 4:
                weapon = true;
                msg = $"You search Car {playerPos} and find... a weapon!";
                break;

            default:
                msg = $"You search Car {playerPos}...";
                break;
        }

        EndTurn(msg);
    }

    // NEW: Hide (skip turn; chance the threat passes or skips)
    void Hide()
    {
        if (gameOver) return;

        isHiding = true;
        EndTurn("You crawl under a seat and hold your breath…");
    }

    // NEW: Dash Past (only if the threat is exactly 1 ahead)
    void DashPast()
    {
        if (gameOver) return;

        if (threatPos != playerPos + 1)
        {
            EndTurn("There’s nothing directly blocking your way to dash past.");
            return;
        }

        if (playerStamina < DASH_COST)
        {
            EndTurn("You’re too exhausted to dash.");
            return;
        }

        // Pay main dash cost now
        ChangeStamina(-DASH_COST);

        if (Random.value < DASH_SUCCESS)
        {
            // success: slip into next car and push threat forward one
            int target = Mathf.Min(playerPos + 1, totalCars - 1);
            int newThreat = Mathf.Min(target + 1, totalCars - 1);

            playerPos = target;
            threatPos = newThreat;

            EndTurn("You dash past it! It stumbles forward as you squeeze by.");
        }
        else
        {
            // small extra penalty; still end turn (threat may catch you)
            ChangeStamina(-1);
            EndTurn("You try to dash, but it blocks you!");
        }
    }

    // ---------------- Turn + Threat ----------------

    // Call at the end of *every* player action
    void EndTurn(string playerMessage)
    {
        // Threat advances
        AdvanceThreat();

        if (gameOver) return;

        // Build clue for the player after threat acts
        string clue = BuildClue();
        if (debugShowDistance)
        {
            int dist = Mathf.Abs(playerPos - threatPos);
            clue += $"  [dist={dist}]";
        }

        UpdateUI(playerMessage + "\n" + clue);
        madeNoise = false; // reset noise each turn
        isHiding = false; // reset hiding each turn
        turn++;
    }

    void AdvanceThreat()
    {
        if (gameOver) return;

        int distance = Mathf.Abs(playerPos - threatPos);

        // Hiding logic first (it can fully resolve the threat step)
        if (isHiding)
        {
            if (distance <= 1)
            {
                // Adjacent: 70% it passes by; 30% it finds you (caught)
                if (Random.value < HIDE_PASS_ADJ)
                {
                    // Passes: move to the other side of the player
                    if (threatPos <= playerPos)
                        threatPos = Mathf.Min(playerPos + 1, totalCars - 1);
                    else
                        threatPos = Mathf.Max(playerPos - 1, 0);
                    CheckCaught();
                    return;
                }
                else
                {
                    EndRun("It finds you while you hide. C A U G H T.");
                    return;
                }
            }
            else
            {
                // Far away: 60% it skips its move
                if (Random.value < HIDE_SKIP_FAR)
                {
                    CheckCaught();
                    return;
                }
                // else: fall through to normal behavior
            }
        }

        // After noise (e.g., crowbar): alert sprint 2–3 toward player
        if (madeNoise)
        {
            MoveThreatTowardPlayer(Random.Range(2, 4)); // 2 or 3
            if (Random.value < NOISE_JUMP_AHEAD)
                threatPos = Mathf.Clamp(playerPos + 1, 0, totalCars - 1);

            CheckCaught();
            return;
        }

        // Ambush if near (jump ahead and wait)
        if (distance <= 2 && Random.value < AMBUSH_CHANCE_NEAR)
        {
            threatPos = Mathf.Clamp(playerPos + 1, 0, totalCars - 1);
            CheckCaught();
            return;
        }

        // Wait if far to create uncertainty
        if (distance >= 4 && Random.value < WAIT_CHANCE_FAR)
        {
            CheckCaught();
            return; // stays put
        }

        // Patrol: move 1 toward player
        MoveThreatTowardPlayer(1);
        CheckCaught();
    }

    void MoveThreatTowardPlayer(int steps)
    {
        if (threatPos < playerPos) threatPos = Mathf.Min(threatPos + steps, totalCars - 1);
        else if (threatPos > playerPos) threatPos = Mathf.Max(threatPos - steps, 0);
    }

    void CheckCaught()
    {
        if (playerPos == threatPos)
        {
            EndRun("It finds you. C A U G H T.");
        }
    }

    string BuildClue()
    {
        int d = threatPos - playerPos; // >0 ahead, <0 behind
        if (d > 0) return "You hear slow footsteps AHEAD.";
        if (d < 0) return "Something scrapes the door BEHIND.";
        return "A nearby door rattles…";
    }

    // ---------------- Stamina / UI ----------------

    void ChangeStamina(int delta)
    {
        playerStamina = Mathf.Clamp(playerStamina + delta, 0, MAX_STAMINA);
    }

    void EndRun(string message)
    {
        gameOver = true;
        DisableAllButtons();
        UpdateUI(message);
    }

    void DisableAllButtons()
    {
        forwardButton.interactable = false;
        backButton.interactable = false;
        searchButton.interactable = false;
        eatButton.interactable = false;
        if (hideButton) hideButton.interactable = false;
        if (dashButton) dashButton.interactable = false;
    }

    void UpdateUI(string message)
    {
        infoText.text = message + $"\n(Current Car: {playerPos})";
        staminaText.text = $"Stamina: {playerStamina}";
        crowbarText.text = $"Crowbar: {(hasCrowbar ? "Yes" : "No")}";
        foodText.text = $"Food: {food}";
        weaponText.text = $"Weapon: {(weapon ? "Yes" : "No")}";

        bool exhausted = playerStamina <= 0;
        bool canAct = !gameOver;

        // Movement disabled when exhausted
        forwardButton.interactable = canAct && !exhausted;
        backButton.interactable = canAct && !exhausted;

        // Search is free in this prototype; leave enabled even when exhausted
        searchButton.interactable = canAct;

        // Eat enabled only if you have food
        eatButton.interactable = canAct && food > 0;

        // NEW: Hide is always allowed if not game over
        if (hideButton) hideButton.interactable = canAct;

        // NEW: Dash only when threat is exactly 1 ahead and you have stamina
        if (dashButton)
            dashButton.interactable = canAct && !exhausted && (threatPos == playerPos + 1) && (playerStamina >= DASH_COST);
    }
}
