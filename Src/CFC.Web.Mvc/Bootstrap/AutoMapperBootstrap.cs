namespace CFC.Web.Mvc.Bootstrap
{
    /// <summary>
    /// Define all object maps here in this class. This is primarily used for mapping input models to domain objects.
    /// Do not map domain objects to view models here. Instead, wrap the domain object in a ViewModel and expose
    /// properties as needed rather than mapping.
    /// </summary>
    public static class AutoMapperBootstrap
    {
        /// <summary>
        /// True if automapper has already been configured.
        /// </summary>
        private static bool _configured = false;

        public static void Configure()
        {
            if (_configured) return;

            _configured = true;
        }
    }
}