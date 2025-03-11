using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;


public class CreteUnit : MonoBehaviour
{
    // Start is called before the first frame update

    public List<Cell> Cells { get; private set; }
    public List<Unit> Units { get; private set; }
  
    [SerializeField] private  CellGrid cellGrid;
    private GameObject ManHeroPrefab;
    private GameObject AiHeroPrefab;
    [SerializeField] private Transform UnitsParent;
    private int playerNumb;
    public GameObject oneHeroPlayer;
    public GameObject twoHeroPlayer;
    private int positionHeroHuman = 2;
    private int positionUnit1 = 5;
    private int positionUnit2 = 6;
    private int positionUnit3 = 12;
    private int positionUnit4 = 8;
    private int positionUnit5 = 9;
    private int positionHeroAi = 47;
    private int positionUnitAi1=40;
    private int positionUnitAi2 = 36;
    private int positionUnitAi3 = 37;
    private int positionUnitAi4=38;
    private int positionUnitAi5=44;
    private int positionGrid;
    private string LoadArmyHeroURL = "http://AAAMyHeroes.com/load_heroes_army.php";
    private List<Cell> allCells;
    [SerializeField] private string User;
    [SerializeField] private List<DB_Army> UnitArmy;
    [SerializeField] private List<Player> Players;
    public bool isPlaying;
    public bool isLoadBatal;
    private LogAutoBatttal logBattal;

    private int amount;
    private int playerNum;
    private Vector3 position;
    private Vector3 position_tar;
    private List<Vector3> path_move = new List<Vector3>();
    private List<Vector3> path_move_buf = new List<Vector3>();
    private List<string> list_targget_unit = new List<string>();
    private float speed;
    private int damagemin;
    private int damagemax;
    private int attack;
    private int defence;
    private int hitp;
    private int range_attack;
    private int range_move;
    private GameObject carentunit;
    private string hash_code;
    private string hash_code_a;
    private int step;
    private int migth;
    private Unit hero_spell;
    private bool add_action;

    private string pathFile;
    private SimpleJSON.JSONNode jsonObject;
    private string UserString;
    private Unit unit_carent;
    private Unit unit_carent_a;
    [SerializeField] private List<Camera> camers;
    [SerializeField] private GameObject terrian;
    [SerializeField] private GameObject diactivCam;
    [SerializeField] private GameObject diactivCamUI;
    private List<DB_Army> ListArm;
    [SerializeField] private StartScene startScene;
    [SerializeField] private GameObject panelEndGame;

    public void StartBatal()
    {
        logBattal = gameObject.AddComponent<LogAutoBatttal>();
        TBS_Settings.logAutoBatttal = logBattal;
        isPlaying = startScene.player_param.GetComponent<ParamPlayer>().isBattal;
        if (isPlaying == false)
        {
            cellGrid.isAutoBattal = true;
        }
        else
        {
            WordCameraOff();
        }
        isLoadBatal = startScene.player_param.GetComponent<ParamPlayer>().isLoadBattal;
        TBS_Settings.camBat = diactivCam;
        cellGrid.InitializeAndStart();
        Cells = new List<Cell>();
        for (int i = 0; i < cellGrid.transform.childCount; i++)
        {
            var Ncell = cellGrid.transform.GetChild(i).gameObject.GetComponent<Cell>();
            if (Ncell != null)
            {
                if (Ncell.gameObject.activeInHierarchy)
                {
                    Cells.Add(Ncell);
                }
            }
        }
        
        LoadUnitBattal(isPlaying);
    }
    void WordCameraOff()
    {
        startScene.player_param.GetComponent<ParamPlayer>().camera_word.gameObject.SetActive(false);
    }
    public void LoadUnitBattal(bool isPlaying)
    {
        PanelOff();
        SetingFoSecBattal(cellGrid);
        
        TBS_Settings.fireBatle = true;
        TBS_Settings.isBlockMouse = true;
        if (diactivCam.active != true)
        {
            diactivCam.SetActive(true);
            diactivCamUI.SetActive(true);
        }
        SetingAutoBattalPlayer(isPlaying);
        
        
        if (isLoadBatal == false)
        {
            CreateHeroes_Human_AI();
            LoadArmyHero();
            
        }
        else
        {
            cellGrid.loadFileBatal = true;
            OpenBattale();

        }
    }
    void CreateHeroes_Human_AI()
    {
        logBattal.SaveFileName();
        cellGrid.loadFileBatal = false;
        var findHero = UnitArmy.Find(u => u.Name == startScene.player_param.GetComponent<ParamPlayer>().heroesCarentName);
        if (findHero)
            ManHeroPrefab = findHero.Unit_Prefab;
        CreateHeroPleyer(0, positionHeroHuman, ManHeroPrefab);
        ManHeroPrefab.GetComponent<SpellCastingAbility>().InitializeSpell(cellGrid);
        SceneTransition.ArmyList.RemoveAll(h => h.Item1 == ManHeroPrefab);
        if (SceneTransition.listHeroPref.Count > 0)
        {
            AiHeroPrefab = SceneTransition.listHeroPref[0].Hero_Prefab;
        }
        else
        {
            AiHeroPrefab = SceneTransition.heroPref;
        }
        CreateHeroPleyer(1, positionHeroAi, AiHeroPrefab);
        AiHeroPrefab.GetComponent<SpellCastingAbility>().InitializeSpellAI(cellGrid);
        if (SceneTransition.listHeroPref.Count > 0)
            SceneTransition.listHeroPref.Remove(SceneTransition.listHeroPref[0]);
    }
    void SetingAutoBattalPlayer(bool isPlaying)
    {
        if (isPlaying == false)
        {
            Players[0].gameObject.AddComponent<AIPlayer>();
            Players[0].gameObject.AddComponent<MovementFreedomUnitSelection>();
            Destroy(Players[0].GetComponent<HumanPlayer>());
            Players.Clear();
            Destroy(terrian.gameObject);
            foreach (Camera cam in camers)
            {
                cam.gameObject.SetActive(false);
            }
        }
        foreach (var player in cellGrid.Players)
        {
            Players.Add(player);
        }
    }
    void PanelOff()
    {
        panelEndGame.SetActive(false);
        panelEndGame.transform.GetChild(1).gameObject.SetActive(false);
        panelEndGame.transform.GetChild(3).gameObject.SetActive(false);
    }
    void SetingFoSecBattal(CellGrid cellGrid)
    {
        if (cellGrid.Units.Count > 0)
        {

            cellGrid.TwoBattal();
            while (cellGrid.Units.Count > 0)
            {

                Destroy(cellGrid.Units[cellGrid.Units.Count - 1].gameObject);
                cellGrid.Units[cellGrid.Units.Count - 1].Cell.IsTaken = false;

                cellGrid.Units.Remove(cellGrid.Units[cellGrid.Units.Count - 1]);
            }
            List<Cell> freeCells1 = Cells.FindAll(h => h.GetComponent<Cell>().IsTaken == true);

        }
    }
    public void CreateEnamyUnit(string NameUnit, int positionAnamy, int amount)
    {
        var nUnit = UnitArmy.Find(unit => unit.Name == NameUnit);
        var unit = Instantiate(nUnit.Unit_Prefab);

        var cell = allCells.ElementAt(positionAnamy);
        cell.GetComponent<Cell>().IsTaken = true;
        unit.transform.position = cell.transform.position;
        unit.GetComponent<Unit>().PlayerNumber = 1;
        unit.GetComponent<Unit>().unit_object = nUnit.Unit_Prefab;
        unit.GetComponent<Unit>().migth = nUnit.Migth;
        unit.GetComponent<Speed>().Value = nUnit.Speed;
        //==========================================================
        unit.GetComponent<Unit>().hashCode = Guid.NewGuid().ToString();
        //=======================================================
        if (isPlaying == false)
        {
            unit.GetComponent<Unit>().MovementAnimationSpeed = 15000;
            Destroy(unit.transform.GetChild(5).gameObject);
        }
        //=======================================================

        var amountAnamy = amount;
        unit.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = amountAnamy.ToString();
        unit.GetComponent<Unit>().Cell = cell.GetComponent<Cell>();
        unit.GetComponent<Unit>().heroesUnit = twoHeroPlayer;

        cellGrid.AddUnit(unit.transform);
        unit.transform.parent = UnitsParent;
        logBattal.SaveBatalLog(unit, cell, "create");
    }

    public void CreateHeroPleyer(int playerNumb_, int positionHeroHuman1, GameObject HeroPrefab)
    {
        allCells = Cells.FindAll(h => h.GetComponent<Cell>());
        List<Cell> freeCells = Cells.FindAll(h => h.GetComponent<Cell>().IsTaken == false);
        var cell = allCells.ElementAt(positionHeroHuman1);
        if (cell.IsTaken == false)
        {

            cell.GetComponent<Cell>().IsTaken = true;
            var unit = Instantiate(HeroPrefab);

            unit.transform.position = cell.transform.position;
            unit.GetComponent<Unit>().PlayerNumber = playerNumb_;
            unit.GetComponent<Unit>().unit_object = HeroPrefab;
            unit.GetComponent<Unit>().Cell = cell.GetComponent<Cell>();
            //====================================
            unit.GetComponent<Unit>().hashCode = Guid.NewGuid().ToString();
            //=================================
            if (isPlaying == false)
            {
                unit.transform.GetChild(5).gameObject.SetActive(false);

                Destroy(unit.transform.GetChild(5).gameObject);
            }

            unit.transform.parent = UnitsParent;

            cellGrid.AddUnit(unit.transform);
            if (playerNumb_ == 0)
            {
                oneHeroPlayer = unit;

            }
            else
            {
                twoHeroPlayer = unit;
            }
            var animationBuf = unit.AddComponent<BeginBatalBuff>();
            //=========
            if (isPlaying == true)
            {
                animationBuf.StartHeroBuf(HeroPrefab);
            }
            logBattal.SaveBatalLog(unit, cell, "create");
        }
    }
    public void LoadArmyHero()
    {
        
        WWWForm form = new WWWForm();
        User = startScene.player_param.GetComponent<ParamPlayer>().playerCastel.GetUserName();
        form.AddField("nameuser", User);
        string nameHiro = startScene.player_param.GetComponent<ParamPlayer>().heroesCarentName;
        form.AddField("namehiro", nameHiro);
        WWW www_arm = new WWW(LoadArmyHeroURL, form);
        StartCoroutine(ArmyCorutineLoad(www_arm));
    }
    public IEnumerator ArmyCorutineLoad(WWW www_arm)
    {

        yield return www_arm;
        if (www_arm.error == null)

        {

            var String_JSON = "{" + User + ":" + "[" + www_arm.text + "]" + "}";
            var jsonObject = SimpleJSON.JSON.Parse(String_JSON);
            string UserString = User.ToString();

            int countArrey = jsonObject[UserString].Count;

            for (int i = 0; i < countArrey; i++)
            {
                SimpleJSON.JSONNode nameObject = jsonObject[UserString][i]["name_unit"];
                int valueObject = jsonObject[UserString][i]["amount"];
                int position = jsonObject[UserString][i]["position"];
                int damagemin = jsonObject[UserString][i]["demagemin"];
                int damagemax = jsonObject[UserString][i]["demagemax"];
                int attack = jsonObject[UserString][i]["attack"];
                int defence = jsonObject[UserString][i]["defence"];
                int hp = jsonObject[UserString][i]["hitp"];
                int speed = jsonObject[UserString][i]["speed"];
                int range_atack = jsonObject[UserString][i]["range_attack"];
                int range_move = jsonObject[UserString][i]["range_move"];
                int migth = jsonObject[UserString][i]["might"];

                if (valueObject > 0)
                {
                    if ((nameObject != null) && (nameObject != "DBA_Empty"))
                    {
                        if (position == 1)
                        {
                            positionGrid = positionUnit1;
                        }
                        if (position == 2)
                        {
                            positionGrid = positionUnit2;
                        }
                        if (position == 3)
                        {
                            positionGrid = positionUnit3;
                        }
                        if (position == 4)
                        {
                            positionGrid = positionUnit4;
                        }
                        if (position == 5)
                        {
                            positionGrid = positionUnit5;
                        }

                        var positinCell = allCells.ElementAt(positionGrid);
                        positinCell.GetComponent<Cell>().IsTaken = true;
                        var nUnit = UnitArmy.Find(unit => unit.Name == nameObject);
                        var unit = Instantiate(nUnit.Unit_Prefab);
                        unit.GetComponent<Speed>().Value = speed;
                        unit.GetComponent<Unit>().amount = valueObject;
                        unit.GetComponent<Unit>().position = position;
                        unit.transform.position = positinCell.transform.position; //+ new Vector3(0, -0.4f, 0);
                        unit.GetComponent<Unit>().hashCode = Guid.NewGuid().ToString();
                        //====================================================
                        if (isPlaying == false)
                        {
                            unit.GetComponent<Unit>().MovementAnimationSpeed = 15000;
                            Destroy(unit.transform.GetChild(7).gameObject);
                            Destroy(unit.transform.GetChild(6).gameObject);
                            Destroy(unit.transform.GetChild(5).gameObject);
                            Destroy(unit.transform.GetChild(3).transform.GetChild(0).gameObject);

                        }
                        //====================================================
                        unit.GetComponent<Unit>().PlayerNumber = playerNumb;
                        unit.GetComponent<Unit>().unit_object = nUnit.Unit_Prefab;
                        unit.GetComponent<Unit>().Cell = positinCell.GetComponent<Cell>();
                        unit.GetComponent<Unit>().damage_min = damagemin;
                        unit.GetComponent<Unit>().damage_max = damagemax;
                        unit.GetComponent<Unit>().AttackFactor = attack;
                        unit.GetComponent<Unit>().DefenceFactor = defence;
                        unit.GetComponent<Unit>().HitPoints = hp;
                        unit.GetComponent<Unit>().AttackRange = range_atack;
                        unit.GetComponent<Unit>().MovementPoints = range_move;
                        unit.GetComponent<Unit>().migth = migth;

                        unit.transform.GetChild(0).gameObject.SetActive(true);
                        unit.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = valueObject.ToString();
                        unit.transform.parent = UnitsParent;
                        SceneTransition.ArmyList.Add(new Tuple<GameObject, DB_Army, int, int>(ManHeroPrefab, nUnit, valueObject, position));

                        if (playerNumb == 0)
                        {
                            unit.GetComponent<Unit>().heroesUnit = oneHeroPlayer;
                        }
                        else
                        {
                            unit.GetComponent<Unit>().heroesUnit = twoHeroPlayer;
                        }
                        cellGrid.AddUnit(unit.transform);
                        logBattal.SaveBatalLog(unit, positinCell, "create");
                    }


                }

            }
            www_arm.Dispose();

            //=============/hend create anamy army/
            if (isLoadBatal == false)
            {
                LoadEnamyArm();
            }

            cellGrid.StartGame();

        }
        else
        {
            Debug.Log("Error: " + www_arm.error);
        }
    }
    public void LoadEnamyArm()
    {
        List<ArmyMob> list_mob_unit;
        if (SceneTransition.listMob.Count > 0)
        {
            list_mob_unit = SceneTransition.listMob[0].Mob_list;
        }
        else
        {
            list_mob_unit = SceneTransition.collider_mob;
        }
        int position_mob = 0;
        foreach (var mob in list_mob_unit)
        {
            switch (mob.position)
            {
                case 1:
                    position_mob = positionUnitAi2;
                    break;
                case 2:
                    position_mob = positionUnitAi3;
                    break;
                case 3:
                    position_mob = positionUnitAi1;
                    break;
                case 4:
                    position_mob = positionUnitAi4;
                    break;
                case 5:
                    position_mob = positionUnitAi5;
                    break;
            }

            CreateEnamyUnit(mob.army.name, position_mob, mob.amount);
        }

        if (SceneTransition.listMob.Count > 0)
            SceneTransition.listMob.Remove(SceneTransition.listMob[0]);

    }

    public void OpenBattale()
    {
        pathFile = TBS_Settings.pathFile;
        var String_ = File.ReadAllText(pathFile);
        User = "Load";
        var String_JSON = "{" + User + ":" + "[" + String_ + "]" + "}";
        jsonObject = SimpleJSON.JSON.Parse(String_JSON);
        UserString = User.ToString();
        int countArrey = jsonObject[UserString].Count;
        for (int i = 0; i < countArrey; i++)
        {
            SimpleJSON.JSONNode Command = jsonObject[UserString][i]["command"];
            if (Command == "StartGame")
            {
                step = i;
                cellGrid.StartGame();
                break;
            }
            SimpleJSON.JSONNode Unit_name = jsonObject[UserString][i]["UnitName"];
            amount = jsonObject[UserString][i]["amount"];
            if (jsonObject[UserString][i]["position"].ToString() != "[]" && jsonObject[UserString][i]["position"] != null)
            {
                var str_targ_pos = jsonObject[UserString][i]["position"].ToString();
                str_targ_pos = str_targ_pos.Trim(new char[] { '[', ']' });
                position_tar = ConvertFromString(str_targ_pos);
            }
            var str_position = jsonObject[UserString][i]["startCellPos"].ToString();
            str_position = str_position.Trim(new char[] { '[', ']' });
            position = ConvertFromString(str_position);
            speed = jsonObject[UserString][i]["speed"];
            playerNumb = jsonObject[UserString][i]["PlayerNumber"];
            damagemin = jsonObject[UserString][i]["damage_min"];
            damagemax = jsonObject[UserString][i]["damage_max"];
            attack = jsonObject[UserString][i]["attack"];
            defence = jsonObject[UserString][i]["defence"];
            hitp = jsonObject[UserString][i]["hitp"];
            range_attack = jsonObject[UserString][i]["range_attack"];
            range_move = jsonObject[UserString][i]["range_move"];
            add_action = jsonObject[UserString][i]["Addaction"];
            hash_code = jsonObject[UserString][i]["HashCode"];
            migth = jsonObject[UserString][i]["migth"];
            if (jsonObject[UserString][i]["path"].ToString() != "[]" && jsonObject[UserString][i]["path"] != null)
            {
                var str_path = jsonObject[UserString][i]["path"].ToString();
                str_path = str_path.Trim(new char[] { '[', ']' });

                var list_point_move_str = str_path.Split(']').Select(s => s.Trim()).ToArray();
                for (int j = 0; j < list_point_move_str.Length; j++)
                {
                    list_point_move_str[j] = list_point_move_str[j].Trim(new char[] { ',', '[' });

                    path_move.Add(ConvertFromString(list_point_move_str[j]));
                }
            }
            if (Command == "create")
            {
                CreateUnit(Unit_name, position);
            }
            if (Command == "Buf")
            {
                string buff_name = jsonObject[UserString][i]["BufF"];
                string str_celltarg = jsonObject[UserString][i]["celltarget"].ToString();
                string hash_code_spown = jsonObject[UserString][i]["hash_code"];
                str_celltarg = str_celltarg.Trim(new char[] { '[', ']' });
                var list_point_move_str = str_celltarg.Split(']').Select(s => s.Trim()).ToArray();
                for (int j = 0; j < list_point_move_str.Length; j++)
                {
                    list_point_move_str[j] = list_point_move_str[j].Trim(new char[] { ',', '[' });

                    path_move_buf.Add(ConvertFromString(list_point_move_str[j]));
                }
                var nUnit = UnitArmy.Find(unit => unit.Unit_Prefab.name == Unit_name);
                cellGrid.StartBufLoad(nUnit.Unit_Prefab, buff_name, path_move_buf, hash_code_spown);
                path_move_buf.Clear();
            }

        }
        RunCommand();


    }
    public void RunCommand()
    {
        step = step + 1;
        Vector3 oldPosition_Spell;
        SimpleJSON.JSONNode Command = jsonObject[UserString][step]["command"];
        if (Command != null)
        {
            SimpleJSON.JSONNode Unit_name = jsonObject[UserString][step]["UnitName"];
            amount = jsonObject[UserString][step]["amount"];
            if (jsonObject[UserString][step]["position"].ToString() != "[]" && jsonObject[UserString][step]["position"] != null)
            {
                var str_targ_pos = jsonObject[UserString][step]["position"].ToString();
                str_targ_pos = str_targ_pos.Trim(new char[] { '[', ']' });
                position_tar = ConvertFromString(str_targ_pos);
            }
            var str_position = jsonObject[UserString][step]["startCellPos"].ToString();
            str_position = str_position.Trim(new char[] { '[', ']' });
            position = ConvertFromString(str_position);
            speed = jsonObject[UserString][step]["speed"];
            playerNumb = jsonObject[UserString][step]["PlayerNumber"];
            damagemin = jsonObject[UserString][step]["damage_min"];
            damagemax = jsonObject[UserString][step]["damage_max"];
            attack = jsonObject[UserString][step]["attack"];
            defence = jsonObject[UserString][step]["defence"];
            hitp = jsonObject[UserString][step]["hitp"];
            range_attack = jsonObject[UserString][step]["range_attack"];
            range_move = jsonObject[UserString][step]["range_move"];
            add_action = jsonObject[UserString][step]["Addaction"];
            //==================================
            hash_code_a = jsonObject[UserString][step]["HashCode_att"];
            hash_code = jsonObject[UserString][step]["HashCode"];
            migth = jsonObject[UserString][step]["migth"];
            path_move.Clear();
            unit_carent = cellGrid.LoadUnit.Find(u => u.hashCode == hash_code);
            if (unit_carent != null)
                unit_carent.addAction = add_action;
            if (jsonObject[UserString][step]["path"].ToString() != "[]" && jsonObject[UserString][step]["path"] != null)
            {
                var str_path = jsonObject[UserString][step]["path"].ToString();
                str_path = str_path.Trim(new char[] { '[', ']' });
                var list_point_move_str = str_path.Split(']').Select(s => s.Trim()).ToArray();
                for (int j = 0; j < list_point_move_str.Length; j++)
                {
                    list_point_move_str[j] = list_point_move_str[j].Trim(new char[] { ',', '[' });
                    path_move.Add(ConvertFromString(list_point_move_str[j]));
                }

            }
            if (Command == "Buf")
            {
                string buff_name = jsonObject[UserString][step]["BufF"];
                string str_celltarg = jsonObject[UserString][step]["celltarget"].ToString();
                str_celltarg = str_celltarg.Trim(new char[] { '[', ']' });
                var list_point_move_str = str_celltarg.Split(']').Select(s => s.Trim()).ToArray();
                for (int j = 0; j < list_point_move_str.Length; j++)
                {
                    list_point_move_str[j] = list_point_move_str[j].Trim(new char[] { ',', '[' });

                    path_move_buf.Add(ConvertFromString(list_point_move_str[j]));
                }
                var nUnit = UnitArmy.Find(unit => unit.Unit_Prefab.name == Unit_name);
                cellGrid.StartBufLoad(nUnit.Unit_Prefab, buff_name, path_move_buf, null);
                path_move_buf.Clear();
            }
            if (Command == "move")
            {
                StartCoroutine(MoveUnit(Unit_name, position_tar, path_move, hash_code));
            }
            if (Command == "attack")
            {
                string unit_attack = Unit_name;
                var unit_to_a = cellGrid.LoadUnit.Find(u => u.hashCode == hash_code);
                var unit_a = cellGrid.LoadUnit.Find(u => u.hashCode == hash_code_a);
                StartCoroutine(AttackUnit(unit_to_a, unit_a));
            }
            if (Command == "endturn")
            {
                TurnResolverLoad();
            }
            if (Command == "UpTurn")
            {
                UpTurnBuff();
            }
            if (Command == "destroy")
            {
                Unit unit_dead = cellGrid.LoadUnit.Find(u => u.hashCode == hash_code);
                TransitionResult transitionResult = cellGrid.GetComponent<TurnResolver>().OnUnitDestroyedLoad(cellGrid, unit_dead);
                unit_dead.OnDestroyed(unit_dead);

                StartCoroutine(RunComandCorutin("destroy"));

            }
            if (Command == "spell")
            {
                StartCoroutine(RunComandCorutin("runspell"));
                string spell_name = jsonObject[UserString][step]["Name_spell"];
                string hash_code_targ = jsonObject[UserString][step]["List_damage"];
                hash_code_targ = hash_code_targ.Trim(new char[] { '[', ']' });
                var list_targ_str = hash_code_targ.Split(',').Select(s => s.Trim()).ToArray();
                list_targget_unit.Clear();
                for (int j = 0; j < list_targ_str.Length; j++)
                {
                    list_targ_str[j] = list_targ_str[j].Trim(new char[] { '\"', ' ' });

                    list_targget_unit.Add((list_targ_str[j]));
                }

                hero_spell = cellGrid.LoadUnit.Find(u => u.hashCode == hash_code);
                oldPosition_Spell = hero_spell.transform.position;
                var abilities = hero_spell.GetComponents<Ability>();
                for (int j = 0; j < abilities.Length; j++)
                {
                    var ability = abilities[j];
                    List<string> spell = ability.GetListAbility();
                    foreach (string s in spell)
                    {
                        if (s == spell_name)
                        {
                            SpellCastingAbility a = (SpellCastingAbility)ability;
                            SpellAbility carrent_spell = a.GetSpell(spell_name);

                            carrent_spell.StartSpell(cellGrid, list_targget_unit);

                        }
                    }
                }

                RunSpell(oldPosition_Spell);
            }

        }
    }
    public void UpTurnBuff()
    {

        unit_carent = cellGrid.LoadUnit.Find(u => u.hashCode == hash_code);
        unit_carent.transform.GetChild(6).gameObject.SetActive(true);

        StartCoroutine(RunComandCorutin("UpTurn"));
    }
    public Vector3 ConvertFromString(string input)
    {
        if (input != null)
        {
            var vals = input.Split(',').Select(s => s.Trim()).ToArray();
            if (vals.Length == 3)
            {
                float v1, v2, v3;

                v3 = float.Parse(vals[2], System.Globalization.CultureInfo.InvariantCulture);
                v2 = float.Parse(vals[1], System.Globalization.CultureInfo.InvariantCulture);
                v1 = float.Parse(vals[0], System.Globalization.CultureInfo.InvariantCulture);
                v1 = MathF.Round(v1, 2);
                v2 = MathF.Round(v2, 2);
                v3 = MathF.Round(v3, 2);
                return new Vector3(v1, v2, v3);

            }
            else
                throw new ArgumentException();
        }
        else
            throw new ArgumentException();
    }
    public IEnumerator MoveUnit(string Unit_name, Vector3 position_tar, List<Vector3> path, string hash_code)
    {
        TransitionResult transitionResult = cellGrid.GetComponent<TurnResolver>().ResolveSelectMove(cellGrid, hash_code);

        var unit_ = transitionResult.PlayableUnits;
        Cell distanation_tar = allCells.Find(h => h.GetComponent<Cell>().transform.position == position_tar);
        List<Cell> path_move = new List<Cell>();
        foreach (var cell in path)
        {
            path_move.Add(allCells.Find(h => h.GetComponent<Cell>().transform.position == cell));

        }
        unit_().ForEach(u => { StartCoroutine(u.Move(distanation_tar, path_move, cellGrid.loadFileBatal)); });
        unit_carent = cellGrid.LoadUnit.Find(u => u.hashCode == hash_code);

        while (unit_carent.transform.position != position_tar)
        {
            yield return null;
        }
        RunCommand();

    }

    public IEnumerator AttackUnit(Unit unit_to_a, Unit unit_a)
    {
        TransitionResult transitionResult;
        unit_carent_a = cellGrid.LoadUnit.Find(u => u.hashCode == unit_a.hashCode);
        if (unit_carent != null)
        {
            unit_carent = unit_carent_a;
            transitionResult = cellGrid.GetComponent<TurnResolver>().ResolveSelectMove(cellGrid, unit_carent.hashCode);
        }
        else
        {
            transitionResult = cellGrid.GetComponent<TurnResolver>().ResolveSelectMove(cellGrid, unit_carent_a.hashCode);
            unit_carent = unit_carent_a;

        }
        var unit_ = transitionResult.PlayableUnits;
        int callback_dmg = jsonObject[UserString][step]["callback_dmg"];
        int demage = jsonObject[UserString][step]["damage"];

        unit_().ForEach(u => u.AttackHandlerLoad(unit_to_a, unit_carent, demage, callback_dmg));

        yield return new WaitUntil(() => unit_carent.GetComponent<Animator>().GetBool("isAttack") == false);

        RunCommand();
    }
    public void TurnResolverLoad()
    {
        cellGrid.EndTurnExecute(false);
        StartCoroutine(RunComandCorutin("endturn"));
    }
    public IEnumerator RunSpell(Vector3 old_pos)
    {
        if (hero_spell.gameObject.name == "HeroMerlin")
            yield return new WaitUntil(() => hero_spell.transform.position == old_pos);
        else
            yield return new WaitForSeconds(0.2f);
        RunCommand();
    }
    public IEnumerator RunComandCorutin(string Command)
    {
        if (Command == "attack")
            yield return new WaitForSeconds(1f);
        if (Command == "str")
            yield return new WaitForSeconds(2f);
        if (Command == "runspell")
            yield return new WaitForSeconds(1f);
        if (Command == "runspell1")
            yield return new WaitForSeconds(0.5f);
        if (Command == "endturn")
            yield return new WaitForSeconds(0.2f);
        if (Command == "UpTurn")
        {
            yield return new WaitForSeconds(0.3f);
            unit_carent.transform.GetChild(6).gameObject.SetActive(false);
        }
        if (Command == "destroy")
            yield return new WaitForSeconds(0.1f);

        RunCommand();
    }
    public void CreateUnit(string Unit_name, Vector3 position)
    {
        allCells = Cells.FindAll(h => h.GetComponent<Cell>());
        List<Cell> freeCells = Cells.FindAll(h => h.GetComponent<Cell>().IsTaken == false);
        List<Cell> freeCells1 = Cells.FindAll(h => h.GetComponent<Cell>().IsTaken == true);

        var cell = allCells.Find(h => h.GetComponent<Cell>().transform.position == position);
        if (cell.IsTaken == false)
        {

            cell.GetComponent<Cell>().IsTaken = true;
            var nUnit = UnitArmy.Find(unit => unit.Unit_Prefab.name == Unit_name);
            var unit = Instantiate(nUnit.Unit_Prefab);
            unit.GetComponent<Speed>().Value = speed;
            unit.GetComponent<Unit>().amount = amount;

            unit.transform.position = cell.transform.position; //+ new Vector3(0, -0.4f, 0);
            unit.GetComponent<Unit>().hashCode = hash_code;
            unit.GetComponent<Unit>().PlayerNumber = playerNumb;
            unit.GetComponent<Unit>().unit_object = nUnit.Unit_Prefab;
            unit.GetComponent<Unit>().Cell = cell.GetComponent<Cell>();
            unit.GetComponent<Unit>().damage_min = damagemin;
            unit.GetComponent<Unit>().damage_max = damagemax;
            unit.GetComponent<Unit>().AttackFactor = attack;
            unit.GetComponent<Unit>().DefenceFactor = defence;
            unit.GetComponent<Unit>().HitPoints = hitp;
            unit.GetComponent<Unit>().AttackRange = range_attack;
            unit.GetComponent<Unit>().MovementPoints = range_move;
            unit.GetComponent<Unit>().migth = migth;
            unit.GetComponent<Unit>().Initialize(cellGrid);
            unit.transform.GetChild(0).gameObject.SetActive(true);
            unit.GetComponent<Unit>().UpdateAmount(unit.GetComponent<Unit>(), amount);

            unit.transform.parent = UnitsParent;
            if (playerNumb == 0)
            {
                unit.GetComponent<Unit>().heroesUnit = oneHeroPlayer;
            }
            else
            {
                unit.GetComponent<Unit>().heroesUnit = twoHeroPlayer;
            }
            cellGrid.AddUnit(unit.transform);
            cellGrid.LoadUnit.Add(unit.GetComponent<Unit>());


        }

    }
}


