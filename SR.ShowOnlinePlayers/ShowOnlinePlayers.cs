//
// ShowOnlinePlayers.cs  –  C# 7.3, Valheim + BepInEx
//

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShowOnlinePlayers
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class ShowOnlinePlayers : BaseUnityPlugin
    {
        private const string ModGUID = "SR.ShowOnlinePlayers";
        private const string ModName = "ShowOnlinePlayers";
        private const string ModVersion = "1.0.8";

        private readonly ManualLogSource _log;
        private readonly Harmony _harmony;
        private readonly List<string> _currentPlayers = new List<string>();

        public ShowOnlinePlayers()
        {
            _log = Logger;
            _harmony = new Harmony(ModGUID);
        }

        private void Awake()
        {
            _harmony.PatchAll();
            _log.LogInfo($"{ModName} {ModVersion} geladen");
        }

        private void OnDestroy() => _harmony.UnpatchSelf();

        private void Update()
        {
            if (ZNet.instance == null) return;

            var playerInfos = ZNet.instance.GetPlayerList() ?? new List<ZNet.PlayerInfo>();

            var names = playerInfos
                        .Select(pi => pi.m_name)
                        .Where(n => !string.IsNullOrEmpty(n))
                        .Distinct()
                        .ToList();

            string self = Player.m_localPlayer ? Player.m_localPlayer.GetPlayerName() : null;
            if (!string.IsNullOrEmpty(self))
            {
                names.Remove(self);
                names.Insert(0, self);
            }

            if (!_currentPlayers.SequenceEqual(names))
            {
                _currentPlayers.Clear();
                _currentPlayers.AddRange(names);
            }
        }

        private void OnGUI()
        {
            if (_currentPlayers.Count == 0) return;

            if (!TryGetMiniMapScreenRect(out Rect mapRect)) return;

            // ---------- Layout‑Konstanten ----------
            const int pad = 8;   // Abstand Minimap → Panel
            int boxWidth = (int)mapRect.width;

            // 3 px nach links rücken, damit es bündig unter der Map ist
            float x = mapRect.xMax - boxWidth - 3f;
            float y = mapRect.yMax + pad;

            var labelStyle = new GUIStyle(GUI.skin.label)   // neuer Label‑Style!
            {
                fontSize = 18
            };
            var headerStyle = new GUIStyle(labelStyle) { fontStyle = FontStyle.Bold };

            var style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 18,
                alignment = TextAnchor.UpperLeft,
                padding = new RectOffset(8, 8, 8, 8)
            };
            int rowHeight = labelStyle.fontSize + 6;   // 6 px Puffer
            int headerHeight = headerStyle.fontSize + 10; // etwas mehr Puffer
            int boxHeight = headerHeight +20 + _currentPlayers.Count * rowHeight;

            GUILayout.BeginArea(new Rect(x, y, boxWidth, boxHeight), style);
            GUILayout.Label($"Spieler online ({_currentPlayers.Count}):", headerStyle);
            foreach (var name in _currentPlayers)
                GUILayout.Label($"• {name}", labelStyle);
            GUILayout.EndArea();
        }

        // Eck‑Berechnung (korrigierte Reihenfolge bleibt)
        private static bool TryGetMiniMapScreenRect(out Rect rect)
        {
            rect = new Rect();
            if (Minimap.instance == null || Minimap.instance.m_smallRoot == null)
                return false;

            var rt = Minimap.instance.m_smallRoot.GetComponent<RectTransform>();
            if (rt == null) return false;

            Vector3[] c = new Vector3[4];
            rt.GetWorldCorners(c);                     // 0=BL 1=TL 2=TR 3=BR
            for (int i = 0; i < 4; i++)
                c[i] = RectTransformUtility.WorldToScreenPoint(null, c[i]);

            Vector3 bl = c[0];
            Vector3 tr = c[2];

            float xMin = bl.x;
            float yMin = Screen.height - tr.y;       // GUI‑y‑0 oben
            float width = tr.x - bl.x;
            float height = tr.y - bl.y;

            rect = new Rect(xMin, yMin, width, height);
            return true;
        }

        internal void ClearPlayerCache() => _currentPlayers.Clear();
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Disconnect))]
    public class DisconnectPatch
    {
        private static void Postfix()
        {
            var mod = BepInEx.Bootstrap.Chainloader.ManagerObject
                     ?.GetComponent<ShowOnlinePlayers>();

            if (mod != null) mod.ClearPlayerCache();
        }
    }
}
