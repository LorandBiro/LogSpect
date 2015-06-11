namespace LogSpect.Formatting
{
    using System;

    public interface IIndentationTracker
    {
        string Current { get; }

        void Increase();

        void Decrease();
    }
}
