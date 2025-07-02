using System;

namespace Zenject
{
    // Base signal type for strongly typed signals
    public abstract class Signal<TSignal> : ISignal where TSignal : Signal<TSignal>
    {
        public Signal()
        {
        }
    }

    public interface ISignal
    {
    }
}