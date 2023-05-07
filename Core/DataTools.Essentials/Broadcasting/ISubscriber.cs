using DataTools.Essentials.Observable;

namespace DataTools.Essentials.Broadcasting
{
    /// <summary>
    /// Implements methods that allow a subscriber to receive data
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// Receive new data
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sideBandData"></param>
        void ReceiveData(object value, ISideBandData sideBandData);

    }
    /// <summary>
    /// Implements methods that allow a subscriber to receive data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISubscriber<T> : ISubscriber
    {
        /// <summary>
        /// Receive new data
        /// </summary>
        /// <param name="value">New data</param>
        /// <param name="sideBandData">Sideband data</param>
        void ReceiveData(T value, ISideBandData sideBandData);
    }



}
