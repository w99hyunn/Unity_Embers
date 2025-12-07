using System.Collections.Generic;
using Michsky.UI.Reach;

namespace NOLDA
{
    public class CustomChapterManager : ChapterManager
    {
        // Content
        public List<CustomChapterItem> customChapters = new List<CustomChapterItem>();

        [System.Serializable]
        public class CustomChapterItem : ChapterItem
        {
            public Class characterClass;
        }

        public override void InitializeChapters()
        {
            if (customChapters == null || customChapters.Count == 0)
            {
                base.InitializeChapters();
                return;
            }

            List<ChapterItem> baseChapters = new List<ChapterItem>();
            foreach (var customItem in customChapters)
            {
                baseChapters.Add(customItem);
            }

            var originalChapters = base.chapters;
            base.chapters = baseChapters;

            base.InitializeChapters();
            base.chapters = originalChapters;

            for (int i = 0; i < identifiers.Count && i < customChapters.Count; ++i)
            {
                if (identifiers[i] != null && customChapters[i] != null)
                {
                    // ClassName
                    if (useLocalization == false && customChapters[i].characterClass != Class.NONE)
                    {
                        identifiers[i].classNameObject.text = customChapters[i].characterClass.ToString();
                    }
                }
            }
        }
    }
}
