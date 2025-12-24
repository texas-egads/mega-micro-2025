using UnityEngine;


public class EncounterManager : MonoBehaviour
{
    // Variables for Encounter Generation
    public GameObject encounterScreen;
    public string screenPrompt = "Choose a factory:";
    public AnimationCurve difficultyCurve;
    public int encounterCount = 0;
    public int maxEncounters = 15;
    public float eliteChance = 0.2f;
    public int[] guaranteedEliteOn = new int[] { 5, 10 };
    public int minimumHealth = 25;
    public int minimumDamage = 30;
    public FlavorTypeImage[] flavorDefinitions = new FlavorTypeImage[3];
    private int cardCount = 2;
    private bool screenActive = false;
    private System.Collections.Generic.List<Encounter> encounters = new System.Collections.Generic.List<Encounter>();
    private Encounter currentEncounter;
    private float difficulty
    {
        get
        {
            return Managers.__instance.minigamesManager.GetCurrentMinigameDifficulty();
        }
    }
    public enum Flavors
    {
        NORMAL,
        TANK,
        CANNON
    }

    [System.Serializable]
    public struct FlavorTypeImage
    {
        public Flavors flavor;
        public string name;
        public Sprite sprite;
    }

    // Elite percentage functions
    public void SetEliteChance(float chance)
    {
        eliteChance = chance;
    }
    public float GetEliteChance()
    {
        return eliteChance;
    }

    public void StartEncounterChoicer(int round, System.Action<Encounter> onEncounterSelected)
    {
        if (screenActive) return;

        screenActive = true;

        encounterCount = round;
        encounters.Clear();
        generateEncounters();


        encounterScreen.transform.GetComponentInChildren<TMPro.TMP_Text>().text = screenPrompt;
        encounterScreen.SetActive(true);

        StartCoroutine(HandleEncounterChoice(onEncounterSelected));
    }

    private System.Collections.IEnumerator HandleEncounterChoice(System.Action<Encounter> onEncounterSelected)
    {
        // Wait a frame
        yield return null;

        EncounterCard card1 = encounterScreen.transform.GetChild(1).GetChild(0).GetComponent<EncounterCard>();
        EncounterCard card2 = encounterScreen.transform.GetChild(1).GetChild(1).GetComponent<EncounterCard>();

        card1.ResetSelection();
        card2.ResetSelection();

        yield return new WaitUntil(() => card1.IsCardSelected || card2.IsCardSelected);

        int choice = card1.IsCardSelected ? 0 : 1;

        if (choice < 0 || choice >= encounters.Count)
        {
            Debug.LogError($"Invalid encounter choice index {choice}. Encounters.Count={encounters.Count}");
            choice = Mathf.Clamp(choice, 0, Mathf.Max(0, encounters.Count - 1));
        }

        currentEncounter = encounters[choice];

        encounterScreen.SetActive(false);
        screenActive = false;

        onEncounterSelected?.Invoke(currentEncounter);
    }

    private void generateEncounters()
    {
        for (int i = 0; i < cardCount; i++)
        {
            Encounter encounter = new Encounter();
            encounter.type = decideEncounterType();

            // Define card for encounter
            GameObject card = encounterScreen.transform.GetChild(1).GetChild(i).gameObject;
            UnityEngine.UI.Image image = card.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
            TMPro.TMP_Text title = card.transform.GetChild(1).GetComponent<TMPro.TMP_Text>();
            UnityEngine.UI.Image eliteImage = card.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>();

            if (encounter.type == UpgradeManager.EncounterType.BOSS)
            {
                //Define only one card as boss card
                if (i == 0)
                {
                    card.SetActive(true);
                    // Define health and damage for encounter
                    float curveWeight = difficultyCurve.Evaluate((float)encounterCount / maxEncounters);

                    encounter.tgtProgress = minimumHealth + (int)(difficulty * 1000 * curveWeight);
                    encounter.failedPunishment = minimumDamage + (difficulty * 1000 * curveWeight);
                    encounter.flavor = checkFlavor(encounter);

                    title.text = "BOSS";

                    title.text += "\n" + encounterCount + " / " + maxEncounters;
                    title.text += "\n" + encounter.tgtProgress + " Progress";
                    title.text += "\n" + encounter.failedPunishment + " DMG taken";
                    title.text += "\n" + curveWeight;

                    encounters.Add(encounter);
                    // Do boss stuff 
                }
                else card.SetActive(false);
            }
            else
            {
                // Define health and damage for encounter
                float curveWeight = difficultyCurve.Evaluate((float)encounterCount / maxEncounters);

                encounter.tgtProgress = minimumHealth + (int)(difficulty * 1000 * curveWeight);
                encounter.failedPunishment = minimumDamage + (difficulty * 1000 * curveWeight);
                encounter.flavor = checkFlavor(encounter);

                encounters.Add(encounter);

                // Get Flavor data if available
                FlavorTypeImage flavorImage = flavorDefinitions[(int)encounter.flavor];

                if (flavorImage.sprite != null) image.sprite = flavorImage.sprite;
                if (flavorImage.name != null) title.text = flavorImage.name;
                else title.text = "Not Found";

                title.text += "\n" + encounterCount + " / " + maxEncounters;
                title.text += "\n" + encounter.tgtProgress + " Progress";
                title.text += "\n" + encounter.failedPunishment + " DMG taken";
                title.text += "\n" + curveWeight;

                if (encounter.type == UpgradeManager.EncounterType.ELITE)
                {
                    eliteImage.gameObject.SetActive(true);
                }
                else
                {
                    eliteImage.gameObject.SetActive(false);
                }

                card.SetActive(true);
            }

        }
    }

    private Flavors checkFlavor(Encounter encounter)
    {
        if (encounter.tgtProgress > 100)
        {
            return Flavors.TANK;
        }
        else if (encounter.failedPunishment > 100)
        {
            return Flavors.CANNON;
        }
        else
        {
            return Flavors.NORMAL;
        }
    }

    private UpgradeManager.EncounterType decideEncounterType()
    {
        int chance = Random.Range(0, 100); // Used to set percentages 

        // Check if final Boss next
        if (encounterCount == maxEncounters)
        {
            Debug.Log("BOSS");
            return UpgradeManager.EncounterType.BOSS;
        }
        // Check if encounter is guaranteed Elite
        for (int i = 0; i < guaranteedEliteOn.Length; i++)
        {
            if (encounterCount == guaranteedEliteOn[i])
            {
                Debug.Log("GUARANTEED ELITE");
                return UpgradeManager.EncounterType.ELITE;
            }
        }
        // Check Elite percentage
        if (chance < eliteChance * 100)
        {
            Debug.Log("ELITE");
            return UpgradeManager.EncounterType.ELITE;
        }
        // Default return Normal
        Debug.Log("NORMAL");
        return UpgradeManager.EncounterType.NORMAL;
    }
}
