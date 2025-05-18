using MudBlazor;

namespace VehicleInformationChecker.Components.Layout
{
    public partial class MainLayout
    {
        private bool _isDarkMode = true;

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        private void DarkModeToggle()
        {
            _isDarkMode = !_isDarkMode;
        }

        public string DarkLightModeButtonIcon => _isDarkMode switch
        {
            true => Icons.Material.Rounded.LightMode,
            false => Icons.Material.Outlined.DarkMode,
        };
    }
}
