using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaSurvivor.Localization
{
    //Una fila de la tabla de textos: la clave y su traducción a cada idioma
    [Serializable]
    public class LocalizationEntry
    {
        public string key;
        [TextArea(1, 6)] public string spanish;
        [TextArea(1, 6)] public string english;

        public string GetText(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return english;
                default:
                    return spanish;
            }
        }
    }

    [CreateAssetMenu(fileName = "LocalizationTable", menuName = "Arena Survivor/Localization Table")]
    public class LocalizationTable : ScriptableObject
    {
        [SerializeField] private List<LocalizationEntry> entries = new List<LocalizationEntry>();

        public IReadOnlyList<LocalizationEntry> Entries => entries;

        [ContextMenu("Completar con textos por defecto")]
        public void MergeDefaults()
        {
            foreach (var (key, spanish, english) in LocalizationDefaults.All)
            {
                if (entries.Exists(entry => entry.key == key))
                {
                    continue;
                }

                entries.Add(new LocalizationEntry { key = key, spanish = spanish, english = english });
            }
        }
    }
}
