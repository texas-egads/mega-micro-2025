using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// !!
// parts of this code was created with ChatGPT or Gemini
// !!


namespace boinkthatboy
{
    public class FishingHookMinigame : MonoBehaviour, IMinigame
    {
        public GameObject logPrefab;

        [Header("Spawn Points")]
        public string spawnPointsRootName = "FishingLogPoints";
        public bool randomizePoints = true;

        public bool overrideLogScale = true;
        public Vector3 logSpawnScale = new Vector3( 0.25f, 0.25f, 0.25f );

        [Header("Log Animation")]
        public float popTime = 0.25f;

        [Header("Timing UI")]
        public FishingHookTimingUI timingUI;
        public Vector3 timingUIWorldOffset = new Vector3( 0f, 0f, 0.15f );

        [Header("Score Text")]
        public TMP_Text resultText;
        public float resultDelay = 0.45f;

        [Header("Result Colors")]
        public Color perfectColor = Color.green;
        public Color okColor = Color.yellow;
        public Color missColor = Color.red;

        [Header("Result Text ANimation")]
        public float floatFadeDuration = 0.6f;
        public float floatUpPixels = 30f;

        [Header("Fishing Line")]
        public LineRenderer line;
        public float lineAttachDuration = 0.25f;


        public float moveDuration = 0.8f;
        public float glideDistance = 0.5f;
        public float finishGlideDistance = 0.3f;
        public float momentumCurvePower = 4.0f;
        public float boatLogClearance = 0.02f;

        public float minimumForwardStep = 0.10f;

        public int minLogs = 4;
        public int maxLogs = 5;

        [Header("Cleanup")]
        public float endDelay = 0.6f;

        MinigameContext ctx;

        readonly List<Transform> logs = new List<Transform>();
        int logCount;
        int currentLogIndex;
        float scoreIntoLog;
        float totalScore;
        bool running;
        bool inputLocked;
        bool outcomeDeclared;
        Coroutine lineRoutine;
        Coroutine moveRoutine;
        bool catOriginalKinematic;
        bool catOriginalUseGravity;
        bool boatOriginalKinematic;
        bool boatOriginalUseGravity;

        PlayerVisualController catVisuals;

        public void begin( MinigameContext context )
        {
            ctx = context;

            if ( timingUI == null ) { timingUI = Object.FindFirstObjectByType<FishingHookTimingUI>( FindObjectsInactive.Include ); }
            if ( line == null && ctx.fishingRodObject != null ) { line = ctx.fishingRodObject.GetComponentInChildren<LineRenderer>( true ); }

            if (resultText == null)
            {
                TMP_Text found = timingUI.GetComponentInChildren<TMP_Text>( true );
                if ( found != null ) { resultText = found; }
            }

            float d = Mathf.Clamp01( Managers.MinigamesManager.GetCurrentMinigameDifficulty() );

            // log count
            logCount = Mathf.RoundToInt( Mathf.Lerp( minLogs, maxLogs, d ) );

            // needle speed
            ApplyDifficultyToUI( d );

            if ( ctx.playerMovement != null )
            {
                ctx.playerMovement.enabled = false;
                catVisuals = ctx.playerMovement.GetComponent<PlayerVisualController>();
                if ( catVisuals != null ) { catVisuals.enabled = false; }
            }

            if ( ctx.fishingRodObject != null ) { ctx.fishingRodObject.SetActive(true); }
            if (ctx.catAnimator != null) { SafeSetBool( ctx.catAnimator, "Tool_FishingHook", true ); }

            SetupLineRenderer();
            StartCoroutine( warmupAssets() ); // preloads shit, fixes stupid ahh freezing
            SpawnLogsFromPoints();

            currentLogIndex = 0;
            scoreIntoLog = 0f;
            totalScore = 0f;

            timingUI.Show( true );

            if (timingUI != null) { timingUI.ResetCycleRandom(); }
            if (resultText != null) { resultText.gameObject.SetActive( false ); }

            StartCoroutine( PlaceUIAfterFirstPop() );

            running = true;
            inputLocked = false;
            outcomeDeclared = false;
        }

        IEnumerator warmupAssets()
        {
            if ( line != null ) { line.enabled = true; line.SetPosition(0, Vector3.zero); line.SetPosition(1, Vector3.zero); }
            if ( resultText != null )
            {
                resultText.gameObject.SetActive( true );
                resultText.color = new Color( 0, 0, 0, 0 );
                resultText.text = "!";
            }
            yield return null;

            if (line != null) { line.enabled = false; }
            if (resultText != null) { resultText.gameObject.SetActive( false ); }
        }

        public void stop()
        {
            running = false;
            CleanupLocal();
            Destroy( gameObject );
        }

        void Update()
        {
            if ( !running ) { return; }
            if ( inputLocked ) { return; }
            if ( outcomeDeclared ) { return; }
            if ( Input.GetKeyDown( KeyCode.Space ) )
            {
                float needle = timingUI.getNeedle();
                float score = timingUI.getScore( needle );
                applyAttempt( score );
            }
        }

        void LateUpdate()
        {
            if ( !running ) { return; }
            if ( inputLocked ) { return; }
            if ( outcomeDeclared ) { return; }

            MoveTimingUIToCurrentLog();
        }

        void ApplyDifficultyToUI( float d )
        {
            if (timingUI == null) { return; }

            float cyclesEasy = 0.6f;
            float cyclesHard = 1.3f;
            timingUI.cyclesPerSecond = Mathf.Lerp( cyclesEasy, cyclesHard, d );
        }

        void SetupLineRenderer()
        {
            if (line == null) { return; }

            line.positionCount = 2;
            line.useWorldSpace = true;
            line.enabled = false;
        }

        void SpawnLogsFromPoints()
        {
            logs.Clear();

            GameObject rootGO = GameObject.Find( spawnPointsRootName );
            List<Transform> points = new List<Transform>();
            if (rootGO != null)
            {
                foreach ( Transform t in rootGO.transform )
                    if ( t != null ) { points.Add(t); }
            }

            if (randomizePoints) { Shuffle(points); }

            int spawnCount = Mathf.Clamp( logCount, 1, points.Count );
            List<Transform> chosen = points.GetRange( 0, spawnCount );

            chosen.Sort( (a, b) => a.position.x.CompareTo( b.position.x ) );

            for ( int i = 0; i < chosen.Count; i++ )
            {
                Transform p = chosen[i];
                Vector3 target = p.position + new Vector3(0f, 0f, 0.6f);
                Vector3 start = p.position + new Vector3(0f, 0f, -1.0f);
                GameObject go = Instantiate( logPrefab, start, Quaternion.identity, transform );

                if ( overrideLogScale ) { go.transform.localScale = logSpawnScale; }

                ConfigureLogPhysics( go );
                logs.Add( go.transform );
                StartCoroutine( Pop( go.transform, target, popTime ) );
            }

            logCount = logs.Count;
        }

        IEnumerator Pop( Transform t, Vector3 to, float time )
        {
            Vector3 from = t.position;
            float s = 0f;

            while ( s < 1f )
            {
                s += Time.deltaTime / Mathf.Max( 0.001f, time );
                t.position = Vector3.Lerp( from, to, Mathf.SmoothStep( 0f, 1f, s ) );
                yield return null;
            }

            t.position = to;
        }

        IEnumerator PlaceUIAfterFirstPop()
        {
            yield return new WaitForSeconds( popTime + 0.02f );
            MoveTimingUIToCurrentLog();
        }

        void MoveTimingUIToCurrentLog()
        {
            if (timingUI == null) { return; }
            if (currentLogIndex < 0 || currentLogIndex >= logs.Count) { return; }

            Transform logT = logs[ currentLogIndex ];

            if ( logT == null ) { return; }

            Vector3 topPos = logT.position;

            Collider col = logT.GetComponentInChildren<Collider>();

            if ( col != null )
            {
                Bounds b = col.bounds;
                topPos = new Vector3( b.center.x, b.center.y, b.max.z );
            } else {
                topPos = topPos + new Vector3( 0f, 0f, 0.5f );
            }

            timingUI.transform.position = topPos + timingUIWorldOffset; // stupid ui wont align (its entirely my fault)

            if ( Camera.main != null ) { timingUI.transform.rotation = Camera.main.transform.rotation; }
        }

        void applyAttempt( float score )
        {
            if (ctx.catAnimator != null) { SafeSetTrigger(ctx.catAnimator, "FH_Cast"); }
            if (score <= 0f)
            {
                StartCoroutine(ShowResultAndFreeze("MISS!", missColor));
                return;
            }

            AttachLineBriefly(currentLogIndex);

            Vector3 stopPos = computeStopPos();
            Vector3 target = stopPos;

            bool isWinningHit = (scoreIntoLog + score >= 1f) && (currentLogIndex >= logCount - 1);

            if (isWinningHit)
            {
                target.x += finishGlideDistance;
            }
            else if (score >= 1f)
            {
                target.x += glideDistance;
            }
            else
            {
                target = Vector3.Lerp(ctx.catRb.position, stopPos, 0.5f);
            }

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(MoveCatAndBoatToTargetWithTempKinematic(target, moveDuration));

            if (timingUI != null) timingUI.ResetCycleRandom();

            scoreIntoLog += score;
            totalScore += score;

            if (scoreIntoLog >= 1f)
            {
                scoreIntoLog -= 1f;
                currentLogIndex++;
            }
            if (totalScore >= logCount || currentLogIndex >= logCount)
            {
                DeclareWin();
                return;
            }
            if (score >= 1f)
                StartCoroutine(ShowResultAndFreeze("PERFECT!", perfectColor));
            else
                StartCoroutine(ShowResultAndFreeze("OK!", okColor));
        }

        Vector3 computeStopPos()
        {
            if (currentLogIndex >= logs.Count) { return ctx.catRb.position; }

            Transform logT = logs[currentLogIndex];
            Collider logCol = logT.GetComponentInChildren<Collider>();
            Collider catCol = ctx.catRb.GetComponentInChildren<Collider>();

            float logCenterX = (logCol != null) ? logCol.bounds.center.x : logT.position.x;
            bool isCatOnRight = ctx.catRb.position.x > logCenterX;

            float logSurfaceX;
            if (logCol != null)
            {
                Vector3 closestPoint = logCol.ClosestPoint(ctx.catRb.position);
                logSurfaceX = closestPoint.x;
            }
            else
            {
                logSurfaceX = logT.position.x;
            }

            float distFromCatPivotToLeadingEdge = 0.5f;
            if (catCol != null)
            {
                if (isCatOnRight)
                    distFromCatPivotToLeadingEdge = ctx.catRb.position.x - catCol.bounds.min.x;
                else
                    distFromCatPivotToLeadingEdge = catCol.bounds.max.x - ctx.catRb.position.x;
            }

            float stopX;
            if (isCatOnRight)
            {
                stopX = logSurfaceX + (distFromCatPivotToLeadingEdge * (0.65f)) + boatLogClearance;
            }
            else
            {
                stopX = logSurfaceX - distFromCatPivotToLeadingEdge * 0.65f - boatLogClearance;
            }

            if (stopX < ctx.catRb.position.x + minimumForwardStep)
                stopX = ctx.catRb.position.x + minimumForwardStep;

            return new Vector3(stopX, ctx.catRb.position.y, ctx.catRb.position.z);
        }

        IEnumerator MoveCatAndBoatToTargetWithTempKinematic(Vector3 catTarget, float duration)
        {
            CacheRigidbodies();

            if (ctx.catRb != null)
            {
                ctx.catRb.linearVelocity = Vector3.zero;
                ctx.catRb.angularVelocity = Vector3.zero;
                ctx.catRb.isKinematic = true;
                ctx.catRb.useGravity = false;
            }

            if (ctx.boatRb != null)
            {
                ctx.boatRb.linearVelocity = Vector3.zero;
                ctx.boatRb.angularVelocity = Vector3.zero;
                ctx.boatRb.isKinematic = true;
                ctx.boatRb.useGravity = false;
            }

            Vector3 catStart = ctx.catRb.position;
            Vector3 boatStart = ctx.boatRb.position;
            Vector3 delta = catTarget - catStart;
            Vector3 boatTarget = boatStart + delta;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(0.001f, duration);
                float s = 1f - Mathf.Pow(1f - t, momentumCurvePower);

                ctx.catRb.position = Vector3.Lerp(catStart, catTarget, s);
                ctx.boatRb.position = Vector3.Lerp(boatStart, boatTarget, s);

                yield return null;
            }

            ctx.catRb.position = catTarget;
            ctx.boatRb.position = boatTarget;

            RestoreRigidbodies();
        }

        void CacheRigidbodies()
        {
            catOriginalKinematic = ctx.catRb.isKinematic;
            catOriginalUseGravity = ctx.catRb.useGravity;
            boatOriginalKinematic = ctx.boatRb.isKinematic;
            boatOriginalUseGravity = ctx.boatRb.useGravity;
        }

        void RestoreRigidbodies()
        {
            if (ctx.catRb != null)
            {
                ctx.catRb.isKinematic = catOriginalKinematic;
                ctx.catRb.useGravity = catOriginalUseGravity;
            }
            if (ctx.boatRb != null)
            {
                ctx.boatRb.isKinematic = boatOriginalKinematic;
                ctx.boatRb.useGravity = boatOriginalUseGravity;
            }
        }

        IEnumerator ShowResultAndFreeze(string msg, Color color)
        {
            inputLocked = true;

            if (timingUI != null)
                timingUI.SetPaused(true);

            if (resultText != null)
            {
                resultText.text = msg;
                resultText.color = color;
                resultText.gameObject.SetActive(true);
                StartCoroutine(FloatAndFadeText(resultText, floatFadeDuration, floatUpPixels));
            }

            yield return new WaitForSeconds(resultDelay);

            if (timingUI != null)
                timingUI.SetPaused(false);

            inputLocked = false;
        }

        IEnumerator FloatAndFadeText(TMP_Text txt, float duration, float upPixels)
        {
            Vector3 startPos = txt.transform.localPosition;
            Vector3 endPos = startPos + new Vector3(0f, upPixels, 0f);

            Color startColor = txt.color;
            Color endColor = startColor;
            endColor.a = 0f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(0.001f, duration);
                txt.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                txt.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }

            txt.gameObject.SetActive(false);
            txt.transform.localPosition = startPos;
            txt.color = startColor;
        }

        void AttachLineBriefly(int targetIndex)
        {
            if (line == null) return;
            if (ctx.rodTip == null) return;

            if (targetIndex < 0 || targetIndex >= logs.Count) return;

            if (lineRoutine != null)
                StopCoroutine(lineRoutine);

            lineRoutine = StartCoroutine(LineAttachRoutine(targetIndex, lineAttachDuration));
        }

        IEnumerator LineAttachRoutine(int targetIndex, float duration)
        {
            line.enabled = true;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;

                if (ctx.rodTip != null)
                    line.SetPosition(0, ctx.rodTip.position);

                if (targetIndex >= 0 && targetIndex < logs.Count && logs[targetIndex] != null)
                    line.SetPosition(1, logs[targetIndex].position);

                yield return null;
            }

            line.enabled = false;
        }

        void DeclareWin()
        {
            if (outcomeDeclared) return;

            outcomeDeclared = true;
            running = false;
            inputLocked = true;

            Managers.MinigamesManager.DeclareCurrentMinigameWon();

            if (timingUI != null) timingUI.Show(false);
            if (resultText != null) resultText.gameObject.SetActive(false);
            if (line != null) line.enabled = false;

            StartCoroutine(CleanupAfterDelay(endDelay));
        }

        IEnumerator CleanupAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            CleanupLocal();

            Managers.MinigamesManager.EndCurrentMinigame(0f);
            Destroy(gameObject);
        }

        void CleanupLocal()
        {
            if (ctx.playerMovement != null) { ctx.playerMovement.enabled = true; }
            if (catVisuals != null) { catVisuals.enabled = true; }
            if (ctx.fishingRodObject != null) { ctx.fishingRodObject.SetActive(false); }
            if (timingUI != null) { timingUI.Show(false); }
            if (line != null) { line.enabled = false; }

            for (int i = 0; i < logs.Count; i++)
            {
                if (logs[i] != null)
                    Destroy(logs[i].gameObject);
            }
        }

        static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }

        void ConfigureLogPhysics(GameObject go)
        {
            Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rbs.Length; i++)
                Destroy(rbs[i]);

            Collider[] cols = go.GetComponentsInChildren<Collider>();
            for (int i = 0; i < cols.Length; i++)
                cols[i].isTrigger = true;
        }

        void SafeSetBool(Animator a, string param, bool value)
        {
            if (a == null) return;

            AnimatorControllerParameter[] ps = a.parameters;
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].type == AnimatorControllerParameterType.Bool && ps[i].name == param)
                {
                    a.SetBool(param, value);
                    return;
                }
            }
        }

        void SafeSetTrigger(Animator a, string param)
        {
            if (a == null) return;

            AnimatorControllerParameter[] ps = a.parameters;
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].type == AnimatorControllerParameterType.Trigger && ps[i].name == param)
                {
                    a.SetTrigger(param);
                    return;
                }
            }
        }
    }
}