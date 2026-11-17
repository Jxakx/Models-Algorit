using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaSurvivor.Localization
{
    /// <summary>Una fila de la tabla de textos: la clave y su traducción a cada idioma.</summary>
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

    /// <summary>
    /// Todos los textos del juego, en un asset editable: clave -> texto en cada idioma.
    /// Agregar o corregir una traducción es editar este asset en el Inspector, sin
    /// recompilar nada.
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizationTable", menuName = "Arena Survivor/Localization Table")]
    public class LocalizationTable : ScriptableObject
    {
        [SerializeField] private List<LocalizationEntry> entries = new List<LocalizationEntry>();

        public IReadOnlyList<LocalizationEntry> Entries => entries;

        /// <summary>Agrega las claves por defecto que todavía no estén (no pisa lo que ya se haya editado).</summary>
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
