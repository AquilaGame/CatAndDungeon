using System.Collections.Generic;

namespace EPOOutline
{
    public static class KeywordsUtility
    {
        private static Dictionary<BlurType, string> BlurTypes = new Dictionary<BlurType, string>
                {
                    { BlurType.Anisotropic,     "ANISOTROPIC_BLUR" },
                    { BlurType.Box,             "BOX_BLUR" },
                    { BlurType.Gaussian5x5,     "GAUSSIAN5X5" },
                    { BlurType.Gaussian9x9,     "GAUSSIAN9X9" },
                    { BlurType.Gaussian13x13,   "GAUSSIAN13X13" }
                };

        private static Dictionary<DilateQuality, string> DilateQualityKeywords = new Dictionary<DilateQuality, string>
                {
                    { DilateQuality.Base,       "BASE_QALITY_DILATE" },
                    { DilateQuality.High,       "HIGH_QUALITY_DILATE" },
                    { DilateQuality.Ultra,      "ULTRA_QUALITY_DILATE" }
                };

        public static string GetTextureArrayCutoutKeyword()
        {
            return "TEXARRAY_CUTOUT";
        }

        public static string GetDilateQualityKeyword(DilateQuality quality)
        {
            switch (quality)
            {
                case DilateQuality.Base:
                    return "BASE_QALITY_DILATE";
                case DilateQuality.High:
                    return "HIGH_QUALITY_DILATE";
                case DilateQuality.Ultra:
                    return "ULTRA_QUALITY_DILATE";
                default:
                    throw new System.Exception("Unknown dilate quality level");
            }
        }

        public static string GetWeightedAverateKeyword()
        {
            return "USE_WEIGHTED_AVERAGE";
        }

        public static string GetEnabledInfoBufferKeyword()
        {
            return "USE_INFO_BUFFER";
        }

        public static string GetInfoBufferStageKeyword()
        {
            return "INFO_BUFFER_STAGE";
        }

        public static string GetBlurKeyword(BlurType type)
        {
            return BlurTypes[type];
        }

        public static string GetCutoutKeyword()
        {
            return "USE_CUTOUT";
        }

        public static void GetAllBlurKeywords(List<string> list)
        {
            list.Clear();
            foreach (var item in BlurTypes)
                list.Add(item.Value);
        }

        public static void GetAllDilateKeywords(List<string> list)
        {
            list.Clear();
            foreach (var item in DilateQualityKeywords)
                list.Add(item.Value);
        }
    }
}