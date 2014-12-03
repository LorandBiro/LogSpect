namespace LogSpect.CoreTests.Formatting.TestSubjects
{
    using System;

    public class Complex
    {
        public int Re { get; set; }

        public int Im { get; set; }

        public override string ToString()
        {
            return this.Im >= 0 ? string.Format("{0} + {1}i", this.Re, this.Im) : string.Format("{0} - {1}i", this.Re, -this.Im);
        }
    }
}