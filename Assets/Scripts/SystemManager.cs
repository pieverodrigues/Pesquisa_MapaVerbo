using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class SystemManager : MonoBehaviour
{
    public Data data;

    [Header("Introduction")]
    public GameObject groupIntroduction;

    [Header("Macro Tag Selection")]
    public GameObject groupMacroTag;
    public TMPro.TMP_Text dateText;

    [Header("Most Or Least Seen")]
    public GameObject groupMostOrLeast;
    public TMPro.TMP_Text dateTextMOLS;
    public RectTransform contentMOLS;
    public GameObject coverSmallPrefab;
    public Button mostSeen;
    public Button leastSeen;

    [Header("Sub Tag Filter")]
    public GameObject groupSubTag;
    public GameObject coverBigPrefab;
    public TMPro.TMP_Text dateTextST;
    public RectTransform contentST;
    public Dropdown subTagsDropdown;

    private string randomDate;
    private string[] weekDays = { "DOMINGO", "SEGUNDA", "TERÇA", "QUARTA", "QUINTA", "SEXTA", "SÁBADO" };
    private string activeMacroTag;
    private int activeMacroTagID = 0;
    private List<GameObject> smallCoversMost;
    private List<GameObject> smallCoversLeast;
    private List<GameObject> bigCovers;
    private GameInfo[] randomizedMostSeen;
    private GameInfo[] randomizedLeastSeen;

    private List<string> dropDownTags;


    private Dictionary<string, List<int>> uniqueTags;


    public enum ActiveWindow
    {
        intro,
        macroTagSelection,
        mostOrLeastSeen,
        subTagFilter
    }

    private ActiveWindow activeWindow;


    private void OnValidate()
    {
        data = (Data)Resources.FindObjectsOfTypeAll(typeof(Data))[0];
    }


    // Start is called before the first frame update
    void Awake()
    {
        groupIntroduction.SetActive(true);
        groupMacroTag.SetActive(false);
        groupMostOrLeast.SetActive(false);
        groupSubTag.SetActive(false);

        smallCoversMost = new List<GameObject>();
        smallCoversLeast = new List<GameObject>();
        bigCovers = new List<GameObject>();
        uniqueTags = new Dictionary<string, List<int>>();
        dropDownTags = new List<string>();
    }


    private void Start()
    {
        SetupIntro();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    #region Intro


    void SetupIntro()
    {
        activeWindow = ActiveWindow.intro;
    }


    public void ButtonStart()
    {
        groupIntroduction.SetActive(false);
        groupMacroTag.SetActive(true);

        SetupMacroTag();
    }


    #endregion


    #region MacroTag


    void SetupMacroTag()
    {
        GenerateRandomDate();
        dateText.text = randomDate;

        activeWindow = ActiveWindow.macroTagSelection;
    }


    public void ButtonBackToIntro()
    {
        groupIntroduction.SetActive(true);
        groupMacroTag.SetActive(false);
    }


    public void ButtonLgbt()
    {
        activeMacroTag = data.macroGroups[0].tag;
        activeMacroTagID = 0;
        groupMacroTag.SetActive(false);
        groupMostOrLeast.SetActive(true);
        SetupMostOrLeast();
    }


    public void ButtonGay()
    {
        activeMacroTag = data.macroGroups[1].tag;
        activeMacroTagID = 1;
        groupMacroTag.SetActive(false);
        groupMostOrLeast.SetActive(true);
        SetupMostOrLeast();
    }


    void GenerateRandomDate()
    {
        randomDate = weekDays[Random.Range(0, weekDays.Length)] + ", " + Mathf.Floor(Random.Range(0, 25)).ToString() + ":" + Mathf.Floor(Random.Range(10, 61)).ToString();

        dateTextMOLS.text = randomDate + ", TAG " + activeMacroTag;
        dateText.text = randomDate;
    }


    #endregion


    #region MostOrLeastSeen


    void SetupMostOrLeast()
    {
        activeWindow = ActiveWindow.mostOrLeastSeen;

        dateTextMOLS.text = randomDate + ", TAG " + activeMacroTag;

        mostSeen.interactable = false;
        leastSeen.interactable = true;

        GenerateMostAndLeastSeenRandomArrays();

        SetMostSeenVisibleState(true);
        SetLeastSeenVisibleState(false);
    }


    void GenerateMostAndLeastSeenRandomArrays()
    {
        randomizedMostSeen = ShuffleArray(data.macroGroups[activeMacroTagID].morePresents);
        randomizedLeastSeen = ShuffleArray(data.macroGroups[activeMacroTagID].lessPresents);

        SetFramesInContent(randomizedMostSeen, contentMOLS, data.contentRowsMOLS, data.contentColumnsMOLS, coverSmallPrefab, smallCoversMost);
        SetFramesInContent(randomizedLeastSeen, contentMOLS, data.contentRowsMOLS, data.contentColumnsMOLS, coverSmallPrefab, smallCoversLeast);
    }


    void SetMostSeenVisibleState(bool state)
    {
        for(int i = 0; i < smallCoversMost.Count; i++)
        {
            smallCoversMost[i].SetActive(state);
        }
    }


    void SetLeastSeenVisibleState(bool state)
    {
        for (int i = 0; i < smallCoversLeast.Count; i++)
        {
            smallCoversLeast[i].SetActive(state);
        }
    }


    public void ButtonBackToMacroTagSelection()
    {
        groupMacroTag.SetActive(true);
        groupMostOrLeast.SetActive(false);
    }


    public void ButtonDisplayMostSeen()
    {
        mostSeen.interactable = false;
        leastSeen.interactable = true;

        SetMostSeenVisibleState(true);
        SetLeastSeenVisibleState(false);
    }


    public void ButtonDisplayLeastSeen()
    {
        mostSeen.interactable = true;
        leastSeen.interactable = false;

        SetMostSeenVisibleState(false);
        SetLeastSeenVisibleState(true);
    }


    public void ButtonGenerateAgain()
    {
        GenerateRandomDate();

        GenerateMostAndLeastSeenRandomArrays();

        dateTextMOLS.text = randomDate + ", TAG " + activeMacroTag;

        if (mostSeen.interactable)
        {
            SetMostSeenVisibleState(false);
            SetLeastSeenVisibleState(true);
        }
        else
        {
            SetMostSeenVisibleState(true);
            SetLeastSeenVisibleState(false);
        }

        
    }


    public void ButtonFilter()
    {
        groupMostOrLeast.SetActive(false);
        groupSubTag.SetActive(true);
        
        SetupSubTag();
    }


    #endregion


    #region SubTag


    void SetupSubTag()
    {
        activeWindow = ActiveWindow.subTagFilter;

        dateTextST.text = randomDate + ", TAG " + activeMacroTag;

        GameInfo[] activeGameInfoArray;
        if (mostSeen.interactable)
        {
            activeGameInfoArray = data.macroGroups[activeMacroTagID].lessPresents;
        }
        else
        {
            activeGameInfoArray = data.macroGroups[activeMacroTagID].morePresents;
        }
        FindUniqueTags(activeGameInfoArray);

        SetDropdown(subTagsDropdown);

        string selectedSubTag = dropDownTags[0];
        GameInfo[] gamesToShow = new GameInfo[uniqueTags[selectedSubTag].Count];

        for (int i = 0; i < gamesToShow.Length; i++)
        {
            gamesToShow[i] = activeGameInfoArray[uniqueTags[selectedSubTag][i]];
        }

        SetFramesInContent(gamesToShow, contentST, data.contentRowsST, data.contentColumnsST, coverBigPrefab, bigCovers);
    }


    void SetDropdown(Dropdown drop)
    {
        var sortedUniqueTagsByValue = uniqueTags.OrderByDescending(utags => utags.Value.Count);

        drop.ClearOptions();

        dropDownTags.Clear();

        List<Dropdown.OptionData> optList = new List<Dropdown.OptionData>();

        foreach (KeyValuePair<string, List<int>> entry in sortedUniqueTagsByValue)
        {
            Dropdown.OptionData opt = new Dropdown.OptionData();
            opt.text = entry.Value.Count + " - " + entry.Key;
            dropDownTags.Add(entry.Key);
            optList.Add(opt);
        }

        drop.AddOptions(optList);
    }


    public void ButtonBackToMostOrLeastSeen()
    {
        groupMostOrLeast.SetActive(true);
        groupSubTag.SetActive(false);
    }


    public void ButtonRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void DropdownSelectOption(Dropdown drop)
    {
        string selectedSubTag = dropDownTags[drop.value];
        GameInfo[] gamesToShow = new GameInfo[uniqueTags[selectedSubTag].Count];

        GameInfo[] activeGameInfoArray;
        if(mostSeen.interactable)
        {
            activeGameInfoArray = data.macroGroups[activeMacroTagID].lessPresents;
        }
        else
        {
            activeGameInfoArray = data.macroGroups[activeMacroTagID].morePresents;
        }

        for(int i = 0; i < gamesToShow.Length; i++)
        {
            //gamesToShow[i] = data.macroGroups[activeMacroTagID].morePresents[uniqueTags[selectedSubTag][i]];
            gamesToShow[i] = activeGameInfoArray[uniqueTags[selectedSubTag][i]];
        }

        SetFramesInContent(gamesToShow, contentST, data.contentRowsST, data.contentColumnsST, coverBigPrefab, bigCovers);
    }

    #endregion



    void SetFramesInContent(GameInfo[] games, RectTransform cont, int rows, int cols, GameObject frame, List<GameObject> coversList)
    {
        float w2 = cont.sizeDelta.x / 2;
        float h2 = cont.sizeDelta.y / 2;
        float colW = cont.sizeDelta.x / cols;
        float rowH = cont.sizeDelta.y / rows;
        float colW2 = colW / 2;
        float rowH2 = rowH / 2;
        int k = 0;


        for(int i = 0; i < coversList.Count; i++)
        {
            Destroy(coversList[i]);
        }
        coversList.Clear();


        for(int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if(k < games.Length)
                {
                    Vector2 pos;
                    pos.x = -w2 + colW2 + j * colW;
                    pos.y = h2 - rowH2 - i * rowH;

                    GameObject clone = Instantiate(frame, cont.transform);
                    clone.transform.localPosition = pos;

                    Cover cover = clone.GetComponent<Cover>();
                    cover.SetGameName(games[k].gameName);
                    cover.SetAutor(games[k].autor);
                    cover.SetGenre(games[k].gender);
                    cover.SetTags(games[k].tags);
                    cover.SetSprite(games[k].cover);

                    coversList.Add(clone);

                    k++;
                }
            }
        }
    }


    GameInfo[] ShuffleArray(GameInfo[] array)
    {
        GameInfo[] newArray = new GameInfo[array.Length];
        List<GameInfo> list = new List<GameInfo>(array);

        int j = 0;

        while(list.Count > 0)
        {
            int id = Random.Range(0, list.Count);
            newArray[j] = list[id];
            list.RemoveAt(id);

            j++;
        }

        return newArray;
    }


    void FindUniqueTags(GameInfo[] games)
    {
        uniqueTags.Clear();
        //data.macroGroups[activeMacroTagID].morePresents

        for (int i = 0; i < games.Length; i++)
        {
            string[] coverTags = games[i].GetTagsArray();

            for(int j = 0; j < coverTags.Length; j++)
            {
                if(uniqueTags.ContainsKey(coverTags[j]))
                {
                    uniqueTags[coverTags[j]].Add(i);
                }
                else
                {
                    uniqueTags.Add(coverTags[j], new List<int>());
                    uniqueTags[coverTags[j]].Add(i);
                }
            }
        }

        foreach (KeyValuePair<string, List<int>> entry in uniqueTags)
        {
            Debug.Log(entry.Key + ", " + entry.Value.Count);
        }
    }
}
