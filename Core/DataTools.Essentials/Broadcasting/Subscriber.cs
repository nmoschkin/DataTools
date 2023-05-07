namespace DataTools.Essentials.Broadcasting
{
    /// <summary>
    /// Base class for subscriber classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Subscriber<T> : ISubscriber<T>
    {

        /// <inheritdoc/>
        public abstract void ReceiveData(T value, ISideBandData sideBandData);

        void ISubscriber.ReceiveData(object value, ISideBandData sideBandData)
        {
            ReceiveData((T)value, sideBandData);
        }
    }



}
