namespace TextFx
{
    public static class TextFxUtils
    {
        /// <summary>
        /// Checks wether the character contributes to the calculations.
        /// </summary>
        /// <param name="ch">The character to check.</param>
        /// <returns>True if the character contributes else false.</returns>
        public static bool IsContributingChar(char ch)
        {
            return !char.IsWhiteSpace(ch) && ch != '\0';
        }

        /// <summary>
        /// Counts the number of contributing characters in a given text.
        /// </summary>
        /// <param name="text">The text to count for.</param>
        /// <returns>The number of contributing characters in the given text.</returns>
        public static int CountContributingChars(string text)
        {
            var result = 0;

            foreach (var character in text)
            {
                if (IsContributingChar(character))
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the extent per contributing character in a given text.
        /// </summary>
        /// <param name="text">The text to calculate for.</param>
        /// <returns>The extent per contributing character in the given text.</returns>
        /// <returns></returns>
        public static float CalculateExtentPerChar(string text)
        {
            var contributingCharsCount = CountContributingChars(text);

            if (contributingCharsCount == 0)
            {
                return 0f;
            }

            return 1f / contributingCharsCount;
        }
    }
}