namespace Etmen_PL.Models.Home
{
    /// <summary>
    /// View model for the public landing page.
    /// Keeps controller thin — no raw domain objects leaked to the view.
    /// </summary>
    public class LandingPageViewModel
    {
        /// <summary>True when a crisis is currently active system-wide.</summary>
        public bool HasActiveCrisis { get; set; }

        /// <summary>Display name of the active crisis (e.g., "كوفيد-19").</summary>
        public string? ActiveCrisisName { get; set; }

        /// <summary>Localised crisis type string (Viral / Radiation / Chemical / Biological).</summary>
        public string? ActiveCrisisType { get; set; }
    }
}
