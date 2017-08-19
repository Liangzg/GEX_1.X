namespace LuaDataBind
{
    /// <summary>
    ///   Type of data binding.
    /// </summary>
    public enum DataBindingType
    {
        /// <summary>
        ///   Data is fetched from a data context.
        /// </summary>
        Context,

        /// <summary>
        ///   Data is taken from a specific provider.
        /// </summary>
        Provider,

        /// <summary>
        ///   Data is a constant value.
        /// </summary>
        Constant
    }
}