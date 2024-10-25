using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.Classes
{
    public static class ReactionTypes
    {
        public static readonly Emoji greenCheckEmoji = new Emoji("✅");
        public static readonly Emoji startEmoji = new Emoji("▶️");
        public static readonly Emoji xEmoji = new Emoji("❌");
        public static readonly Emoji blankEmoji = new Emoji("⬜");

        // 0-tól 10-ig szám emojik
        public static readonly Emoji zeroEmoji = new Emoji("0️⃣");
        public static readonly Emoji oneEmoji = new Emoji("1️⃣");
        public static readonly Emoji twoEmoji = new Emoji("2️⃣");
        public static readonly Emoji threeEmoji = new Emoji("3️⃣");
        public static readonly Emoji fourEmoji = new Emoji("4️⃣");
        public static readonly Emoji fiveEmoji = new Emoji("5️⃣");
        public static readonly Emoji sixEmoji = new Emoji("6️⃣");
        public static readonly Emoji sevenEmoji = new Emoji("7️⃣");
        public static readonly Emoji eightEmoji = new Emoji("8️⃣");
        public static readonly Emoji nineEmoji = new Emoji("9️⃣");
        public static readonly Emoji tenEmoji = new Emoji("🔟");

        public static readonly Emoji cooperateEmoji = new Emoji("🛡️");
        public static readonly Emoji notCooperateEmoji = new Emoji("⚔️");
    }
    
}
