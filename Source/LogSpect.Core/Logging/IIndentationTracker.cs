namespace LogSpect.Logging
{
    using System;

    public interface IIndentationTracker
    {
        string Current { get; }

        void Increase();

        void Decrease();
    }
}
