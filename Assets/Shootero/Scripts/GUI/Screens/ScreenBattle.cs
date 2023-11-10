using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hiker.GUI;
namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class ScreenBattle : ScreenBase
    {
        public Joystick joystick;
        public Transform HPBarContainer;
        public HPBar playerHPBarPrefab;
        public HPBar enemyHPBarPrefab;

        public Transform HUDContainer;
        public HUD_GUI normDMG;
        public HUD_GUI critDMG;
        public HUD_GUI eleCritDMG;
        public HUD_GUI regenHP;
        public HUD_GUI redDMG;
        public HUD_GUI normText;

        public CanvasGroup sliderExpGrp;
        public Slider sliderExp;
        public Text lblLevel;
        public Text lblExp;
        public Text lbGold;

        public CanvasGroup sliderBossHPGrp;
        public Slider sliderBossHP;
        public Text lblBossName;
        public Text lblWave;

        public CanvasGroup battleMsg;
        public Text lbBattleMessage;
        public CanvasGroup battleMsg2;
        public Text lbBattleMessage2;

        public Text lbMapName;

        public Canvas HUDCanvas;
        public Canvas GUICanvas;

        public Button btnActiveSkillleft;
        public Button btnActiveSkillright;
        public Text lbSkillLeft;
        public Text lbSkillRight;
        public Image skillCDLeft;
        public Image skillCDRight;
        public Image iconSkillLeft;
        public Image iconSkillRight;

        public static ScreenBattle instance;
        List<HPBar> mListEnemyHPBars = new List<HPBar>();
        List<HPBar> mListPlayerHPBars = new List<HPBar>();

        //List<HUD_GUI> mlistRegenHUDs = new List<HUD_GUI>();
        //List<HUD_GUI> mlistNormDmgHUDs = new List<HUD_GUI>();
        //List<HUD_GUI> mlistCritDmgHUDs = new List<HUD_GUI>();
        Coroutine routineShow;
        public override bool OnBackBtnClick()
        {
            if (Time.timeScale > 0 && PopupManager.instance.activePopups.Count == 0)
            {
                OnBtnPauseBattle();
            }
            
            return true;
        }
        public void ShowMessage(string message_content)
        {
            this.lbBattleMessage.text = message_content;

            //var tween_position = this.lbBattleMessage.GetComponent<TweenPosition>();
            //if (tween_position != null) tween_position.enabled = false;

            //var tween_scale = this.lbBattleMessage.GetComponent<TweenScale>();
            //if (tween_scale != null) tween_scale.enabled = false;

            //var tween_alpha = this.lbBattleMessage.GetComponent<TweenAlpha>();
            //if (tween_alpha != null) tween_alpha.enabled = false;

            //this.lblMessage.alpha = 1f;
            this.battleMsg.alpha = 1;
            var color = this.lbBattleMessage.color;
            this.lbBattleMessage.color = new Color(color.r, color.g, color.b, 1f);
            
            //this.lbBattleMessage.transform.localPosition = new Vector3(0, -120, 0);
            //this.lbBattleMessage.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            if (routineShow != null)
                this.StopCoroutine(routineShow);
            routineShow = this.StartCoroutine(this.CoShow());

            //this.transform.SetAsLastSibling();
        }

        public void ShowMessage2(string message_content)
        {
            this.lbBattleMessage2.text = message_content;

            //var tween_position = this.lbBattleMessage.GetComponent<TweenPosition>();
            //if (tween_position != null) tween_position.enabled = false;

            //var tween_scale = this.lbBattleMessage.GetComponent<TweenScale>();
            //if (tween_scale != null) tween_scale.enabled = false;

            //var tween_alpha = this.lbBattleMessage.GetComponent<TweenAlpha>();
            //if (tween_alpha != null) tween_alpha.enabled = false;

            //this.lblMessage.alpha = 1f;
            this.battleMsg2.alpha = 1;
            var color = this.lbBattleMessage2.color;
            this.lbBattleMessage2.color = new Color(color.r, color.g, color.b, 1f);

            //this.lbBattleMessage.transform.localPosition = new Vector3(0, -120, 0);
            //this.lbBattleMessage.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            StopCoroutine("CoShow2");
            this.StartCoroutine("CoShow2");

            //this.transform.SetAsLastSibling();
        }
        [GUIDelegate]
        private IEnumerator CoShow2()
        {
            //TweenScale.Begin(this.lbBattleMessage.gameObject, 0.6f, Vector3.one);
            //TweenScale tween_scale = this.lbBattleMessage2.gameObject.GetComponent<TweenScale>();
            //tween_scale.PlayForward();
            //tween_scale.ResetToBeginning();

            TweenPosition tween_pos = this.lbBattleMessage2.gameObject.GetComponent<TweenPosition>();
            var originPos = tween_pos.from;
            originPos.x = -1010;
            tween_pos.from = originPos;
            tween_pos.PlayForward();
            tween_pos.ResetToBeginning();

            TweenAlpha tween_alpha = this.lbBattleMessage2.gameObject.GetComponent<TweenAlpha>();
            tween_alpha.PlayForward();
            tween_alpha.ResetToBeginning();

            //yield return new WaitForSecondsRealtime(0.2f);
            //TweenPosition.Begin(this.lbBattleMessage.gameObject, .6f, new Vector3(0, 35, 0));

            yield return new WaitForSecondsRealtime(4.0f);
            //TweenAlpha.Begin(this.lbBattleMessage.gameObject, .6f, 0f);
            //tween_scale.PlayReverse();
            originPos.x = 1010;
            tween_pos.from = originPos;
            tween_pos.PlayReverse();
            tween_alpha.PlayReverse();

            yield return new WaitForSecondsRealtime(1.0f);
            this.battleMsg2.alpha = 0;
            originPos.x = -1010;
            tween_pos.from = originPos;
        }

        [GUIDelegate]
        private IEnumerator CoShow()
        {
            //TweenScale.Begin(this.lbBattleMessage.gameObject, 0.6f, Vector3.one);
            TweenScale tween_scale = this.lbBattleMessage.gameObject.GetComponent<TweenScale>();
            tween_scale.PlayForward();
            tween_scale.ResetToBeginning();

            TweenPosition tween_pos = this.lbBattleMessage.gameObject.GetComponent<TweenPosition>();
            tween_pos.PlayForward();
            tween_pos.ResetToBeginning();

            TweenAlpha tween_alpha = this.lbBattleMessage.gameObject.GetComponent<TweenAlpha>();
            tween_alpha.PlayForward();
            tween_alpha.ResetToBeginning();

            //yield return new WaitForSecondsRealtime(0.2f);
            //TweenPosition.Begin(this.lbBattleMessage.gameObject, .6f, new Vector3(0, 35, 0));

            yield return new WaitForSecondsRealtime(4.0f);
            //TweenAlpha.Begin(this.lbBattleMessage.gameObject, .6f, 0f);
            tween_alpha.PlayReverse();

            yield return new WaitForSecondsRealtime(1.0f);
            this.battleMsg.alpha = 0;
        }

        public void FadeOutSkillDesc()
        {
            TweenAlpha tween_alpha = this.lbBattleMessage.gameObject.GetComponent<TweenAlpha>();
            tween_alpha.PlayReverse();
        }

        private void Awake()
        {
            instance = this;
            sliderExpGrp.alpha = 1;
            sliderBossHPGrp.alpha = 0;
            lblWave.text = string.Empty;
            if (joystick != null)
            {
                joystick.gameObject.SetActive(false);
            }
            joystick = QuanlyNguoichoi.Instance.joystick;

            if (NutBamKyNang.instance == null) NutBamKyNang.instance = QuanlyNguoichoi.Instance.nutBam;
            if (NutBamKyNang.instance)
            {
                NutBamKyNang.instance.gameObject.SetActive(true);

                btnActiveSkillleft = NutBamKyNang.instance.btnActiveSkillleft;
                btnActiveSkillright = NutBamKyNang.instance.btnActiveSkillright;
                lbSkillLeft = NutBamKyNang.instance.lbSkillLeft;
                lbSkillRight = NutBamKyNang.instance.lbSkillRight;
                skillCDLeft = NutBamKyNang.instance.skillCDLeft;
                skillCDRight = NutBamKyNang.instance.skillCDRight;

                btnActiveSkillright.onClick.AddListener(OnBtnActiveSkill);
                btnActiveSkillleft.onClick.AddListener(OnBtnActiveSkill);
            }
        }

        public void OnUnitDied(DonViChienDau unit)
        {
            if (unit.IsBoss)
            {
                //sliderExpGrp.alpha = 1;
                //sliderBossHPGrp.alpha = 0;
                HPBar hpBar = sliderBossHPGrp.GetComponent<HPBar>();
                bool isClearAllBoss = hpBar.RemoveUnit(unit);
                if(isClearAllBoss)
                {
                    sliderExpGrp.alpha = 1;
                    sliderBossHPGrp.alpha = 0;
                    hpBar.ResetBossHPBar();
                }

                var unitBar = unit.GetCurHPBar();
                if (unitBar != hpBar && unitBar != null)
                {
                    if (mListEnemyHPBars.Contains(unitBar) == false)
                        mListEnemyHPBars.Add(unitBar);
#if DEBUG
                    //Debug.Log("Disable hp bar " + unit.gameObject.name);
#endif
                    unitBar.gameObject.SetActive(false);
                }
            }
            else
            {
                if (unit.TeamID != QuanlyManchoi.PlayerTeam)
                {
                    var hpBar = unit.GetCurHPBar();
                    if (hpBar != null)
                    {
                        if (mListEnemyHPBars.Contains(hpBar) == false)
                            mListEnemyHPBars.Add(hpBar);
#if DEBUG
                        //Debug.Log("Disable hp bar " + unit.gameObject.name);
#endif
                        hpBar.gameObject.SetActive(false);
                    }
                    else
                    {
#if DEBUG
                        //Debug.Log(unit.gameObject.name + " hp bar is null");
#endif
                    }
                }
                else
                {
                    var hpBar = unit.GetCurHPBar();
                    if (hpBar != null)
                    {
                        mListPlayerHPBars.Add(hpBar);
                        hpBar.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void OnUnitRevive(DonViChienDau unit)
        {
            if (unit.IsBoss)
            {
                sliderExpGrp.alpha = 0;
                sliderBossHPGrp.alpha = 1;
            }
            else
            {
                if (unit.TeamID != QuanlyManchoi.PlayerTeam)
                {
                    var hpBar = unit.GetCurHPBar();
                    if (hpBar != null)
                    {
                        mListEnemyHPBars.Remove(hpBar);
                        hpBar.gameObject.SetActive(true);
                    }
                }
                else
                {
                    var hpBar = unit.GetCurHPBar();
                    if (hpBar != null)
                    {
                        mListPlayerHPBars.Remove(hpBar);
                        hpBar.gameObject.SetActive(true);
                    }
                }
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            //Debug.Log("OnApplicationFocus - ScreenBattle");
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (PopupBattleSecretShop.instance && PopupBattleSecretShop.instance.gameObject.activeInHierarchy)
                {

                }
                else if (PopupManager.instance.activePopups.Count == 0)
                {
                    PopupBattlePause.Create();
                }
                else if (Time.timeScale > 0)
                {
                    PopupBattlePause.Create();
                }
            }
            else
            {
                //if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
                //{
                //    Debug.Log("Ad Available");
                //}
            }
        }

        public static void PauseGame(bool isPause)
        {
            Time.timeScale = isPause ? 0 : 1;
        }

        public override void OnActive()
        {
            base.OnActive();
            this.battleMsg2.alpha = 0;
            this.battleMsg.alpha = 0;
            //            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            //            {
            //#if DEBUG
            //                Debug.Log("Ad Available");
            //#endif
            //            }

            if (ObjectPoolManager.instance &&
                ObjectPoolManager.instance.poolList != null &&
                ObjectPoolManager.instance.poolList.Count > 0)
            {
                var poolRegen = ObjectPoolManager.instance.poolList.Find(e => e.prefab == regenHP.gameObject);
                if (poolRegen != null)
                    poolRegen.UnspawnAll();

                var poolNormDmg = ObjectPoolManager.instance.poolList.Find(e => e.prefab == normDMG.gameObject);
                if (poolNormDmg != null)
                    poolNormDmg.UnspawnAll();

                var poolCritDmg = ObjectPoolManager.instance.poolList.Find(e => e.prefab == critDMG.gameObject);
                if (poolCritDmg != null)
                    poolCritDmg.UnspawnAll();

                var poolEleCritDmg = ObjectPoolManager.instance.poolList.Find(e => e.prefab == eleCritDMG.gameObject);
                if (poolEleCritDmg != null)
                    poolEleCritDmg.UnspawnAll();
            }

            if (ObjectPoolManager.instance)
            {
                ObjectPoolManager.instance.PreCachePool(regenHP.gameObject, 3);
                ObjectPoolManager.instance.PreCachePool(normText.gameObject, 3);
                ObjectPoolManager.instance.PreCachePool(normDMG.gameObject, 15);
                ObjectPoolManager.instance.PreCachePool(critDMG.gameObject, 10);
                ObjectPoolManager.instance.PreCachePool(eleCritDMG.gameObject, 10);
            }

            if (ResourceBar.instance)
            {
                ResourceBar.instance.gameObject.SetActive(false);
            }

            if (BattleJoystick.instance)
            {
                joystick = BattleJoystick.instance.GetComponent<VariableJoystick>();
            }

            joystick.gameObject.SetActive(true);

            ApplyJoystickCfg();
            lblWave.text = string.Empty;

            btnActiveSkillleft.gameObject.SetActive(false);
            btnActiveSkillright.gameObject.SetActive(false);

            if (NutBamKyNang.instance)
            {
                NutBamKyNang.instance.gameObject.SetActive(true);

                btnActiveSkillleft = NutBamKyNang.instance.btnActiveSkillleft;
                btnActiveSkillright = NutBamKyNang.instance.btnActiveSkillright;
                lbSkillLeft = NutBamKyNang.instance.lbSkillLeft;
                lbSkillRight = NutBamKyNang.instance.lbSkillRight;
                skillCDLeft = NutBamKyNang.instance.skillCDLeft;
                skillCDRight = NutBamKyNang.instance.skillCDRight;
                iconSkillLeft = NutBamKyNang.instance.iconSkillLeft;
                iconSkillRight = NutBamKyNang.instance.iconSkillRight;

                btnActiveSkillright.onClick.AddListener(OnBtnActiveSkill);
                btnActiveSkillleft.onClick.AddListener(OnBtnActiveSkill);
            }
            joystick.GetComponent<JoystickSwipeOut>().onActivate += OnJoystickSwipeOut;
#if UNITY_ANDROID || UNITY_IOS
            Hiker.QualityRenderSetting.instance.SetTargetFPS();
#endif
        }

        public void ApplyJoystickCfg()
        {
            VariableJoystick js = joystick as VariableJoystick;
            js.SetMode(GameManager.instance.FixedJoystick ? JoystickType.Floating : JoystickType.Dynamic);

        }

        public override void OnDeactive()
        {
            joystick.gameObject.SetActive(false);
            btnActiveSkillleft.gameObject.SetActive(false);
            btnActiveSkillright.gameObject.SetActive(false);
            btnActiveSkillright.onClick.RemoveAllListeners();
            btnActiveSkillleft.onClick.RemoveAllListeners();
            joystick.GetComponent<JoystickSwipeOut>().onActivate -= OnJoystickSwipeOut;

            base.OnDeactive();
        }

        [GUIDelegate]
        public void OnBtnPauseBattle()
        {
            PopupBattlePause.Create();
        }

        void OnJoystickSwipeOut(Vector2 dir)
        {
            Debug.Log("SwipeOut " + dir.ToString());
            if (QuanlyNguoichoi.Instance)
            {
                if (QuanlyNguoichoi.Instance.PlayerUnit)
                {
                    if (mTuyetKyMan == null)
                        mTuyetKyMan = QuanlyNguoichoi.Instance.PlayerUnit.GetComponent<TuyetKyNhanVat>();

                    if (mTuyetKyMan && mTuyetKyMan.HaveTuyetKyChuDong())
                    {
                        var tk = mTuyetKyMan.GetTuyetKy(0);
                        if(ConfigManager.IsHeroSkillSwipe(tk.TKName))
                        {
                            OnBtnActiveSkill();
                        }
                    }
                }
            }
        }

        void OnBtnActiveSkill()
        {
            //if (joystick.Horizontal != 0 || joystick.Vertical != 0) return;

            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.ActivateSkill();
            }
        }

        public HPBar CreateHPBar(DonViChienDau unit, bool useTotalBar = true)
        {
            HPBar hpBar;
            if (unit.IsBoss == false)
            {
                if (unit.TeamID == QuanlyManchoi.PlayerTeam)
                {
                    if (mListPlayerHPBars.Count == 0)
                    {
                        hpBar = Instantiate(playerHPBarPrefab,
                            HPBarContainer);
                    }
                    else
                    {
                        hpBar = mListPlayerHPBars[mListPlayerHPBars.Count - 1];
                        mListPlayerHPBars.RemoveAt(mListPlayerHPBars.Count - 1);
                    }
                    
                }
                else
                if (mListEnemyHPBars.Count == 0)
                {
                    hpBar = Instantiate(enemyHPBarPrefab,
                        HPBarContainer);
                }
                else
                {
                    hpBar = mListEnemyHPBars[mListEnemyHPBars.Count - 1];
                    mListEnemyHPBars.RemoveAt(mListEnemyHPBars.Count - 1);
                }
#if DEBUG
                //Debug.Log("Enable hp bar " + unit.gameObject.name);
#endif

                hpBar.Init(unit);
                hpBar.UpdateHP();
                hpBar.gameObject.SetActive(true);
            }
            else
            {
                if (useTotalBar)
                {
                    hpBar = sliderBossHPGrp.GetComponent<HPBar>();

                    sliderExpGrp.alpha = 0;
                    sliderBossHPGrp.gameObject.SetActive(true);
                    sliderBossHPGrp.alpha = 1;
                }
                else
                {
                    if (mListEnemyHPBars.Count == 0)
                    {
                        hpBar = Instantiate(enemyHPBarPrefab,
                            HPBarContainer);
                    }
                    else
                    {
                        hpBar = mListEnemyHPBars[mListEnemyHPBars.Count - 1];
                        mListEnemyHPBars.RemoveAt(mListEnemyHPBars.Count - 1);
                    }
                    hpBar.gameObject.SetActive(true);
                }
                hpBar.Init(unit, useTotalBar);
                hpBar.UpdateHP();
            }

            return hpBar;
        }

        public void UpdatePlayerExp()
        {
            var exp = QuanlyNguoichoi.Instance.PlayerExp;
            var level = QuanlyNguoichoi.Instance.PlayerLvl;
            lblLevel.text = string.Format(Localization.Get("BattlePlayerLevel"), level);
            lblExp.text = exp.ToString();

            var lastLevelExp = QuanlyNguoichoi.Instance.gameConfig.GetTotalExpAtLevel(level - 1);
            var nextLevelExp = QuanlyNguoichoi.Instance.gameConfig.GetTotalExpAtLevel(level);

            sliderExp.value = Mathf.Clamp01((float)(exp - lastLevelExp) / (nextLevelExp - lastLevelExp));
        }

        public void UpdatePlayerGold()
        {
            var totalGold = QuanlyNguoichoi.Instance.GetTotalGoldByRate();
            lbGold.text = totalGold.ToString();
        }

        public struct DisplayHUDCommand
        {
            public string Text;
            public DonViChienDau Target;
            public GameObject Prefab;
            public bool canMerge;
            public int frame;
            public float Scale;
            public Color Color;
        }

        public void DisplayHUD(DisplayHUDCommand hudCommand)
        {
            if (hudCommand.Prefab && hudCommand.Target != null)
            {
                var hudObj = ObjectPoolManager.Spawn(hudCommand.Prefab, Vector3.zero, Quaternion.identity, HUDContainer);
                hudObj.transform.localScale = Vector3.one * hudCommand.Scale;
                var hud = hudObj.GetComponent<HUD_GUI>();
                hud.Display(hudCommand.Text, 0.65f, hudCommand.Color);
                var follower = hud.GetComponent<UIFollowTarget>();

                var offsetRand = Random.insideUnitCircle * 0.8f;
                var offset = hudCommand.Target.OffsetHUD + new Vector3(offsetRand.x, 0, offsetRand.y);
                follower.SetTarget(hudCommand.Target.transform, 0.6f, 1f, offset);
            }
        }

        static readonly Color ColorRegen = new Color32(46, 190, 37, 255);
        static readonly Color ColorCrit = new Color32(255, 0, 0, 255);
        static readonly Color ColorNorm = Color.white;

        public void DisplayTextHud(string text, DonViChienDau target)
        {
            if (normText == null) return;

            var comand = new DisplayHUDCommand
            {
                Text = text,
                Target = target,
                Color = ColorRegen,
                Scale = 1,
                Prefab = normText.gameObject
            };

            //queueHUDs.Enqueue(comand);
            if (target)
            {
                target.QueueHudDisplay(comand);
            }
            

            //var hudObj = ObjectPoolManager.Spawn(regenHP.gameObject, Vector3.zero, Quaternion.identity, HUDContainer);
            //hudObj.transform.localScale = Vector3.one;
            //var hud = hudObj.GetComponent<HUD_GUI>();
            //hud.Display(text, 1f);
            //var follower = hud.GetComponent<UIFollowTarget>();
            //follower.SetTarget(target.transform, 0.6f, 1f, target.OffsetHUD);
            //return hud;
        }

        public void DisplayDmgHud(long dmg, bool isCrit, DonViChienDau target, bool isEleCrit = false)
        {
            if (target == null) return;
            if (QuanlyNguoichoi.Instance == null) return;
            if (QuanlyNguoichoi.Instance.PlayerUnit == null) return;
            if (normDMG == null) return;
            if (QuanlyNguoichoi.Instance.IsLeoThapMode && 
                GameManager.instance.ShowDamageHUD == false &&
                target != QuanlyNguoichoi.Instance.PlayerUnit) return;

            var comand = new DisplayHUDCommand
            {
                Text = dmg.ToString(),
                Target = target,
                frame = Time.frameCount,
                canMerge = true,
                Color = isCrit ? ColorCrit : ColorNorm,
                Scale = 1,
                Prefab = normDMG.gameObject
                //Prefab = isCrit ? critDMG.gameObject : normDMG.gameObject
            };

            if (QuanlyNguoichoi.Instance && target == QuanlyNguoichoi.Instance.PlayerUnit)
            {
                comand.Prefab = redDMG.gameObject;
            }

            float baseDmg = QuanlyNguoichoi.Instance.PlayerUnit.OriginStat.DMG;
            float curDmg = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat().DMG;

            if (isCrit == false)
            {
                comand.Scale = Mathf.Min(2f, 0.5f + dmg / baseDmg * 0.5f);
            }
            else if(isEleCrit)
            {
                comand.Prefab = eleCritDMG.gameObject;
            }
            else
            {
                comand.Prefab = critDMG.gameObject;
                //EZCameraShake.CameraShaker.Instance.ShakeOnce(0.5f, 0.5f, 0.1f, 0.1f);
            }

            // chan hien thi qua nhieu hud len Unit
            if (target.GetCurrentHUDQueue() < ConfigManager.GetMaxHUDQueue() )
            {
                target.QueueHudDisplay(comand);
            }

            //var hudObj = ObjectPoolManager.Spawn(isCrit ? critDMG.gameObject : normDMG.gameObject, Vector3.zero, Quaternion.identity, HUDContainer);
            //var hud = hudObj.GetComponent<HUD_GUI>();
            //hudObj.transform.localScale = Vector3.one;
            //hud.Display(dmg.ToString(), 1f);
            //var follower = hud.GetComponent<UIFollowTarget>();
            //follower.SetTarget(target.transform, 0.6f, 1f, target.OffsetHUD);
            //return hud;
        }

        public void DisplayRegenHP(long hp, DonViChienDau target)
        {
            var comand = new DisplayHUDCommand
            {
                Text = hp.ToString(),
                Target = target,
                Color = ColorRegen,
                Scale = 1,
                Prefab = regenHP.gameObject
            };
            target.QueueHudDisplay(comand);

            //var hudObj = ObjectPoolManager.Spawn(regenHP.gameObject, Vector3.zero, Quaternion.identity, HUDContainer);
            //hudObj.transform.localScale = Vector3.one;
            //var hud = hudObj.GetComponent<HUD_GUI>();
            //hud.Display(hp.ToString(), 1f);
            //var follower = hud.GetComponent<UIFollowTarget>();
            //follower.SetTarget(target.transform, 0.6f, 1f, target.OffsetHUD);
            //return hud;
        }

        TuyetKyNhanVat mTuyetKyMan;
        SpriteCollection skillAvaCol;

        private void UpdateActiveSkill()
        {
            if (QuanlyNguoichoi.Instance)
            {
                if (QuanlyNguoichoi.Instance.PlayerUnit)
                {
                    if (mTuyetKyMan == null)
                        mTuyetKyMan = QuanlyNguoichoi.Instance.PlayerUnit.GetComponent<TuyetKyNhanVat>();

                    if (mTuyetKyMan && mTuyetKyMan.HaveTuyetKyChuDong())
                    {
                        var tk = mTuyetKyMan.GetTuyetKy(0);
                        if (ConfigManager.IsHeroSkillSwipe(tk.TKName))
                        {
                            var coolDown = mTuyetKyMan.GetTuyetKyCoolDown(0);
                            var durability = mTuyetKyMan.GetTuyetKyDurability(0);

                            var coolDownTime = mTuyetKyMan.GetTuyetKyCoolDownMaxTime(0);
                            var cd = coolDown / coolDownTime;

                            if (durability < 0)
                            {
                                cd = 1;
                            }

                            var playerHPBar = QuanlyNguoichoi.Instance.PlayerUnit.GetCurHPBar();
                            if (playerHPBar != null)
                            {
                                var skillBar = playerHPBar.transform.Find("PlayerSkillBar").GetComponent<Slider>();
                                if (skillBar.gameObject.activeSelf == false)
                                    skillBar.gameObject.SetActive(true);

                                skillBar.value = 1 - cd;
                            }

                            btnActiveSkillleft.gameObject.SetActive(false);
                            btnActiveSkillright.gameObject.SetActive(false);
                        }
                        else
                        {
                            var playerHPBar = QuanlyNguoichoi.Instance.PlayerUnit.GetCurHPBar();
                            if (playerHPBar != null)
                            {
                                var skillBar = playerHPBar.transform.Find("PlayerSkillBar").GetComponent<Slider>();
                                if (skillBar.gameObject.activeSelf)
                                    skillBar.gameObject.SetActive(false);
                            }

                            btnActiveSkillleft.gameObject.SetActive(GameManager.instance.RightActiveSkill == false);
                            btnActiveSkillright.gameObject.SetActive(GameManager.instance.RightActiveSkill);

                            if (skillAvaCol == null)
                            {
                                skillAvaCol = Resources.Load<SpriteCollection>("HeroSkillAvatar");
                            }
                            var skillName = QuanlyNguoichoi.Instance.PlayerUnit.UnitName + "Skill";
                            if (iconSkillLeft.sprite == null || iconSkillLeft.sprite.name != skillName)
                            {
                                iconSkillLeft.sprite = skillAvaCol.GetSprite(skillName);
                            }
                            if (iconSkillRight.sprite == null || iconSkillRight.sprite.name != skillName)
                            {
                                iconSkillRight.sprite = skillAvaCol.GetSprite(skillName);
                            }

                            var coolDown = mTuyetKyMan.GetTuyetKyCoolDown(0);
                            var durability = mTuyetKyMan.GetTuyetKyDurability(0);

                            if (QuanlyNguoichoi.Instance.PlayerUnit.UnitName == "Necromancer")
                            {
                                NecromancerSkill _skill = (NecromancerSkill)mTuyetKyMan.GetTuyetKy(0);
                                var charge_count = _skill.CurrCharge;
                                var str = Mathf.CeilToInt(charge_count).ToString();
                                lbSkillLeft.text = str;
                                lbSkillRight.text = str;
                            }
                            else
                            {
                                var str = Mathf.CeilToInt(durability).ToString();
                                lbSkillLeft.text = str;
                                lbSkillRight.text = str;
                            }

                            if (coolDown <= 0 && durability > 0)
                            {
                                btnActiveSkillleft.interactable = true;
                                btnActiveSkillright.interactable = true;
                                //string str = Localization.Get("BtnActiveSkill");
                                //lbSkillLeft.text = str;
                                //lbSkillRight.text = str;
                            }
                            else
                            {
                                btnActiveSkillleft.interactable = false;
                                btnActiveSkillright.interactable = false;
                            }

                            var coolDownTime = mTuyetKyMan.GetTuyetKyCoolDownMaxTime(0);
                            var cd = coolDown / coolDownTime;

                            if (durability < 0)
                            {
                                cd = 1;
                            }

                            skillCDLeft.fillAmount = cd;
                            skillCDRight.fillAmount = cd;
                        }
                    }
                    else
                    {
                        var playerHPBar = QuanlyNguoichoi.Instance.PlayerUnit.GetCurHPBar();
                        if (playerHPBar != null)
                        {
                            var playerSkillBar = playerHPBar.transform.Find("PlayerSkillBar");
                            if (playerSkillBar )
                            {
                                var skillBar = playerSkillBar.GetComponent<Slider>();
                                if (skillBar.gameObject.activeSelf)
                                    skillBar.gameObject.SetActive(false);
                            }
                        }

                        btnActiveSkillleft.gameObject.SetActive(false);
                        btnActiveSkillright.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void Update()
        {
            UpdateActiveSkill();
#if UNITY_STANDALONE
            if (Input.GetKeyDown(KeyCode.F8))
            {
                //if (HUDCanvas)
                //{
                //    HUDCanvas.gameObject.SetActive(false);
                //}
                if (sliderExpGrp)
                {
                    sliderExpGrp.gameObject.SetActive(false);
                }
                if (GUICanvas)
                {
                    GUICanvas.gameObject.SetActive(false);
                }
                if (joystick)
                {
                    joystick.gameObject.SetActive(false);
                }
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                //if (HUDCanvas)
                //{
                //    HUDCanvas.gameObject.SetActive(true);
                //}
                if (sliderExpGrp)
                {
                    sliderExpGrp.gameObject.SetActive(true);
                }
                if (GUICanvas)
                {
                    GUICanvas.gameObject.SetActive(true);
                }
                if (joystick)
                {
                    joystick.gameObject.SetActive(true);
                }
            }
#endif
        }
    }
}