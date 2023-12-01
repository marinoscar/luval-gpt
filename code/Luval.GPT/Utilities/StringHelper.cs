using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Utilities
{
    public static class StringHelper
    {
        public static IEnumerable<string> SplitByWords(string input, int maxCharCount)
        {
            var totalChars = input.Length;
            if(totalChars <= maxCharCount) return new[] { input };
            var result = new List<string>();
            var words = new List<string>();
            var charIndex = 0;
            var countTracker = charIndex;
            var sb = new StringBuilder();
            while (charIndex < totalChars)
            {
                var i = input[charIndex];
                sb.Append(i);
                if(i == '\n' || i == '\t' || i == '\r' || i == ' ')
                {
                    words.Add(sb.ToString());
                    if(sb.ToString().Length + countTracker > maxCharCount)
                    {
                        result.Add(new string(words.SelectMany(i => i.ToArray()).ToArray()));
                        words.Clear();
                        countTracker = -1;
                    }
                    sb.Clear();
                }
                charIndex++;
                countTracker++;
            }
            result.Add(new string(words.SelectMany(i => i.ToArray()).ToArray()) + sb.ToString());
            return result;
        }
    }
}
