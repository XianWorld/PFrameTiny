using Unity.Collections;

namespace PFrame.Tiny.Authoring
{
    public class TinyAuthoringUtil
    {
        public static Unity.Tiny.Rect Convert(UnityEngine.Rect rect)
        {
            return new Unity.Tiny.Rect(rect.x, rect.y, rect.width, rect.height);
        }

        public static Unity.Tiny.Color Convert(UnityEngine.Color color)
        {
            return new Unity.Tiny.Color(color.r, color.g, color.b, color.a);
        }

        public static CharacterInfo Convert(UnityEngine.CharacterInfo ucharacterInfo)
        {
            var characterInfo = new PFrame.Tiny.CharacterInfo();
            characterInfo.index = ucharacterInfo.index;
            characterInfo.uv = TinyAuthoringUtil.Convert(ucharacterInfo.uv);
            characterInfo.vert = TinyAuthoringUtil.Convert(ucharacterInfo.vert);
            characterInfo.width = ucharacterInfo.advance;
            characterInfo.size = ucharacterInfo.size;
            characterInfo.style = (FontStyle)((int)ucharacterInfo.style);
            characterInfo.flipped = ucharacterInfo.flipped;

            return characterInfo;
        }

        public static PFrame.Tiny.Font Convert(UnityEngine.Font ufont)
        {
            var font = new PFrame.Tiny.Font();
            font.Ascent = ufont.ascent;
            font.LineHeight = ufont.lineHeight;
            font.FontSize = ufont.fontSize;

            var ucharInfos = ufont.characterInfo;
            var num = ucharInfos.Length;
            var characterInfoMap = new NativeHashMap<int, PFrame.Tiny.CharacterInfo>(num, Allocator.Temp);
            for (int i = 0; i < num; i++)
            {
                var ucharInfo = ucharInfos[i];
                var charInfo = TinyAuthoringUtil.Convert(ucharInfo);
                characterInfoMap.Add(charInfo.index, charInfo);
            }
            font.CharacterInfoMap = characterInfoMap;

            return font;
        }

    }

    public static class TinyExtensions
    {
    }
}
