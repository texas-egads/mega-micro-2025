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
    public EncounterType[] typeDefinitions;
    private int cardCount = 2;
    private bool screenActive = false;
    private System.Collections.Generic.List<Encounter> encounters = new System.Collections.Generic.List<Encounter>();
    private Encounter currentEncounter;

    //UI
    public GameObject rectProgPrefab;
    private GameObject progressBarUI;
    public TextMeshProUGUI showEncounter;
    public TextMeshProUGUI currentDifficulty;
    public Sprite[] objectTypeSprite = new Sprite[5];
    [SerializeField] private GameObject minigameCanvas;

    //Encounter Wall and Factory Line
    public SpriteEncounter wall;
    public SpriteEncounter line;
    public EncounterObject encounterObject;
    public Sprite bossIcon;

    [System.Serializable]
    public struct EncounterType
    {
        public string flavor;
        public float healthScalar;
        public float damageScalar;
        public AudioClip winSound;
    }

    public void Initialize()
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
        if (currentEncounter != null)
        {
            applyEncounterTypeObject(currentEncounter.objectType);//assign previous factory assets
        }
        encounterCount = round;
        encounters.Clear();
        generateEncounters();


        encounterScreen.transform.GetComponentInChildren<TMPro.TMP_Text>().text = screenPrompt;

        if (encounterCount != 0)
        {
            Instantiate(rectProgPrefab, progressBarUI.transform);

        }
        encounterUI.SetActive(true);
        minigameCanvas.SetActive(false);
        showEncounter.text = "Encounters: " + encounterCount + " / " + maxEncounters;
        int difficultyText = Mathf.RoundToInt(difficulty * 100);
        currentDifficulty.text = $"Difficulty: {difficultyText}%";


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
        minigameCanvas.SetActive(true);

        screenActive = false;

        onEncounterSelected?.Invoke(currentEncounter);
    }

    private void generateEncounters()
    {
        var objectOptions = new System.Collections.Generic.List<int> { 0, 1, 2, 3, 4 };
        for (int i = 0; i < cardCount; i++)
        {
            Encounter encounter = new Encounter();
            encounter.type = decideEncounterType();

            // Define card for encounter
            GameObject card = encounterScreen.transform.GetChild(1).GetChild(i).gameObject;
            UnityEngine.UI.Image image = card.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
            TMPro.TMP_Text title = card.transform.GetChild(1).GetComponent<TMPro.TMP_Text>();
            TMPro.TMP_Text typeMinigame = card.transform.GetChild(2).GetComponent<TMPro.TMP_Text>();
            UnityEngine.UI.Image objectImage = card.transform.GetChild(3).GetComponent<UnityEngine.UI.Image>();//object image

            UnityEngine.UI.Image eliteImage = card.transform.GetChild(4).GetComponent<UnityEngine.UI.Image>();
            MiniGameImageType typeMinigameImage = card.transform.GetChild(5).GetComponent<MiniGameImageType>();//object image
            TMPro.TMP_Text typeStats = card.transform.GetChild(6).GetComponent<TMPro.TMP_Text>();

            //add one image for minigame type
            int listIndex = Random.Range(0, objectOptions.Count);
            int chosenType = objectOptions[listIndex];
            int lastType = -1;
            objectOptions.RemoveAt(listIndex);

            if (encounter.type == UpgradeManager.EncounterType.BOSS)
            {
                //Define only one card as boss card
                if (i == 0)
                {
                    card.SetActive(true);
                    // Define health and damage for encounter
                    float curveWeight = difficultyCurve.Evaluate((float)encounterCount / (maxEncounters-1));

                    encounter.minigameType = Encounter.MinigameType.ALL;
                    encounter.tgtProgress = 200;
                    encounter.failedPunishment = 80;
                    encounter.winSound = typeDefinitions[chosenType].winSound;
                    typeStats.text = "JOLLY";

                    title.text = "???";

                    encounter.objectType = chosenType;
                    objectImage.sprite = bossIcon;
                    typeMinigame.text = encounter.minigameType.ToString();
                    typeMinigameImage.setSprite(encounter.minigameType);

                    encounters.Add(encounter);
                }
                else card.SetActive(false);
            }
            else
            {
                // Define health and damage for encounter
                float curveWeight = difficultyCurve.Evaluate((float)encounterCount / (maxEncounters - 1));

                int randGame = lastType == -1 ? Random.Range(0, 5) : (lastType + Random.Range(1, 5)) % 5;
                lastType = randGame;
                encounter.minigameType = (Encounter.MinigameType)randGame;
                encounter.tgtProgress = (int)((minimumHealth + 150 * curveWeight) * typeDefinitions[chosenType].healthScalar);
                encounter.failedPunishment = (minimumDamage + 120 * curveWeight) * typeDefinitions[chosenType].damageScalar;
                encounter.winSound = typeDefinitions[chosenType].winSound;

                encounters.Add(encounter);


                encounter.objectType = chosenType;
                objectImage.sprite = encounterObject.returnSpecificObjectTypeSprite(numtoObjectType(chosenType));
                title.text = numtoObjectType(chosenType).ToString();
                typeMinigame.text = encounter.minigameType.ToString();
                typeMinigameImage.setSprite(encounter.minigameType);
                typeStats.text = typeDefinitions[chosenType].flavor;

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

    private UpgradeManager.EncounterType decideEncounterType()
    {
        int chance = Random.Range(0, 100); // Used to set percentages 

        // Check if final Boss next
        if (encounterCount == maxEncounters - 1)
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
            //Debug.Log("ELITE");
            return UpgradeManager.EncounterType.ELITE;
        }
        // Default return Normal
        //Debug.Log("NORMAL");
        return UpgradeManager.EncounterType.NORMAL;
    }

    private void applyEncounterTypeObject(int setObject)
    {
        if (encounterObject == null) encounterObject = GameObject.Find("EncounterObject")?.GetComponent<EncounterObject>();
        if (line == null) line = GameObject.Find("FactoryLine")?.GetComponent<SpriteEncounter>();
        if (wall == null) wall = GameObject.Find("Wall")?.GetComponent<SpriteEncounter>();
        wall.setEncounterSprite(setObject);
        line.setEncounterSprite(setObject);
        encounterObject.changeObject(numtoObjectType(setObject));
    }

    private EncounterObject.Object numtoObjectType(int objectNum)
    {
        switch (objectNum)
        {
            case 0: return EncounterObject.Object.CANDY;
            case 1: return EncounterObject.Object.CONSOLE;
            case 2: return EncounterObject.Object.PRESENT;
            case 3: return EncounterObject.Object.SOCK;
            case 4: return EncounterObject.Object.TEDDY;
            default:
                return EncounterObject.Object.CANDY;
        }
    }
}
