namespace LogSpect.Logging
{
    using System;

    public interface IIndentationService
    {
        string Current { get; }

        void Increase();

        void Decrease();
    }
}
