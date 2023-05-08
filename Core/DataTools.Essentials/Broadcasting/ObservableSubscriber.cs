using DataTools.Essentials.Observable;

namespace DataTools.Essentials.Broadcasting
{
    /// <summary>
    /// Base class for subscriber classes that inherits from <see cref="ObservableBase"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObservableSubscriber<T> : ObservableBase, ISubscriber<T>
    {
        /// <inheritdoc/>
        public abstract void ReceiveData(T value, ISideBandData sideBandData);

    }



}
