using UnityEngine;
using TMPro;

public class EncounterManager : MonoBehaviour
{
    // Variables for Encounter Generation
    public GameObject encounterUI;
    private GameObject encounterScreen;
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

    //UI
    public GameObject rectProgPrefab;
    private GameObject progressBarUI;
    public TextMeshProUGUI showEncounter;
    public Sprite[] objectTypeSprite = new Sprite[5];

    //Encounter Wall and Factory Line
    private SpriteEncounter wall;
    private SpriteEncounter line;
    private EncounterObject encounterObject;
    public void Start()
    {
        encounterScreen = encounterUI.transform.GetChild(1).gameObject;
        progressBarUI = encounterUI.transform.GetChild(3).gameObject;
    }
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
        ReconnectReferences();

        if (screenActive) return;

        screenActive = true;

        encounterCount = round;
        encounters.Clear();
        generateEncounters();


        encounterScreen.transform.GetComponentInChildren<TMPro.TMP_Text>().text = screenPrompt;
        
        if (encounterCount != 0)
        {
            Instantiate(rectProgPrefab, progressBarUI.transform);

        }
        encounterUI.SetActive(true);
        showEncounter.text = "Encounters: " + encounterCount + " / " + maxEncounters;

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
        applyEncounterTypeObject(currentEncounter.objectType);

        encounterUI.SetActive(false);
            

        //change object based on rand num

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

            UnityEngine.UI.Image objectImage = card.transform.GetChild(3).GetComponent<UnityEngine.UI.Image>();//object image
            //add one image for minigame type

            if (encounter.type == UpgradeManager.EncounterType.BOSS)
            {
                //Define only one card as boss card
                if (i == 0)
                {
                    card.SetActive(true);
                    // Define health and damage for encounter
                    float curveWeight = difficultyCurve.Evaluate((float)encounterCount / maxEncounters);

                    encounter.minigameType = Encounter.MinigameType.ALL;
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

                encounter.minigameType = Encounter.MinigameType.ALL; //TODO fix
                encounter.tgtProgress = minimumHealth + (int)(difficulty * 1000 * curveWeight);
                encounter.failedPunishment = minimumDamage + (difficulty * 1000 * curveWeight);
                encounter.flavor = checkFlavor(encounter);

                encounters.Add(encounter);

                // Get Flavor data if available
                FlavorTypeImage flavorImage = flavorDefinitions[(int)encounter.flavor];

                if (flavorImage.sprite != null) image.sprite = flavorImage.sprite;
                
                if (flavorImage.name != null) title.text = flavorImage.name;
                else title.text = "Not Found";

                //title.text += "\n" + encounterCount + " / " + maxEncounters;
                //title.text += "\n" + encounter.tgtProgress + " Progress";
                title.text += "\n" + encounter.failedPunishment + " DMG taken";
                title.text += "\n" + curveWeight;

                encounter.objectType = setEncounterTypeObject(objectImage);
                //Debug.Log("card "+ i+ " was set encounterObjectType and ObjectImage " + encounter.objectType);
                //Debug.Log("ObjectImage was assigned sprite: "+objectImage.sprite + "in card " + i);

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

    private void ReconnectReferences()
    {
        if (encounterObject == null)
        {
            // FindObjectOfType (singular) only looks for active objects in the scene
            encounterObject = GameObject.Find("EncounterObject")?.GetComponent<EncounterObject>();

            // If it's still null, then try the deeper search (but be careful)
            if (encounterObject == null)
            {
                EncounterObject[] allEncounters = Resources.FindObjectsOfTypeAll<EncounterObject>();
                foreach (var s in allEncounters)
                {
                    // Only pick it if it's part of a scene (not a prefab)
                    if (s.name == "EncounterObject" && s.gameObject.scene.name != null)
                    {
                        encounterObject = s;
                        break;
                    }
                }
            }
        }
        if (wall == null || line == null)
        {
            SpriteEncounter[] allEncounters = Resources.FindObjectsOfTypeAll<SpriteEncounter>();
            foreach (var s in allEncounters)
            {
                if (s.name == "Wall") wall = s;
                if (s.name == "FactoryLine") line = s;
            }
        }
    }

    private void applyEncounterTypeObject(int setObject)
    {
        wall.randomEncounterSprite(setObject);
        line.randomEncounterSprite(setObject);

        switch (setObject)
        {
            case 0: encounterObject.changeObject(EncounterObject.Object.CANDY); break;
            case 1: encounterObject.changeObject(EncounterObject.Object.CONSOLE); break;
            case 2: encounterObject.changeObject(EncounterObject.Object.PRESENT); break;
            case 3: encounterObject.changeObject(EncounterObject.Object.SOCK); break;
            case 4: encounterObject.changeObject(EncounterObject.Object.TEDYY); break;
        }

        //Debug.Log("Wall  was set the sprite: " + setObject + wall.mySprite.sprite);
        //Debug.Log("line was set the sprite: " + setObject + line.mySprite.sprite);
        //Debug.Log("encounterObject was set the sprite: " + encounterObject.returnObjectTypeSprite());
    }

    private int setEncounterTypeObject(UnityEngine.UI.Image objectImage)
    {
        int ranNum = Random.Range(0, 4);

        switch (ranNum)
        {
            case 0: objectImage.sprite = encounterObject.returnSpecificObjectType(EncounterObject.Object.CANDY); break;
            case 1: objectImage.sprite = encounterObject.returnSpecificObjectType(EncounterObject.Object.CONSOLE); break;
            case 2: objectImage.sprite = encounterObject.returnSpecificObjectType(EncounterObject.Object.PRESENT); break;
            case 3: objectImage.sprite = encounterObject.returnSpecificObjectType(EncounterObject.Object.SOCK); break;
            case 4: objectImage.sprite = encounterObject.returnSpecificObjectType(EncounterObject.Object.TEDYY); break;
        }

        return ranNum;

    }
}
