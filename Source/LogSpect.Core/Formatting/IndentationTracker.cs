namespace LogSpect.Formatting
{
    using System;
    using System.Linq;

    public sealed class IndentationTracker : IIndentationTracker
    {
        [ThreadStatic]
        private static int currentLevel;

        private readonly string[] indentationStrings;

        public IndentationTracker()
            : this(4, 20)
        {
        }

        public IndentationTracker(int indentationWidth, int maxLevel)
        {
            if (indentationWidth < 1)
            {
                throw new ArgumentException("Indentation width must be at least 1.", "indentationWidth");
            }

            if (maxLevel < 0)
            {
                throw new ArgumentException("Maximum level cannot be negative.");
            }

            this.indentationStrings = Enumerable.Range(0, maxLevel).Select(i => new string(' ', i * indentationWidth)).ToArray();
        }

        public string Current
        {
            get
            {
                if (currentLevel < 0)
                {
                    return this.indentationStrings[0];
                }
                
                if (currentLevel >= this.indentationStrings.Length)
                {
                    return this.indentationStrings[this.indentationStrings.Length - 1];
                }

                return this.indentationStrings[currentLevel];
            }
        }

        public void Increase()
        {
            currentLevel++;
        }

        public void Decrease()
        {
            currentLevel--;
        }
    }
}